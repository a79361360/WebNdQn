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
                    return Content("授权失败");
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/ZxdtController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "Index     ctype：" + ctype + "issue：" + issue + "gzstate：" + gz);
                #region 获取微信用户的openid
                ViewBag.openid = openid;
                #endregion
                ViewBag.cooperid = dto.id;
                //取得当前用户还可摇几次，需要用到openid
                ViewBag.lotteyn = Abll.GetOpenidCount(dto.id, 2, ViewBag.openid);
                //手机号码
                ViewBag.curphone = Abll.GetActivityPhone(dto.id, 2, ViewBag.openid);
                #region 分享到朋友
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
                ViewBag.score = dto_act.dt_fs;      //每个题目的分数
                ViewBag.explain = dto_act.explain;  //答题的说明
                #region 取得题目列表b
                var list = zbll.GetDttsTopic(39, dto_act.dt_tmts);
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
            int cooperid = 0; string openid = ""; string phone = ""; string area = "";
            if (Request.Form["cooperid"] != null && Request.Form["openid"] != null && Request.Form["phone"] != null && Request.Form["area"] != null)
            {
                cooperid = Convert.ToInt32(Request.Form["cooperid"]);
                openid = Request.Form["openid"];
                phone = Request.Form["phone"];
                area = Request.Form["area"];
                //验证手机号码
                string txtpath = "/Content/Txt/pwebconfig.txt";
                if (area == "2") txtpath = "/Content/Txt/putianconfig.txt";
                string path = Server.MapPath(txtpath);
                bool result = bll.ReadPhoneFliter(phone, path); //验证手机号码
                if (!result)
                    return JsonFormat(new ExtJson { success = false, code = -1000, msg = "手机号码不符合活动规则." });
                //验证摇奖次数,2可答题次数
                int lotteyn = Abll.GetOpenidCount(cooperid, 2, openid);
                if (lotteyn < 1)
                    return JsonFormat(new ExtJson { success = false, code = -1000, msg = "已无摇奖次数.", jsonresult = 0 });
                //查询配置
                T_ActivityConfig dto = Abll.FindActivityConfigByCooperid(cooperid);
                float f = Abll.GetWinProb(dto.id);
                if (f != Convert.ToSingle(0.99))
                    return JsonFormat(new ExtJson { success = false, code = -1000, msg = "参数配置错误.", jsonresult = 0 });
                //取得摇奖结果
                int resultnum = Abll.Getprob(cooperid, openid, phone);
                return JsonFormat(new ExtJson { success = true, code = 1000, msg = "成功.", jsonresult = resultnum });
            }
            return JsonFormat(new ExtJson { success = false, code = -1000, msg = "失败.", jsonresult = 0 });
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
            int id = Convert.ToInt32(Request.Form["id"]);                //id,如果没有为0是新增
            int cooperid = Convert.ToInt32(Request.Form["cooperid"]);           //cooperid
            string topic = Request.Form["topic"];                               //题目
            string answer = Request.Form["answer"];                             //答案列表
            int keyanswer = Convert.ToInt32(Request.Form["keyanswer"]);         //答案值
            int result = zbll.SetZxdtTopic(id, cooperid, topic, answer, keyanswer);
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
            return View();
        }
        //后台提交在线答题方法
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
            string list = Request.Form["list"];                     //流量配置列表

            IList<T_ZxdtScore> Configlist = FrameWork.Common.SerializeJson<T_ZxdtScore>.JSONStringToList(list);    //流量配置列表
            int result = zbll.SetZxdtConfig(Convert.ToInt32(configid), Convert.ToInt32(cooperid), title, Convert.ToInt32(share), explain, bgurl, wxtitle, wxdescride, wximgurl, wxlinkurl, Convert.ToInt32(tmfs), Convert.ToInt32(tmts), Configlist);
            if (result > 0)
                return JsonFormat(new ExtJson { success = true, code = 1000, msg = "成功." });
            else if (result == -2) {
                return JsonFormat(new ExtJson { success = false, code = -1002, msg = "失败,该配置已存在." });
            }else
                return JsonFormat(new ExtJson { success = false, code = -1000, msg = "失败." });
        }
        //后台列表查询方法
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
        
    }
    public class questions {
        public string question { get; set; }
        public string[] answers { get; set; }
        public int correctAnswer { get; set; }
    }
}