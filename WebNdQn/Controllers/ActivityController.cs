using BLL;
using Fgly.Common.Expand;
using FJSZ.OA.Common.Web;
using FrameWork;
using FrameWork.Common;
using Model.ViewModel;
using Model.WxModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebNdQn.Controllers
{
    public class ActivityController : BaseController
    {
        ActivityBLL Abll = new ActivityBLL();
        WeiXinBLL wxll = new WeiXinBLL();
        AdminBLL adbll = new AdminBLL();
        CommonBLL bll = new CommonBLL();
        /// <summary>
        /// 大转盘这里是入口/activity/index?ctype=1&issue=1
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            int ctype = 0, issue = 1;
            if (Request["ctype"] == null || Request["issue"] == null)
                return JsonFormat(new ExtJson { success = false, msg = "参数不能为空" });
            ctype = Convert.ToInt32(Request.QueryString["ctype"]);
            issue = Convert.ToInt32(Request.QueryString["issue"]);
            T_CooperConfig dto = wxll.Get_CooperConfig(ctype, issue);                              //取得配置
            if (dto != null) {
                string state = "";                                              //snsapi_base,snsapi_userinfo(需要用户确认登入)
                string backurl = System.Web.HttpUtility.UrlEncode("http://" + Request.Url.Host + "/Activity/Dzp?ctype=" + ctype + "&issue=" + issue);
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/ActivityController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "方法Index backurl :" + backurl);
                string url = wxll.Wx_Auth_Code(dto.wx_appid, backurl, "snsapi_base", state);
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/ActivityController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "方法Index url :" + url);
                return Redirect(url);
            }
            return View();
        }
        public ActionResult Dzp() {
            Common.Expend.LogTxtExpend.WriteLogs("/Logs/ActivityController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "Request.Url.AbsoluteUri :" + Request.Url.AbsoluteUri);
            int ctype = 0, issue = 1;
            if (Request["ctype"] == null|| Request["issue"] == null || Request["code"] == null || Request["state"] == null)
                return JsonFormat(new ExtJson { success = false, msg = "参数不能为空" });
            ctype = Convert.ToInt32(Request.QueryString["ctype"]);
            issue = Convert.ToInt32(Request.QueryString["issue"]);
            string code = Request["code"].ToString();
            string state = Request["state"].ToString();
            T_CooperConfig dto = wxll.Get_CooperConfig(ctype, issue);                              //取得配置
            if (dto == null)
                return Content("缺少配置");
            #region 获取微信用户的openid
            WxJsApi_token dto1 = wxll.Wx_Auth_AccessToken(dto.wx_appid, dto.wx_secret, code);
            if (dto1.openid == null)
                return Redirect("/Activity/Index?ctype=" + ctype + "&issue=" + issue);  //进行一次重新跳转
            //return JsonFormat(new ExtJson { success = false, msg = "微信用户信息不正确" });
            ViewBag.openid = dto1.openid;
            Common.Expend.LogTxtExpend.WriteLogs("/Logs/ActivityController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "dto1.openid :" + dto1.openid);
            //ViewBag.openid = "oIW7Uwk5tMFZ7aakoLLlPF4IOHkY";
            #endregion
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

                //ViewBag.title = dto.title;              //标题
                //ViewBag.desc = dto.descride;            //描述
                //ViewBag.imgurl = dto.imgurl;            //图片地址
                //ViewBag.linkurl = dto.linkurl;          //链接地址
            }
            #endregion
            #region 奖品的列表,当前用户还可摇奖次数,是否有手机号码
            if (dto != null) {
                ViewBag.cooperid = dto.id;                  //配置的ID号
                var list = Abll.GetProbNameData(dto.id);    //奖品列表保存成字典
                string namelist = "";
                foreach (var item in list)
                {
                    if (string.IsNullOrEmpty(namelist)) { namelist = item.Value; } else { namelist += "," + item.Value; }
                }
                ViewBag.namelist = namelist;              //奖品名称列表
                //取得当前用户还可摇几次，需要用到openid
                ViewBag.lotteyn = Abll.GetOpenidCount(dto.id, 1, ViewBag.openid);
                //手机号码
                ViewBag.curphone = Abll.GetActivityPhone(dto.id, 1, ViewBag.openid);
            }
            #endregion
            #region 大转盘的背景图和标题,活动说明
            if (dto != null)
            {
                string ptitle = "大转盘";string bgurl = "/Content/img/bg/body_bg1.jpg"; string explain = "暂时没有游戏说明";
                T_ActivityConfig dtoc = Abll.FindActivityConfigByCooperid(ViewBag.cooperid);
                if (dtoc != null)
                {
                    ptitle = dtoc.title;
                    bgurl = string.IsNullOrEmpty(dtoc.bgurl) == false ? bgurl = dtoc.bgurl : "";
                    explain = dtoc.explain.Replace("\n", "<br/>");
                    ViewBag.title = dtoc.wx_title;              //标题
                    ViewBag.desc = dtoc.wx_descride;            //描述
                    ViewBag.imgurl = WebHelp.GetCurHttpHost() + dtoc.wx_imgurl;            //图片地址
                    ViewBag.linkurl = dtoc.wx_linkurl;          //链接地址
                }
                ViewBag.ptitle = ptitle;ViewBag.bgurl = bgurl;ViewBag.explain = explain;
            }
            #endregion
            return View();
        }
        /// <summary>
        /// 验证手机号
        /// </summary>
        /// <returns></returns>
        public ActionResult SignPhoneArea() {
            if (Request["area"] == null || Request["phone"] == null) {
                return JsonFormat(new ExtJson { success = false, msg = "参数不能为空" });
            }
            string area = Request["area"].ToString();
            string phone = Request["phone"].ToString();
            //string txtpath = "/Content/Txt/pwebconfig.txt";
            //if (area == "2") txtpath = "/Content/Txt/putianconfig.txt";
            string txtpath = bll.ReturnConfigTxt(area); //取得限制号码txt
            string path = Server.MapPath(txtpath);
            bool result = bll.ReadPhoneFliter(phone, path); //验证手机号码
            return JsonFormat(new ExtJson { success = true, msg = "返回结果", jsonresult = result });
        }
        /// <summary>
        /// 摇奖
        /// </summary>
        /// <returns></returns>
        public ActionResult DzpProb() {
            int cooperid = 0;
            string openid = ""; string phone = "";string area = "";
            if (Request.Form["cooperid"] != null && Request.Form["openid"] != null && Request.Form["phone"] != null && Request.Form["area"] != null)
            {
                cooperid = Convert.ToInt32(Request.Form["cooperid"]);
                openid = Request.Form["openid"];
                phone = Request.Form["phone"];
                area = Request.Form["area"];
                //验证手机号码
                //string txtpath = "/Content/Txt/pwebconfig.txt";
                //if (area == "2") txtpath = "/Content/Txt/putianconfig.txt";
                string txtpath = bll.ReturnConfigTxt(area); //取得限制号码txt
                string path = Server.MapPath(txtpath);
                bool result = bll.ReadPhoneFliter(phone, path); //验证手机号码
                if (!result)
                    return JsonFormat(new ExtJson { success = false, code = -1000, msg = "手机号码不符合活动规则." });
                //验证摇奖次数
                int lotteyn = Abll.GetOpenidCount(cooperid, 1, openid);
                if (lotteyn < 1) 
                    return JsonFormat(new ExtJson { success = false, code = -1001, msg = "已无摇奖次数.", jsonresult = 0 });
                //查询配置
                T_ActivityConfig dto = Abll.FindActivityConfigByCooperid(cooperid);
                //奖品抽完
                var list = Abll.FindConfigList(dto.id);
                int pcount = 0, dcount = 0;
                foreach (var item in list) {
                    pcount += item.count;dcount += item.drowcount;
                }
                if (pcount == dcount)
                    return JsonFormat(new ExtJson { success = false, code = -1002, msg = "活动已结束.", jsonresult = 0 });
                //概率非等于0.99
                float f = Abll.GetWinProb(dto.id);
                if (f != Convert.ToSingle(0.99))
                    return JsonFormat(new ExtJson { success = false, code = -1003, msg = "参数配置错误,或者活动结束.", jsonresult = 0 });
                //取得摇奖结果
                int resultnum = Abll.Getprob(cooperid, openid, phone);
                return JsonFormat(new ExtJson { success = true, code = 1000, msg = "成功.", jsonresult = resultnum });
            }
            return JsonFormat(new ExtJson { success = false, code = -1004, msg = "失败.", jsonresult = 0 });
        }
        /// <summary>
        /// 取得当前活动的奖品记录
        /// </summary>
        /// <returns></returns>
        public ActionResult DzpDrawLog() {
            if (Request["cooperid"] == null || Request["openid"] == null) 
                return JsonFormat(new ExtJson { success = false, msg = "参数不能为空" });

            string cooperid = Request["cooperid"].ToString();       //cooperid
            string openid = Request["openid"].ToString();            //微信的openid
            var list = Abll.GetDrawList(Convert.ToInt32(cooperid), 1, openid);
            if (list.Count > 0) 
                return JsonFormat(new ExtJson { success = true, code = 1000, msg = "查询成功.", jsonresult = list });
            else
                return JsonFormat(new ExtJson { success = false, code = -1000, msg = "没有中奖记录.", jsonresult = null });
        }
        /// <summary>
        /// 更新大转盘中奖记录的手机号码
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateDzpPhone()
        {
            int cooperid = 0;
            string openid = "";
            string phone = "";
            if (Request.Form["cooperid"] != null && Request.Form["openid"] != null && Request.Form["phone"] != null)
            {
                cooperid = Convert.ToInt32(Request.Form["cooperid"]);
                openid = Request.Form["openid"];
                phone = Request.Form["phone"];
                int result = Abll.UpdateActivityPhone(cooperid, 1, openid, phone);
                if (result == 1) return JsonFormat(new ExtJson { success = true, code = 1000, msg = "成功." });
                else return JsonFormat(new ExtJson { success = false, code = -1000, msg = "失败." });
            }
            return JsonFormat(new ExtJson { success = false, code = -1000, msg = "失败." });
        }
        public ActionResult ActivityShare() {
            int cooperid = 0;string openid = ""; int sharetype = 0;
            if (Request.Form["cooperid"] != null && Request.Form["openid"] != null && Request.Form["sharetype"] != null)
            {
                cooperid = Convert.ToInt32(Request.Form["cooperid"]);
                openid = Request.Form["openid"];
                sharetype = Convert.ToInt32(Request.Form["sharetype"]);
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/ActivityController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "方法ActivityShare cooperid :" + cooperid+ " openid: "+ openid + " sharetype: " + sharetype);
                int result = Abll.ActivityeShare(cooperid, 1, openid, sharetype);
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/ActivityController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "方法ActivityShare result :" + result);
                if (result == 1) return JsonFormat(new ExtJson { success = true, code = 1000, msg = "成功." });
                else return JsonFormat(new ExtJson { success = false, code = -1000, msg = "失败." });
            }
            return JsonFormat(new ExtJson { success = false, code = -1000, msg = "失败." });
        }
        /// <summary>
        /// 设置大转盘的页面
        /// </summary>
        /// <param name="cooperid">T_CooperConfig的ID值</param>
        /// <returns></returns>
        public ActionResult SetDzpPortal(int cooperid) {
            if (cooperid != 0) {
                T_ActivityConfig dto = Abll.FindActivityConfigByCooperid(cooperid);
                ViewBag.cooperid = cooperid;
                return View(dto);
            }
            return View();
        }
        /// <summary>
        /// 更新背景图片
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateBgUrl() {
            int cooperid = Convert.ToInt32(Request.Form["cooperid"]);
            string url = Abll.DzpUploadBgUrl(cooperid, "myfile");
            if (!string.IsNullOrEmpty(url))
                return JsonFormat(new ExtJson { success = true, code = 1000, msg = "成功.", jsonresult = url });
            else
                return JsonFormat(new ExtJson { success = false, code = -1000, msg = "失败." });
        }
        //根据主表的ID，取得附表的列表
        public ActionResult ConfigListByCId() {
            if (Request.Form["configid"] != null)
            {
                int Configid = Convert.ToInt32(Request.Form["configid"]);
                var list = Abll.FindConfigList(Configid);
                return JsonFormat(new ExtJson { success = true, code = 1000, msg = "成功.", jsonresult = list });
            }
            return JsonFormat(new ExtJson { success = false, code = -1000, msg = "失败." });
        }
        public ActionResult SetDzpConfig() {
            string configid = Request.Form["configid"];             //会员人数
            string cooperid = Request.Form["cooperid"];             //会员人数
            string title = Request.Form["title"];                   //会员人数
            string share = Request.Form["share"];                   //会员人数
            string explain = Request.Form["explain"];               //会员人数
            string bgurl = Request.Form["bgurl"];                   //会员人数

            string wxtitle = Request.Form["wxtitle"];                   //会员人数
            string wxdescride = Request.Form["wxdescride"];                   //会员人数
            string wximgurl = Request.Form["wximgurl"];                   //会员人数
            string wxlinkurl = Request.Form["wxlinkurl"];                   //会员人数

            string list = Request.Form["list"];                     //会员人数
            IList<T_ActivityConfigList> Configlist = FrameWork.Common.SerializeJson<T_ActivityConfigList>.JSONStringToList(list);    //会员列表
            int result = Abll.SetDzpConfig(Convert.ToInt32(configid), Convert.ToInt32(cooperid), title, Convert.ToInt32(share), explain, bgurl, wxtitle, wxdescride, wximgurl, wxlinkurl, Configlist);
            if (result > 0)
                return JsonFormat(new ExtJson { success = true, code = 1000, msg = "成功.", jsonresult = list });
            else
                return JsonFormat(new ExtJson { success = false, code = -1000, msg = "失败." });
        }
        public ActionResult DzpPortal() {
            return View();
        }
        public ActionResult ActicityListSearch()
        {
            string name = Request["name"].ToString();       //用户手机号码
            string value = Request["value"].ToString();     //公司类型
            int pageIndex = Convert.ToInt32(Request["pageIndex"]);
            int pageSize = Convert.ToInt32(Request["pageSize"]);
            int Total = 0;

            var list = Abll.GetActivity_Page(1, name, value, pageSize, pageIndex, ref Total);
            if (list.Count > 0)
                return JsonFormat(new ExtJsonPage { success = true, code = 1000, msg = "查询成功", total = Total, list = list });
            else
                return JsonFormat(new ExtJsonPage { success = false, code = -1000, msg = "查询失败" });
        }
        public ActionResult RemoveActivity() {
            string data = Request.Form["data"];  //用户的IDS数组
            string type = Request.Form["type"];  //1大转盘,2在线答题
            IList<IdListDto> list = SerializeJson<IdListDto>.JSONStringToList(data);
            int result = Abll.RemoveActivitys(list, Convert.ToInt32(type));
            if (result == list.Count)
                return JsonFormat(new ExtJson { success = true, msg = "删除成功！共删除" + result });
            else
                return JsonFormat(new ExtJson { success = false, msg = "删除失败！共" + list.Count + " 成功" + result });
        }
        public ActionResult ActivityDrawPortal() {
            ViewBag.CooperDrop = bll.GetCooperConfigDrop(1);    //取得配置信息列表
            return View();
        }
        /// <summary>
        /// 查询活动中奖记录
        /// </summary>
        /// <returns></returns>
        public ActionResult FindActivityDrawSearch()
        {
            string cooperid = Request["cooperid"].ToString();     //cooperid
            string phone = Request["phone"].ToString();     //用户手机号码
            string state = Request["state"].ToString();    //状态
            
            var list = adbll.GetActivityDrawList_Search(Convert.ToInt32(cooperid), phone, Convert.ToInt32(state));
            return JsonFormat(new ExtJson { success = true, code = 1000, msg = "查询成功", jsonresult = list });
        }
        public ActionResult ActivitySharePortal() {
            ViewBag.CooperDrop = bll.GetCooperConfigDrop(1);    //取得配置信息列表
            return View();
        }
        public ActionResult FindActivityShareSearch() {
            string cooperid = Request["cooperid"].ToString();               //cooperid
            string atype = Request["atype"].ToString();                     //活动类型
            string sharetype = Request["sharetype"].ToString();             //分享类型
            var list = adbll.GetActivityShareList_Search(Convert.ToInt32(cooperid), Convert.ToInt32(atype), Convert.ToInt32(sharetype));
            return JsonFormat(new ExtJson { success = true, code = 1000, msg = "查询成功", jsonresult = list });
        }
    }
}