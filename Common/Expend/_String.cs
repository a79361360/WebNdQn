using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;

namespace Fgly.Common.Expand
{
    public static class _String
    {
        /// <summary>
        /// 是否是DateTime类型
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsDateTime(this string s)
        {
            DateTime time;
            if (DateTime.TryParse(s, out time))
                return true;
            return false;
        }

        /// <summary>
        /// 转换成时间类型
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string s)
        {
            DateTime time;
            if (DateTime.TryParse(s, out time))
                return time;
            return default(DateTime);
        }

        /// <summary>
        /// 是否是Int8类型
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsByte(this string s)
        {
            byte i;
            if (byte.TryParse(s, out i))
                return true;
            return false;
        }

        /// <summary>
        /// 是否是Int16类型
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsShort(this string s)
        {
            short i;
            if (short.TryParse(s, out i))
                return true;
            return false;
        }

        /// <summary>
        /// 是否是Int32类型
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsInt(this string s)
        {
            int i;
            if (int.TryParse(s, out i))
                return true;
            return false;
        }

        /// <summary>
        /// 是否是Int64类型
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsLong(this string s)
        {
            long i;
            if (long.TryParse(s, out i))
                return true;
            return false;
        }

        /// <summary>
        /// 转换成Int8类型
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static byte ToByte(this string s)
        {
            byte i;
            if (byte.TryParse(s, out i))
                return i;
            return default(byte);
        }

        /// <summary>
        /// 转换成Int16类型
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static short ToShort(this string s)
        {
            short i;
            if (short.TryParse(s, out i))
                return i;
            return default(short);
        }

        /// <summary>
        /// 转换成Int32类型
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static int ToInt(this string s)
        {
            int i;
            if (int.TryParse(s, out i))
                return i;
            return default(int);
        }

        /// <summary>
        /// 转换成Int64类型
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static long ToLong(this string s)
        {
            long l;
            if(long.TryParse(s,out l))
                return l;
            return default(long);
        }

        /// <summary>
        /// 转换成Decimal类型
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this string s)
        {
            decimal i;
            if (decimal.TryParse(s, out i))
                return i;
            return default(decimal);
        }        

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string MD5(this string s)
        {
            string md5 = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(s, "MD5");
            return md5.ToLower();
        }

        /// <summary>
        /// MD5加密16位
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string MD516(this string s)
        {
            return MD5(s).Substring(8, 16);
        }

        /// <summary>
        /// Base64编码
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Base64Encode(this string s)
        {
            byte[] bt = Encoding.UTF8.GetBytes(s);
            return Convert.ToBase64String(bt);
        }

        /// <summary>
        /// Base64解码
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Base64Decode(this string s)
        {
            byte[] bt = Convert.FromBase64String(s);
            return Encoding.UTF8.GetString(bt);
        }

        /// <summary>
        /// Url编码
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string UrlEncode(this string s)
        {
            return HttpUtility.UrlEncode(s);
        }

        /// <summary>
        /// Url解码
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string UrlDecode(this string s)
        {
            return HttpUtility.UrlDecode(s);
        }

        /// <summary>
        /// 字符截取部分，后部分省略
        /// </summary>
        /// <param name="s"></param>
        /// <param name="len"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        public static string Cut(this string s, int len, string replace)
        {
            if (s.Length <= len)
                return s;
            return s.Substring(0, len) + replace;
        }

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string DESEncrypt(this string s)
        { 
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            provider.Key = Encoding.ASCII.GetBytes("@g$k&9%v");
            provider.IV = Encoding.ASCII.GetBytes("@g$k&9%v");

            MemoryStream ms = new MemoryStream();
            byte[] bytes = Encoding.Default.GetBytes(s);

            CryptoStream cs = new CryptoStream(ms, provider.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(bytes, 0, bytes.Length);
            cs.FlushFinalBlock();
            cs.Close();
            ms.Close();

            StringBuilder encrptDbStr = new StringBuilder();
            foreach (byte num in ms.ToArray())
                encrptDbStr.AppendFormat("{0:X2}", num);

            return encrptDbStr.ToString().ToLower();
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string DESDecrypt(this string s)
        {
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            provider.Key = Encoding.ASCII.GetBytes("@g$k&9%v");
            provider.IV = Encoding.ASCII.GetBytes("@g$k&9%v");

            byte[] bytes = new byte[s.Length / 2];
            for (int i = 0; i < s.Length / 2; i++)
                bytes[i] = (byte)(Convert.ToInt32(s.Substring(i * 2, 2), 16));

            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, provider.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(bytes, 0, bytes.Length);
            cs.FlushFinalBlock();
            cs.Close();
            ms.Close();
            return System.Text.Encoding.ASCII.GetString(ms.ToArray());
        }

        /// <summary>
        /// 是否是对的DES加密串
        /// </summary>
        /// <param name="s"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool IsRightDES(this string s, string param)
        {
            try
            {
                return param == s.DESDecrypt();
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 过滤SQL危害字符串
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string FilterSql(this string s)
        {
            if (s == null)
                return "";
            return s.ToLower().Replace("'", "''").Replace(";", "").Replace("--", "＃")
                .Replace("delete", "＃").Replace("exec", "＃").Replace("insert", "＃")
                .Replace("update", "＃").Replace("sp_", "＃").Replace("char", "＃")
                .Replace("truncate", "＃").Replace("drop", "＃").Replace("create", "＃")
                .Replace("alter", "＃").Replace("declare", "＃").Replace("execute", "＃")
                .Replace("or", "＃");
        }

        /// <summary>
        /// 过滤Html标签
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string FilterHtml(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return "";
            string returnValue = Regex.Replace(s, "<(?!br|p)[^>]+>", "", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return returnValue.Replace("<br />", "\r\n").Replace("<br>", "\r\n");
        }

    }
}
