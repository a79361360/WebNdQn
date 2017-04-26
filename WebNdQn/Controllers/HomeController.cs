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
        WeiXinBLL wxll = new WeiXinBLL();
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
            //string access_token = wxll.Get_Access_Token("wxc4b8dfa5424ac1e9","c064bf2901b00931c0aa654f5d21cb74");
            string access_token = "Bgbznk2ods_Y_vfDikpMAd_cwbM2tsBAJgZAZQ2O0bEbmCn1Q9AZ8mBPCSalthgBrJP2wqb6AMbI4ZPx9j7qLV4MwhovhvXWUE37BCXRmXS5i0Ht6R-nKy8urDVuoQ4rYAQaAEAJLB";
            //string jsapi_ticket = wxll.Get_Jsapi_Ticket(access_token);
            string jsapi_ticket = "kgt8ON7yVITDhtdwci0qeWuN-3Mn_q_dPJGra0ooR2HLQNbupFT-S95u5DfpnCt2Q3PBr88Fy0wfZ3IHBaiybQ";
            //string test = Request["test"].ToString();

            string signatrue = wxll.Get_signature(jsapi_ticket);
            
            return JsonFormat(new ExtJson { success = true, msg = jsapi_ticket });
        }
        public JavaScriptResult TestShare()
        {
            //long timestamp = ConvertDateTimeInt(DateTime.Now);
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
    }
}