using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FJSZ.OA.Common.Web
{
    public class WebHelp
    {
        public static bool RequestIsHost()
        {
            string server_referrer = string.Empty, server_host = string.Empty;

            if (System.Web.HttpContext.Current.Request.UrlReferrer == null)
                return false;
            else
                server_referrer = System.Web.HttpContext.Current.Request.UrlReferrer.Host.ToLower();

            server_host = System.Web.HttpContext.Current.Request.Url.Host.ToLower();
            if (server_referrer.Equals(server_host))
                return true;
            return false;
        }
        public static string GetIp() {
            string ip = string.Empty;
            if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null) // using proxy
            {
                ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();  // Return real client IP.
            }
            else// not using proxy or can't get the Client IP
            {
                ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString(); //While it can't get the Client IP, it will return proxy IP.
            }
            return ip;
        }
        public static string GetUrl() {
            string url = System.Web.HttpContext.Current.Request.Url.ToString();
            return url;
        }
        /// <summary>
        /// 取得当前域名,将Http的协议头加上例: http://www.qingqiu.com
        /// </summary>
        /// <returns></returns>
        public static string GetCurHttpHost()
        {
            return "http://" + HttpContext.Current.Request.Url.Host;
        }
        /// <summary>
        /// 取得当前域名
        /// </summary>
        /// <returns></returns>
        public static string GetCurHost()
        {
            return HttpContext.Current.Request.Url.Host;
        }
        public static string HttpUploadFile(string virtualpath, string filename)
        {
            string path = "", suffix = "";
            if (HttpContext.Current.Request.Files.AllKeys.Length > 0)
            {
                try
                {
                    string filePath = HttpContext.Current.Server.MapPath(virtualpath);
                    if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
                    string fname = HttpContext.Current.Request.Files[0].FileName;
                    suffix = fname.Substring(fname.LastIndexOf(".") + 1, fname.Length - (fname.LastIndexOf(".") + 1));
                    if (string.IsNullOrEmpty(filename))
                    {
                        filename = HttpContext.Current.Request.Files[0].FileName;
                    }
                    //这里我直接用索引来获取第一个文件，如果上传了多个文件，可以通过遍历HttpContext.Current.Request.Files.AllKeys取“key值”，再通过HttpContext.Current.Request.Files[“key值”]获取文件
                    path = Path.Combine(filePath, filename);
                    HttpContext.Current.Request.Files[0].SaveAs(path);
                }
                catch
                {

                }
            }
            return path;
        }
        public static string HttpUploadFile(string virtualpath, string filename,string fileid)
        {
            string path = "", suffix = "";
            if (HttpContext.Current.Request.Files.AllKeys.Length > 0)
            {
                try
                {
                    string filePath = HttpContext.Current.Server.MapPath(virtualpath);
                    if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
                    string fname = HttpContext.Current.Request.Files[fileid].FileName;
                    suffix = fname.Substring(fname.LastIndexOf(".") + 1, fname.Length - (fname.LastIndexOf(".") + 1));
                    if (string.IsNullOrEmpty(filename))
                    {
                        filename = HttpContext.Current.Request.Files[fileid].FileName;
                    }
                    //这里我直接用索引来获取第一个文件，如果上传了多个文件，可以通过遍历HttpContext.Current.Request.Files.AllKeys取“key值”，再通过HttpContext.Current.Request.Files[“key值”]获取文件
                    path = Path.Combine(filePath, filename);
                    HttpContext.Current.Request.Files[fileid].SaveAs(path);
                }
                catch(Exception er)
                {
                    path = "";
                }
            }
            return path;
        }
    }
}
