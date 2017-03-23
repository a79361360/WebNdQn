using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
    }
}
