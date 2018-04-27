using BLL;
using Fgly.Common.Expand;
using FJSZ.OA.Common.DEncrypt;
using FJSZ.OA.Common.Web;
using FrameWork;
using FrameWork.Common;
using Model.ViewModel;
using Model.WxModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebNdQn.Controllers
{
    public class ZxdtController : BaseController
    {
        CommonBLL bll = new CommonBLL();
        WeiXinBLL wxll = new WeiXinBLL();
        ZxdtBLL zbll = new ZxdtBLL();
        ActivityBLL Abll = new ActivityBLL();
        /// <summary>
        /// 最新的
        /// </summary>
        /// <returns></returns>
        public ActionResult QaDefault() {
            if (Request["ctype"] == null || Request["issue"] == null)
                return Content("参数为空");
            string ctype = Request["ctype"].ToString();
            string issue = Request["issue"].ToString();
            T_CooperConfig dto = wxll.Get_CooperConfig(Convert.ToInt32(ctype), Convert.ToInt32(issue));                              //取得配置
            if (dto == null)
                return Content("配置为空");
            if (Request["p"] == null)
            {
                string c = "&c=" + DEncrypt.DESEncrypt1("CGI|1|" + WebHelp.GetCurHttpHost() + "/Zxdt/QaDefault");   //c参数进行加密
                string param = Request.Url.Query + c;   //参数串,例如:http://wx.ndll800.com/home/default?ctype=1&issue=1 取的param为:   ?ctype=1&issue=1
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/ZxdtController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "QaDefault     param： " + param);
                string state = "";                  //state的值暂时为空,如果后面有需要验签,再用起来,现在就直接用参数来做校验
                string url = wxll.Wx_Auth_Code(dto.wx_appid, System.Web.HttpUtility.UrlEncode(WebHelp.GetCurHttpHost() + "/WeiX/Wx_Auth_Code" + param), "snsapi_userinfo", state);  //snsapi_base,snsapi_userinfo
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/ZxdtController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "QaDefault     URL： " + url);
                return Redirect(url);
            }
            else {
                string gz = "0"; string openid = "";
                if (Request["p"] != null)
                {
                    try
                    {
                        string p = Request["p"].ToString(); //1|subscribe|openid  微信发送|是否关注|openid
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/ZxdtController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "Index     p：" + Request["p"].ToString());
                        string temp = DEncrypt.DESDecrypt1(p);    //取得p参数,并且进行解密
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/ZxdtController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "Index     p：" + temp);
                        string[] plist = temp.Split('|');   //微信发送|是否
                        if (plist[0] != "1") return Content("配置参数异常");
                        gz = plist[1]; openid = plist[2];   //是否关注,微信用户id
                    }
                    catch
                    {
                        return Content("参数错误");
                    }
                }
                if (string.IsNullOrEmpty(openid))
                    return Content("授权失败");
                #region 获取微信用户的openid
                ViewBag.openid = openid;
                //ViewBag.openid = "oIW7Uwk5tMFZ7aakoLLlPF4IOHkL";
                #endregion
                //cooperid
                ViewBag.cooperid = dto.id;
                //取得当前用户还可摇几次，需要用到openid
                ViewBag.lotteyn = Abll.GetOpenidCount(dto.id, 2, ViewBag.openid);
                //手机号码
                ViewBag.curphone = Abll.GetActivityPhone(dto.id, 2, ViewBag.openid);
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/ZxdtController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "Index     ctype：" + ctype + "issue：" + issue + "gzstate：" + gz);
                if (dto != null)
                {
                    ViewBag.appid = dto.wx_appid;
                    long timestamp = DateTime.Now.ToUnixTimeStamp();                                        //时间戳
                    string noncestr = TxtHelp.GetRandomString(16, true, true, true, false, "");             //随机字符串
                    string signatrue = wxll.Get_signature(timestamp, noncestr);                             //signatrue
                    //string signatrue = "1111";                             //signatrue
                    ViewBag.timestamp = timestamp;
                    ViewBag.noncestr = noncestr;
                    ViewBag.signatrue = signatrue;
                    ViewBag.RemoteIP = WebHelp.GetIp();     //用户IP
                    ViewBag.areatype = dto.areatype;        //1为宁德2为莆田


                    var dto_act = zbll.GetByCooperId(dto.id, 2);                //取得在线答题配置信息
                    if (dto_act == null)
                        return Content("在线答题配置为空");
                    //答题的分享部分
                    ViewBag.tmfs = dto_act.dt_fs;       //题目分数
                    ViewBag.sright = dto_act.sright;    //0显化答案(答对才继续)1不显化答案(答对错,都只能下一题)
                    ViewBag.Go = dto_act.sright == 0 ? 1 : 2;   //是否继续下一题,1只有答对才继续,2答错(不能修改了)也继续
                    ViewBag.random = dto_act.random;    //0随机1常规
                    ViewBag.WxTitle = dto_act.wx_title;
                    ViewBag.ShareImgPath = WebHelp.GetCurHttpHost() + dto_act.wx_imgurl;
                    ViewBag.ShareContent = dto_act.wx_descride;
                    ViewBag.ShareUrl = dto_act.wx_linkurl;
                    ViewBag.Tmts = dto_act.dt_tmts;         //题目条数

                    int userdcount = zbll.ZxdtDrawNumber(dto_act.cooperid);     //已经用掉的流量
                    if (dto_act.flowamount - userdcount < 100) {
                        ViewBag.activityEnd = "0";  //0活动结束
                        ViewBag.activityEndTips = "流量赠送完毕，活动结束，请下次再来参与！";  //活动结束提示语
                    }
                    ViewBag.flowamount = dto_act.flowamount;                    //流量池量
                    ViewBag.curflowcount = zbll.ZxdtDrawNumber(dto_act.cooperid);   //用户流量
                    
                    string explain = "暂时没有游戏说明"; string bgurl = "/Content/Img/bg/body_bg2.png";
                    bgurl = string.IsNullOrEmpty(dto_act.bgurl) == false ? bgurl = dto_act.bgurl : "";
                    explain = dto_act.explain.Replace("\n", "<br/>");
                    ViewBag.explain = explain;                  //答题的说明
                    ViewBag.ptitle = dto_act.title;             //页面的标题
                    ViewBag.bgurl = bgurl;                      //页面的背景

                    #region 取得题目列表b
                    var list = zbll.GetDttsTopic(dto.id, dto_act.dt_tmts, dto_act.random);

                    string str = ""; int index = 0;     //题目字符串，索引
                    foreach (var item in list)
                    {
                        string tipstr = "";
                        if (!string.IsNullOrEmpty(item.tips)) {
                            tipstr = "<i><span>" + item.tips + "</span></i>";
                        }
                        str += "<div class=\"page page_" + index + "\">";
                        str += "<div class=\"title\">"+ tipstr + "<em>" + index + "</em>、" + item.topic + "</div><ul>";
                        string[] sstr = item.answer.Split('|'); int temindex = 1;
                        foreach (var tem in sstr)
                        {
                            str += "<li class=\"" + Rzm1(temindex) + " " + Rzm3(item.keyanswer, temindex.ToString()) + "\" data-value=\"" + Rzm(temindex) + "\"><em>" + Rzm(temindex) + "</em>" + tem + "</li>";
                            temindex++;
                        }
                        str += "<div class=\"zqda\"><p>正确答案：<em>" + Rzm2(item.keyanswer) + "</em>我的答案：<i></i></p></div>";
                        str += "<a href=\"javascript:void(0)\" class=\"btn-00 btn-xyt\" style=\"background:url(../Content/Activity/Zxdt/kankan/content/20171102102845.png) no-repeat 0 0; background-size:100% 100%; \">下一题</a>";
                        str += "<div class=\"pic-01\"></div>";
                        str += "<span class=\"icon-right none\">对</span>";
                        str += "<span class=\"icon-wrong none\">错</span>";
                        str += "</ul></div>";
                        index++;
                    }
                    ViewBag.list = str;
                    #endregion 取得题目列表e

                }
            }


            return View();
        }
        private string Rzm(int num) {
            if (num == 1) return "A";
            else if (num == 2) return "B";
            else if (num == 3) return "C";
            else if (num == 4) return "D";
            else if (num == 5) return "E";
            else if (num == 6) return "F";
            else return "";
        }
        private string Rzm1(int num)
        {
            if (num == 1) return "a";
            else if (num == 2) return "b";
            else if (num == 3) return "c";
            else if (num == 4) return "d";
            else if (num == 5) return "e";
            else if (num == 6) return "f";
            else return "";
        }
        private string Rzm2(string keystr) {
            string result = "";
            string[] intstr = keystr.Split(',');
            foreach (var item in intstr)
            {
                string zm = Rzm(Convert.ToInt32(item));
                result += zm;
            }
            return result;
        }
        private string Rzm3(string keystr, string index)
        {
            string[] intstr = keystr.Split(',');
            if (intstr.Contains(index)) return "right";
            return "";
        }

        public ActionResult Last() {
            return View();
        }
        public ActionResult QA() {
            return View();
        }
        public ActionResult Default() {
            if (Request["ctype"] == null || Request["issue"] == null)
                return JsonFormat(new ExtJson { success = false, msg = "参数不能为空" });
            string ctype = Request["ctype"].ToString(); string issue = Request["issue"].ToString();
            T_CooperConfig dto = wxll.Get_CooperConfig(Convert.ToInt32(ctype), Convert.ToInt32(issue));     //取得配置
            if (dto == null)
                return Content("配置为空");
            if (Request["p"] == null)
            {
                string c = "&c=" + DEncrypt.DESEncrypt1("CGI|1|" + WebHelp.GetCurHttpHost() + "/Zxdt/Index");   //c参数进行加密
                string param = Request.Url.Query + c;   //参数串,例如:http://wx.ndll800.com/home/default?ctype=1&issue=1 取的param为:   ?ctype=1&issue=1
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/ZxdtController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "Index     param： " + param);
                string state = "";                  //state的值暂时为空,如果后面有需要验签,再用起来,现在就直接用参数来做校验
                string url = wxll.Wx_Auth_Code(dto.wx_appid, System.Web.HttpUtility.UrlEncode(WebHelp.GetCurHttpHost() + "/WeiX/Wx_Auth_Code" + param), "snsapi_userinfo", state);  //snsapi_base,snsapi_userinfo
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/ZxdtController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "Index     URL： " + url);
                return Redirect(url);
            }
            else
            {
                string gz = "0"; string openid = "";
                if (Request["p"] != null)
                {
                    try
                    {
                        string p = Request["p"].ToString(); //1|subscribe|openid  微信发送|是否关注|openid
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/ZxdtController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "Index     p：" + Request["p"].ToString());
                        string temp = DEncrypt.DESDecrypt1(p);    //取得p参数,并且进行解密
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/ZxdtController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "Index     p：" + temp);
                        string[] plist = temp.Split('|');   //微信发送|是否
                        if (plist[0] != "1") return Content("配置参数异常");
                        gz = plist[1]; openid = plist[2];   //是否关注,微信用户id
                    }
                    catch
                    {
                        return Content("参数错误");
                    }
                }
                if (string.IsNullOrEmpty(openid))
                    return Content("授权失败");
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/ZxdtController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "Index     ctype：" + ctype + "issue：" + issue + "gzstate：" + gz);
                #region 获取微信用户的openid
                ViewBag.openid = openid;
                //ViewBag.openid = "oIW7Uwk5tMFZ7aakoLLlPF4IOHkL";
                #endregion
                //cooperid
                ViewBag.cooperid = dto.id;
                //取得当前用户还可摇几次，需要用到openid
                ViewBag.lotteyn = Abll.GetOpenidCount(dto.id, 2, ViewBag.openid);
                //手机号码
                ViewBag.curphone = Abll.GetActivityPhone(dto.id, 2, ViewBag.openid);
                #region 分享到朋友 基础数据
                if (dto != null)
                {
                    long timestamp = DateTime.Now.ToUnixTimeStamp();                                        //时间戳
                    string noncestr = TxtHelp.GetRandomString(16, true, true, true, false, "");             //随机字符串
                    string signatrue = wxll.Get_signature(timestamp, noncestr);                             //signatrue
                    ViewBag.appid = Wx_config.appid;        //分享到朋友，这里的appid用的是自己公司的，域名只能自己操作
                    ViewBag.timestamp = timestamp;
                    ViewBag.noncestr = noncestr;
                    ViewBag.signatrue = signatrue;
                    ViewBag.areatype = dto.areatype;        //1为宁德2为莆田
                }
                #endregion

                var dto_act = zbll.GetByCooperId(dto.id, 2);                //取得在线答题配置信息
                if (dto_act == null)
                    return Content("在线答题配置为空");
                string ptitle = "在线答题"; string bgurl = "/Content/Img/bg/body_bg2.png"; string explain = "暂时没有游戏说明";
                ptitle = dto_act.title; bgurl = string.IsNullOrEmpty(dto_act.bgurl) == false ? bgurl = dto_act.bgurl : ""; explain = dto_act.explain.Replace("\n", "<br/>");
                ViewBag.ptitle = ptitle;            //页面的标题
                ViewBag.bgurl = bgurl;              //页面的背景
                ViewBag.explain = explain;          //答题的说明
                ViewBag.score = dto_act.dt_fs;      //每个题目的分数
                ViewBag.sright = dto_act.sright;    //是否显化答案
                ViewBag.flowamount = dto_act.flowamount;    //流量池量
                ViewBag.curflowcount = zbll.ZxdtDrawNumber(dto_act.cooperid);   //用户流量
                #region 分享到朋友 具体数据
                ViewBag.title = dto_act.wx_title;              //标题
                ViewBag.desc = dto_act.wx_descride;            //描述
                ViewBag.imgurl = WebHelp.GetCurHttpHost() + dto_act.wx_imgurl;            //图片地址
                ViewBag.linkurl = dto_act.wx_linkurl;          //链接地址
                #endregion

                #region 取得题目列表b
                var list = zbll.GetDttsTopic(dto.id, dto_act.dt_tmts);
                string str = "";int index = 1; string cardstr = "";     //题目字符串，索引，card样式字符串
                foreach (var item in list)
                {
                    index++;
                    if (index < 4) {
                        cardstr = " card" + index;
                    }
                    str += "<div class=\"card_cont" + cardstr + "\">";
                    str += "<div class=\"card\">";
                    str += "<p class=\"question\"><span>Q" + index + "、</span>" + item.topic + "</p>";
                    str += "<ul class=\"select\">";
                    string[] sstr = item.answer.Split('|');
                    int temindex = 1;string checktype = "checkbox";
                    foreach (var tem in sstr) {
                        temindex++;
                        if (item.checkbox == 1) {
                            checktype = "radio";
                        }
                        str += "<li><input id=\"q" + index + "_" + temindex + "\" type=\"" + checktype + "\" name=\"r - group - " + index + "\" ><label for=\"q" + index + "_" + temindex + "\">" + tem + "</label></li>";
                    }
                    if(index==1)
                        str += "</ul><div class=\"card_bottom\"><a class=\"next\">下一题</a><span><b>" + index + "</b>/" + dto_act.dt_tmts + "</span></div></div></div>";
                    if(index!= dto_act.dt_tmts)
                        str += "</ul><div class=\"card_bottom\"><a class=\"prev\">上一题</a><a class=\"next\">下一题</a><span><b>" + index + "</b>/" + dto_act.dt_tmts + "</span></div></div></div>";
                    if (index == dto_act.dt_tmts)
                        str += "</ul><div class=\"card_bottom\"><a class=\"prev\">上一题</a><a class=\"ok\">完成</a><span><b>" + index + "</b>/" + dto_act.dt_tmts + "</span></div></div></div>";
                    //List<questions> qdtolist = new List<questions>();
                    //questions qdto;
                    //qdto = new questions();
                    //qdto.question = item.topic;
                    //string[] sstr = item.answer.Split('|');
                    //qdto.answers = sstr;
                    //qdto.correctAnswer = item.keyanswer;
                    //qdtolist.Add(qdto);
                }
                //string json = JsonConvert.SerializeObject(qdtolist);
                ViewBag.json = str;
                #endregion 取得题目列表e
            }
            return View();
        }
        /// <summary>
        /// 展示给客户的页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (Request["ctype"] == null || Request["issue"] == null)
                return JsonFormat(new ExtJson { success = false, msg = "参数不能为空" });
            string ctype = Request["ctype"].ToString(); string issue = Request["issue"].ToString();
            T_CooperConfig dto = wxll.Get_CooperConfig(Convert.ToInt32(ctype), Convert.ToInt32(issue));     //取得配置
            if (dto == null)
                return Content("配置为空");
            if (Request["p"] == null)
            {
                string c = "&c=" + DEncrypt.DESEncrypt1("CGI|1|" + WebHelp.GetCurHttpHost() + "/Zxdt/Index");   //c参数进行加密
                string param = Request.Url.Query + c;   //参数串,例如:http://wx.ndll800.com/home/default?ctype=1&issue=1 取的param为:   ?ctype=1&issue=1
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/ZxdtController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "Index     param： " + param);
                string state = "";                  //state的值暂时为空,如果后面有需要验签,再用起来,现在就直接用参数来做校验
                string url = wxll.Wx_Auth_Code(dto.wx_appid, System.Web.HttpUtility.UrlEncode(WebHelp.GetCurHttpHost() + "/WeiX/Wx_Auth_Code" + param), "snsapi_userinfo", state);  //snsapi_base,snsapi_userinfo
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/ZxdtController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "Index     URL： " + url);
                return Redirect(url);
            }
            else
            {
                string gz = "0"; string openid = "";
                if (Request["p"] != null)
                {
                    try
                    {
                        string p = Request["p"].ToString(); //1|subscribe|openid  微信发送|是否关注|openid
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/ZxdtController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "Index     p：" + Request["p"].ToString());
                        string temp = DEncrypt.DESDecrypt1(p);    //取得p参数,并且进行解密
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/ZxdtController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "Index     p：" + temp);
                        string[] plist = temp.Split('|');   //微信发送|是否
                        if (plist[0] != "1") return Content("配置参数异常");
                        gz = plist[1]; openid = plist[2];   //是否关注,微信用户id
                    }
                    catch
                    {
                        return Content("参数错误");
                    }
                }
                if (string.IsNullOrEmpty(openid))
                    //    return Content("授权失败");
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/ZxdtController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "Index     ctype：" + ctype + "issue：" + issue + "gzstate：" + gz);
                #region 获取微信用户的openid
                ViewBag.openid = openid;
                //ViewBag.openid = "oIW7Uwk5tMFZ7aakoLLlPF4IOHkL";
                #endregion
                //cooperid
                ViewBag.cooperid = dto.id;
                //取得当前用户还可摇几次，需要用到openid
                ViewBag.lotteyn = Abll.GetOpenidCount(dto.id, 2, ViewBag.openid);
                //手机号码
                ViewBag.curphone = Abll.GetActivityPhone(dto.id, 2, ViewBag.openid);
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/ZxdtController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "Index     cooperid：" + dto.id + "lotteyn：" + ViewBag.lotteyn + "curphone：" + ViewBag.curphone);
                #region 分享到朋友 基础数据
                if (dto != null)
                {
                    long timestamp = DateTime.Now.ToUnixTimeStamp();                                        //时间戳
                    string noncestr = TxtHelp.GetRandomString(16, true, true, true, false, "");             //随机字符串
                    string signatrue = wxll.Get_signature(timestamp, noncestr);                             //signatrue
                    ViewBag.appid = Wx_config.appid;        //分享到朋友，这里的appid用的是自己公司的，域名只能自己操作
                    ViewBag.timestamp = timestamp;
                    ViewBag.noncestr = noncestr;
                    ViewBag.signatrue = signatrue;
                    ViewBag.areatype = dto.areatype;        //1为宁德2为莆田
                }
                #endregion

                var dto_act = zbll.GetByCooperId(dto.id, 2);                //取得在线答题配置信息
                if (dto_act == null) 
                    return Content("在线答题配置为空");
                string ptitle = "在线答题"; string bgurl = "/Content/Img/bg/body_bg2.png"; string explain = "暂时没有游戏说明";
                ptitle = dto_act.title; bgurl = string.IsNullOrEmpty(dto_act.bgurl) == false ? bgurl = dto_act.bgurl : ""; explain = dto_act.explain.Replace("\n", "<br/>");
                ViewBag.ptitle = ptitle;            //页面的标题
                ViewBag.bgurl = bgurl;              //页面的背景
                ViewBag.explain = explain;          //答题的说明
                ViewBag.score = dto_act.dt_fs;      //每个题目的分数
                ViewBag.sright = dto_act.sright;    //是否显化答案
                ViewBag.flowamount = dto_act.flowamount;    //流量池量
                ViewBag.curflowcount = zbll.ZxdtDrawNumber(dto_act.cooperid);   //用户流量
                #region 分享到朋友 具体数据
                ViewBag.title = dto_act.wx_title;              //标题
                ViewBag.desc = dto_act.wx_descride;            //描述
                ViewBag.imgurl = WebHelp.GetCurHttpHost() + dto_act.wx_imgurl;            //图片地址
                ViewBag.linkurl = dto_act.wx_linkurl;          //链接地址
                #endregion

                #region 取得题目列表b
                var list = zbll.GetDttsTopic(dto.id, dto_act.dt_tmts);
                List<questions> qdtolist = new List<questions>();
                questions qdto;
                foreach (var item in list)
                {
                    qdto = new questions();
                    qdto.question = item.topic;
                    string[] sstr = item.answer.Split('|');
                    qdto.answers = sstr;
                    qdto.correctAnswer = item.keyanswer;
                    qdtolist.Add(qdto);
                }
                string json = JsonConvert.SerializeObject(qdtolist);
                ViewBag.json = json;
                #endregion 取得题目列表e
            }
            return View();
        }
        public ActionResult SubmitZxdt() {
            int cooperid = 0, score = 0; string openid = ""; string phone = ""; string area = "";
            if (Request.Form["cooperid"] != null && Request.Form["openid"] != null && Request.Form["phone"] != null && Request.Form["area"] != null && Request.Form["score"] != null)
            {
                cooperid = Convert.ToInt32(Request.Form["cooperid"]);
                openid = Request.Form["openid"];
                phone = Request.Form["phone"];
                area = Request.Form["area"];
                score = Convert.ToInt32(Request.Form["score"]);
                //验证手机号码
                //string txtpath = "/Content/Txt/pwebconfig.txt";
                //if (area == "2") txtpath = "/Content/Txt/putianconfig.txt";
                string txtpath = bll.ReturnConfigTxt(area); //取得限制号码txt
                string path = Server.MapPath(txtpath);
                bool result = bll.ReadPhoneFliter(phone, path); //验证手机号码
                if (!result)
                    return JsonFormat(new ExtJson { success = false, code = -1000, msg = "手机号码不符合活动规则." });
                //验证摇奖次数,2可答题次数
                int lotteyn = Abll.GetOpenidCount(cooperid, 2, openid);
                if (lotteyn < 1)
                    return JsonFormat(new ExtJson { success = false, code = -1000, msg = "已无摇奖次数.", jsonresult = 0 });
                //提交答题的分数
                int resultnum = zbll.SubmitZxdt(cooperid, openid, phone, score);
                if (resultnum > 0)
                    return JsonFormat(new ExtJson { success = true, code = 1000, msg = "成功.", jsonresult = resultnum });
                else if (resultnum == -2) 
                    return JsonFormat(new ExtJson { success = false, code = -1000, msg = "活动已经结束.", jsonresult = 0 });
                return JsonFormat(new ExtJson { success = false, code = -1000, msg = "提交失败.", jsonresult = 0 });
            }
            return JsonFormat(new ExtJson { success = false, code = -1000, msg = "失败.", jsonresult = 0 });
        }
        /// <summary>
        /// 在线答题分享
        /// </summary>
        /// <returns></returns>
        public ActionResult ZxdtShare()
        {
            int cooperid = 0; string openid = ""; int sharetype = 0;
            if (Request.Form["cooperid"] != null && Request.Form["openid"] != null && Request.Form["sharetype"] != null)
            {
                cooperid = Convert.ToInt32(Request.Form["cooperid"]);
                openid = Request.Form["openid"];
                sharetype = Convert.ToInt32(Request.Form["sharetype"]);
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/ZxdtController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "方法ZxdtShare cooperid :" + cooperid + " openid: " + openid + " sharetype: " + sharetype);
                int result = Abll.ActivityeShare(cooperid, 2, openid, sharetype);
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/ZxdtController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "方法ZxdtShare result :" + result);
                if (result == 1) return JsonFormat(new ExtJson { success = true, code = 1000, msg = "成功." });
                else return JsonFormat(new ExtJson { success = false, code = -1000, msg = "失败." });
            }
            return JsonFormat(new ExtJson { success = false, code = -1000, msg = "失败." });
        }
        /// <summary>
        /// 查询答题记录
        /// </summary>
        /// <returns></returns>
        public ActionResult ZxdtDrawLog() {
            if (Request["cooperid"] == null || Request["openid"] == null)
                return JsonFormat(new ExtJson { success = false, msg = "参数不能为空" });

            string cooperid = Request["cooperid"].ToString();       //cooperid
            string openid = Request["openid"].ToString();            //微信的openid
            var list = zbll.GetDrawList(Convert.ToInt32(cooperid), 2, openid);
            if (list.Count > 0)
                return JsonFormat(new ExtJson { success = true, code = 1000, msg = "查询成功.", jsonresult = list });
            else
                return JsonFormat(new ExtJson { success = false, code = -1000, msg = "没有中奖记录.", jsonresult = null });
        }
        #region 题库部分
        /// <summary>
        /// 后台查询题库页面
        /// </summary>
        /// <returns></returns>
        public ActionResult TopicPortal() {
            ViewBag.CooperDrop = bll.GetCooperConfigDrop(1);    //取得配置信息列表
            return View();
        }
        /// <summary>
        /// 后台设置题库页面
        /// </summary>
        /// <returns></returns>
        public ActionResult SetTopicPortal() {
            T_TopicBank dto = new T_TopicBank();
            if (Request["id"] != null)
            {
                int id = Convert.ToInt32(Request["id"]);
                dto = zbll.GetTopicById(id);
            }
            //新增的时候需要cooperid值
            if (Request["cooperid"] != null) {
                dto.cooperid = Convert.ToInt32(Request["cooperid"]);
            }
            return View(dto);
        }
        /// <summary>
        /// 后台题库翻页列表查询
        /// </summary>
        /// <returns></returns>
        public ActionResult TopicListPage()
        {
            string id = Request["id"].ToString();           //用户手机号码
            string title = Request["title"].ToString();     //公司类型
            int pageIndex = Convert.ToInt32(Request["pageIndex"]);
            int pageSize = Convert.ToInt32(Request["pageSize"]);
            int Total = 0;
            var list = zbll.FindTopicList_Page(Convert.ToInt32(id), title, pageSize, pageIndex, ref Total);
            if (list.Count > 0)
                return JsonFormat(new ExtJsonPage { success = true, code = 1000, msg = "查询成功", total = Total, list = list });
            else
                return JsonFormat(new ExtJsonPage { success = false, code = -1000, msg = "查询失败" });
        }
        /// <summary>
        /// 后台设置题库方法
        /// </summary>
        /// <returns></returns>
        public ActionResult SetZxdtTopic() {
            int id = Convert.ToInt32(Request.Form["id"]);                       //id,如果没有为0是新增
            int cooperid = Convert.ToInt32(Request.Form["cooperid"]);           //cooperid
            string topic = Request.Form["topic"];                               //题目
            int checkbox = Convert.ToInt32(Request.Form["checkbox"]);           //是否为多选
            string answer = Request.Form["answer"];                             //答案列表
            string keyanswer = Request.Form["keyanswer"];                       //答案值
            string tips = Request.Form["tips"];                                 //提示语
            int result = zbll.SetZxdtTopic(id, cooperid, checkbox, topic, answer, keyanswer, tips);
            if (result > 0)
                return JsonFormat(new ExtJson { success = true, code = 1000, msg = "操作成功." });
            else
                return JsonFormat(new ExtJson { success = false, code = -1000, msg = "操作失败." });
        }
        /// <summary>
        /// 后台删除题库方法
        /// </summary>
        /// <returns></returns>
        public ActionResult RemoveTopic() {
            string data = Request.Form["data"];  //用户的IDS数组
            IList<IdListDto> list = SerializeJson<IdListDto>.JSONStringToList(data);
            int result = zbll.RemoveTopics(list);
            if (result == list.Count)
                return JsonFormat(new ExtJson { success = true, msg = "删除成功！共删除" + result });
            else
                return JsonFormat(new ExtJson { success = false, msg = "删除失败！共" + list.Count + " 成功" + result });
        }
        #endregion

        //后台设置在线答题页面
        public ActionResult SetZxdtPortal() {
            T_ActivityConfig dto = new T_ActivityConfig();
            if (Request["cooperid"] != null)
            {
                int cooperid = Convert.ToInt32(Request["cooperid"]);
                dto = zbll.GetByCooperId(cooperid, 2); //取得在线答题配置信息
                ViewBag.cooperid = cooperid;
                var list = zbll.GetZxdtScore(dto.id);
                ViewBag.list = list;
            }
            return View(dto);
        }
        //后台查询在线答题页面
        public ActionResult ZxdtPortal() {
            if (!bll.VerSession()) Response.Redirect("/Login/index");
            return View();
        }
        //后台提交设置在线答题方法
        public ActionResult SetZxdtConfig() {
            string configid = Request.Form["configid"];             //配置ID,0新增,其他更新
            string cooperid = Request.Form["cooperid"];             //客户的ID号
            string title = Request.Form["title"];                   //页面的title
            string share = Request.Form["share"];                   //分享后增加几次机会
            string explain = Request.Form["explain"];               //说明:活动说明,游戏规则等等
            string bgurl = Request.Form["bgurl"];                   //背景图片

            string wxtitle = Request.Form["wxtitle"];               //微信分享时显示的标题
            string wxdescride = Request.Form["wxdescride"];         //微信分享时显示的描述
            string wximgurl = Request.Form["wximgurl"];             //微信分享时小图标的地址
            string wxlinkurl = Request.Form["wxlinkurl"];           //微信分享时的链接地址
            string tmfs = Request.Form["tmfs"];                     //每题的分数
            string tmts = Request.Form["tmts"];                     //随机抽取题库的条数
            string sright = Request.Form["sright"];                 //是否显化答案
            string random = Request.Form["random"];                 //0随机1常规排序显示
            string flowamount = Request.Form["flowamount"];         //
            string list = Request.Form["list"];                     //流量配置列表

            IList<T_ZxdtScore> Configlist = FrameWork.Common.SerializeJson<T_ZxdtScore>.JSONStringToList(list);    //流量配置列表
            int result = zbll.SetZxdtConfig(Convert.ToInt32(configid), Convert.ToInt32(cooperid), title, Convert.ToInt32(share), explain, bgurl, wxtitle, wxdescride, wximgurl, wxlinkurl, Convert.ToInt32(tmfs), Convert.ToInt32(tmts), Convert.ToInt32(sright), Convert.ToInt32(flowamount), Configlist,Convert.ToInt32(random));
            if (result > 0)
                return JsonFormat(new ExtJson { success = true, code = 1000, msg = "成功." });
            else if (result == -2) {
                return JsonFormat(new ExtJson { success = false, code = -1002, msg = "失败,该配置已存在." });
            }else
                return JsonFormat(new ExtJson { success = false, code = -1000, msg = "失败." });
        }
        //后台查询在线答题的方法
        public ActionResult ZxdtListSearch()
        {
            string name = Request["name"].ToString();       //用户手机号码
            string value = Request["value"].ToString();     //公司类型
            int pageIndex = Convert.ToInt32(Request["pageIndex"]);
            int pageSize = Convert.ToInt32(Request["pageSize"]);
            int Total = 0;

            var list = zbll.GetActivity_Page(2, name, value, pageSize, pageIndex, ref Total);
            if (list.Count > 0)
                return JsonFormat(new ExtJsonPage { success = true, code = 1000, msg = "查询成功", total = Total, list = list });
            else
                return JsonFormat(new ExtJsonPage { success = false, code = -1000, msg = "查询失败" });
        }
        //查询在线答题的答题记录
        public ActionResult ZxdtDrawPortal() {
            ViewBag.CooperDrop = bll.GetCooperConfigDrop(1);    //取得配置信息列表
            return View();
        }
        //查询在线答题的方法
        public ActionResult ZxdtDrawSearch() {
            string cooperid = Request["cooperid"].ToString();     //cooperid
            string phone = Request["phone"].ToString();     //用户手机号码
            string state = Request["state"].ToString();    //状态

            var list = zbll.ZxdtDrawList_Search(Convert.ToInt32(cooperid), phone, Convert.ToInt32(state));
            return JsonFormat(new ExtJson { success = true, code = 1000, msg = "查询成功", jsonresult = list });
        }
    }
    public class questions {
        public string question { get; set; }
        public string[] answers { get; set; }
        public string correctAnswer { get; set; }
    }
}