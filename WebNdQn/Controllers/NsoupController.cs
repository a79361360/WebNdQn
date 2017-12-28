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
        //发送短信
        public ActionResult SendLoginMsg() {
            if (Request["ctype"] == null || Request["issue"] == null)
                return JsonFormat(new ExtJson { success = false, msg = "参数不能为空" });
            int ctype = Convert.ToInt32(Request["ctype"].ToString());
            int issue = Convert.ToInt32(Request["issue"].ToString());

            int result = nbll.SendLoginMsg(ctype, issue);
            if (result > 0) {
                return JsonFormat(new ExtJson { success = true, msg = "短信发送成功" });
            }
            return JsonFormat(new ExtJson { success = false, msg = "短信发送失败" });
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
                    int result_1 = nbll.CreateLoginCookie(Convert.ToInt32(code));
                }
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "TakeMobileCode 4 将短信内容写入数据库成功: ");
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
                return JsonFormat(new ExtJson { success = true, msg = "登入Cookie有效" });
            else
                return JsonFormat(new ExtJson { success = false, msg = "登入Cookie无效" });
        }
    }
}