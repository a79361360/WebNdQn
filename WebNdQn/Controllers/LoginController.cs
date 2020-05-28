using BLL;
using FJSZ.OA.Common.Web;
using FrameWork;
using FrameWork.Common;
using Model.ViewModel;
using Model.WxModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebNdQn.Controllers
{
    public class LoginController : BaseController
    {
        CommonBLL cbll = new CommonBLL();
        AdminBLL abll = new AdminBLL();
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

        public ActionResult TAdminIndex() {
            if (!cbll.VerSession()) Response.Redirect("/Login/index");
            return View();
        }
        public ActionResult SetTadminPortal() {
            T_AdminManager dto = new T_AdminManager();
            if (Request["id"] != null)
            {
                int id = Convert.ToInt32(Request["id"]);
                dto = abll.GetTadminById(id);
            }
            return View(dto);
        }
        /// <summary>
        /// 查询管理员
        /// </summary>
        /// <returns></returns>
        public ActionResult TAdminListPage() {
            string username = Request["username"];
            int pageIndex = Convert.ToInt32(Request["pageIndex"]);
            int pageSize = Convert.ToInt32(Request["pageSize"]);
            int Total = 0;
            var list = abll.FindAdminList(username, pageSize, pageIndex, ref Total);
            if (list.Count > 0)
                return JsonFormat(new ExtJsonPage { success = true, code = 1000, msg = "查询成功", total = Total, list = list });
            else
                return JsonFormat(new ExtJsonPage { success = false, code = -1000, msg = "查询失败" });
        }
        /// <summary>
        /// 移除管理员
        /// </summary>
        /// <returns></returns>
        public ActionResult RemoveTadmin() {
            string data = Request.Form["data"];  //用户的IDS数组
            IList<IdListDto> list = SerializeJson<IdListDto>.JSONStringToList(data);
            int result = abll.RemoveTadmin(list);
            if (result == list.Count)
                return JsonFormat(new ExtJson { success = true, msg = "删除成功！共删除" + result });
            else
                return JsonFormat(new ExtJson { success = false, msg = "删除失败！共" + list.Count + " 成功" + result });
        }
        /// <summary>
        /// 添加修改管理员
        /// </summary>
        /// <returns></returns>
        public ActionResult SetTadmin() {
            if (Request.Form["username"] == null || Request.Form["userpwd"] == null)
                return JsonFormat(new ExtJson { success = false, msg = "参数不能为空！" });
            string username = Request.Form["username"];                     //账号
            string userpwd = Request.Form["userpwd"];                       //密码
            string id = "0";                                                //id
            if (Request.Form["id"] != null) id = Request.Form["id"];
            userpwd = userpwd.MD5();                                        //密码加密
            if (id == "0") {
                int result_1 = abll.GetTadminIdByUserName(username);
                if (result_1 > 0)
                    return JsonFormat(new ExtJson { success = false, msg = "账户已经存在！" });
            }
            int result = abll.UpdateTadmin(username, userpwd, Convert.ToInt32(id));      //
            if (result > 0)
                return JsonFormat(new ExtJson { success = true, msg = "提交成功！" });
            else
                return JsonFormat(new ExtJson { success = false, msg = "提交失败！" });
        }
    }
}