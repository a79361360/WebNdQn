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
        NsoupBLL nbll = new NsoupBLL();
        // GET: Flow
        public ActionResult Index()
        {
            if (!bll.VerSession()) Response.Redirect("/Login/index");
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



        /// <summary>
        /// 手动生成充值的Execl，以便手动提交
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateExecl() {
            string ctype = Request["ctype"].ToString();         //公司类型
            string issue = Request["issue"].ToString();         //活动期号
            //将充值记录生成DT再生成Execl表文件flowPoolExcel.xls
            int result_3 = nbll.CreateCzExecl(Convert.ToInt32(ctype), Convert.ToInt32(issue), "ProPool.xls");
            if (result_3 != 1)
                return JsonFormat(new ExtJson { success = false, msg = "生成批量充值的Execl表失败了,需要去排查日志", code = -1002, jsonresult = "" });
            return JsonFormat(new ExtJson { success = true, msg = "生成成功!" });
        }
        /// <summary>
        /// 更新直充记录的状态
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateFlowState() {
            string data = Request.Form["data"];  //用户的IDS数组
            string state = Request.Form["state"];  //需要修改的状态
            IList<IdListDto> list = SerializeJson<IdListDto>.JSONStringToList(data);
            int result = bll.UpdateFlowState(list,Convert.ToInt32(state));
            if (result == list.Count)
                return JsonFormat(new ExtJson { success = true, msg = "更新成功！共更新" + result });
            else
                return JsonFormat(new ExtJson { success = false, msg = "更新失败！共" + list.Count + " 成功" + result });
        }
    }
}