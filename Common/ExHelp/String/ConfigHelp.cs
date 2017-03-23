using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Web;
using FJSZ.OA.Common;
using FJSZ.OA.Common.Web;

namespace System
{
    /// <summary>
    /// 文本处理
    /// </summary>
    public static class ConfigHelper
    {
        
        #region 获取配置值
        /// <summary>
        /// 获取XMLConfig的值
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GX(this string str)
        {
            return XmlConfigHelper.GetXmlConfig(str);
        }
        /// <summary>
        /// 获取WEB.config配置
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GW(this string str)
        {
            return WebConfigHelper.GetWebConfig(str);
        }
        /// <summary>
        /// 获取当前域名
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GHU(this String str)
        {
            var u = new StringBuilder();
            u.Append("http://");            
            if(HttpContext.Current.Request.Url.Port != 80)
            {                
                u.Append(HttpContext.Current.Request.Url.Authority);
            }
            else
            {
                u.Append(HttpContext.Current.Request.Url.Host);
            }            
            return u.ToString();
        }        
        #endregion

    }
}
