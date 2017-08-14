using BLL;
using Fgly.Common.Expand;
using FrameWork;
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
        // GET: Activity
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
                string url = wxll.Wx_Auth_Code(dto.wx_appid, "http://wx.ndll800.com/Activity/Dzp?ctype=" + ctype + "&issue=" + issue, "snsapi_base", state);
                return Redirect(url);
            }
            return View();
        }
        public ActionResult Dzp() {
            int ctype = 0, issue = 1;
            if (Request["ctype"] == null || Request["issue"] == null|| Request["code"] == null || Request["state"] == null)
                return JsonFormat(new ExtJson { success = false, msg = "参数不能为空" });
            ctype = Convert.ToInt32(Request.QueryString["ctype"]);
            issue = Convert.ToInt32(Request.QueryString["issue"]);
            string code = Request["code"].ToString();
            string state = Request["state"].ToString();
            T_CooperConfig dto = wxll.Get_CooperConfig(ctype, issue);                              //取得配置
            if (dto == null)
                return JsonFormat(new ExtJson { success = false, msg = "缺少配置" });
            #region 获取微信用户的openid
            //WxJsApi_token dto1 = wxll.Wx_Auth_AccessToken(dto.wx_appid, dto.wx_secret, code);
            //if (dto1 == null)
            //    return JsonFormat(new ExtJson { success = false, msg = "微信用户信息不正确" });
            //ViewBag.openid = dto1.openid;
            ViewBag.openid = "oIW7Uwk5tMFZ7aakoLLlPF4IOHkY";
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

                ViewBag.title = dto.title;              //标题
                ViewBag.desc = dto.descride;            //描述
                ViewBag.imgurl = dto.imgurl;            //图片地址
                ViewBag.linkurl = dto.linkurl;          //链接地址
            }
            #endregion
            #region 奖品的列表,当前用户还可摇奖次数
            if (dto != null) {
                ViewBag.cooperid = dto.id;                  //配置的ID号
                var list = Abll.GetProbNameData(dto.id);    //奖品列表保存成字典
                string namelist = "";
                foreach (var item in list)
                {
                    if (string.IsNullOrEmpty(namelist)) { namelist = item.Key; } else { namelist += "," + item.Key; }
                }
                ViewBag.namelist = namelist;              //奖品名称列表
                //取得当前用户还可摇几次，需要用到openid
                ViewBag.lotteyn = Abll.GetOpenidCount(dto.id, 1, ViewBag.openid);
            }
            #endregion
            return View();
        }
        /// <summary>
        /// 摇奖
        /// </summary>
        /// <returns></returns>
        public ActionResult DzpProb() {
            int cooperid = 0;
            string openid = "";
            if (Request.Form["cooperid"] != null && Request.Form["openid"] != null)
            {
                cooperid = Convert.ToInt32(Request.Form["cooperid"]);
                openid = Request.Form["openid"];
                int lotteyn = Abll.GetOpenidCount(cooperid, 1, openid);
                if (lotteyn < 1) 
                    return JsonFormat(new ExtJson { success = false, code = -1000, msg = "已无摇奖次数.", jsonresult = 0 });
                int resultnum = Abll.Getprob(cooperid, openid);
                return JsonFormat(new ExtJson { success = true, code = 1000, msg = "成功.", jsonresult = resultnum });
            }
            return JsonFormat(new ExtJson { success = false, code = -1000, msg = "失败.", jsonresult = 0 });
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
                int result = Abll.ActivityeShare(cooperid, 1, openid, sharetype);
                if (result == 1) return JsonFormat(new ExtJson { success = true, code = 1000, msg = "成功." });
                else return JsonFormat(new ExtJson { success = false, code = -1000, msg = "失败." });
            }
            return JsonFormat(new ExtJson { success = false, code = -1000, msg = "失败." });
        }
    }
}