using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
