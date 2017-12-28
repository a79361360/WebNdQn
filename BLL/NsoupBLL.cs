using Common;
using CsharpHttpHelper;
using DAL;
using FJSZ.OA.Common.Web;
using Model.WxModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class NsoupBLL
    {
        CommonDAL cdal = new CommonDAL();
        NsoupDAL ndal = new NsoupDAL();
        public static string dlmobile = "10657047";     //登入短信号码
        public static string czmobile = "10657047";     //充值短信号码
        /// <summary>
        /// 发送登入短信动态码
        /// </summary>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        /// <returns>1成功-1异常-2没有配置T_CooperConfig-3未取到_csrf的值-4未添加到监控列表或者添加过多-5更新corpid,phone,csrf,cookie值时失败-6短信发送失败</returns>
        public int SendLoginMsg(int ctype,int issue) {
            Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SendLoginMsg 类型: " + ctype + " 期号：" + issue + " 开始发送登入短信");
            string url = "http://www.fj.10086.cn/power/ll800/ht/index.jsp";
            IList<T_CooperConfig> list = DataTableToList.ModelConvertHelper<T_CooperConfig>.ConvertToModel(cdal.GetCooperConfig(ctype, issue));
            if (list.Count > 0)
            {
                T_CooperConfig dto = list[0];
                NSoup.Nodes.Document doc;
                HttpHelper helpweb = new HttpHelper();  //初始实例化HttpHelper
                HttpResult result = new HttpResult();   //初始实例化HttpResult
                HttpItem item = new HttpItem()          //初始实例化HttpItem
                {
                    URL = url,//URL     必需项    
                    Method = "GET",//URL     可选项 默认为Get   
                    ProxyIp = "ieproxy",
                    ContentType = "application/x-www-form-urlencoded",//ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
                };
                try
                {
                    result = helpweb.GetHtml(item);
                    string cookie = fhcookie(result.Cookie);                //合并Cookie
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SendLoginMsg 类型: " + ctype + " 期号：" + issue + " Cookie值为:" + cookie);
                    doc = NSoup.NSoupClient.Parse(result.Html);
                    string csrf = doc.Select("#form2 input[name=_csrf]").Val();
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SendLoginMsg 类型: " + ctype + " 期号：" + issue + " csrf值为:" + csrf);
                    if (!string.IsNullOrEmpty(csrf))
                    {
                        int result_1 = ndal.IsExitsLogCache(ctype, issue);      //是否存在监控的项
                        if (result_1 == 1)
                        {
                            int result_2 = ndal.UpdateLogCacheInfo(ctype, issue, dto.corpid, dto.signphone, csrf, cookie);      //更新csrf值和临时cookie值(需要这个值才能发送短信)
                            if (result_2 == 1)
                            {
                                url = "http://www.fj.10086.cn/power/ll800/ht/login/checkInfo.do";
                                string data = "acount=" + dto.corpid + "&mobile=" + dto.signphone.MD5();
                                Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SendLoginMsg  类型: " + ctype + " 期号：" + issue + " 发短信的参数 " + data);
                                item = new HttpItem()
                                {
                                    URL = url,//URL     必需项    
                                    Method = "POST",//URL     可选项 默认为Get   
                                    ProxyIp = "ieproxy",
                                    Cookie = cookie,
                                    ContentType = "application/x-www-form-urlencoded",//ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
                                    Postdata = data
                                };
                                try
                                {
                                    result = helpweb.GetHtml(item);
                                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SendLoginMsg  类型: " + ctype + " 期号：" + issue + " 短信发送后的返回结果 " + result.Html);
                                    nsoupmsgdto msgdto = JsonConvert.DeserializeObject<nsoupmsgdto>(result.Html);
                                    if (msgdto.result == "ok")
                                        return 1;
                                    else
                                        return -6;
                                }
                                catch (Exception er)
                                {
                                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SendLoginMsg 短信发送异常:" + er.Message);
                                    return -1;
                                }
                            }
                            else {
                                Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SendLoginMsg  类型: " + ctype + " 期号：" + issue + " 更新corpid,phone,csrf,cookie值时失败 ");
                                return -5;
                            }
                        }
                        else {
                            Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SendLoginMsg  类型: " + ctype + " 期号：" + issue + " 未添加到监控列表或者添加过多 ");
                            return -4;
                        }
                    }
                    else {
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SendLoginMsg  类型: " + ctype + " 期号：" + issue + " 未取到_csrf的值 ");
                        return -3;
                    }
                }
                catch (Exception er)
                {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SendLoginMsg 类型: " + ctype + " 期号：" + issue + "访问异常:" + er.Message);
                    return -1;
                }
            }
            else
            {
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SendLoginMsg 类型: " + ctype + " 期号：" + issue + " 没有配置T_CooperConfig表:");
                return -2;
            }
        }
        /// <summary>
        /// 解析短信内容返回类型(登入|充值)和短信动态码,格式为: "1|000000"
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public string FilterContentTC(string mobile, string content)
        {
            string pstr = "本次登录流量800平台的动态码为：";
            if (content.IndexOf(pstr) != -1)
            {
                string yzm = content.Substring(content.LastIndexOf(pstr) + 17, 6);
                return "1|" + yzm;
            }
            else if (content.IndexOf("本次动态码为：") != -1) {
                string yzm = content.Substring(content.LastIndexOf(pstr) + 7, 6);
                return "2|" + yzm;
            }
            return "0|0";
        }
        /// <summary>
        /// 将收到短信保存到数据库
        /// </summary>
        /// <param name="type">1为登入2为充值</param>
        /// <param name="phone">发送短信的号码</param>
        /// <param name="xh">序号,新版无序号默认给0</param>
        /// <param name="code">动态码(短信验证码)</param>
        /// <param name="content">完整的短信字符串</param>
        /// <returns></returns>
        public int TakeMsgCode(int type,string phone,string xh,string code,string content) {
            return cdal.TakeMsgCode(type, phone, xh, code, content);
        }
        /// <summary>
        /// 生成登入Cookie并保存到数据库,根据短信动态码
        /// </summary>
        /// <param name="code"></param>
        /// <returns>1成功-1异常-2校验失败-3不存在正在进行时的监控列-4登入的Cookie更新到数据库失败</returns>
        public int CreateLoginCookie(int code) {
            IList<T_LogCache> list = DataTableToList.ModelConvertHelper<T_LogCache>.ConvertToModel(ndal.FindLogCacheState());
            if (list.Count > 0)
            {
                T_LogCache dto = list[0];
                string url = "http://www.fj.10086.cn/power/ll800/ht/login/toLoginUp.do";
                string data = "acount=" + dto.corpid + "&vcode=" + code.ToString().MD5() + "&_csrf=" + dto.csrf;
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "CreateLoginCookie提交账号与短信动态码 data: " + data);
                HttpHelper helpweb = new HttpHelper();  //初始实例化HttpHelper
                HttpResult result = new HttpResult();   //初始实例化HttpResult
                HttpItem item = new HttpItem()
                {
                    URL = url,                                          //URL     必需项    
                    Method = "POST",                                    //URL     可选项 默认为Get   
                    ProxyIp = "ieproxy",
                    ContentType = "application/x-www-form-urlencoded",  //ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
                    Postdata = data,
                    Cookie = dto.dlcookie
                };
                try
                {
                    result = helpweb.GetHtml(item);
                    logindto dto_1 = JsonConvert.DeserializeObject<logindto>(result.Html);
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "CreateLoginCookie " + dto.dlcookie);
                    string cookie = fhcookie(result.Cookie);                //合并Cookie
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "CreateLoginCookie" + cookie);
                    int result_1 = ndal.UpdateLogCacheDlCookie(cookie);     //更新登入cookie
                    if (result_1 == 1)
                    {
                        int result_2 = SignDlCookie();      //校验一下是否数据库更新成功并且登入Cookie是否有效
                        if (result_2 == 1)
                        {
                            Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "CreateLoginCookie 校验登入cookie成功");
                            return 1;
                        }
                        else {
                            Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "CreateLoginCookie 校验登入cookie失败");
                            return -2;
                        }
                    }
                    else {
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "CreateLoginCookie 将登入的Cookie更新到数据库失败");
                        return -4;
                    }
                }
                catch (Exception er)
                {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "CreateLoginCookie 生成登入Cookie异常 " + er.Message);
                    return -1;
                }
            }
            else {
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "CreateLoginCookie 不存在正在进行时的监控列");
                return -3;
            }
        }
        /// <summary>
        /// 合并Cookie
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string fhcookie(string str)
        {
            string[] str1 = str.Split(','); string tempstr = "";
            foreach (var item in str1)
            {
                string[] str2 = item.Split(';');
                if (tempstr == "") tempstr = str2[0];
                else tempstr += ";" + str2[0];
            }
            return tempstr;
        }
        /// <summary>
        /// 校验登入的cookie是否有效
        /// </summary>
        /// <returns>1成功-1异常-2校验失败-3不存在进行时的监控项</returns>
        public int SignDlCookie() {
            IList<T_LogCache> list = DataTableToList.ModelConvertHelper<T_LogCache>.ConvertToModel(ndal.FindLogCacheState());
            if (list.Count > 0)
            {
                T_LogCache dto = list[0];
                string url = "http://www.fj.10086.cn/power/ll800/ht/home/main.do";
                HttpHelper helpweb = new HttpHelper();  //初始实例化HttpHelper
                HttpResult result = new HttpResult();   //初始实例化HttpResult
                HttpItem item = new HttpItem()
                {
                    URL = url,                                          //URL     必需项    
                    Method = "GET",                                    //URL     可选项 默认为Get   
                    ProxyIp = "ieproxy",
                    ContentType = "application/x-www-form-urlencoded",  //ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
                    Cookie = dto.dlcookie
                };
                try
                {
                    result = helpweb.GetHtml(item);
                    NSoup.Nodes.Document doc = NSoup.NSoupClient.Parse(result.Html);
                    string text = doc.Select(".current a")[0].Text();
                    if (text == "首页" & doc.ChildNodes.Count == 2)
                    {
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SignDlCookie 校验登入Cookie成功 ctype=" + dto.ctype + " issue=" + dto.issue);
                        return 1;
                    }
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SignDlCookie 校验登入Cookie失败 ctype=" + dto.ctype + " issue=" + dto.issue);
                    return -2;
                }
                catch (Exception er)
                {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SignDlCookie 校验登入Cookie异常 " + er.Message);
                    return -1;
                }
            }
            else {
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SignDlCookie 不存在正在进行时的监控列");
                return -3;
            }
        }
    }
    //新发送短信返回结果实体类
    public class nsoupmsgdto {
        public string result { get; set; }
        public string message { get; set; }
    }
    //短信登入返回结果实体类
    public class logindto {
        public bool flag { get; set; }
    }
}
