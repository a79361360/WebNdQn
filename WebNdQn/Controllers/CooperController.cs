using BLL;
using FrameWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebNdQn.Controllers
{
    public class CooperController : BaseController
    {
        AdminBLL abll = new AdminBLL();
        // GET: Cooper
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CooperConfig() {
            return View();
        }
        public ActionResult GetCooperList() {
            string name = Request["name"].ToString();       //用户手机号码
            string value = Request["value"].ToString();     //公司类型
            string state = Request["state"].ToString();     //公司类型
            int pageIndex = Convert.ToInt32(Request["pageIndex"]);
            int pageSize = Convert.ToInt32(Request["pageSize"]);
            int Total = 0;

            var list = abll.GetCooper_Page(name, value, Convert.ToInt32(state),pageSize,pageIndex,ref Total);
            if (list.Count > 0)
                return JsonFormat(new ExtJsonPage { success = true, code = 1000, msg = "查询成功", total = Total, list = list });
            else
                return JsonFormat(new ExtJsonPage { success = false, code = -1000, msg = "查询失败" });
        }
    }
}