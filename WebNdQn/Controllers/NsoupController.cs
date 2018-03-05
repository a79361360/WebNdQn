using BLL;
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
    public class NsoupController : BaseController
    {
        NsoupBLL nbll = new NsoupBLL();
        // GET: Nsoup
        public ActionResult Index()
        {
            if (Session["AdminID"] == null) Response.Redirect("/Login/index");
            return View();
        }
        //发送登入短信
        public ActionResult SendLoginMsg() {
            if (Request["ctype"] == null || Request["issue"] == null)
                return JsonFormat(new ExtJson { success = false, msg = "参数不能为空" });
            int ctype = Convert.ToInt32(Request["ctype"].ToString());
            int issue = Convert.ToInt32(Request["issue"].ToString());

            int result = nbll.SendLoginMsg(ctype, issue);
            if (result > 0) {
                return JsonFormat(new ExtJson { success = true, code = 1000, msg = "短信发送成功" });
            }
            return JsonFormat(new ExtJson { success = false, code = -1000, msg = "短信发送失败" });
        }
        //获取短信
        public ActionResult TakeMobileCode()
        {
            Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "TakeMobileCode 1 短信接收控制器开始");
            if (Request["mobile"] == null || Request["content"] == null)
                return JsonFormat(new ExtJson { success = false, msg = "参数不能为空" });
            string phone = Request["mobile"].ToString();        //哪个手机号码接收到的
            string content = Request["content"];                //短信内容
            Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "TakeMobileCode 2 mobile: " + phone + " content: " + content);
            //解析短信
            string str = nbll.FilterContentTC(phone, content);  //
            string[] str_1 = str.Split('|');
            int type = Convert.ToInt32(str_1[0]);                   //通过手机号码判断，1为登入2为充值
            string code = str_1[1];                                 //6位验证码
            Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "TakeMobileCode 3 type: " + type + " code: " + code);
            //保存动态码
            int result = nbll.TakeMsgCode(type, phone, "0", code, content);
            if (result == 1)
            {
                if (type == 1)
                {
                    //创建登入cookie并保存到数据库
                    int result_1 = nbll.CreateLoginCookie(Convert.ToInt32(code));
                }
                else if (type == 2)
                {
                    //接收充值短信并完成充值
                    int result_1 = nbll.SubmitCzMsg(Convert.ToInt32(code));
                }
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "TakeMobileCode 4 将短信内容写入数据库成功: ");
                return JsonFormat(new ExtJson { success = true, msg = "执行成功" + "结果：" + content });
            }
            return JsonFormat(new ExtJson { success = false, msg = "保存验证码失败" + "结果：" + content });
        }
        /// <summary>
        /// 校验登入Cookie是否有效
        /// </summary>
        /// <returns></returns>
        public ActionResult SignDlCookie() {
            int result = nbll.SignDlCookie();
            if (result == 1)
                return JsonFormat(new ExtJson { success = true, code = 1000, msg = "登入Cookie有效" });
            else
                return JsonFormat(new ExtJson { success = false, code = -1000, msg = "登入Cookie无效" });
        }


        public ActionResult One()
        {
            if (Request["ctype"] == null || Request["issue"] == null)
                return JsonFormat(new ExtJson { success = false, msg = "参数不能为空" });
            int ctype = Convert.ToInt32(Request["ctype"].ToString());
            int issue = Convert.ToInt32(Request["issue"].ToString());
            //判断是否有待充值记录
            int result_1 = nbll.IsExistsCzList(ctype, issue);
            if (result_1 != 1)
                return JsonFormat(new ExtJson { success = false, msg = "没有待充值的记录", code = -1000, jsonresult = "" });
            //校验登入cookie是否有效
            int result_2 = nbll.SignDlCookie(ctype, issue);
            if (result_2 != 1)
                return JsonFormat(new ExtJson { success = false, msg = "Cookie已经失效,需要重新获取", code = -1001, jsonresult = "" });
            //将充值记录生成DT再生成Execl表文件
            int result_3 = nbll.CreateCzExecl(ctype, issue);
            if (result_3 != 1)
                return JsonFormat(new ExtJson { success = false, msg = "生成批量充值的Execl表失败了,需要去排查日志", code = -1002, jsonresult = "" });
            //提交Cz的Execl表
            int result_4 = nbll.SubmitCzExecl(ctype, issue);
            if (result_4 != 1)
                return JsonFormat(new ExtJson { success = false, msg = "上传批量用户的EXECL并且发送短信失败了,需要去排查日志", code = -1003, jsonresult = "" });
            return JsonFormat(new ExtJson { success = true, msg = "成功运行发送充值短信.", code = 1000, jsonresult = "" });
        }
        /// <summary>
        /// 取得监控列表
        /// </summary>
        /// <returns></returns>
        public ActionResult FindLogCacheList() {
            var list = nbll.FindLogCacheList();
            if (list.Count > 0)
                return JsonFormat(new ExtJson { success = true, code = 1000, msg = "查询成功", jsonresult = list });
            else
                return JsonFormat(new ExtJson { success = false, code = -1000, msg = "查询失败", jsonresult = "" });
        }
        /// <summary>
        /// 删除超端记录
        /// </summary>
        /// <returns></returns>
        public ActionResult RemoveLoginCache()
        {
            string data = Request.Form["data"];  //用户的IDS数组
            IList<IdListDto> list = SerializeJson<IdListDto>.JSONStringToList(data);
            int result = nbll.RemoveLoginCache(list);
            if (result == list.Count)
                return JsonFormat(new ExtJson { success = true, msg = "删除成功！共删除" + result });
            else
                return JsonFormat(new ExtJson { success = false, msg = "删除失败！共" + list.Count + " 成功" + result });
        }
        /// <summary>
        /// 添加超端记录
        /// </summary>
        /// <returns></returns>
        public ActionResult AddLoginCache()
        {
            if (Request.Form["ctype"] == null || Request.Form["issue"] == null)
            {
                return JsonFormat(new ExtJson { success = false, code = -1000, msg = "参数不能为空" });
            }
            int ctype = Convert.ToInt32(Request.Form["ctype"]);
            int issue = Convert.ToInt32(Request.Form["issue"]);
            int result = nbll.InsertLoginCache(ctype, issue);
            if (result > 0)
                return JsonFormat(new ExtJson { success = true, code = 1000, msg = "添加超端记录成功" });
            else if (result == -1000)
                return JsonFormat(new ExtJson { success = false, code = -1000, msg = "已经存在此超端记录" });
            else
                return JsonFormat(new ExtJson { success = false, code = -1000, msg = "添加超端记录失败" });
        }
    }
}