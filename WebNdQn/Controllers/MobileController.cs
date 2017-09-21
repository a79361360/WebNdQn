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
    public class MobileController : BaseController
    {
        CommonBLL bll = new CommonBLL();
        MobileBLL mbll = new MobileBLL();
        // GET: Mobile
        /// <summary>
        /// 超端控制页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 第一步
        /// </summary>
        /// <returns></returns>
        public ActionResult One() {
            if (Request["ctype"] == null || Request["issue"] == null)
                return JsonFormat(new ExtJson { success = false, msg = "参数不能为空" });
            int ctype = Convert.ToInt32(Request["ctype"].ToString());
            int issue = Convert.ToInt32(Request["issue"].ToString());
            //判断是否有待充值记录
            int result_1 = mbll.IsExistsCzList(ctype, issue);
            if (result_1 == 0)
                return JsonFormat(new ExtJson { success = false, msg = "没有待充值的记录", code = -1000, jsonresult = "" });
            int result_2 = mbll.KeepSessionUsered(ctype, issue, 2);
            if (result_2 == -1)
                return JsonFormat(new ExtJson { success = false, msg = "充值Session已经失效,需要重新获取", code = -1001, jsonresult = "" });
            int result_3 = mbll.FindDtByCtype(ctype, issue);
            if (result_3 != 1)
                return JsonFormat(new ExtJson { success = false, msg = "生成批量用户的Execl表失败了,需要去排查日志", code = -1002, jsonresult = "" });
            int result_4 = mbll.one_plyhfp(ctype, issue);
            if(result_4!=1)
                return JsonFormat(new ExtJson { success = false, msg = "上传批量用户的EXECL并且发送短信失败了,需要去排查日志", code = -1003, jsonresult = "" });
            return JsonFormat(new ExtJson { success = true, msg = "成功运行发送充值短信.", code = 1000, jsonresult = "" });
        }
        /// <summary>
        /// 充值Session丢失的情况下,就需要用登入Session去重新获取
        /// </summary>
        /// <returns></returns>
        public ActionResult Two() {
            if (Request["ctype"] == null || Request["issue"] == null)
                return JsonFormat(new ExtJson { success = false, msg = "参数不能为空" });
            int ctype = Convert.ToInt32(Request["ctype"].ToString());
            int issue = Convert.ToInt32(Request["issue"].ToString());
            int result_1 = mbll.GetHtmlByLoginCache(ctype, issue);
            if (result_1 == -1010)
                return JsonFormat(new ExtJson { success = false, msg = "登入Session已经失效,需要重新获取", code = -1001, jsonresult = "" });
            if(result_1!=1)
                return JsonFormat(new ExtJson { success = false, msg = "生成充值Session已经失效过程中,出错异常错误,需要去排查日志", code = -1002, jsonresult = "" });
            return JsonFormat(new ExtJson { success = true, msg = "成功生成充值Session", code = 1000, jsonresult = "" });
        }
        /// <summary>
        /// 登入Session失效的情况下,需要用账号和短信验证码生成
        /// </summary>
        /// <returns></returns>
        public ActionResult Three() {
            if (Request["ctype"] == null || Request["issue"] == null)
                return JsonFormat(new ExtJson { success = false, msg = "参数不能为空" });
            int ctype = Convert.ToInt32(Request["ctype"].ToString());
            int issue = Convert.ToInt32(Request["issue"].ToString());
            int result = mbll.HelpWebSend(Convert.ToInt32(ctype), Convert.ToInt32(issue));
            if (result == 1)
                return JsonFormat(new ExtJson { success = true, msg = "发送成功", code = 1000, jsonresult = "" });
            else
                return JsonFormat(new ExtJson { success = false, msg = "发送失败", code = -1000, jsonresult = "" });
        }
        /// <summary>
        /// 持续校验登入Session有效性
        /// </summary>
        /// <returns></returns>
        public ActionResult Four() {
            if (Request["ctype"] == null || Request["issue"] == null)
                return JsonFormat(new ExtJson { success = false, msg = "参数不能为空" });
            int ctype = Convert.ToInt32(Request["ctype"].ToString());
            int issue = Convert.ToInt32(Request["issue"].ToString());
            int result_1 = mbll.KeepSessionUsered(ctype, issue, 1);    //先判断登入Session是否有效,返回1为有效
            if (result_1 == 1)
                return JsonFormat(new ExtJson { success = true, msg = "发送成功", code = 1000, jsonresult = "" });
            else
                return JsonFormat(new ExtJson { success = false, msg = "发送失败", code = -1000, jsonresult = "" });
        }




        /// <summary>
        /// 批量调用状态正在进行中的单子进行直充
        /// </summary>
        /// <returns></returns>
        public ActionResult ToExeclSendMsgCode() {
            mbll.ExecuteCooperList();
            return JsonFormat(new ExtJson { success = true, msg = "执行完成", code = 1000 });
        }
        //手动执行充值
        public ActionResult SingleSendToExecl() {
            if (Request["ctype"] == null || Request["issue"] == null)
                return JsonFormat(new ExtJson { success = false, msg = "参数不能为空" });
            string ctype = Request["ctype"].ToString();
            string issue = Request["issue"].ToString();
            //判断是否有待充值记录
            int result_1 = mbll.IsExistsCzList(Convert.ToInt32(ctype), Convert.ToInt32(issue));
            if (result_1 == 0) 
                return JsonFormat(new ExtJson { success = false, msg = "没有待充值的记录", code = -1000, jsonresult = "" });
            int result_2 = mbll.ToExeclSendMsgCode(Convert.ToInt32(ctype), Convert.ToInt32(issue));
            if (result_2 == -1010) 
                return JsonFormat(new ExtJson { success = false, msg = "登入状态无效", code = -1010, jsonresult = "" });
            if (result_2 == -11)
                return JsonFormat(new ExtJson { success = false, msg = "当前账号已无流量池权限", code = -11, jsonresult = "" });
            if (result_2 == 1)
                return JsonFormat(new ExtJson { success = true, msg = "执行完成", code = 1000, jsonresult = "" });
            else 
                return JsonFormat(new ExtJson { success = false, msg = "执行异常,查看具体原因", code = -1, jsonresult = "" });

        }
        /// <summary>
        /// 手动发送登入短信
        /// </summary>
        /// <returns></returns>
        public ActionResult SignleSendLoginMsg() {
            if (Request["ctype"] == null || Request["issue"] == null)
                return JsonFormat(new ExtJson { success = false, msg = "参数不能为空" });
            string ctype = Request["ctype"].ToString();
            string issue = Request["issue"].ToString();
            int result = mbll.HelpWebSend(Convert.ToInt32(ctype), Convert.ToInt32(issue));
            if (result == 1) 
                return JsonFormat(new ExtJson { success = true, msg = "发送成功", code = 1000, jsonresult = "" });
            else
                return JsonFormat(new ExtJson { success = false, msg = "发送失败", code = -1000, jsonresult = "" });
        }
        /// <summary>
        /// 接收APP发过来的短信
        /// </summary>
        /// <returns></returns>
        public ActionResult TakeMobileCode()
        {
            if (Request["mobile"] == null || Request["content"] == null)
                return JsonFormat(new ExtJson { success = false, msg = "参数不能为空" });
            string phone = Request["mobile"].ToString();        //哪个手机号码接收到的
            int type = Convert.ToInt32("1");                    //通过手机号码判断，1为登入2为充值
            if (phone == "10657532190000761") type = 2;
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
        /// 通过定时的连接操作,保存Session的活性
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// 超端查询
        /// </summary>
        /// <returns></returns>
        public ActionResult FindCacheList() {
            var list = mbll.FindCacheList();
            if (list.Count > 0)
                return JsonFormat(new ExtJson { success = true, msg = "查询成功", code = 1000, jsonresult = list });
            else
                return JsonFormat(new ExtJson { success = false, msg = "查询失败", code = -1000, jsonresult = "" });
        }

        /// <summary>
        /// 手动更新登入的cookie
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
        /// <summary>
        /// 添加超端记录
        /// </summary>
        /// <returns></returns>
        public ActionResult AddLoginCache() {
            if (Request.Form["ctype"] == null || Request.Form["issue"] == null) {
                return JsonFormat(new ExtJson { success = false, code = -1000, msg = "参数不能为空" });
            }
            int ctype = Convert.ToInt32(Request.Form["ctype"]);
            int issue = Convert.ToInt32(Request.Form["issue"]);
            int result = mbll.InsertLoginCache(ctype, issue);
            if (result > 0)
                return JsonFormat(new ExtJson { success = true, code = 1000, msg = "添加超端记录成功" });
            else if (result == -1000)
                return JsonFormat(new ExtJson { success = false, code = -1000, msg = "添加失败" });
            else
                return JsonFormat(new ExtJson { success = false, code = -1000, msg = "已经存在此超端记录" });
        }
        /// <summary>
        /// 删除超端记录
        /// </summary>
        /// <returns></returns>
        public ActionResult RemoveLoginCache() {
            string data = Request.Form["data"];  //用户的IDS数组
            IList<IdListDto> list = SerializeJson<IdListDto>.JSONStringToList(data);
            int result = mbll.RemoveLoginCache(list);
            if (result == list.Count)
                return JsonFormat(new ExtJson { success = true, msg = "删除成功！共删除" + result });
            else
                return JsonFormat(new ExtJson { success = false, msg = "删除失败！共" + list.Count + " 成功" + result });
        }
    }
}