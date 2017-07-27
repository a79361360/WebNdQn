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
            return View();
        }
        public ActionResult Dzp() {
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
                if (!string.IsNullOrEmpty(dto.bgurl))
                {
                    ViewBag.bg = dto.bgurl;             //背景图
                    ViewBag.btn = dto.btnurl;           //背景按钮图
                }
            }
            return View();
        }
        public ActionResult DzpProb() {
            int resultnum = Abll.Getprob();
            return JsonFormat(new ExtJson { success = true, code = 1000, msg = "成功.", jsonresult = resultnum });
        }
    }
}