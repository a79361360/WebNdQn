using BLL;
using FrameWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Fgly.Common.Expand;
using Model.WxModel;
using FJSZ.OA.Common.Web;

namespace WebNdQn.Controllers
{
    public class HomeController : BaseController
    {
        CommonBLL bll = new CommonBLL();
        WeiXinBLL wxll = new WeiXinBLL();
        public ActionResult Index()
        {
            int cooper = 0, issue = 1;
            if(Request.QueryString["utype"]!=null){
                cooper = Convert.ToInt32(Request.QueryString["utype"]);
                issue = Convert.ToInt32(Request.QueryString["issue"]);
            }
            T_CooperConfig dto = wxll.Get_CooperConfig(cooper, issue);                              //取得配置
            long timestamp = DateTime.Now.ToUnixTimeStamp();                                        //时间戳
            string noncestr = TxtHelp.GetRandomString(16, true, true, true, false, "");             //随机字符串
            string signatrue = wxll.Get_signature(timestamp, noncestr);                             //signatrue
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
            if (Request["phone"] == null || Request["ctype"] == null || Request["issue"] == null) {
                return JsonFormat(new ExtJson { success = false, msg = "参数不能为空" });
            }
            string phone = Request["phone"].ToString();     //用户手机号码
            string ctype = Request["ctype"].ToString();     //公司类型
            string issue = Request["issue"].ToString();     //活动期号
            phone = phone.Substring(0, 7);
            string path = Server.MapPath(@"/Content/Txt/pwebconfig.txt");
            bool result = bll.ReadPhoneFliter(phone, path); //验证手机号码
            if (result)
            {
                int de_reslut = bll.DecidePhone(phone, Convert.ToInt32(ctype), Convert.ToInt32(issue));   //手机号码是否已经参加过活动
                if (de_reslut > 0)
                {
                    return JsonFormat(new ExtJson { success = false, msg = "当前手机号已经添加过活动" });
                }
                else {
                    //产生Session状态
                    bll.SendLoginMsgCode(Convert.ToInt32(ctype), Convert.ToInt32(issue));  //调用发送流量充值，这个方法里面判断一下登入状态是否已经存在，如果存在直接调用，否则先调用登入的短信,做到这里，考虑到一个问题，充值的流量是不是一个固定值???
                }
                return JsonFormat(new ExtJson { success = true, msg = "验证通过允许充值" });
            }
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
            if (Request["type"] == null || Request["code"] == null|| Request["phone"] == null) {
                return JsonFormat(new ExtJson { success = false, msg = "参数不能为空" });
            }
            string phone = Request["phone"].ToString();     //哪个手机号码接收到的
            int type = Convert.ToInt32(Request["type"]);    //1为登入2为充值
            int code = Convert.ToInt32(Request["code"]);    //验证码
            if (type == 1) {

            }
            int result = bll.TakeMsgCode(type, phone, code);    //将收到的验证码保存
            if (result > 0) {
                return JsonFormat(new ExtJson { success = true, msg = "保存验证码成功" });
            }
            return JsonFormat(new ExtJson { success = false, msg = "保存验证码失败" });
        }
        /// <summary>
        /// 测试signatrue
        /// </summary>
        /// <returns></returns>
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
        //test
        public ActionResult SendLoginPost() {
            string url = "http://www.fj.10086.cn/power/ADCECPortal/PowerLogin.aspx?ReturnUrl=ADCQDLPortal&test=t";
            bll.SendLoginPost(url);
            return View();
        }
    }
}