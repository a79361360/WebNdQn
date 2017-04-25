using BLL;
using FrameWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebNdQn.Controllers
{
    public class HomeController : BaseController
    {
        CommonBLL bll = new CommonBLL();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SignPhoneFilter() {
            string phone = Request["phone"].ToString();
            phone = phone.Substring(0, 7);
            string path = Server.MapPath(@"/Content/Txt/pwebconfig.txt");
            bool result = bll.ReadPhoneFliter(phone, path);
            if (result)
                return JsonFormat(new ExtJson { success = true, msg = "验证通过允许充值" });
            else
                return JsonFormat(new ExtJson { success = false, msg = "活动仅限宁德移动手机用户参与" });
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult ShareWeixi() {
            string test = Request["test"].ToString();
            return JsonFormat(new ExtJson { success = true, msg = test });
        }
        public JavaScriptResult TestShare()
        {
            long timestamp = ConvertDateTimeInt(DateTime.Now);
            string javastr = "wx.config({";
            javastr += "debug: false,";
            javastr += "appId: 'wxf8b4f85f3a794e77',";
            javastr += "timestamp: 1493132657,";
            javastr += "nonceStr: '3u26tVwtjaA6y8Bf',";
            javastr += "signature: '5c14c41cd082f9bb8853475c8b9c1e8f97698ebe',";
            javastr += "jsApiList: [";
            javastr += "'checkJsApi',";
            javastr += "'onMenuShareTimeline',";
            javastr += "'onMenuShareAppMessage',";
            javastr += "'onMenuShareQQ',";
            javastr += "'onMenuShareWeibo',";
            javastr += "'onMenuShareQZone',";
            javastr += "'hideMenuItems',";
            javastr += "'showMenuItems',";
            javastr += "'hideAllNonBaseMenuItem',";
            javastr += "'showAllNonBaseMenuItem',";
            javastr += "'translateVoice',";
            javastr += "'startRecord',";
            javastr += "'stopRecord',";
            javastr += "'onVoiceRecordEnd',";
            javastr += "'playVoice',";
            javastr += "'onVoicePlayEnd',";
            javastr += "'pauseVoice',";
            javastr += "'stopVoice',";
            javastr += "'uploadVoice',";
            javastr += "'downloadVoice',";
            javastr += "'chooseImage',";
            javastr += "'previewImage',";
            javastr += "'uploadImage',";
            javastr += "'downloadImage',";
            javastr += "'getNetworkType',";
            javastr += "'openLocation',";
            javastr += "'getLocation',";
            javastr += "'hideOptionMenu',";
            javastr += "'showOptionMenu',";
            javastr += "'closeWindow',";
            javastr += "'scanQRCode',";
            javastr += "'chooseWXPay',";
            javastr += "'openProductSpecificView',";
            javastr += "'addCard',";
            javastr += "'chooseCard',";
            javastr += "'openCard'";
            javastr += "]";
            javastr += "});";
            return JavaScript(javastr);
        }
        public int ConvertDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }
    }
}