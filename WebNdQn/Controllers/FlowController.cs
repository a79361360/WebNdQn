using BLL;
using Fgly.Common.Expand;
using FrameWork;
using FrameWork.Common;
using Model.ViewModel;
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
        //添加充值记录
        public ActionResult AddFlow() {
            if (Request["phone"] == null || Request["ctype"] == null || Request["issue"] == null || Request["openid"] == null)
                return JsonFormat(new ExtJson { success = false, msg = "参数不能为空!" });
            string phone = Request["phone"].ToString();         //用户手机号码
            string ctype = Request["ctype"].ToString();         //公司类型
            string issue = Request["issue"].ToString();         //活动期号
            string openid = Request["openid"].ToString();       //微信的Openid

            int result_f = bll.TakeFlowLog(Convert.ToInt32(ctype), Convert.ToInt32(issue), phone, openid);
            if (result_f > 0)
                return JsonFormat(new ExtJson { success = true, msg = "添加成功!" });
            else
                return JsonFormat(new ExtJson { success = false, msg = "添加失败!" });
        }
        //删除充值记录
        public ActionResult RemoveFlow() {
            string data = Request.Form["data"];  //用户的IDS数组
            IList<IdListDto> list = SerializeJson<IdListDto>.JSONStringToList(data);
            int result = bll.RemoveFlowLog(list);
            if (result == list.Count)
                return JsonFormat(new ExtJson { success = true, msg = "删除成功！共删除" + result });
            else
                return JsonFormat(new ExtJson { success = false, msg = "删除失败！共" + list.Count + " 成功" + result });
        }
    }
}