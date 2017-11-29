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
using Fgly.Common.Expand;
using Model.ViewModel;

namespace BLL
{
    public class CommonBLL
    {
        CommonDAL dal = new CommonDAL();
        public bool ReadPhoneFliter(string phone, string path) {
            phone = phone.Substring(0, 7);
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
        /// 校验当前Openid是否已经参加过活动
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        /// <returns></returns>
        public int DecideOpenid(string openid, int ctype, int issue)
        {
            int result = dal.DecideOpenid(openid, ctype, issue);
            return result;
        }
        public int CtypeCount(int ctype,int issue) {
            return dal.CtypeInt(ctype, issue);
        }
        public int TakeFlowLog(int ctype, int issue, string phone)
        {
            return dal.TakeFlowLog(ctype, issue, phone, "");
        }
        /// <summary>
        /// 添加领取流量的记录
        /// </summary>
        /// <param name="ctype">公司类型</param>
        /// <param name="phone">手机号码</param>
        /// <returns>返回影响行数</returns>
        public int TakeFlowLog(int ctype, int issue, string phone,string openid) {
            return dal.TakeFlowLog(ctype, issue, phone, openid);
        }
        /// <summary>
        /// 删除flow记录
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public int RemoveFlowLog(IList<IdListDto> ids)
        {
            int sresult = 0;    //成功的数量
            if (ids.Count == 0)
                throw new ArgumentNullException();
            else
            {
                try
                {
                    foreach (var item in ids)
                    {
                        int gid = item.id;        //ID
                        int result = dal.RemoveFlowLog(gid);
                        sresult = sresult + result;
                    }
                }
                catch
                {
                    return -1000;
                }
            }
            return sresult;
        }
        /// <summary>
        /// 取得传送过来的验证码信息
        /// </summary>
        /// <param name="type">1登入验证码，2充值验证码</param>
        /// <param name="phone">手机号码</param>
        /// <param name="code">验证码</param>
        /// <param name="content">短信内容</param>
        /// <returns></returns>
        public int TakeMsgCode(int type,string phone, string xh, string code,string content) {
            return dal.TakeMsgCode(type, phone, xh, code, content);
        }
        /// <summary>
        /// 提取短信里面的验证码，并返回
        /// </summary>
        /// <param name="mobile">哪个号码发送的短信</param>
        /// <param name="contnet">短信的内容</param>
        /// <returns></returns>
        public string FilterMobileCode(string mobile, string content)
        {
            string pstr = "短信数字随机码为：";
            if (mobile == "10657532190000761") pstr = "下发的短信验证码是";
            if (content.IndexOf(pstr) != -1)
            {
                string yzm = content.Substring(content.LastIndexOf(pstr) + 9, 6);
                return yzm;
            }
            return "0";
        }
        public string FilterMobileXh(string mobile,string content) {
            string pstr = "序号为：";int len = 2;
            if (mobile == "10657532190000761")
            {
                pstr = "信序列号";
                len = content.Length - (content.LastIndexOf(pstr) + 4);
            }
            if (content.IndexOf(pstr) != -1)
            {
                string yzm = content.Substring(content.LastIndexOf(pstr) + 4, len);
                return yzm;
            }
            return "0";
        }
        /// <summary>
        /// 发送登入短信验证码
        /// </summary>
        /// <returns></returns>
        //public bool HttpCliendSendMsg(int ctype,int issue) {
        //    HttpClient cache = (HttpClient)FJSZ.OA.Common.CacheAccess.GetFromCache(ctype.ToString() + "login_state" + issue.ToString()); //Session状态是否存在
        //    if (cache == null) {
        //        IList<T_CooperConfig> list = DataTableToList.ModelConvertHelper<T_CooperConfig>.ConvertToModel(dal.GetCooperConfig(ctype, issue));
        //        if (list.Count > 0)
        //        {
        //            T_CooperConfig dto = list[0];
        //            WebHttp web = new WebHttp();
        //            web.HttpCliendSendMsg(dto.corpid, dto.username);
        //            //web.SendLoginPost(dto.corpid, dto.username, dto.userpwd, ctype, issue);   //生成登入cache,等待短信
        //            //HelpWebLogin(dto.corpid, dto.username, dto.userpwd, ctype, issue);
        //        }
        //    }
        //    return true;
        //}
        /// <summary>
        /// HttpClient形式取得短信登入
        /// </summary>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        /// <returns></returns>
        //public bool HttpCliendGetMsg(int ctype,int issue) {
        //    IList<T_CooperConfig> list = DataTableToList.ModelConvertHelper<T_CooperConfig>.ConvertToModel(dal.GetCooperConfig(ctype, issue));
        //    if (list.Count > 0)
        //    {
        //        T_CooperConfig dto = list[0];
        //        WebHttp web = new WebHttp();
        //        web.HttpCliendGetMsg(dto.corpid, dto.username, dto.userpwd);
        //    }
        //    return true;
        //}
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
        //public int SaveLoginState(string phone,int code) {
        //    IList<T_CooperConfig> list = DataTableToList.ModelConvertHelper<T_CooperConfig>.ConvertToModel(dal.GetCooperConfig(phone));
        //    if (list.Count > 0) {
        //        T_CooperConfig dto = list[0];
        //        int ctype = dto.ctype;int issue = dto.issue;    //公司类型，活动期号
        //        //HttpClient cache = (HttpClient)FJSZ.OA.Common.CacheAccess.GetFromCache(ctype.ToString() + "login_cache" + issue.ToString());    //登入cache
        //        HttpClient cache = (HttpClient)HttpContext.Current.Session[ctype.ToString() + "login_cache" + issue.ToString()];
        //        if (cache == null)
        //        {
        //            return -1005;   //登入cache已经失效，接收到登入的短信也没有用了
        //        }
        //        string result = (string)FJSZ.OA.Common.CacheAccess.GetFromCache(ctype.ToString() + "login_cache" + issue.ToString() + "str");
        //        WebHttp web = new WebHttp();
        //        //web.TakeCodeSaveLoginState(cache, code, result);
        //        web.TakeCodeSaveLoginState(cache, code, result);
        //    }
        //    return 1;
        //}
        /// <summary>
        /// 取得公司的活动配置信息，下拉列表
        /// </summary>
        /// <param name="state">状态</param>
        /// <returns></returns>
        public IList<T_CooperConfig> GetCooperConfigDrop(int state) {
            IList<T_CooperConfig> list = DataTableToList.ModelConvertHelper<T_CooperConfig>.ConvertToModel(dal.GetCooperConfigDrop(state));
            return list;
        }
        public T_CooperConfig GetCooperConfigById(int id)
        {
            IList<T_CooperConfig> list = DataTableToList.ModelConvertHelper<T_CooperConfig>.ConvertToModel(dal.GetCooperConfigById(id));
            if (list.Count > 0)
            {
                return list[0];
            }
            return new T_CooperConfig();
        }
        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="type">1背景图，2微信小图标，3公众号二维码</param>
        /// <param name="fileid">上传控件的名称</param>
        /// <param name="filename">文件名称</param>
        /// <returns></returns>
        public string DzpUploadBgUrl(int type, string fileid,string filename)
        {
            string pathx = "";                                        //图片地址
            string fname = "";
            if (type == 1){
                pathx = "/Content/Img/bg/";
                fname = "bg_";
            }
            if (type == 2)
            {
                pathx = "/Content/Img/wxico/";
                fname = "wxico_";
            }
            if (type == 3)
            {
                pathx = "/Content/Img/gzh/";
                fname = "gzh_";
            }
            if(string.IsNullOrEmpty(filename))
                filename = fname + DateTime.Now.ToUnixTimeStamp().ToString() + ".png";                 //图片名称 
            string vtime = "?v=" + DateTime.Now.ToUnixTimeStamp().ToString();           //用时间戳来做版本号
            string path = WebHelp.HttpUploadFile(pathx, filename, fileid); //返回完整的上传地址 
            if (!string.IsNullOrEmpty(path))
            {
                return pathx + filename + vtime;
                //int result = adal.UpdateDzpBgUrlByCooperid(cooperid, pathx + filename+ vtime);
                //if (result > 0) return pathx + filename + vtime;
            }
            return "";
        }
        public int SetCooper(T_CooperConfig dto) {
            int result = dal.SetCooper(dto.id, dto.ctype, dto.issue,dto.areatype,dto.gener, dto.title, dto.descride, dto.imgurl, dto.btnurl, dto.bgurl, dto.linkurl,dto.redirecturi,
               dto.corpid, dto.username, dto.userpwd, dto.signphone, dto.wx_appid, dto.wx_secret, dto.qrcode_url, dto.eachflow, dto.uplimit, dto.cutdate, dto.state);
            return result;
        }
        public int RemoveCoopers(IList<IdListDto> ids)
        {
            int sresult = 0;    //成功的数量
            if (ids.Count == 0)
                throw new ArgumentNullException();
            else
            {
                try
                {
                    foreach (var item in ids)
                    {
                        int gid = item.id;        //ID
                        int result = dal.CooperRemoveById(gid);
                        sresult = sresult + result;
                    }
                }
                catch
                {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/ActivityBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "方法 RemoveActivitys 异常：" + " 成功执行=" + sresult);
                    return -1000;
                }
            }
            return sresult;
        }
        /// <summary>
        /// 返回限制地址
        /// </summary>
        /// <returns></returns>
        public string ReturnConfigTxt(string area) {
            string txtpath = "/Content/Txt/pwebconfig.txt";
            if (area == "2") txtpath = "/Content/Txt/putianconfig.txt";
            if (area == "3") txtpath = "/Content/Txt/fujianconfig.txt";
            return txtpath;
        }








