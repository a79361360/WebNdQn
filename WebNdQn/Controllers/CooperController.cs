using BLL;
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
    public class CooperController : BaseController
    {
        AdminBLL abll = new AdminBLL();
        CommonBLL cbll = new CommonBLL();
        // GET: Cooper
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 上传背景图，微信小图标，公众号二维码图片
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateImg() {
            string type = Request.Form["type"]; //类型1背景图2微信分享小图标3公众号二维码
            string vurl = Request.Form["vurl"]; //图片的名称包括后缀名，如果是更新就不修改图片的名称，不然图片会多出来很多
            string fileid = Request.Form["myfile"];
            string url = cbll.DzpUploadBgUrl(Convert.ToInt32(type), fileid, vurl);
            if (!string.IsNullOrEmpty(url))
                return JsonFormat(new ExtJson { success = true, code = 1000, msg = "成功.", jsonresult = url });
            else
                return JsonFormat(new ExtJson { success = false, code = -1000, msg = "失败." });
        }
        public ActionResult SetCooperPortal() {
            T_CooperConfig dto = new T_CooperConfig();
            if (Request["cooperid"] != null) {
                int id = Convert.ToInt32(Request["cooperid"]);
                dto = cbll.GetCooperConfigById(id);
            }
            return View(dto);
        }
        public ActionResult CooperConfig() {
            return View();
        }
        public ActionResult GetCooperList() {
            string name = Request["name"].ToString();       //用户手机号码
            string value = Request["value"].ToString();     //公司类型
            string state = Request["state"].ToString();     //公司类型
            int pageIndex = Convert.ToInt32(Request["pageIndex"]);
            int pageSize = Convert.ToInt32(Request["pageSize"]);
            int Total = 0;

            var list = abll.GetCooper_Page(name, value, Convert.ToInt32(state), pageSize, pageIndex, ref Total);
            if (list.Count > 0)
                return JsonFormat(new ExtJsonPage { success = true, code = 1000, msg = "查询成功", total = Total, list = list });
            else
                return JsonFormat(new ExtJsonPage { success = false, code = -1000, msg = "查询失败" });
        }
        public ActionResult GetConfigById() {
            string cooperid = Request["cooperid"];
            var dto = cbll.GetCooperConfigById(Convert.ToInt32(cooperid));
            if (dto != null)
                return JsonFormat(new ExtJson { success = true, msg = "查询成功.", jsonresult = dto.ctype + "|" + dto.issue });
            else
                return JsonFormat(new ExtJson { success = false, msg = "查询失败." });
        }
        public ActionResult SetCooper() {
            string cooperid = Request.Form["cooperid"];             //会员人数
            string ctype = Request.Form["ctype"];                   //会员人数
            string areatype = Request.Form["areatype"];                   //会员人数
            string gener = Request.Form["gener"];                   //会员人数
            string title = Request.Form["title"];                   //会员人数
            string descride = Request.Form["descride"];                   //会员人数
            string btnurl = "";
            string bgurl = Request.Form["bgurl"];               //会员人数
            string imgurl = Request.Form["imgurl"];                   //会员人数
            string linkurl = Request.Form["linkurl"];                   //会员人数
            string redirecturi = Request.Form["redirecturi"];
            string corpid = Request.Form["corpid"];                   //会员人数
            string username = Request.Form["username"];                   //会员人数
            string userpwd = "";                   //会员人数
            string signphone = Request.Form["signphone"];                   //会员人数
            string appid = Request.Form["appid"];                   //会员人数
            string secret = Request.Form["secret"];                   //会员人数
            string qrcode_url = Request.Form["qrcode_url"];                   //会员人数
            string eachflow = Request.Form["eachflow"];                   //会员人数
            string uplimit = Request.Form["uplimit"];                   //会员人数
            string cutdate = Request.Form["cutdate"];                   //会员人数
            string state = Request.Form["state"];                   //会员人数
            T_CooperConfig dto = new T_CooperConfig();
            dto.issue = 1;
            dto.id = Convert.ToInt32(cooperid); dto.ctype = Convert.ToInt32(ctype);dto.areatype = Convert.ToInt32(areatype);dto.gener = gener; dto.title = title;
            dto.descride = descride; dto.btnurl = btnurl; dto.bgurl = bgurl; dto.imgurl = imgurl; dto.linkurl = linkurl;dto.redirecturi = redirecturi;
            dto.corpid = corpid; dto.username = username; dto.userpwd = userpwd; dto.signphone = signphone; dto.wx_appid = appid;
            dto.wx_secret = secret; dto.qrcode_url = qrcode_url; dto.eachflow = Convert.ToInt32(eachflow);
            dto.uplimit = Convert.ToInt32(uplimit); dto.cutdate = cutdate; dto.state = Convert.ToInt32(state);
            int result = cbll.SetCooper(dto);
            if (result > 0)
                return JsonFormat(new ExtJson { success = true, code = 1000, msg = "成功." });
            else
                return JsonFormat(new ExtJson { success = false, code = -1000, msg = "失败." });
        }
        public ActionResult RemoveCooper() {
            string data = Request.Form["data"];  //用户的IDS数组
            IList<IdListDto> list = SerializeJson<IdListDto>.JSONStringToList(data);
            int result = cbll.RemoveCoopers(list);
            if (result == list.Count)
                return JsonFormat(new ExtJson { success = true, msg = "删除成功！共删除" + result });
            else
                return JsonFormat(new ExtJson { success = false, msg = "删除失败！共" + list.Count + " 成功" + result });
        }
        //查询短信页面
        public ActionResult MsgCodePortal() {
            return View();
        }
        public ActionResult MsgCodeAction() {
            string phone = Request["phone"].ToString();     //用户手机号码
            string type = Request["type"].ToString();     //公司类型

            if (!type.IsInt())
                return JsonFormat(new ExtJson { success = false, msg = "输入的条件格式错误" });

            var list = abll.GetMsgCodeList(Convert.ToInt32(type), phone);
            return JsonFormat(new ExtJson { success = true, code = 1000, msg = "查询成功", jsonresult = list });
        }
    }
}