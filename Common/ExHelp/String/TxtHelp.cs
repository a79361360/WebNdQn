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

        #region 5.0 生成随机字符串 + static string GetRandomString(int length, bool useNum, bool useLow, bool useUpp, bool useSpe, string custom)
        ///<summary>
        ///生成随机字符串 
        ///</summary>
        ///<param name="length">目标字符串的长度</param>
        ///<param name="useNum">是否包含数字，1=包含，默认为包含</param>
        ///<param name="useLow">是否包含小写字母，1=包含，默认为包含</param>
        ///<param name="useUpp">是否包含大写字母，1=包含，默认为包含</param>
        ///<param name="useSpe">是否包含特殊字符，1=包含，默认为不包含</param>
        ///<param name="custom">要包含的自定义字符，直接输入要包含的字符列表</param>
        ///<returns>指定长度的随机字符串</returns>
        public static string GetRandomString(int length, bool useNum, bool useLow, bool useUpp, bool useSpe, string custom)
        {
            byte[] b = new byte[4];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
            Random r = new Random(BitConverter.ToInt32(b, 0));
            string s = null, str = custom;
            if (useNum == true) { str += "0123456789"; }
            if (useLow == true) { str += "abcdefghijklmnopqrstuvwxyz"; }
            if (useUpp == true) { str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; }
            if (useSpe == true) { str += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~"; }
            for (int i = 0; i < length; i++)
            {
                s += str.Substring(r.Next(0, str.Length - 1), 1);
            }
            return s;
        }
        #endregion
    }
}
