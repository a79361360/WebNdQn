using BLL;
using FrameWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebNdQn.Controllers
{
    public class NsoupController : BaseController
    {
        NsoupBLL nbll = new NsoupBLL();
        // GET: Nsoup
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SendLoginMsg() {
            nbll.SendLoginMsg(1, 1);
            return JsonFormat(new ExtJson { success = false, msg = "参数不能为空" });
        }
    }
}