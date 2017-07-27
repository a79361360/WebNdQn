using BLL;
using Fgly.Common.Expand;
using FrameWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebNdQn.Controllers
{
    public class MobileController : BaseController
    {
        CommonBLL bll = new CommonBLL();
        MobileBLL mbll = new MobileBLL();
        // GET: Mobile
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ToExeclSendMsgCode() {
            //上传execl并调发送短信接口
            //if (Request["ctype"] == null || Request["issue"] == null)
            //    return JsonFormat(new ExtJson { success = false, msg = "参数不能为空" });
            //string ctype = Request["ctype"].ToString();
            //string issue = Request["issue"].ToString();
            //int resutint = mbll.ToExeclSendMsgCode(Convert.ToInt32(ctype), Convert.ToInt32(issue));
            mbll.ExecuteCooperList();
            //int resutint = mbll.ToExeclSendMsgCode(21, 1);
            return JsonFormat(new ExtJson { success = true, msg = "执行完成", code = 1000 });
        }
        //手动执行
        public ActionResult SingleSendToExecl() {
            if (Request["ctype"] == null || Request["issue"] == null)
                return JsonFormat(new ExtJson { success = false, msg = "参数不能为空" });
            string ctype = Request["ctype"].ToString();
            string issue = Request["issue"].ToString();
            int resutint = mbll.ToExeclSendMsgCode(Convert.ToInt32(ctype), Convert.ToInt32(issue));
            return JsonFormat(new ExtJson { success = true, msg = "执行完成", code = 1000, jsonresult = resutint });
        }
        public ActionResult KeepSessionEd() {
            if (Request["lx"] == null) 
                return JsonFormat(new ExtJson { success = false, msg = "参数不能为空" });
            string lx = Request["lx"].ToString();   //1为总Session2为充值Session
            int Num = 0, ANum = 0;
            string pl = "0";   //1为批量,0为单个保持
            if (Request["pl"] != null)
                pl = Request["pl"].ToString();
            //单个
            if (pl == "0")
            {
                if (Request["ctype"] == null || Request["issue"] == null)
                    return JsonFormat(new ExtJson { success = false, msg = "参数不能为空" });
                string ctype = Request["ctype"].ToString();
                string issue = Request["issue"].ToString();
                mbll.KeepSessionUsered(Convert.ToInt32(ctype), Convert.ToInt32(issue), Convert.ToInt32(lx));
            }
            else if (pl == "1") {
                var list = mbll.FindCacheList();
                ANum = list.Count;  //附值
                foreach (var item in list)
                {
                    int result = mbll.KeepSessionUsered(Convert.ToInt32(item.ctype), Convert.ToInt32(item.issue), Convert.ToInt32(lx));
                    if (result == 1)
                        Num++;
                }
            }
            return JsonFormat(new ExtJson { success = true, code = 1000, msg = "运行结束。", jsonresult = Num + "|" + ANum });
        }
        public ActionResult FindCacheList() {
            var list = mbll.FindCacheList();
            if (list.Count > 0)
                return JsonFormat(new ExtJson { success = true, msg = "查询成功", code = 1000, jsonresult = list });
            else
                return JsonFormat(new ExtJson { success = false, msg = "查询失败", code = -1000, jsonresult = "" });
        }
        public ActionResult TakeMobileCode()
        {
            if (Request["mobile"] == null || Request["content"] == null)
                return JsonFormat(new ExtJson { success = false, msg = "参数不能为空" });
            string phone = Request["mobile"].ToString();        //哪个手机号码接收到的
            int type = Convert.ToInt32("1");                    //通过手机号码判断，1为登入2为充值
            if (phone == "10657532190000624") type = 2;
            string content = Request["content"];     //短信内容
            if (content.Length < 15) return JsonFormat(new ExtJson { success = false, msg = "太短了不用保存" });
            string code = bll.FilterMobileCode(phone, content);     //将短信里面的验证码解析出来
            string xh = bll.FilterMobileXh(phone, content);         //将短信里面的序列号解析出来
            //登入部分,先更新密码
            if (type == 1)
                mbll.UpdateConfigPwd(phone, xh, code);
            if (!code.IsInt()) return JsonFormat(new ExtJson { success = false, msg = "验证码取值错误" });
            int clresult = -1;  //处理结果变量
            if (type == 2)
                clresult = mbll.OverCzWithMsgCode(phone, xh, code);
            int result = bll.TakeMsgCode(type, phone, xh, code, content);       //将收到的验证码保存
            if (result > 0)
            {
                return JsonFormat(new ExtJson { success = true, msg = "保存验证码成功" + "结果：" + clresult });
            }
            return JsonFormat(new ExtJson { success = false, msg = "保存验证码失败" + "结果：" + clresult });
        }


        /// <summary>
        /// 更新登入的cookie
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateDlCookie() {
            if (Request["ctype"] == null || Request["issue"] == null || Request["dlcookie"] == null)
                return JsonFormat(new ExtJson { success = false, msg = "参数不能为空" });
            string ctype = Request["ctype"].ToString();
            string issue = Request["issue"].ToString();
            string dlcookie = Request["dlcookie"].ToString();
            int result = mbll.UpdateDlCookie(Convert.ToInt32(ctype), Convert.ToInt32(issue), dlcookie);
            if (result == 1) 
                return JsonFormat(new ExtJson { success = true, code = 1000, msg = "更新成功" });
            else
                return JsonFormat(new ExtJson { success = false, code = -1000, msg = "更新失败" });
        }
    }
}