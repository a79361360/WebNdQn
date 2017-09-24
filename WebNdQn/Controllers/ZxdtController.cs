using BLL;
using FJSZ.OA.Common.DEncrypt;
using FJSZ.OA.Common.Web;
using FrameWork;
using Model.WxModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebNdQn.Controllers
{
    public class ZxdtController : BaseController
    {
        CommonBLL bll = new CommonBLL();
        WeiXinBLL wxll = new WeiXinBLL();
        // GET: Zxdt
        public ActionResult Index()
        {
            if (Request["ctype"] == null || Request["issue"] == null)
                return JsonFormat(new ExtJson { success = false, msg = "参数不能为空" });
            string ctype = Request["ctype"].ToString(); string issue = Request["issue"].ToString();
            T_CooperConfig dto = wxll.Get_CooperConfig(Convert.ToInt32(ctype), Convert.ToInt32(issue));     //取得配置
            if (dto == null)
                return Content("配置为空");
            if (Request["p"] == null)
            {
                string c = "&c=" + DEncrypt.DESEncrypt1("CGI|1|" + WebHelp.GetCurHttpHost() + "/Home/Index");   //c参数进行加密
                string param = Request.Url.Query + c;   //参数串,例如:http://wx.ndll800.com/home/default?ctype=1&issue=1 取的param为:   ?ctype=1&issue=1
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/ZxdtController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "Index     param： " + param);
                string state = "";                  //state的值暂时为空,如果后面有需要验签,再用起来,现在就直接用参数来做校验
                string url = wxll.Wx_Auth_Code(dto.wx_appid, System.Web.HttpUtility.UrlEncode(WebHelp.GetCurHttpHost() + "/WeiX/Wx_Auth_Code" + param), "snsapi_userinfo", state);  //snsapi_base,snsapi_userinfo
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/ZxdtController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "Index     URL： " + url);
                return Redirect(url);
            }
            else {
                string gz = "0"; string openid = "";
                if (Request["p"] != null)
                {
                    try
                    {
                        string p = Request["p"].ToString(); //1|subscribe|openid  微信发送|是否关注|openid
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/ZxdtController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "Index     p：" + Request["p"].ToString());
                        string temp = DEncrypt.DESDecrypt1(p);    //取得p参数,并且进行解密
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/ZxdtController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "Index     p：" + temp);
                        string[] plist = temp.Split('|');   //微信发送|是否
                        if (plist[0] != "1") return Content("配置参数异常");
                        gz = plist[1]; openid = plist[2];   //是否关注,微信用户id
                    }
                    catch
                    {
                        return Content("参数错误");
                    }
                }
                if (string.IsNullOrEmpty(openid))
                {
                    return Content("授权失败");
                }
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/ZxdtController_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "Index     ctype：" + ctype + "issue：" + issue + "gzstate：" + gz);

                //业务细节,参考大转盘
            }

            return View();
        }
    }
}