﻿using System;
using System.Collections.Generic;
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
                ECLogin(httpClient);
            //} while (result.Contains("验证码错误，麻烦您重新输入"));
            //用完要记得释放
            httpClient.Dispose();
        }
        public void ECLogin(HttpClient httpClient) {
            string url = "http://www.fj.10086.cn/power/ADCECPortal/EC/ECUserLoginProcess.aspx?loginType=1&SMSCheck=0&Ref=%2fpower%2fNewGroupPortal%2fMYPower100%2fIndex.html";
            HttpResponseMessage response = httpClient.GetAsync(new Uri(url)).Result;
            string result = response.Content.ReadAsStringAsync().Result;
        }
        public void SendLoginPost(string corpid, string username, string userpwd)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.MaxResponseContentBufferSize = 256000;
            httpClient.DefaultRequestHeaders.ExpectContinue = false;
            httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.143 Safari/537.36");
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
            ECLogin(httpClient);
            //用完要记得释放
            httpClient.Dispose();
        }
    }
}