        //public void HelpWebSend(int ctype, int issue) {
        //    IList<T_CooperConfig> list = DataTableToList.ModelConvertHelper<T_CooperConfig>.ConvertToModel(dal.GetCooperConfig(ctype, issue));
        //    if (list.Count > 0)
        //    {
        //        T_CooperConfig dto = list[0];
        //        string baseurl = "http://www.fj.10086.cn/power/ADCECPortal/PowerLogin.aspx?ReturnUrl=ADCQDLPortal&test=t";
        //        //访问首页
        //        HttpHelper helpweb = new HttpHelper();
        //        HttpItem item = new HttpItem()
        //        {
        //            URL = baseurl,//URL     必需项    
        //            Method = "GET",//URL     可选项 默认为Get   
        //            ProxyIp = "ieproxy",
        //            ContentType = "application/x-www-form-urlencoded",//ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
        //        };
        //        HttpResult result = helpweb.GetHtml(item);
        //        string cookie = result.Cookie;
        //        //选择短信登入
        //        helpweb = new HttpHelper();
        //        item = new HttpItem()
        //        {
        //            URL = baseurl,//URL     必需项    
        //            Method = "POST",//URL     可选项 默认为Get   
        //            ProxyIp = "ieproxy",
        //            Cookie = cookie.ToString(),
        //            ContentType = "application/x-www-form-urlencoded",//ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
        //            Postdata = "__EVENTTARGET=rbl_PType%241&__EVENTARGUMENT=&__LASTFOCUS=&__VIEWSTATE=&__VIEWSTATEGENERATOR=CC3279BD&__VIEWSTATEENCRYPTED=&LoginType=1&SMSTimes=90&SMSAliasTimes=90&txtCorpCode=&txtUserName=&rbl_PType=2&txtPd=&txtCheckCode=&txtQDLRegisterUrl=%2FADCQDLPortal%2FProduction%2FProductOrderControl.aspx"
        //        };
        //        //请求的返回值对象
        //        result = helpweb.GetHtml(item);
        //        //发送短信
        //        helpweb = new HttpHelper();
        //        item = new HttpItem()
        //        {
        //            URL = baseurl,//URL     必需项    
        //            Method = "POST",//URL     可选项 默认为Get   
        //            ProxyIp = "ieproxy",
        //            Cookie = cookie.ToString(),
        //            ContentType = "application/x-www-form-urlencoded",//ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
        //            Postdata = "__EVENTTARGET=lbtn_GetSMS&__EVENTARGUMENT=&__LASTFOCUS=&__VIEWSTATE=&__VIEWSTATEGENERATOR=CC3279BD&__VIEWSTATEENCRYPTED=&LoginType=1&SMSTimes=90&SMSAliasTimes=90&txtCorpCode=" + dto.corpid + "&txtUserName=" + dto.username + "&rbl_PType=2&SMSP=&txtCheckCode=&txtQDLRegisterUrl=%2FADCQDLPortal%2FProduction%2FProductOrderControl.aspx"
        //        };
        //        result = helpweb.GetHtml(item);
        //        //获取请请求的Html
        //        string html = result.Html;
        //        //获取请求的Cookie
        //        //string cookie = result.Cookie;
        //        FJSZ.OA.Common.CacheAccess.InsertToCacheByTime(dto.corpid + "_cookie", cookie.ToString(), 3600);
        //    }
        //}
        //public void HelpWebLogin(int ctype, int issue) {
        //    IList<T_CooperConfig> list = DataTableToList.ModelConvertHelper<T_CooperConfig>.ConvertToModel(dal.GetCooperConfig(ctype, issue));
        //    if (list.Count > 0)
        //    {
        //        T_CooperConfig dto = list[0];
        //        string strCookies = (string)FJSZ.OA.Common.CacheAccess.GetFromCache(dto.corpid + "_cookie");
        //        HttpHelper helpweb = (HttpHelper)FJSZ.OA.Common.CacheAccess.GetFromCache(dto.corpid + "_helpweb");
        //        HttpItem item = new HttpItem()
        //        {
        //            URL = "http://www.fj.10086.cn/power/ADCECPortal/PowerLogin.aspx?ReturnUrl=ADCQDLPortal&test=t",//URL     必需项    
        //            Method = "post",//URL     可选项 默认为Get
        //            ProxyIp = "ieproxy",
        //            ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值
        //            Postdata = "__EVENTTARGET=&__EVENTARGUMENT=&__LASTFOCUS=&__VIEWSTATE=&__VIEWSTATEGENERATOR=CC3279BD&__VIEWSTATEENCRYPTED=&LoginType=1&SMSTimes=25&SMSAliasTimes=90&txtCorpCode=" + dto.corpid + "&txtUserName=" + dto.username + "&rbl_PType=2&SMSP=" + dto.userpwd + "&txtCheckCode=&button3=%E7%99%BB%E5%BD%95&txtQDLRegisterUrl=%2FADCQDLPortal%2FProduction%2FProductOrderControl.aspx",//Post要发送的数据
        //            //Cookie = strCookies,
        //            Allowautoredirect = true,//自动跳转
        //            AutoRedirectCookie = true//是否自动处理Cookie 
        //            ,Referer= "http://www.fj.10086.cn/power/ADCECPortal/PowerLogin.aspx?ReturnUrl=ADCQDLPortal&test=t"
        //        };
        //        //请求的返回值对象
        //        HttpResult result = helpweb.GetHtml(item);
        //        //获取请请求的Html
        //        string html = result.Html;
        //        //获取请求的Cookie
        //        string cookie1 = result.Cookie;
        //        string curcookie = HttpHelper.MergerCookies(strCookies, cookie1);
        //    }
        //}





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
