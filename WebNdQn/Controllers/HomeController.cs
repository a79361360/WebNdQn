using BLL;
using FrameWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebNdQn.Controllers
{
    public class HomeController : BaseController
    {
        CommonBLL bll = new CommonBLL();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SignPhoneFilter() {
            string phone = Request["phone"].ToString();
            phone = phone.Substring(0, 7);
            string path = Server.MapPath(@"/Content/Txt/pwebconfig.txt");
            bool result = bll.ReadPhoneFliter(phone, path);
            if (result)
                return JsonFormat(new ExtJson { success = true, msg = "验证通过允许充值" });
            else
                return JsonFormat(new ExtJson { success = false, msg = "活动仅限宁德移动手机用户参与" });
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
    }
}