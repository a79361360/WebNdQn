﻿using BLL;
using FrameWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Fgly.Common.Expand;
using Model.WxModel;
using FJSZ.OA.Common.Web;
using System.Net.Http;

namespace WebNdQn.Controllers
{
    public class HomeController : BaseController
    {
        CommonBLL bll = new CommonBLL();
        WeiXinBLL wxll = new WeiXinBLL();
        MobileBLL mbll = new MobileBLL();
        public ActionResult Wx_Auth_Code() {
            if (Request["code"] == null || Request["state"] == null) {
                return Content("参数为空");
            }
            Common.Expend.LogTxtExpend.WriteLogs("/Logs/WxDefault_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "Wx_Auth_Code");
            string code = Request["code"].ToString();
            string state = Request["state"].ToString();
            Dictionary<string, string> dic = ParmToDic(state);      //字典
            string ctype = "0", issue = "0";
            if (dic.Count > 0)
            {
                ctype = dic["ctype"];
                issue = dic["issue"];
            }
            else {
                return Content("缺少参数");
            }

            T_CooperConfig dto = wxll.Get_CooperConfig(Convert.ToInt32(ctype), Convert.ToInt32(issue));                              //取得配置
            if (dto != null)
            {
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/WxDefault_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "code：" + code + " state: " + state);
                WxJsApi_token dto1 = wxll.Wx_Auth_AccessToken(dto.wx_appid, dto.wx_secret, code);
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/WxDefault_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "取得token值：" + dto1.access_token + " 取得Openid值: " + dto1.openid);
                //取得CGI的token值
                string cgi_token = wxll.Get_Cgi_Taoke(dto.wx_appid, dto.wx_secret);
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/WxDefault_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "cgi_token：" + cgi_token);
                Wx_UserInfo dto2 = wxll.Get_Cgi_UserInfo(dto1.openid, cgi_token);
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/WxDefault_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "subscribe：" + dto2.subscribe + "openid: " + dto2.openid);
                string url = state.Replace("|", "&") + "&gzstate=" + dto2.subscribe + "&openid=" + dto2.openid;
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/WxDefault_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "url：" + url);
                return Redirect(url);
            }
            else {
                return Content("缺少配置");
            }
        }
        //public ActionResult DefaultWx() {

        //}
        //public ActionResult DefaultWxPortal() {
        //    int ctype = 0, issue = 1;
        //    if (Request["ctype"] == null || Request["code"] == null || Request["state"] == null)
        //        return JsonFormat(new ExtJson { success = false, msg = "参数不能为空" });
        //    ctype = Convert.ToInt32(Request.QueryString["ctype"]);
        //    //issue = Convert.ToInt32(Request.QueryString["issue"]);
        //    string code = Request["code"].ToString();
        //    string state = Request["state"].ToString();
        //    T_CooperConfig dto = wxll.Get_CooperConfig(ctype, issue);                              //取得配置
        //    if (dto == null)
        //        return JsonFormat(new ExtJson { success = false, msg = "缺少配置" });
        //    #region 获取微信用户的openid
        //    WxJsApi_token dto1 = wxll.Wx_Auth_AccessToken(dto.wx_appid, dto.wx_secret, code);
        //    if (dto1 == null)
        //        return JsonFormat(new ExtJson { success = false, msg = "微信用户信息不正确" });
        //    ViewBag.openid = dto1.openid;
        //    Common.Expend.LogTxtExpend.WriteLogs("/Logs/HomeController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "dto1.openid :" + dto1.openid);
        //    //ViewBag.openid = "oIW7Uwk5tMFZ7aakoLLlPF4IOHkY";
        //    #endregion
        //    return View();
        //}
        //转换成数组
        public Dictionary<string, string> ParmToDic(string url) {
            string[] str = url.Split('?');
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (str.Length > 1) {
                string[] str1 = str[1].Split('|');
                foreach(string item in str1){
                    string[] str2 = item.Split('=');
                    dic.Add(str2[0], str2[1]);
                }
            }
            //string lll = "http://wx.ndll800.com/home/default?ctype=1&issue=1";
            //ParmToDic(lll);
            return dic;
        }
        //关注公众号
        public ActionResult Default() {
            if (Request["gzstate"] == null)
            {
                if (Request["ctype"] == null || Request["issue"] == null)
                {
                    return JsonFormat(new ExtJson { success = false, msg = "参数不能为空" });
                }
                string ctype = Request["ctype"].ToString(); string issue = Request["issue"].ToString();
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/WxDefault_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "参数非空 ctype: " + ctype + "issue: " + issue);
                T_CooperConfig dto = wxll.Get_CooperConfig(Convert.ToInt32(ctype), Convert.ToInt32(issue));                              //取得配置
                if (dto != null)
                {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/WxDefault_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "gzstate为空");
                    //snsapi_base,snsapi_userinfo
                    string state = Request.Url.AbsoluteUri.Replace("&", "|");
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/WxDefault_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "state的值： " + state);
                    string url = wxll.Wx_Auth_Code(dto.wx_appid, "http://wx.ndll800.com/Home/Wx_Auth_Code", "snsapi_base", state);
                    return Redirect(url);
                }
                else
                {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/WxDefault_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "未进行客户信息配置： ctype: " + ctype + "issue: " + issue);
                    return JsonFormat(new ExtJson { success = false, msg = "配置为空" });
                }
            }
            else
            {
                if (Request["ctype"] == null || Request["issue"] == null || Request["gzstate"] == null || Request["openid"] == null) {
                    return JsonFormat(new ExtJson { success = false, msg = "参数不能为空" });
                }
                string ctype = Request["ctype"].ToString();
                string issue = Request["issue"].ToString();
                string gz = Request["gzstate"].ToString();
                string openid = Request["openid"].ToString();
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/WxDefault_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "ctype：" + ctype+ "issue：" + issue + "gzstate：" + gz);
                T_CooperConfig dto = wxll.Get_CooperConfig(Convert.ToInt32(ctype), Convert.ToInt32(issue));                              //取得配置
                if (dto != null)
                {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/WxDefault_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "gzstate：" + gz);
                    ViewBag.qrcode = dto.qrcode_url;
                    ViewBag.Gz = gz;
                    ViewBag.Openid = openid;
                    if (!string.IsNullOrEmpty(dto.bgurl))
                    {
                        ViewBag.bg = dto.bgurl;             //背景图
                        ViewBag.btn = dto.btnurl;           //背景按钮图
                    }
                }
                else {
                    return JsonFormat(new ExtJson { success = false, msg = "配置为空" });
                }
                
            }
            return View();
        }
        public ActionResult SignPhoneFilter_gz() {
            if (Request["phone"] == null || Request["ctype"] == null || Request["issue"] == null || Request["openid"] == null)
            {
                return JsonFormat(new ExtJson { success = false, msg = "参数不能为空!" });
            }
            string phone = Request["phone"].ToString();         //用户手机号码
            string ctype = Request["ctype"].ToString();         //公司类型
            string issue = Request["issue"].ToString();         //活动期号
            string openid = Request["openid"].ToString();       //微信的Openid
            string path = Server.MapPath(@"/Content/Txt/pwebconfig.txt");
            bool result = bll.ReadPhoneFliter(phone, path); //验证手机号码
            if (result)
            {
                int de_Openid = bll.DecideOpenid(openid, Convert.ToInt32(ctype), Convert.ToInt32(issue));   //Openid是否已经参加过活动
                if (de_Openid > 0) return JsonFormat(new ExtJson { success = false, msg = "当前微信已经添加过活动!" });
                int de_reslut = bll.DecidePhone(phone, Convert.ToInt32(ctype), Convert.ToInt32(issue));   //手机号码是否已经参加过活动
                if (de_reslut > 0)
                {
                    return JsonFormat(new ExtJson { success = false, msg = "当前手机号已经添加过活动!" });
                }
                else {
                    T_CooperConfig dto = wxll.Get_CooperConfig(Convert.ToInt32(ctype), Convert.ToInt32(issue));                              //取得配置
                    if (dto != null)
                    {
                        int limitnum = dto.uplimit;     //活动人数上限
                        if (limitnum != 0 && bll.CtypeCount(Convert.ToInt32(ctype), Convert.ToInt32(issue)) >= limitnum)
                        {
                            return JsonFormat(new ExtJson { success = false, msg = "已经超过活动人数上限!" });
                        }
                    }
                    else
                        return JsonFormat(new ExtJson { success = false, msg = "活动已结束，谢谢参考!" });
                    int result_f = bll.TakeFlowLog(Convert.ToInt32(ctype), Convert.ToInt32(issue), phone, openid);
                    if (result_f == 1)
                        return JsonFormat(new ExtJson { success = true, msg = "记得分享给更多朋友领取流量!" });
                    else
                        return JsonFormat(new ExtJson { success = false, msg = "写入充值记录失败，请重新尝试!" });
                }
            }
            else
                return JsonFormat(new ExtJson { success = false, msg = "本活动仅限宁德移动手机用户参与!" });
        }
        //分享到朋友或者朋友圈
        public ActionResult Index()
        {
            int cooper = 0, issue = 1;
            if(Request.QueryString["ctype"] !=null){
                cooper = Convert.ToInt32(Request.QueryString["ctype"]);
                issue = Convert.ToInt32(Request.QueryString["issue"]);
            }
            T_CooperConfig dto = wxll.Get_CooperConfig(cooper, issue);                              //取得配置
            long timestamp = DateTime.Now.ToUnixTimeStamp();                                        //时间戳
            string noncestr = TxtHelp.GetRandomString(16, true, true, true, false, "");             //随机字符串
            string signatrue = wxll.Get_signature(timestamp, noncestr);                             //signatrue
            ViewBag.appid = Wx_config.appid;
            ViewBag.timestamp = timestamp;
            ViewBag.noncestr = noncestr;
            ViewBag.signatrue = signatrue;
            if (dto != null)
            {
                ViewBag.title = dto.title;              //标题
                ViewBag.desc = dto.descride;            //描述
                ViewBag.imgurl = dto.imgurl;            //图片地址
                ViewBag.linkurl = dto.linkurl;          //链接地址
                if (!string.IsNullOrEmpty(dto.bgurl))
                {
                    ViewBag.bg = dto.bgurl;             //背景图
                    ViewBag.btn = dto.btnurl;           //背景按钮图
                }
                //ViewBag.dto = dto;                      //实体类
            }
            return View(dto);
        }
        public ActionResult SIndex() {
            int cooper = 0, issue = 1;
            if (Request.QueryString["ctype"] != null)
            {
                cooper = Convert.ToInt32(Request.QueryString["ctype"]);
                issue = Convert.ToInt32(Request.QueryString["issue"]);
            }
            T_CooperConfig dto = wxll.Get_CooperConfig(cooper, issue);                              //取得配置
            long timestamp = DateTime.Now.ToUnixTimeStamp();                                        //时间戳
            string noncestr = TxtHelp.GetRandomString(16, true, true, true, false, "");             //随机字符串
            string signatrue = wxll.Get_signature(timestamp, noncestr);                             //signatrue
            ViewBag.appid = Wx_config.appid;
            ViewBag.timestamp = timestamp;
            ViewBag.noncestr = noncestr;
            ViewBag.signatrue = signatrue;
            if (dto != null)
            {
                ViewBag.title = dto.title;              //标题
                ViewBag.desc = dto.descride;            //描述
                ViewBag.imgurl = dto.imgurl;            //图片地址
                ViewBag.linkurl = dto.linkurl;          //链接地址
                ViewBag.redirecturi = dto.redirecturi;          //其他链接地址
                if (!string.IsNullOrEmpty(dto.bgurl))
                {
                    ViewBag.bg = dto.bgurl;             //背景图
                    ViewBag.btn = dto.btnurl;           //背景按钮图
                }
                //ViewBag.dto = dto;                      //实体类
            }
            return View(dto);
        }
        public ActionResult OppoIndex() {
            int ctype = 0, issue = 1;
            if (Request.QueryString["ctype"] != null)
            {
                ctype = Convert.ToInt32(Request.QueryString["ctype"]);
                issue = Convert.ToInt32(Request.QueryString["issue"]);
            }
            T_CooperConfig dto = wxll.Get_CooperConfig(ctype, issue);                              //取得配置
            long timestamp = DateTime.Now.ToUnixTimeStamp();                                        //时间戳
            string noncestr = TxtHelp.GetRandomString(16, true, true, true, false, "");             //随机字符串
            string signatrue = wxll.Get_signature(timestamp, noncestr);                             //signatrue
            ViewBag.appid = Wx_config.appid;
            ViewBag.timestamp = timestamp;
            ViewBag.noncestr = noncestr;
            ViewBag.signatrue = signatrue;
            if (dto != null)
            {
                ViewBag.title = dto.title;              //标题
                ViewBag.desc = dto.descride;            //描述
                ViewBag.imgurl = dto.imgurl;            //图片地址
                ViewBag.linkurl = dto.linkurl;          //链接地址
            }
            return View();
        }
        public ActionResult SignPhoneFilter() {
            if (Request["phone"] == null || Request["ctype"] == null || Request["issue"] == null)
            {
                return JsonFormat(new ExtJson { success = false, msg = "参数不能为空!" });
            }
            string phone = Request["phone"].ToString();         //用户手机号码
            string ctype = Request["ctype"].ToString();         //公司类型
            string issue = Request["issue"].ToString();         //活动期号
            string path = Server.MapPath(@"/Content/Txt/pwebconfig.txt");
            bool result = bll.ReadPhoneFliter(phone, path); //验证手机号码
            if (result)
            {
                int de_reslut = bll.DecidePhone(phone, Convert.ToInt32(ctype), Convert.ToInt32(issue));   //手机号码是否已经参加过活动
                if (de_reslut > 0)
                {
                    return JsonFormat(new ExtJson { success = false, msg = "当前手机号已经添加过活动!" });
                }
                else {
                    T_CooperConfig dto = wxll.Get_CooperConfig(Convert.ToInt32(ctype), Convert.ToInt32(issue));                              //取得配置
                    if (dto != null)
                    {
                        int limitnum = dto.uplimit;     //活动人数上限
                        if (limitnum != 0 && bll.CtypeCount(Convert.ToInt32(ctype), Convert.ToInt32(issue)) >= limitnum)
                        {
                            return JsonFormat(new ExtJson { success = false, msg = "已经超过活动人数上限!" });
                        }
                    }
                    else
                        return JsonFormat(new ExtJson { success = false, msg = "活动已结束，谢谢参考!" });
                    int result_f = bll.TakeFlowLog(Convert.ToInt32(ctype), Convert.ToInt32(issue), phone);
                    if (result_f == 1)
                        return JsonFormat(new ExtJson { success = true, msg = "记得分享给更多朋友领取流量!" });
                    else
                        return JsonFormat(new ExtJson { success = false, msg = "写入充值记录失败，请重新尝试!" });
                }
            }
            else
                return JsonFormat(new ExtJson { success = false, msg = "本活动仅限宁德移动手机用户参与!" });
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        /// <summary>
        /// 接收验证码
        /// </summary>
        /// <returns></returns>
        public ActionResult TakeCode() {
            if (Request["type"] == null || Request["code"] == null|| Request["phone"] == null) {
                return JsonFormat(new ExtJson { success = false, msg = "参数不能为空" });
            }
            string phone = Request["phone"].ToString();     //哪个手机号码接收到的
            int type = Convert.ToInt32(Request["type"]);    //1为登入2为充值
            string code = Request["code"];    //验证码
            if (type == 1) {
                bll.SaveLoginState(phone, Convert.ToInt32(code));
            }
            int result = bll.TakeMsgCode(type, phone,"0", code,"");    //将收到的验证码保存
            if (result > 0) {
                return JsonFormat(new ExtJson { success = true, msg = "保存验证码成功" });
            }
            return JsonFormat(new ExtJson { success = false, msg = "保存验证码失败" });
        }
        public ActionResult TakeMobileCode()
        {
            if (Request["mobile"] == null || Request["content"] == null)
                return JsonFormat(new ExtJson { success = false, msg = "参数不能为空" });
            string phone = Request["mobile"].ToString();        //哪个手机号码接收到的
            int type = Convert.ToInt32("1");                    //通过手机号码判断，1为登入2为充值
            if (phone == "10657532190000761") type = 2;
            string content = Request["content"];     //短信内容
            if (content.Length < 15) return JsonFormat(new ExtJson { success = false, msg = "太短了不用保存" });
            //登入部分还没有解决，就先放一下
            if (type == 1)
            {
                bll.SaveLoginState(phone, 0);
            }
            string code = bll.FilterMobileCode(phone, content);     //将短信里面的验证码解析出来
            string xh = bll.FilterMobileXh(phone, content);         //将短信里面的序列号解析出来
            if (!code.IsInt()) return JsonFormat(new ExtJson { success = false, msg = "验证码取值错误" });
            int clresult = -1;  //处理结果变量
            if (type == 2)
                clresult = mbll.OverCzWithMsgCode(phone, xh, code);
            int result = bll.TakeMsgCode(type, phone, xh, code, content);       //将收到的验证码保存
            if (result > 0)
            {
                return JsonFormat(new ExtJson { success = true, msg = "保存验证码成功" + "结果：" + clresult });
            }
            return JsonFormat(new ExtJson { success = false, msg = "保存验证码失败" + "结果：" + clresult });
        }
        /// <summary>
        /// 测试signatrue
        /// </summary>
        /// <returns></returns>
        public ActionResult ShareWeixi() {
            string access_token = wxll.Get_Access_Token(Wx_config.appid, Wx_config.appsecret);
            //string access_token = "Bgbznk2ods_Y_vfDikpMAd_cwbM2tsBAJgZAZQ2O0bEbmCn1Q9AZ8mBPCSalthgBrJP2wqb6AMbI4ZPx9j7qLV4MwhovhvXWUE37BCXRmXS5i0Ht6R-nKy8urDVuoQ4rYAQaAEAJLB";
            string jsapi_ticket = wxll.Get_Jsapi_Ticket(access_token);
            //string jsapi_ticket = "kgt8ON7yVITDhtdwci0qeWuN-3Mn_q_dPJGra0ooR2HLQNbupFT-S95u5DfpnCt2Q3PBr88Fy0wfZ3IHBaiybQ";
            //string test = Request["test"].ToString();
            long timestamp = DateTime.Now.ToUnixTimeStamp();        //时间戳
            string noncestr = TxtHelp.GetRandomString(16, true, true, true, false, "");
            string signatrue = wxll.Get_signature(timestamp, noncestr);
            
            return JsonFormat(new ExtJson { success = true, msg = jsapi_ticket });
        }
        //test
        public ActionResult SendLoginPost() {
            string url = "http://www.fj.10086.cn/power/ADCECPortal/PowerLogin.aspx?ReturnUrl=ADCQDLPortal&test=t";
            bll.SendLoginPost(url);
            return View();
        }
        public ActionResult TestShare() {
            string phone = Request["mobile"].ToString();        //哪个手机号码接收到的
            string content = Request["content"];     //短信内容
            string xh = bll.FilterMobileXh(phone, content);   //将短信里面的序列号解析出来
            return View();
        }
        /// <summary>
        /// 发送验证码
        /// </summary>
        /// <returns></returns>
        public ActionResult SendMobileCode() {
            //bll.SendLoginMsgCode(21, 1);
            mbll.HelpWebSend(21, 1);
            return View();
        }
        public ActionResult LoginByMobileCode()
        {
            bll.LoginByMobileCode(21, 1);
            //bll.HelpWebLogin(21, 1);
            return View();
        }



        public ActionResult Navigation() {
            return View();
        }
        public ActionResult CooperIndex() {
            return View();
        }
    }
}