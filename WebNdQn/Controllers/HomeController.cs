using BLL;
using FrameWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Fgly.Common.Expand;
using Model.WxModel;

namespace WebNdQn.Controllers
{
    public class HomeController : BaseController
    {
        CommonBLL bll = new CommonBLL();
        WeiXinBLL wxll = new WeiXinBLL();
        public ActionResult Index()
        {
            int cooper = 0;
            if(Request.QueryString["utype"]!=null){
                cooper = Convert.ToInt32(Request.QueryString["utype"]);
            }
            T_CooperConfig dto = wxll.Get_CooperConfig(cooper);                             //取得配置
            long timestamp = DateTime.Now.ToUnixTimeStamp();                                //时间戳
            string noncestr = TxtHelp.GetRandomString(16, true, true, true, false, "");     //随机字符串
            string signatrue = wxll.Get_signature(timestamp, noncestr);                     //signatrue
            //ViewBag.appid = "wx905707332cae0c38";
            ViewBag.appid = Wx_config.appid;
            ViewBag.timestamp = timestamp;
            ViewBag.noncestr = noncestr;
            ViewBag.signatrue = signatrue;
            ViewBag.title = dto.title;              //标题
            ViewBag.desc = dto.descride;            //描述
            ViewBag.imgurl = dto.imgurl;            //图片地址
            ViewBag.linkurl = dto.linkurl;          //链接地址
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
        /// <summary>
        /// 接收验证码
        /// </summary>
        /// <returns></returns>
        public ActionResult TakeCode() {
            if (Request["type"] == null || Request["code"] == null) {
                return JsonFormat(new ExtJson { success = false, msg = "参数不能为空" });
            }
            int type = Convert.ToInt32(Request["type"]);  //1为登入2为充值
            int code = Convert.ToInt32(Request["code"]);  //验证码
            int result = bll.TakeMsgCode(type, code);
            if (result > 0) {
                return JsonFormat(new ExtJson { success = true, msg = "保存验证码成功" });
            }
            return JsonFormat(new ExtJson { success = false, msg = "保存验证码失败" });
        }
        public ActionResult ShareWeixi() {
            string access_token = wxll.Get_Access_Token(Wx_config.appid, Wx_config.appsecret);
            //string access_token = "Bgbznk2ods_Y_vfDikpMAd_cwbM2tsBAJgZAZQ2O0bEbmCn1Q9AZ8mBPCSalthgBrJP2wqb6AMbI4ZPx9j7qLV4MwhovhvXWUE37BCXRmXS5i0Ht6R-nKy8urDVuoQ4rYAQaAEAJLB";
            string jsapi_ticket = wxll.Get_Jsapi_Ticket(access_token);
            //string jsapi_ticket = "kgt8ON7yVITDhtdwci0qeWuN-3Mn_q_dPJGra0ooR2HLQNbupFT-S95u5DfpnCt2Q3PBr88Fy0wfZ3IHBaiybQ";
            //string test = Request["test"].ToString();
            long timestamp = DateTime.Now.ToUnixTimeStamp();        //时间戳
            string noncestr = TxtHelp.GetRandomString(16, true, true, true, false, "");
            string signatrue = wxll.Get_signature(timestamp, noncestr);
            
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