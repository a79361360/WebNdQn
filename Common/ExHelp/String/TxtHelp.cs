using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Web;
using FJSZ.OA.Common;
using FJSZ.OA.Common.DEncrypt;
using System.Security.Cryptography;

namespace System
{
    /// <summary>
    /// 文本处理
    /// </summary>
    public static class TxtHelp
    {  
        #region 通用加密解密
        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public static string EP(this string str, string sKey = "56780b57b7cb59192c095606")
        {
            return DESEncrypt.Encrypt(str, sKey);
        }
        /// <summary>
        /// 解密已加密的字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public static string DP(this string str, string sKey = "56780b57b7cb59192c095606")
        {
            return DESEncrypt.Decrypt(str, sKey);
        }
        public static string MD5(this string str)
        {
            //string md5 = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5");
            //return md5.ToLower();
            //MD5 md5 = new MD5CryptoServiceProvider();
            //byte[] result = md5.ComputeHash(System.Text.Encoding.Default.GetBytes(str));
            //return System.Text.Encoding.Default.GetString(result);

            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] encryptedBytes = md5.ComputeHash(Encoding.ASCII.GetBytes(str));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < encryptedBytes.Length; i++)
            {
                sb.AppendFormat("{0:x2}", encryptedBytes[i]);
            }
            return sb.ToString();
        }

        #endregion
    }
}
