using BLL;
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
using Model.ViewModel;
using Model.EnumModel;
using FJSZ.OA.Common.DEncrypt;

namespace WebNdQn.Controllers
{
    public class HomeController : BaseController
    {
        CommonBLL bll = new CommonBLL();
        WeiXinBLL wxll = new WeiXinBLL();
        MobileBLL mbll = new MobileBLL();
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
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/WxDefault_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "subscribe：" + dto2.subscribe + "openid: " + dto2.openid + "nickname:" + dto2.nickname + "headurl:" + dto2.headimgurl);
                string url = state.Replace("|", "&") + "&gzstate=" + dto2.subscribe + "&openid=" + dto2.openid;
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/WxDefault_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "url：" + url);
                return Redirect(url);
            }
            else {
                return Content("缺少配置");
            }
        }
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
        //关注公众号,弃用
        public ActionResult Default() {
            //if (Request["gzstate"] == null)
            //{
            //    if (Request["ctype"] == null || Request["issue"] == null)
            //    {
            //        return JsonFormat(new ExtJson { success = false, msg = "参数不能为空" });
            //    }
            //    string ctype = Request["ctype"].ToString(); string issue = Request["issue"].ToString();
            //    Common.Expend.LogTxtExpend.WriteLogs("/Logs/WxDefault_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "参数非空 ctype: " + ctype + "issue: " + issue);
            //    T_CooperConfig dto = wxll.Get_CooperConfig(Convert.ToInt32(ctype), Convert.ToInt32(issue));                              //取得配置
            //    if (dto != null)
            //    {
            //        Common.Expend.LogTxtExpend.WriteLogs("/Logs/WxDefault_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "gzstate为空");
            //        //snsapi_base,snsapi_userinfo
            //        string state = Request.Url.AbsoluteUri.Replace("&", "|");
            //        Common.Expend.LogTxtExpend.WriteLogs("/Logs/WxDefault_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "state的值： " + state);
            //        string url = wxll.Wx_Auth_Code(dto.wx_appid, "http://wx.ndll800.com/Home/Wx_Auth_Code", "snsapi_base", state);
            //        return Redirect(url);
            //    }
            //    else
            //    {
            //        Common.Expend.LogTxtExpend.WriteLogs("/Logs/WxDefault_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "未进行客户信息配置： ctype: " + ctype + "issue: " + issue);
            //        return JsonFormat(new ExtJson { success = false, msg = "配置为空" });
            //    }
            //}
            //else
            //{
            //    if (Request["ctype"] == null || Request["issue"] == null || Request["gzstate"] == null || Request["openid"] == null) {
            //        return JsonFormat(new ExtJson { success = false, msg = "参数不能为空" });
            //    }
            //    string ctype = Request["ctype"].ToString();
            //    string issue = Request["issue"].ToString();
            //    string gz = Request["gzstate"].ToString();
            //    string openid = Request["openid"].ToString();
            //    Common.Expend.LogTxtExpend.WriteLogs("/Logs/WxDefault_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "ctype：" + ctype+ "issue：" + issue + "gzstate：" + gz);
            //    T_CooperConfig dto = wxll.Get_CooperConfig(Convert.ToInt32(ctype), Convert.ToInt32(issue));                              //取得配置
            //    if (dto != null)
            //    {
            //        Common.Expend.LogTxtExpend.WriteLogs("/Logs/WxDefault_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "gzstate：" + gz);
            //        ViewBag.qrcode = dto.qrcode_url;
            //        ViewBag.Gz = gz;
            //        ViewBag.Openid = openid;
            //        if (!string.IsNullOrEmpty(dto.bgurl))
            //        {
            //            ViewBag.bg = dto.bgurl;             //背景图
            //            ViewBag.btn = dto.btnurl;           //背景按钮图
            //        }
            //    }
            //    else {
            //        return JsonFormat(new ExtJson { success = false, msg = "配置为空" });
            //    }
                
            //}
            return View();
        }
        public ActionResult SignPhoneFilter_gz() {
            if (Request["phone"] == null || Request["ctype"] == null || Request["issue"] == null || Request["openid"] == null || Request["area"] == null)
            {
                return JsonFormat(new ExtJson { success = false, msg = "参数不能为空!" });
            }
            string phone = Request["phone"].ToString();         //用户手机号码
            string ctype = Request["ctype"].ToString();         //公司类型
            string issue = Request["issue"].ToString();         //活动期号
            string openid = Request["openid"].ToString();       //微信的Openid
            string area = Request["area"].ToString();           //地区1宁德2莆田
            //string txtpath = "/Content/Txt/pwebconfig.txt";
            //if(area=="2") txtpath = "/Content/Txt/putianconfig.txt";
            string txtpath = bll.ReturnConfigTxt(area); //取得限制号码txt
            string path = Server.MapPath(txtpath);
            bool result = bll.ReadPhoneFliter(phone, path); //验证手机号码
            if (result)
            {
                int de_Openid = bll.DecideOpenid(openid, Convert.ToInt32(ctype), Convert.ToInt32(issue));   //Openid是否已经参加过活动,当前微信已经添加过活动!
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/SignPhoneFilter_gz_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "ctype:"+ ctype + "de_Openid: " + de_Openid+ "openid: "+ openid);
                if (de_Openid > 0) return JsonFormat(new ExtJson { success = false, msg = "您已参与，不可重复提交！" });
                int de_reslut = bll.DecidePhone(phone, Convert.ToInt32(ctype), Convert.ToInt32(issue));   //手机号码是否已经参加过活动
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/SignPhoneFilter_gz_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "ctype:" + ctype + "de_reslut: " + de_reslut);
                if (de_reslut > 0)
                {
                    return JsonFormat(new ExtJson { success = false, msg = "您已参与，不可重复提交！" });//当前手机号已经添加过活动
                }
                else
                {
                    T_CooperConfig dto = wxll.Get_CooperConfig(Convert.ToInt32(ctype), Convert.ToInt32(issue));                              //取得配置
                    if (dto != null)
                    {
                        int limitnum = dto.uplimit;     //活动人数上限
                        if (limitnum != 0 && bll.CtypeCount(Convert.ToInt32(ctype), Convert.ToInt32(issue)) >= limitnum)
                            return JsonFormat(new ExtJson { success = false, msg = "已经超过活动人数上限!" });
                        //状态修改为结束时
                        if (dto.state == 2)
                            return JsonFormat(new ExtJson { success = false, msg = "活动已结束，谢谢参与!" });
                    }
                    else
                        return JsonFormat(new ExtJson { success = false, msg = "活动已结束，谢谢参与!" });
                    int result_f = bll.TakeFlowLog(Convert.ToInt32(ctype), Convert.ToInt32(issue), phone, openid);
                    if (result_f == 1)
                        return JsonFormat(new ExtJson { success = true, msg = "记得分享给更多朋友领取流量!" });
                    else
                        return JsonFormat(new ExtJson { success = false, msg = "写入充值记录失败，请重新尝试!" });
                }
            }
            else
            {
                string arean = Enum.GetName(typeof(ConstDefine.AreaType), Convert.ToInt32(area));
                return JsonFormat(new ExtJson { success = false, msg = "本活动仅限"+ arean + "移动手机用户参与!" });
            }
        }
        //分享到朋友或者朋友圈
        public ActionResult Index()
        {
            if (Request["ctype"] == null || Request["issue"] == null)
                return JsonFormat(new ExtJson { success = false, msg = "参数不能为空" });
            string ctype = Request["ctype"].ToString(); string issue = Request["issue"].ToString();
            T_CooperConfig dto = wxll.Get_CooperConfig(Convert.ToInt32(ctype), Convert.ToInt32(issue));     //取得配置
            if (dto == null) 
                return Content("配置为空");
            string[] str = dto.gener.Split('|');    //分享推广|关注推广
            if (str.Length < 2)
                return Content("推广配置错误");
            if (Request["p"] == null)
            {
                string c = "&c="+DEncrypt.DESEncrypt1("CGI|1|" + WebHelp.GetCurHttpHost() + "/Home/Index");   //c参数进行加密
                string param = Request.Url.Query + c;   //参数串,例如:http://wx.ndll800.com/home/default?ctype=1&issue=1 取的param为:   ?ctype=1&issue=1
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/HomeController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "Index     param： " + param);
                string state = "";                  //state的值暂时为空,如果后面有需要验签,再用起来,现在就直接用参数来做校验
                string url = wxll.Wx_Auth_Code(dto.wx_appid, System.Web.HttpUtility.UrlEncode(WebHelp.GetCurHttpHost() + "/WeiX/Wx_Auth_Code" + param), "snsapi_userinfo", state);  //snsapi_base,snsapi_userinfo
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/HomeController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "Index     URL： " + url);
                return Redirect(url);
            }
            else {
                string gz = "0"; string openid = "";
                if (Request["p"] != null){
                    try
                    {
                        string p = Request["p"].ToString(); //1|subscribe|openid  微信发送|是否关注|openid
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/HomeController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "Index     p：" + Request["p"].ToString());
                        string temp = DEncrypt.DESDecrypt1(p);    //取得p参数,并且进行解密
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/HomeController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "Index     p：" + temp);
                        string[] plist = temp.Split('|');   //微信发送|是否
                        if (plist[0] != "1") return Content("配置参数异常");
                        gz = plist[1]; openid = plist[2];   //是否关注,微信用户id
                    }
                    catch {
                        return Content("参数错误");
                    }
                }
                if (string.IsNullOrEmpty(openid)) {
                    return Content("授权失败");
                }
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/HomeController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "Index     ctype：" + ctype + "issue：" + issue + "gzstate：" + gz);
                if (dto != null)
                {
                    ViewBag.cooperid = dto.id;  //
                    int lognum = bll.GetLogNum(dto.id); //第几位
                    V_IndexDto rdto = new V_IndexDto();
                    
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/HomeController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "Index     gzstate：" + gz);
                    //公共部分
                    if (!string.IsNullOrEmpty(dto.bgurl))
                    {
                        rdto.bgurl = dto.bgurl;             //背景图
                        rdto.btnurl = dto.btnurl;           //背景按钮图
                    }
                    
                    rdto.genner = dto.gener;                //推广类型 分享推广|关注推广
                    rdto.areatype = dto.areatype;           //1宁德地区2莆田地区       
                    //关注公众号部分
                    rdto.qrcodeurl = dto.qrcode_url;    //关注公众号的二维码图片地址
                    rdto.gz = gz;                       //微信用户是否关注过当前公众号
                    rdto.openid = openid;               //微信用户的openid

                    //分享部分
                    rdto.wx_appid = Wx_config.appid;    //微信公众号
                    long timestamp = DateTime.Now.ToUnixTimeStamp();                                        //时间戳
                    string noncestr = TxtHelp.GetRandomString(16, true, true, true, false, "");             //随机字符串
                    string signatrue = wxll.Get_signature(timestamp, noncestr);                             //signatrue
                    rdto.timestamp = timestamp; rdto.noncestr = noncestr; rdto.signatrue = signatrue;         //上面三个附值
                    string logstr = ""; if (!string.IsNullOrEmpty(dto.username)) { logstr = dto.username + lognum + "位"; }
                    rdto.fx_title = logstr + dto.title; rdto.fx_descride = dto.descride; rdto.fx_imgurl = WebHelp.GetCurHttpHost() + dto.imgurl; rdto.fx_linkurl = dto.linkurl;    //分享的标题,描述,小图标,链接
                    return View(rdto);
                }
                else
                {
                    return JsonFormat(new ExtJson { success = false, msg = "配置为空" });
                }
            }
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
            string code = bll.FilterMobileCode(phone, content);     //将短信里面的验证码解析出来
            string xh = bll.FilterMobileXh(phone, content);         //将短信里面的序列号解析出来
            return Content("成功");
        }
        /// <summary>
        /// 测试signatrue
        /// </summary>
        /// <returns></returns>
        //test
        public ActionResult SendLoginPost() {
            string name = Enum.GetName(typeof(ConstDefine.AwardType), 1);
            int value = System.EnumHelp.GetEnumValue(typeof(ConstDefine.AwardType), "乐豆");
            string des = System.EnumHelp.GetDescription(ConstDefine.AwardType.乐豆);

            return JsonFormat(new ExtJson { success = false, msg = name + "|" + value + "|" + des });
            //string url = "http://www.fj.10086.cn/power/ADCECPortal/PowerLogin.aspx?ReturnUrl=ADCQDLPortal&test=t";
            //bll.SendLoginPost(url);
            //return View();
        }

        public ActionResult Navigation() {
            return View();
        }
        public ActionResult CooperIndex() {
            return View();
        }
        /// <summary>
        /// 记录登入的用户数
        /// </summary>
        /// <returns></returns>
        public ActionResult LogNum() {
            string cooperid = Request["cooperid"];
            int result = bll.LogNum(Convert.ToInt32(cooperid));
            if (result > 0)
                return JsonFormat(new ExtJson { success = true, msg = "succeed" });
            else
                return JsonFormat(new ExtJson { success = false, msg = "faile" });
        }
    }
}