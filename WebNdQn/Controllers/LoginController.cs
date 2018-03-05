using BLL;
using FJSZ.OA.Common.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebNdQn.Controllers
{
    public class LoginController : Controller
    {
        CommonBLL cbll = new CommonBLL();
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 生成登入验证码
        /// </summary>
        /// <returns></returns>
        public ActionResult LoginVerifyCode()
        {
            FJSZ.OA.Common.Web.VerifyCode.ShowImg(VerifyCodeType.Login);
            return View();
        }
        /// <summary>
        /// 登入并生成Session
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            if (Request.Form["username"] == null || Request.Form["password"] == null || Request.Form["yzm"] == null)
                return Json(new { code = -1000, tips = "登入参数不能为空" });
            if (Request.Cookies["VerifyCode_Login"] == null)
                return Json(new { code = -1001, tips = "验证码失败,错误代码1002" });
            string u = Request.Form["username"];       //用户名
            string p = Request.Form["password"];       //密码
            string yzm = Request.Form["yzm"];   //用户输入的验证码
            string yzmcode = Request.Cookies["VerifyCode_Login"].Value; //生成的验证码
            bool result = cbll.VerCode(yzm, yzmcode);
            if (result)
            {
                p = FJSZ.OA.Common.DEncrypt.Encryptor.MD5Encrypt(p);
                int result_1 = cbll.GetAdminId(u, p);
                if (result_1 != -1)
                {
                    Session["AdminID"] = result_1.ToString();
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/LoginController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "这个人登入 ID :" + result_1.ToString());
                    return Redirect("/Home/Navigation");
                }
                return Content("登入名或密码错误,3秒后重新登入....<script>setTimeout(\"location.href = '/Login/Index'\", 3000);</script>");
            }
            else
                return Content("验证码不正确,3秒后重新登入....<script>setTimeout(\"location.href = '/Login/Index'\", 3000);</script>");
        }
    }
}