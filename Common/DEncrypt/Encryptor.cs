using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Security.Cryptography;
using System.Text;

namespace FJSZ.OA.Common.DEncrypt
{

　/**//// <summary>
　/// 辅助类-用于对敏感数据进行Hash散列，达到加密效果
　/// </summary>
　　public sealed class Encryptor
　　{
　　　　/**//// <summary>
　　　　/// 使用MD5算法求Hash散列
　　　　/// </summary>
　　　　/// <param name="text">明文</param>
　　　　/// <returns>散列值</returns>
　　　　public static string MD5Encrypt(string text)
　　　　{
　　　　　　return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(text, "MD5");
　　　　}

　　　　/**//// <summary>
　　　　/// 使用SHA1算法求Hash散列
　　　　/// </summary>
　　　　/// <param name="text">明文</param>
　　　　/// <returns>散列值</returns>
　　　　public static string SHA1Encrypt(string text)
　　　　{
　　　　　　return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(text, "SHA1");
　　　　}
        #region 获取由SHA1加密的字符串
        public static string EncryptToSHA1(string str)
        {
            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
            byte[] str1 = Encoding.UTF8.GetBytes(str);
            byte[] str2 = sha1.ComputeHash(str1);
            sha1.Clear();
            (sha1 as IDisposable).Dispose();
            //return Convert.ToBase64String(str2);
            return str2.ToString();
        }
        #endregion
　　}


}

