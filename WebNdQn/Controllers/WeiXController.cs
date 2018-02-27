using BLL;
using FJSZ.OA.Common.DEncrypt;
using Model.WxModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebNdQn.Controllers
{
    public class WeiXController : Controller
    {
        WeiXinBLL wxbll = new WeiXinBLL();
        // GET: WeiX
        public ActionResult Wx_Auth_Code()
        {
            Common.Expend.LogTxtExpend.WriteLogs("/Logs/WeiXController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "Wx_Auth_Code   开始");
            if (Request["code"] == null || Request["state"] == null|| Request["ctype"] == null || Request["issue"] == null || Request["c"] == null)
                return Content("参数为空");
            string code = Request["code"].ToString();
            string state = Request["state"].ToString();
            string ctype = Request["ctype"].ToString();
            string issue = Request["issue"].ToString();
            string c = Request["c"].ToString();
            Common.Expend.LogTxtExpend.WriteLogs("/Logs/WeiXController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "Wx_Auth_Code  code: " + code + " state: " + state + " ctype: " + ctype + " issue: " + issue + " c: " + c);
            //把c解密出来,如果异常就是验证出错.
            try { c = DEncrypt.DESDecrypt1(c); } catch { return Content("验证错误"); }
            Common.Expend.LogTxtExpend.WriteLogs("/Logs/WeiXController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "Wx_Auth_Code  code: " + code + " state: " + state + " ctype: " + ctype + " issue: " + issue + " c: " + c);
            string[] clist = c.Split('|');      //例如:   SNS|1|http://www.ndll800.com/Home/Index   SNS方式取用户信息|1继续取明细写入数据库|http://www.ndll800.com/Home/Index 再跳转的地址
            if (clist.Length != 3 || !clist[1].IsNum()|| (clist[0].ToUpper() != "SNS" && clist[0].ToUpper() != "CGI")) {
                return Content("参数错误");
            }
            T_CooperConfig dto = wxbll.Get_CooperConfig(Convert.ToInt32(ctype), Convert.ToInt32(issue));                              //取得配置
            if (dto != null)
            {
                string param = "", backurl = "";    //param参数,backurl跳转地址
                WxJsApi_token dto1 = wxbll.Wx_Auth_AccessToken(dto.wx_appid, dto.wx_secret, code);   //主要是取得SNS的access_token,但是openid在这个阶段也已经能获取到了
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/WeiXController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "Wx_Auth_Code 取得token值：" + dto1.access_token + " 取得Openid值: " + dto1.openid);

                if (clist[0].ToUpper() == "SNS")
                {
                    if (clist[1] == "1") {
                        Wx_UserInfo dto2 = wxbll.Get_SNS_UserInfo(dto1.openid, dto1.access_token);
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/WeiXController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "Wx_Auth_Code 取得nickname值：" + dto2.nickname + " 取得sex值: " + dto2.sex + " 取得headimgurl值: " + dto2.headimgurl + " 取得unionid值: " + dto2.unionid);
                        //将微信用户的详细信息写入数据库
                        wxbll.SetWxUserInfo(dto.wx_appid, dto1.openid, dto2.nickname, dto2.sex, dto2.headimgurl, dto2.unionid);
                    }
                    param = "?ctype=" + ctype + "&issue=" + issue + "&p=" + DEncrypt.DESEncrypt1("1|0|" + dto1.openid);   //1|0|openid  1微信发送过来的|是否关注|openid微信用户id
                    backurl = clist[2] + param;
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/WeiXController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "Wx_Auth_Code 取得backurl值：" + backurl + " 取得param值: " + param);
                    return Redirect(backurl);
                }
                if (clist[0].ToUpper() == "CGI")
                {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/WeiXController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "Wx_Auth_Code CGI进入开始：");
                    //取得CGI的token值,这是一个全局的token
                    object obj_cgi_token = FJSZ.OA.Common.CacheAccess.GetFromCache(dto.wx_appid + "cgi_token");
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/WeiXController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "Wx_Auth_Code 取得cgi_token值：" + obj_cgi_token);
                    string cgi_token = "";
                    if (obj_cgi_token == null)
                    {
                        cgi_token = wxbll.Get_Cgi_Taoke(dto.wx_appid, dto.wx_secret);
                        FJSZ.OA.Common.CacheAccess.InsertToCacheByTime(dto.wx_appid + "cgi_token", cgi_token, 7000);
                    }
                    else cgi_token = obj_cgi_token.ToString();
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/WeiXController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "Wx_Auth_Code 取得cgi_token值：" + cgi_token);
                    //和通过授权code取得的openid值一起,取到用户的详细信息
                    Wx_UserInfo dto2 = wxbll.Get_Cgi_UserInfo(dto1.openid, cgi_token);
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/WeiXController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "subscribe：" + dto2.subscribe + "openid: " + dto2.openid + "nickname:" + dto2.nickname + "headurl:" + dto2.headimgurl);
                    if (clist[1] == "1")
                    {
                        //当用户已经关注了公众号就能取到详细信息,直接写入
                        if (dto2.subscribe == 1)
                        {
                            wxbll.SetWxUserInfo(dto.wx_appid, dto1.openid, dto2.nickname, dto2.sex, dto2.headimgurl, dto2.unionid);
                        }
                        else
                        {
                            //当用户没有关注公众号,取不到详细信息,就需要用sns_token去取详细信息写入数据库
                            dto2 = wxbll.Get_SNS_UserInfo(dto1.openid, dto1.access_token);
                            wxbll.SetWxUserInfo(dto.wx_appid, dto1.openid, dto2.nickname, dto2.sex, dto2.headimgurl, dto2.unionid);
                        }
                    }
                    param = "?ctype=" + ctype + "&issue=" + issue + "&p=" + DEncrypt.DESEncrypt1("1|" + dto2.subscribe + "|" + dto1.openid); //1|subscribe|openid  微信发送|是否关注|openid
                    backurl = clist[2] + param;
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/WeiXController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "Wx_Auth_Code 取得backurl值：" + backurl + " 取得param值: " + param);
                    return Redirect(backurl);
                }
            }
            return Content("配置无效");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Wx_Auth_Code1() {
            if (Request["code"] == null || Request["state"] == null || Request["c"] == null)
                return Content("参数为空");
            string code = Request["code"].ToString();
            string state = Request["state"].ToString();
            string c = Request["c"].ToString();
            //把c解密出来,如果异常就是验证出错.
            try { c = DEncrypt.DESDecrypt1(c); } catch { return Content("验证错误"); }
            string[] clist = c.Split('|');      //例如:   SNS|1|http://www.ndll800.com/Home/Index   SNS方式取用户信息|1继续取明细写入数据库|http://www.ndll800.com/Home/Index 再跳转的地址
            if (clist.Length != 3 || !clist[1].IsNum() || (clist[0].ToUpper() != "SNS"))
                return Content("参数错误");
            string param = "", backurl = "", str = "", nickname = "";    //param参数,backurl跳转地址
            WxJsApi_token dto1 = wxbll.Wx_Auth_AccessToken(Wx_config.appid, Wx_config.appsecret, code);   //主要是取得SNS的access_token,但是openid在这个阶段也已经能获取到了
            Common.Expend.LogTxtExpend.WriteLogs("/Logs/ComExpendController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "Wx_Auth_Code 取得token值：" + dto1.access_token + " 取得Openid值: " + dto1.openid);
            if (clist[0].ToUpper() == "SNS")
            {
                if (clist[1] == "1")
                {
                    Wx_UserInfo dto2 = wxbll.Get_SNS_UserInfo(dto1.openid, dto1.access_token);
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/ComExpendController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "Wx_Auth_Code 取得nickname值：" + dto2.nickname + " 取得sex值: " + dto2.sex + " 取得headimgurl值: " + dto2.headimgurl + " 取得unionid值: " + dto2.unionid);
                    //将微信用户的详细信息写入数据库
                    //wxbll.WeiX_Execute_User(0, 0, cbll.GetIp(), dto2.nickname, dto1.openid, dto2.headimgurl);
                    nickname = dto2.nickname;
                    str = "|" + dto2.headimgurl;
                }
                param = "?p=" + DEncrypt.DESEncrypt1("1|" + dto1.openid + str) + "&n=" + nickname+"&state="+ state;   //1|openid|headurl  1|openid微信用户id|头像URL
                backurl = clist[2] + param;
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/ComExpendController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "Wx_Auth_Code 取得backurl值：" + backurl + " 取得param值: " + param);
                return Redirect(backurl);
            }
            return Content("配置无效");
        }
    }
}