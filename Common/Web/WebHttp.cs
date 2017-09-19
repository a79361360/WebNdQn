using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace FJSZ.OA.Common.Web
{
    public sealed class WebHttp
    {
        /// <summary>
        /// 发送Get请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <returns></returns>
        public string Get(string url)
        {
            //System.Net.ServicePointManager.DefaultConnectionLimit = 512;
            //int i = System.Net.ServicePointManager.DefaultPersistentConnectionLimit;
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Timeout = 20000;
            request.AllowAutoRedirect = false;
            request.ServicePoint.Expect100Continue = false;

            try
            {
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        return reader.ReadToEnd();
                    }
                }
                else
                    return response.StatusDescription;
            }
            catch (Exception e)
            {
                return e.Message;
            }
            finally
            {
                request = null;
            }
        }
        /// <summary>
        /// 发送Post请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="data">发送的数据</param>
        /// <returns></returns>
        public string Post(string url, string data)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Timeout = 20000;
            request.AllowAutoRedirect = false;
            request.ServicePoint.Expect100Continue = false;

            try
            {
                using (System.IO.StreamWriter write = new System.IO.StreamWriter(request.GetRequestStream()))
                {
                    write.Write(data);
                }

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        return reader.ReadToEnd();
                    }
                }
                else
                    return response.StatusDescription;
            }
            catch (Exception e)
            {
                return e.Message;
            }
            finally
            {
                request = null;
            }
        }
















        //public List<string> GetHttpCookies(string url, string data, ref CookieContainer cookie)
        public void GetHttpCookies(string url, string data, ref CookieContainer cookie)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Timeout = 20000;
            request.CookieContainer = new CookieContainer();
            request.AllowAutoRedirect = false;
            request.ServicePoint.Expect100Continue = false;

            try
            {
                using (System.IO.StreamWriter write = new System.IO.StreamWriter(request.GetRequestStream()))
                {
                    write.Write(data);
                }

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    cookie.Add(response.Cookies);
                    //var resultAsync = request.GetResponseAsync();
                    //return resultAsync.Result.Headers.GetValues("Set-Cookie").ToList();
                }
            }
            catch (Exception e)
            {

            }
            finally
            {
                request = null;
            }
        }

        //test
        public void LoginCnblogs()
        {
            HttpClient httpClient = new HttpClient();
            httpClient.MaxResponseContentBufferSize = 256000;
            httpClient.DefaultRequestHeaders.ExpectContinue = false;
            httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.143 Safari/537.36");
            String url = "http://www.fj.10086.cn/power/ADCECPortal/PowerLogin.aspx?ReturnUrl=ADCQDLPortal&test=t";
            HttpResponseMessage response = httpClient.GetAsync(new Uri(url)).Result;
            String result = response.Content.ReadAsStringAsync().Result;
            
            //do
            //{
                String __EVENTTARGET = new Regex("id=\"__EVENTTARGET\" value=\"(.*?)\"").Match(result).Groups[1].Value;
                String __EVENTARGUMENT = new Regex("id=\"__EVENTARGUMENT\" value=\"(.*?)\"").Match(result).Groups[1].Value;
                String __LASTFOCUS = new Regex("id=\"__LASTFOCUS\" value=\"(.*?)\"").Match(result).Groups[1].Value;
                String __VIEWSTATE = new Regex("id=\"__VIEWSTATE\" value=\"(.*?)\"").Match(result).Groups[1].Value;
                String __VIEWSTATEENCRYPTED = new Regex("id=\"__VIEWSTATEENCRYPTED\" value=\"(.*?)\"").Match(result).Groups[1].Value;
                //string data = "LoginType=1&SMSTimes=90&SMSAliasTimes=90&txtCorpCode=5913855431&txtUserName=administrator&rbl_PType=1&txtPd=nd11@3S23456&txtCheckCode=&button3=登录&txtQDLRegisterUrl=/ADCQDLPortal/Production/ProductOrderControl.aspx";
                //开始登录
                url = "http://www.fj.10086.cn/power/ADCECPortal/PowerLogin.aspx?ReturnUrl=ADCQDLPortal&test=t";
                List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
                paramList.Add(new KeyValuePair<string, string>("__EVENTTARGET", __EVENTTARGET));
                paramList.Add(new KeyValuePair<string, string>("__EVENTARGUMENT", __EVENTARGUMENT));
                paramList.Add(new KeyValuePair<string, string>("__LASTFOCUS", __LASTFOCUS));
                paramList.Add(new KeyValuePair<string, string>("__VIEWSTATE", __VIEWSTATE));
                paramList.Add(new KeyValuePair<string, string>("__VIEWSTATEENCRYPTED", __VIEWSTATEENCRYPTED));

                paramList.Add(new KeyValuePair<string, string>("LoginType", "1"));
                paramList.Add(new KeyValuePair<string, string>("SMSTimes", "90"));
                paramList.Add(new KeyValuePair<string, string>("SMSAliasTimes", "90"));
                paramList.Add(new KeyValuePair<string, string>("txtCorpCode", "5913855431"));
                paramList.Add(new KeyValuePair<string, string>("txtUserName", "administrator"));
                paramList.Add(new KeyValuePair<string, string>("rbl_PType", "1"));

                paramList.Add(new KeyValuePair<string, string>("txtPd", "nd11@3S23456"));
                paramList.Add(new KeyValuePair<string, string>("txtCheckCode", ""));
                paramList.Add(new KeyValuePair<string, string>("button3", "登录"));
                paramList.Add(new KeyValuePair<string, string>("txtQDLRegisterUrl", "/ADCQDLPortal/Production/ProductOrderControl.aspx"));
            
                response = httpClient.PostAsync(new Uri(url), new FormUrlEncodedContent(paramList)).Result;
                result = response.Content.ReadAsStringAsync().Result;
                string[] strCookies = (string[])response.Headers.GetValues("Set-Cookie");
                //临时跳转
                ECLogin(httpClient,0,0);
            //} while (result.Contains("验证码错误，麻烦您重新输入"));
            //用完要记得释放
            httpClient.Dispose();
        }
        /// <summary>
        /// 发送登入的短信验证码
        /// </summary>
        /// <param name="corpid">企业代码</param>
        /// <param name="username">企业账号</param>
        public void HttpCliendSendMsg(string corpid,string username) {
            HttpClient httpClient = new HttpClient();
            httpClient.MaxResponseContentBufferSize = 256000;
            httpClient.DefaultRequestHeaders.ExpectContinue = false;
            httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.143 Safari/537.36");
            String url = "http://www.fj.10086.cn/power/ADCECPortal/PowerLogin.aspx?ReturnUrl=ADCQDLPortal&test=t";
            //HttpResponseMessage response = httpClient.GetAsync(new Uri(url)).Result;
            //String result = response.Content.ReadAsStringAsync().Result;

            //String __EVENTTARGET = new Regex("id=\"__EVENTTARGET\" value=\"(.*?)\"").Match(result).Groups[1].Value;
            //String __EVENTARGUMENT = new Regex("id=\"__EVENTARGUMENT\" value=\"(.*?)\"").Match(result).Groups[1].Value;
            //String __LASTFOCUS = new Regex("id=\"__LASTFOCUS\" value=\"(.*?)\"").Match(result).Groups[1].Value;
            //String __VIEWSTATE = new Regex("id=\"__VIEWSTATE\" value=\"(.*?)\"").Match(result).Groups[1].Value;
            //String __VIEWSTATEGENERATOR = new Regex("id=\"__VIEWSTATEGENERATOR\" value=\"(.*?)\"").Match(result).Groups[1].Value;
            //String __VIEWSTATEENCRYPTED = new Regex("id=\"__VIEWSTATEENCRYPTED\" value=\"(.*?)\"").Match(result).Groups[1].Value;

            List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
            //paramList.Add(new KeyValuePair<string, string>("__EVENTTARGET", __EVENTTARGET));
            //paramList.Add(new KeyValuePair<string, string>("__EVENTARGUMENT", __EVENTARGUMENT));
            //paramList.Add(new KeyValuePair<string, string>("__LASTFOCUS", __LASTFOCUS));
            //paramList.Add(new KeyValuePair<string, string>("__VIEWSTATE", __VIEWSTATE));
            //paramList.Add(new KeyValuePair<string, string>("__VIEWSTATEGENERATOR", __VIEWSTATEGENERATOR));
            //paramList.Add(new KeyValuePair<string, string>("__VIEWSTATEENCRYPTED", __VIEWSTATEENCRYPTED));
            paramList.Add(new KeyValuePair<string, string>("__EVENTTARGET", "lbtn_GetSMS"));
            paramList.Add(new KeyValuePair<string, string>("__EVENTARGUMENT", ""));
            paramList.Add(new KeyValuePair<string, string>("__LASTFOCUS", ""));
            paramList.Add(new KeyValuePair<string, string>("__VIEWSTATE", ""));
            paramList.Add(new KeyValuePair<string, string>("__VIEWSTATEGENERATOR", "CC3279BD"));
            paramList.Add(new KeyValuePair<string, string>("__VIEWSTATEENCRYPTED", ""));
            

            paramList.Add(new KeyValuePair<string, string>("LoginType", "1"));
            paramList.Add(new KeyValuePair<string, string>("SMSTimes", "90"));
            paramList.Add(new KeyValuePair<string, string>("SMSAliasTimes", "90"));
            paramList.Add(new KeyValuePair<string, string>("txtCorpCode", corpid));
            paramList.Add(new KeyValuePair<string, string>("txtUserName", username));
            paramList.Add(new KeyValuePair<string, string>("rbl_PType", "2"));
            paramList.Add(new KeyValuePair<string, string>("SMSP", ""));
            paramList.Add(new KeyValuePair<string, string>("txtCheckCode", ""));
            paramList.Add(new KeyValuePair<string, string>("txtQDLRegisterUrl", "/ADCQDLPortal/Production/ProductOrderControl.aspx"));
            HttpResponseMessage response = httpClient.PostAsync(new Uri(url), new FormUrlEncodedContent(paramList)).Result;
            string result = response.Content.ReadAsStringAsync().Result;
            string[] strCookies = (string[])response.Headers.GetValues("Set-Cookie");
            FJSZ.OA.Common.CacheAccess.InsertToCacheByTime(corpid+"_cookie", strCookies, 3600);
            FJSZ.OA.Common.CacheAccess.InsertToCacheByTime(corpid + "_httpclient", httpClient, 3600);
        }
        /// <summary>
        /// 根据企业代码，企业账号，短信验证码登入
        /// </summary>
        /// <param name="corpid">企业代码</param>
        /// <param name="username">企业账号</param>
        /// <param name="code">短信验证码</param>
        public void HttpCliendGetMsg(string corpid, string username, string code) {
            //HttpClient httpClient = new HttpClient();
            HttpClient httpClient = (HttpClient)FJSZ.OA.Common.CacheAccess.GetFromCache(corpid + "_httpclient");
            //httpClient.MaxResponseContentBufferSize = 256000;
            //httpClient.DefaultRequestHeaders.ExpectContinue = false;
            //httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.143 Safari/537.36");
            //下面的设置cookie无法成功
            //string[] strCookies = (string[])FJSZ.OA.Common.CacheAccess.GetFromCache(corpid + "_cookie");
            //httpClient.DefaultRequestHeaders.Add("Cookie", strCookies);
            

            String url = "http://www.fj.10086.cn/power/ADCECPortal/PowerLogin.aspx?ReturnUrl=ADCQDLPortal&test=t";
            
            List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
            paramList.Add(new KeyValuePair<string, string>("__EVENTTARGET", ""));
            paramList.Add(new KeyValuePair<string, string>("__EVENTARGUMENT", ""));
            paramList.Add(new KeyValuePair<string, string>("__LASTFOCUS", ""));
            paramList.Add(new KeyValuePair<string, string>("__VIEWSTATE", ""));
            paramList.Add(new KeyValuePair<string, string>("__VIEWSTATEGENERATOR", "CC3279BD"));
            paramList.Add(new KeyValuePair<string, string>("__VIEWSTATEENCRYPTED", ""));

            paramList.Add(new KeyValuePair<string, string>("LoginType", "1"));
            paramList.Add(new KeyValuePair<string, string>("SMSTimes", "0"));
            paramList.Add(new KeyValuePair<string, string>("SMSAliasTimes", "90"));
            paramList.Add(new KeyValuePair<string, string>("txtCorpCode", corpid));
            paramList.Add(new KeyValuePair<string, string>("txtUserName", username));
            paramList.Add(new KeyValuePair<string, string>("rbl_PType", "2"));
            paramList.Add(new KeyValuePair<string, string>("SMSP", code.ToString()));
            paramList.Add(new KeyValuePair<string, string>("txtCheckCode", ""));
            paramList.Add(new KeyValuePair<string, string>("button3", "登录"));
            paramList.Add(new KeyValuePair<string, string>("txtQDLRegisterUrl", "/ADCQDLPortal/Production/ProductOrderControl.aspx"));

            HttpResponseMessage response = httpClient.PostAsync(new Uri(url), new FormUrlEncodedContent(paramList)).Result;
            string result = response.Content.ReadAsStringAsync().Result;
            ECLogin(httpClient, 21, 1);
        }


        /// <summary>
        /// 生成登入cache
        /// </summary>
        /// <param name="corpid">企业代码</param>
        /// <param name="username">用户账号</param>
        /// <param name="userpwd">用户密码</param>
        /// <param name="ctype">公司类型</param>
        /// <param name="issue">活动期号</param>
        /// <returns></returns>
        public void SendLoginPost(string corpid, string username, string userpwd,int ctype,int issue)
        {
            HttpClient httpClient = new HttpClient();
            //httpClient.MaxResponseContentBufferSize = 256000;
            httpClient.DefaultRequestHeaders.ExpectContinue = false;
            //httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.143 Safari/537.36");
            String url = "http://www.fj.10086.cn/power/ADCECPortal/PowerLogin.aspx?ReturnUrl=ADCQDLPortal&test=t";
            HttpResponseMessage response = httpClient.GetAsync(new Uri(url)).Result;
            String result = response.Content.ReadAsStringAsync().Result;

            String __EVENTTARGET = new Regex("id=\"__EVENTTARGET\" value=\"(.*?)\"").Match(result).Groups[1].Value;
            String __EVENTARGUMENT = new Regex("id=\"__EVENTARGUMENT\" value=\"(.*?)\"").Match(result).Groups[1].Value;
            String __LASTFOCUS = new Regex("id=\"__LASTFOCUS\" value=\"(.*?)\"").Match(result).Groups[1].Value;
            String __VIEWSTATE = new Regex("id=\"__VIEWSTATE\" value=\"(.*?)\"").Match(result).Groups[1].Value;
            String __VIEWSTATEENCRYPTED = new Regex("id=\"__VIEWSTATEENCRYPTED\" value=\"(.*?)\"").Match(result).Groups[1].Value;
            //string data = "LoginType=1&SMSTimes=90&SMSAliasTimes=90&txtCorpCode=5913855431&txtUserName=administrator&rbl_PType=1&txtPd=nd11@3S23456&txtCheckCode=&button3=登录&txtQDLRegisterUrl=/ADCQDLPortal/Production/ProductOrderControl.aspx";
            //开始登录
            url = "http://www.fj.10086.cn/power/ADCECPortal/PowerLogin.aspx?ReturnUrl=ADCQDLPortal&test=t";
            List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
            paramList.Add(new KeyValuePair<string, string>("__EVENTTARGET", __EVENTTARGET));
            paramList.Add(new KeyValuePair<string, string>("__EVENTARGUMENT", __EVENTARGUMENT));
            paramList.Add(new KeyValuePair<string, string>("__LASTFOCUS", __LASTFOCUS));
            paramList.Add(new KeyValuePair<string, string>("__VIEWSTATE", __VIEWSTATE));
            paramList.Add(new KeyValuePair<string, string>("__VIEWSTATEENCRYPTED", __VIEWSTATEENCRYPTED));

            paramList.Add(new KeyValuePair<string, string>("LoginType", "1"));
            paramList.Add(new KeyValuePair<string, string>("SMSTimes", "90"));
            paramList.Add(new KeyValuePair<string, string>("SMSAliasTimes", "90"));
            paramList.Add(new KeyValuePair<string, string>("txtCorpCode", corpid));
            paramList.Add(new KeyValuePair<string, string>("txtUserName", username));
            paramList.Add(new KeyValuePair<string, string>("rbl_PType", "1"));

            paramList.Add(new KeyValuePair<string, string>("txtPd", userpwd.Insert(3, "1@3S")));
            paramList.Add(new KeyValuePair<string, string>("txtCheckCode", ""));
            paramList.Add(new KeyValuePair<string, string>("button3", "登录"));
            paramList.Add(new KeyValuePair<string, string>("txtQDLRegisterUrl", "/ADCQDLPortal/Production/ProductOrderControl.aspx"));

            response = httpClient.PostAsync(new Uri(url), new FormUrlEncodedContent(paramList)).Result;
            result = response.Content.ReadAsStringAsync().Result;
            string[] strCookies = (string[])response.Headers.GetValues("Set-Cookie");
            //临时跳转
            ECLogin(httpClient, ctype, issue);
            //FJSZ.OA.Common.CacheAccess.InsertToCacheByTime(ctype.ToString() + "login_cache" + issue.ToString(), httpClient, 300);   //20分钟
            HttpContext.Current.Session[ctype.ToString() + "login_cache" + issue.ToString()] = httpClient;
            //HttpContext.Current.Session[ctype.ToString() + "login_cookie" + issue.ToString()] = CookietoString(strCookies);
            //用完要记得释放
            //httpClient.Dispose();
        }
        /// <summary>
        /// 生成登入cache的过程有一个后台的自动跳转，这边也需要模拟
        /// </summary>
        /// <param name="httpClient"></param>
        public void ECLogin(HttpClient httpClient,int ctype,int issue)
        {
            //string url = "http://www.fj.10086.cn/power/ADCECPortal/EC/ECUserLoginProcess.aspx?loginType=1&SMSCheck=0&Ref=%2fpower%2fNewGroupPortal%2fMYPower100%2fIndex.html";
              string url = "http://www.fj.10086.cn/power/ADCECPortal/EC/ECUserLoginProcess.aspx?loginType=1&SMSCheck=1&Ref=%2fpower%2fNewGroupPortal%2fMYPower100%2fIndex.html";
            HttpResponseMessage response = httpClient.GetAsync(new Uri(url)).Result;
            string result = response.Content.ReadAsStringAsync().Result;
            FJSZ.OA.Common.CacheAccess.InsertToCacheByTime(ctype.ToString() + "login_cache" + issue.ToString() + "str", result, 300);   //20分钟
        }
        public void TakeCodeSaveLoginState(HttpClient httpClient, int code)
        {
            string url = "http://www.fj.10086.cn/power/ADCECPORTAL/EC/SMSLogin.aspx?msgType=SMS&logType=1";
            HttpResponseMessage response = httpClient.GetAsync(new Uri(url)).Result;
            String result = response.Content.ReadAsStringAsync().Result;

            String __LASTFOCUS = new Regex("id=\"__LASTFOCUS\" value=\"(.*?)\"").Match(result).Groups[1].Value;
            String __EVENTTARGET = new Regex("id=\"__EVENTTARGET\" value=\"(.*?)\"").Match(result).Groups[1].Value;
            String __EVENTARGUMENT = new Regex("id=\"__EVENTARGUMENT\" value=\"(.*?)\"").Match(result).Groups[1].Value;
            String __VIEWSTATE = new Regex("id=\"__VIEWSTATE\" value=\"(.*?)\"").Match(result).Groups[1].Value;
            String __VIEWSTATEGENERATOR = new Regex("id=\"__VIEWSTATEGENERATOR\" value=\"(.*?)\"").Match(result).Groups[1].Value;
            String __VIEWSTATEENCRYPTED = new Regex("id=\"__VIEWSTATEENCRYPTED\" value=\"(.*?)\"").Match(result).Groups[1].Value;

            List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
            paramList.Add(new KeyValuePair<string, string>("__LASTFOCUS", __LASTFOCUS));
            paramList.Add(new KeyValuePair<string, string>("__EVENTTARGET", __EVENTTARGET));
            paramList.Add(new KeyValuePair<string, string>("__EVENTARGUMENT", __EVENTARGUMENT));
            paramList.Add(new KeyValuePair<string, string>("__VIEWSTATE", __VIEWSTATE));
            paramList.Add(new KeyValuePair<string, string>("__VIEWSTATEGENERATOR", __VIEWSTATEGENERATOR));

            paramList.Add(new KeyValuePair<string, string>("__VIEWSTATEENCRYPTED", __VIEWSTATEENCRYPTED));

            paramList.Add(new KeyValuePair<string, string>("ScriptManager1", "UpdatePanel1|btnSubmit"));
            paramList.Add(new KeyValuePair<string, string>("txtVCode", code.ToString()));
            paramList.Add(new KeyValuePair<string, string>("MobileAndEmailCheck", ""));
            paramList.Add(new KeyValuePair<string, string>("__ASYNCPOST", "true"));
            paramList.Add(new KeyValuePair<string, string>("btnSubmit", "登录"));
            url = "http://www.fj.10086.cn/power/ADCECPORTAL/EC/SMSLogin.aspx?msgType=SMS&logType=1";
            response = httpClient.PostAsync(new Uri(url), new FormUrlEncodedContent(paramList)).Result;

            // HttpResponseMessage response = httpClient.GetAsync(new Uri(url)).Result;
            string[] strCookies = (string[])response.Headers.GetValues("Set-Cookie");
            String result1 = response.Content.ReadAsStringAsync().Result;
        }
        public void TakeCodeSaveLoginState(HttpClient httpClient,int code,string result) {
            string url = "http://www.fj.10086.cn/power/ADCECPORTAL/EC/SMSLogin.aspx?msgType=SMS&logType=1";
            
            String __LASTFOCUS = new Regex("id=\"__LASTFOCUS\" value=\"(.*?)\"").Match(result).Groups[1].Value;
            String __EVENTTARGET = new Regex("id=\"__EVENTTARGET\" value=\"(.*?)\"").Match(result).Groups[1].Value;
            String __EVENTARGUMENT = new Regex("id=\"__EVENTARGUMENT\" value=\"(.*?)\"").Match(result).Groups[1].Value;
            String __VIEWSTATE = new Regex("id=\"__VIEWSTATE\" value=\"(.*?)\"").Match(result).Groups[1].Value;
            String __VIEWSTATEGENERATOR = new Regex("id=\"__VIEWSTATEGENERATOR\" value=\"(.*?)\"").Match(result).Groups[1].Value;
            String __VIEWSTATEENCRYPTED = new Regex("id=\"__VIEWSTATEENCRYPTED\" value=\"(.*?)\"").Match(result).Groups[1].Value;

            List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
            paramList.Add(new KeyValuePair<string, string>("__LASTFOCUS", __LASTFOCUS));
            paramList.Add(new KeyValuePair<string, string>("__EVENTTARGET", __EVENTTARGET));
            paramList.Add(new KeyValuePair<string, string>("__EVENTARGUMENT", __EVENTARGUMENT));
            paramList.Add(new KeyValuePair<string, string>("__VIEWSTATE", __VIEWSTATE));
            paramList.Add(new KeyValuePair<string, string>("__VIEWSTATEGENERATOR", __VIEWSTATEGENERATOR));
            
            paramList.Add(new KeyValuePair<string, string>("__VIEWSTATEENCRYPTED", __VIEWSTATEENCRYPTED));

            paramList.Add(new KeyValuePair<string, string>("ScriptManager1", "UpdatePanel1|btnSubmit"));
            paramList.Add(new KeyValuePair<string, string>("txtVCode", code.ToString()));
            paramList.Add(new KeyValuePair<string, string>("MobileAndEmailCheck", ""));
            paramList.Add(new KeyValuePair<string, string>("__ASYNCPOST", "true"));
            paramList.Add(new KeyValuePair<string, string>("btnSubmit", "登录"));
            HttpResponseMessage response = httpClient.PostAsync(new Uri(url), new FormUrlEncodedContent(paramList)).Result;
            
            string[] strCookies = (string[])response.Headers.GetValues("Set-Cookie");
            String result1 = response.Content.ReadAsStringAsync().Result;
            url = "http://www.fj.10086.cn/power/adcecportal/EC/ECTransPage.aspx?refurl=NpK9bxYuGA%2bywHG1pfz8t8OgWt7%2fBdI2svgx3Q0663yX4lYfMd4eDijWpyadY7xdwOWhBbUiR7D3GkWXzztfZwtt2nK5xkg%2b0wljagpkaGF5RudZ9ahjoR7yEIEtQ9I4VggqI9mIrV%2bfUAbO8q%2f7mdw9shPUtRtRHnhjj4ufgSns53198sggPY%2b5AAsDkOumo6Y9QmeDzB5pIZXGzYeSPIHJBP1ZU09JyijyEysY%2f3w%3d";
            response = httpClient.GetAsync(new Uri(url)).Result;
            String result2 = response.Content.ReadAsStringAsync().Result;
        }
        //public void TakeCodeSaveLoginState(string strcookie, int code, string result)
        //{
        //    string url = "http://www.fj.10086.cn/power/ADCECPORTAL/EC/SMSLogin.aspx?msgType=SMS&logType=1";

        //    var baseAddress = new Uri(url);
        //    using (var handler = new HttpClientHandler { UseCookies = false })
        //    using (var httpClient = new HttpClient(handler) { BaseAddress = baseAddress })
        //    {
        //        String __LASTFOCUS = new Regex("id=\"__LASTFOCUS\" value=\"(.*?)\"").Match(result).Groups[1].Value;
        //        String __EVENTTARGET = new Regex("id=\"__EVENTTARGET\" value=\"(.*?)\"").Match(result).Groups[1].Value;
        //        String __EVENTARGUMENT = new Regex("id=\"__EVENTARGUMENT\" value=\"(.*?)\"").Match(result).Groups[1].Value;
        //        String __VIEWSTATE = new Regex("id=\"__VIEWSTATE\" value=\"(.*?)\"").Match(result).Groups[1].Value;
        //        String __VIEWSTATEGENERATOR = new Regex("id=\"__VIEWSTATEGENERATOR\" value=\"(.*?)\"").Match(result).Groups[1].Value;
        //        String __VIEWSTATEENCRYPTED = new Regex("id=\"__VIEWSTATEENCRYPTED\" value=\"(.*?)\"").Match(result).Groups[1].Value;

        //        List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
        //        paramList.Add(new KeyValuePair<string, string>("__LASTFOCUS", __LASTFOCUS));
        //        paramList.Add(new KeyValuePair<string, string>("__EVENTTARGET", __EVENTTARGET));
        //        paramList.Add(new KeyValuePair<string, string>("__EVENTARGUMENT", __EVENTARGUMENT));
        //        paramList.Add(new KeyValuePair<string, string>("__VIEWSTATE", __VIEWSTATE));
        //        paramList.Add(new KeyValuePair<string, string>("__VIEWSTATEGENERATOR", __VIEWSTATEGENERATOR));

        //        paramList.Add(new KeyValuePair<string, string>("__VIEWSTATEENCRYPTED", __VIEWSTATEENCRYPTED));

        //        paramList.Add(new KeyValuePair<string, string>("ScriptManager1", "UpdatePanel1|btnSubmit"));
        //        paramList.Add(new KeyValuePair<string, string>("txtVCode", code.ToString()));
        //        paramList.Add(new KeyValuePair<string, string>("MobileAndEmailCheck", ""));
        //        paramList.Add(new KeyValuePair<string, string>("__ASYNCPOST", "true"));
        //        paramList.Add(new KeyValuePair<string, string>("btnSubmit", "登录"));
        //        HttpResponseMessage response = httpClient.GetAsync(new Uri(url)).Result;
        //        string[] strCookies = (string[])response.Headers.GetValues("Set-Cookie");
        //        String result1 = response.Content.ReadAsStringAsync().Result;
        //        var message = new HttpRequestMessage(HttpMethod.Post, "/test");
        //        message.Headers.Add("Cookie", strcookie);
        //        message.Content
        //        HttpResponseMessage result2 = client.SendAsync(message).Result;

        //    }
        //}


        private string CookietoString(string[] cookie)
        {
            string strcookie = "";
            foreach (var item in cookie)
            {
                if (strcookie == "")
                    strcookie = item;
                else
                    strcookie += ";" + item;
            }
            return strcookie;
        }

    }
}

