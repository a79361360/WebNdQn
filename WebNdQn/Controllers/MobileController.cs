using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebNdQn.Controllers
{
    public class MobileController : Controller
    {
        MobileBLL mbll = new MobileBLL();
        // GET: Mobile
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TakeConfigMain() {
            mbll.GetHtmlByLoginCache(21, 1);
            return View();
        }
    }
}