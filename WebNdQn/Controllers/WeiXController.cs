﻿using BLL;
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
                    //取得CGI的token值,这是一个全局的token
                    string cgi_token = wxbll.Get_Cgi_Taoke(dto.wx_appid, dto.wx_secret);
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/WeiXController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "Wx_Auth_Code 取得cgi_token值：" + cgi_token);
                    //和通过授权code取得的openid值一起,取到用户的详细信息
                    Wx_UserInfo dto2 = wxbll.Get_Cgi_UserInfo(dto1.openid, cgi_token);
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/WxDefault_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "subscribe：" + dto2.subscribe + "openid: " + dto2.openid + "nickname:" + dto2.nickname + "headurl:" + dto2.headimgurl);
                    if (clist[1] == "1")
                    {
                        //将微信用户的详细信息写入数据库
                        wxbll.SetWxUserInfo(dto.wx_appid, dto1.openid, dto2.nickname, dto2.sex, dto2.headimgurl, dto2.unionid);
                    }
                    param = "?ctype=" + ctype + "&issue=" + issue + "&p=" + DEncrypt.DESEncrypt1("1|" + dto2.subscribe + "|" + dto1.openid); //1|subscribe|openid  微信发送|是否关注|openid
                    backurl = clist[2] + param;
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/WeiXController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "Wx_Auth_Code 取得backurl值：" + backurl + " 取得param值: " + param);
                    return Redirect(backurl);
                }
            }
            return Content("配置无效");
        }
        public ActionResult testss() {
            //string appid = "wx0d8924c9bc2c6e11";
            //string openid = "oIW7Uwk5tMFZ7aakoLLlPF4IOHkY";
            //string nickname = "秋秋";
            //string headurl = "http://wx.qlogo.cn/mmopen/vi_32/PiajxSqBRaELZH3yJ64IB7c1tC1sgUYPvz88FjVFdUpiapZ1MHsa2OZ8DG0iaJ3b83kVJ03JicLmicAn7nXSQ2lYIVg/0";
            //string union = "";
            //int result = wxbll.SetWxUserInfo(appid, openid, nickname, 1, headurl, null);
            return Content("参数错误");
            //string str = "RicHorQDi7Cbg2mSBnPTVUOR+4KG30Di2Lc7f0lN/P8=";
            //string p = Request["p"].ToString();
            //string temp = DEncrypt.Decrypt(str, DEncrypt.signkey);    //取得p参数,并且进行解密
            //return Content("参数错误" + temp);
        }
    }
}