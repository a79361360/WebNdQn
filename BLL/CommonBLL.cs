using Common;
using Common.ExHelp;
using CsharpHttpHelper;
using DAL;
using FJSZ.OA.Common.Web;
using Model.CommonModel;
using Model.WxModel;
using System;
using System.Web;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class CommonBLL
    {
        CommonDAL dal = new CommonDAL();
        public bool ReadPhoneFliter(string phone, string path) {
            List<string> list = (List<string>)FJSZ.OA.Common.CacheAccess.GetFromCache("MoneyList");
            Common.Expend.LogTxtExpend.WriteLogs("/Logs/ExtProc_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "开始时间:" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fffff"));
            if (list == null)
            {
                list = new List<string>();
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/ExtProc_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "开始读txt时间:" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fffff"));
                //按行读取为字符串数组
                string[] lines = System.IO.File.ReadAllLines(path);
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/ExtProc_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "结束读txt时间:" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fffff"));
                foreach (string line in lines)
                {
                    list.Add(line);
                }
                FJSZ.OA.Common.CacheAccess.InsertToCacheByTimeE("MoneyList", list, 30);
            }
            Common.Expend.LogTxtExpend.WriteLogs("/Logs/ExtProc_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "list时间:" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fffff"));
            if (list.Contains(phone)) {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 校验当前手机号是否已经参加过活动
        /// </summary>
        /// <param name="phone">手机号码</param>
        /// <param name="ctype">公司类型</param>
        /// <param name="issue">活动期号</param>
        /// <returns></returns>
        public int DecidePhone(string phone,int ctype,int issue) {
            int result = dal.DecidePhone(phone, ctype, issue);
            return result;
        }
        /// <summary>
        /// 添加领取流量的记录
        /// </summary>
        /// <param name="ctype">公司类型</param>
        /// <param name="phone">手机号码</param>
        /// <returns>返回影响行数</returns>
        public int TakeFlowLog(int ctype,string phone) {
            return dal.TakeFlowLog(ctype, phone);
        }
        /// <summary>
        /// 取得传送过来的验证码信息
        /// </summary>
        /// <param name="type">1登入验证码，2充值验证码</param>
        /// <param name="phone">手机号码</param>
        /// <param name="code">验证码</param>
        /// <returns></returns>
        public int TakeMsgCode(int type,string phone, int code) {
            return dal.TakeMsgCode(type, phone, code);
        }
        /// <summary>
        /// 发送登入短信验证码
        /// </summary>
        /// <returns></returns>
        public bool SendLoginMsgCode(int ctype,int issue) {
            HttpClient cache = (HttpClient)FJSZ.OA.Common.CacheAccess.GetFromCache(ctype.ToString() + "login_state" + issue.ToString()); //Session状态是否存在
            if (cache == null) {
                IList<T_CooperConfig> list = DataTableToList.ModelConvertHelper<T_CooperConfig>.ConvertToModel(dal.GetCooperConfig(ctype, issue));
                if (list.Count > 0)
                {
                    T_CooperConfig dto = list[0];
                    WebHttp web = new WebHttp();
                    web.SendLoginPost(dto.corpid, dto.username, dto.userpwd, ctype, issue);   //生成登入cache,等待短信
                    //HelpWebLogin(dto.corpid, dto.username, dto.userpwd, ctype, issue);
                }
            }
            //WebHttp web = new WebHttp();
            //string url = "";
            //string data = "";
            //try {
            //    web.Post(url, data);
                return true;
            //}
            //catch
            //{
            //    return false;
            //}
        }
        /// <summary>
        /// 发送充值流量短信验证码
        /// </summary>
        /// <returns></returns>
        public bool SendFlowMsgCode()
        {
            WebHttp web = new WebHttp();
            string url = "";
            string data = "";
            try
            {
                web.Post(url, data);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 保存需要模拟POST提交需要的数据信息
        /// </summary>
        /// <returns></returns>
        public int SaveLoginState(string phone,int code) {
            IList<T_CooperConfig> list = DataTableToList.ModelConvertHelper<T_CooperConfig>.ConvertToModel(dal.GetCooperConfig(phone));
            if (list.Count > 0) {
                T_CooperConfig dto = list[0];
                int ctype = dto.ctype;int issue = dto.issue;    //公司类型，活动期号
                //HttpClient cache = (HttpClient)FJSZ.OA.Common.CacheAccess.GetFromCache(ctype.ToString() + "login_cache" + issue.ToString());    //登入cache
                HttpClient cache = (HttpClient)HttpContext.Current.Session[ctype.ToString() + "login_cache" + issue.ToString()];
                if (cache == null)
                {
                    return -1005;   //登入cache已经失效，接收到登入的短信也没有用了
                }
                string result = (string)FJSZ.OA.Common.CacheAccess.GetFromCache(ctype.ToString() + "login_cache" + issue.ToString() + "str");
                WebHttp web = new WebHttp();
                //web.TakeCodeSaveLoginState(cache, code, result);
                web.TakeCodeSaveLoginState(cache, code, result);
            }
            return 1;
        }














        public void HelpWebLogin(string corpid, string username, string userpwd, int ctype, int issue) {
            HttpHelper helpweb = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = "http://www.fj.10086.cn/power/ADCECPortal/PowerLogin.aspx?ReturnUrl=ADCQDLPortal&test=t",//URL     必需项    
                Method = "get",//URL     可选项 默认为Get   
                ContentType = "text/html",//返回类型    可选项有默认值   
                //ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
            };
            //请求的返回值对象
            HttpResult result = helpweb.GetHtml(item);
            //获取请请求的Html
            string html = result.Html;
            //获取请求的Cookie
            string cookie = result.Cookie;

            string postdata = "LoginType=1&SMSTimes=90&SMSAliasTimes=90&txtCorpCode=" + corpid + "&txtUserName" + username + "&rbl_PType=1&txtPd=" + userpwd.Insert(3, "1@3S") + "&txtCheckCode=&button3=登录&txtQDLRegisterUrl=/ADCQDLPortal/Production/ProductOrderControl.aspx";
            helpweb = new HttpHelper();
            item = new HttpItem()
            {
                URL = "http://www.fj.10086.cn/power/ADCECPortal/PowerLogin.aspx?ReturnUrl=ADCQDLPortal&test=t",//URL     必需项    
                Method = "post",//URL     可选项 默认为Get   
                ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值
                Postdata = postdata,//Post要发送的数据
                Allowautoredirect = true,//自动跳转
                AutoRedirectCookie = true//是否自动处理Cookie 
            };
            //请求的返回值对象
            result = helpweb.GetHtml(item);
            //获取请请求的Html
            html = result.Html;
            //获取请求的Cookie
            string cookie1 = result.Cookie;
            string curcookie = HttpHelper.MergerCookies(cookie, cookie1);

        }





        /// <summary>
        /// 模拟提交数据，完成给用户发送流量功能
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool SendFlowToUsered(int code) {
            WebHttp web = new WebHttp();
            string url = "";
            string data = "";
            try
            {
                string result = web.Post(url, data);
                return true;
            }
            catch
            {
                return false;
            }
        }
        //test
        public void SendLoginPost(string url) {
            WebHttp web = new WebHttp();
            //string data = "LoginType=1&SMSTimes=90&SMSAliasTimes=90&txtCorpCode=5913855431&txtUserName=administrator&rbl_PType=1&txtPd=nd11@3S23456&txtCheckCode=&button3=登录&txtQDLRegisterUrl=/ADCQDLPortal/Production/ProductOrderControl.aspx";
            //CookieContainer cookie = new CookieContainer();
            //web.GetHttpCookies(url, data,ref cookie);
            web.LoginCnblogs();
            //web.GetHttpCookies(url, data, ref cookie);
        }

    }
}
