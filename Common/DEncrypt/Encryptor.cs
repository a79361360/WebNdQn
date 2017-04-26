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

��/**//// <summary>
��/// ������-���ڶ��������ݽ���Hashɢ�У��ﵽ����Ч��
��/// </summary>
����public sealed class Encryptor
����{
��������/**//// <summary>
��������/// ʹ��MD5�㷨��Hashɢ��
��������/// </summary>
��������/// <param name="text">����</param>
��������/// <returns>ɢ��ֵ</returns>
��������public static string MD5Encrypt(string text)
��������{
������������return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(text, "MD5");
��������}

��������/**//// <summary>
��������/// ʹ��SHA1�㷨��Hashɢ��
��������/// </summary>
��������/// <param name="text">����</param>
��������/// <returns>ɢ��ֵ</returns>
��������public static string SHA1Encrypt(string text)
��������{
������������return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(text, "SHA1");
��������}
        #region ��ȡ��SHA1���ܵ��ַ���
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
����}


}

