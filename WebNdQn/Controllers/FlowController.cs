using BLL;
using FrameWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebNdQn.Controllers
{
    public class FlowController : BaseController
    {
        CommonBLL bll = new CommonBLL();
        AdminBLL abll = new AdminBLL();
        // GET: Flow
        public ActionResult Index()
        {
            ViewBag.CooperDrop = bll.GetCooperConfigDrop(1);    //取得配置信息列表
            return View();
        }
        public ActionResult FindFlowSearch() {
            string phone = Request["phone"].ToString();     //用户手机号码
            string ctype = Request["ctype"].ToString();     //公司类型
            string issue = "1";                             //活动期号,暂时都为1

            if (!ctype.IsInt() || !issue.IsInt() || phone.Length > 18)
                return JsonFormat(new ExtJson { success = false, msg = "输入的条件格式错误" });

            var list = abll.GetFlowList_Search(Convert.ToInt32(ctype), Convert.ToInt32(issue), phone);
            return JsonFormat(new ExtJson { success = true, code = 1000, msg = "查询成功", jsonresult = list });
        }
    }
}