using CsharpHttpHelper.BaseBll;
using CsharpHttpHelper.Helper;
using CsharpHttpHelper.Item;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Net;
using System.Text;

namespace CsharpHttpHelper
{
	public class HttpHelper
	{
		private HttpHelperBll bll = new HttpHelperBll();

		public HttpResult GetHtml(HttpItem item)
		{
			return this.bll.GetHtml(item);
		}

		public Image GetImage(HttpItem item)
		{
			return this.bll.GetImage(item);
		}

		public HttpResult FastRequest(HttpItem item)
		{
			return this.bll.FastRequest(item);
		}

		public static string GetSmallCookie(string strcookie)
		{
			return HttpCookieHelper.GetSmallCookie(strcookie);
		}

		public static CookieCollection StrCookieToCookieCollection(string strcookie)
		{
			return HttpCookieHelper.StrCookieToCookieCollection(strcookie);
		}

		public static string CookieCollectionToStrCookie(CookieCollection cookie)
		{
			return HttpCookieHelper.CookieCollectionToStrCookie(cookie);
		}

		public static string URLDecode(string text, Encoding encoding = null)
		{
			return HttpUrlHelper.URLDecode(text, encoding);
		}

		public static string URLEncode(string text, Encoding encoding = null)
		{
			return HttpUrlHelper.URLEncode(text, encoding);
		}

		public static NameValueCollection GetNameValueCollection(string str)
		{
			return HttpUrlHelper.GetNameValueCollection(str);
		}

		public static string GetUrlHost(string url)
		{
			return HttpUrlHelper.GetUrlHost(url);
		}

		public static string GetUrlIp(string url)
		{
			return HttpUrlHelper.GetUrlIp(url);
		}

		public static string ToMD5(string str)
		{
			return MD5Helper.ToMD5_32(str);
		}

		public static string ToSHA1(string str)
		{
			return MD5Helper.ToSHA1(str);
		}

		public static object JsonToObject<T>(string jsonstr)
		{
			return JsonHelper.JsonToObject<T>(jsonstr);
		}

		public static string ObjectToJson(object obj)
		{
			return JsonHelper.ObjectToJson(obj);
		}

		public static List<AItem> GetAList(string html)
		{
			return HtmlHelper.GetAList(html);
		}

		public static List<ImgItem> GetImgList(string html)
		{
			return HtmlHelper.GetImgList(html);
		}

		public static string StripHTML(string html)
		{
			return HtmlHelper.StripHTML(html);
		}

		public static string ReplaceNewLine(string html)
		{
			return HtmlHelper.ReplaceNewLine(html);
		}

		public static string GetBetweenHtml(string html, string s, string e)
		{
			return HtmlHelper.GetBetweenHtml(html, s, e);
		}

		public static string GetHtmlTitle(string html)
		{
			return HtmlHelper.GetHtmlTitle(html);
		}

		public static string JavaScriptEval(string strJs, string main)
		{
			return ExecJsHelper.JavaScriptEval(strJs, main);
		}

		public static Image GetImage(byte[] b)
		{
			return ImageHelper.ByteToImage(b);
		}

		public static string ByteToString(byte[] b, Encoding e = null)
		{
			return EncodingHelper.ByteToString(b, e);
		}

		public static byte[] StringToByte(string s, Encoding e = null)
		{
			return EncodingHelper.StringToByte(s, e);
		}

		public static string Base64ToString(string strbase, Encoding encoding)
		{
			return Base64Helper.Base64ToString(strbase, encoding);
		}

		public static string StringToBase64(byte[] bytebase)
		{
			return Base64Helper.StringToBase64(bytebase);
		}

		public static string StringToBase64(string str, Encoding encoding)
		{
			return Base64Helper.StringToBase64(str, encoding);
		}
        public static string MergerCookies(string OldCookie, string NewCookie)
        {
            if (!string.IsNullOrEmpty(OldCookie) && !string.IsNullOrEmpty(NewCookie))
            {
                if (OldCookie == NewCookie) return OldCookie;
                else
                {
                    List<string> Old = new List<String>(OldCookie.Split(';'));
                    List<string> New = new List<String>(NewCookie.Split(';'));
                    foreach (string n in New)
                    {
                        foreach (string o in Old)
                        {
                            if (o == n || o.Split('=')[0] == n.Split('=')[0])
                            {
                                Old.Remove(o);
                                break;
                            }
                        }
                    }
                    List<string> list = new List<string>(Old);
                    list.AddRange(New);
                    return string.Join(";", list.ToArray());
                }
            }
            else if (!string.IsNullOrEmpty(OldCookie)) return OldCookie;
            else if (!string.IsNullOrEmpty(NewCookie)) return NewCookie;
            else return "";
        }
    }
}
