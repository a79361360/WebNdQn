using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebNdQn.Controllers
{
    public class CooperController : Controller
    {
        // GET: Cooper
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CooperConfig() {
            return View();
        }
    }
}