using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
namespace FJSZ.OA.Common.DEncrypt
{
	/// <summary>
	/// Encrypt 的摘要说明。    
	/// </summary>
	public class DEncrypt
	{
		/// <summary>
		/// 构造方法
		/// </summary>
		public DEncrypt()  
		{  
		}
        public static string signkey = "adc9ee659ca881f1c3096688fff9fc58";
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string DESEncrypt1(string s)
        {
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            provider.Key = Encoding.ASCII.GetBytes("@w$2k!9%");
            provider.IV = Encoding.ASCII.GetBytes("@w$2k!9%");

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

            return encrptDbStr.ToString().ToUpper();
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string DESDecrypt1(string s)
        {
            DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
            dESCryptoServiceProvider.Key = Encoding.ASCII.GetBytes("@w$2k!9%");
            dESCryptoServiceProvider.IV = Encoding.ASCII.GetBytes("@w$2k!9%");
            byte[] array = new byte[s.Length / 2];
            for (int i = 0; i < s.Length / 2; i++)
            {
                array[i] = (byte)Convert.ToInt32(s.Substring(i * 2, 2), 16);
            }
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, dESCryptoServiceProvider.CreateDecryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(array, 0, array.Length);
            cryptoStream.FlushFinalBlock();
            cryptoStream.Close();
            memoryStream.Close();
            return Encoding.ASCII.GetString(memoryStream.ToArray());
        }

        #region 使用 缺省密钥字符串 加密/解密string

        /// <summary>
        /// 使用缺省密钥字符串加密string
        /// </summary>
        /// <param name="original">明文</param>
        /// <returns>密文</returns>
        public static string Encrypt(string original)
		{
            return Encrypt(original, "HOHO18Tools");
		}
		/// <summary>
		/// 使用缺省密钥字符串解密string
		/// </summary>
		/// <param name="original">密文</param>
		/// <returns>明文</returns>
		public static string Decrypt(string original)
		{
            return Decrypt(original, "HOHO18Tools", System.Text.Encoding.Default);
		}

		#endregion

		#region 使用 给定密钥字符串 加密/解密string
		/// <summary>
		/// 使用给定密钥字符串加密string
		/// </summary>
		/// <param name="original">原始文字</param>
		/// <param name="key">密钥</param>
		/// <param name="encoding">字符编码方案</param>
		/// <returns>密文</returns>
		public static string Encrypt(string original, string key)  
		{  
			byte[] buff = System.Text.Encoding.Default.GetBytes(original);  
			byte[] kb = System.Text.Encoding.Default.GetBytes(key);
			return Convert.ToBase64String(Encrypt(buff,kb));      
		}
		/// <summary>
		/// 使用给定密钥字符串解密string
		/// </summary>
		/// <param name="original">密文</param>
		/// <param name="key">密钥</param>
		/// <returns>明文</returns>
		public static string Decrypt(string original, string key)
		{
			return Decrypt(original,key,System.Text.Encoding.Default);
		}

		/// <summary>
		/// 使用给定密钥字符串解密string,返回指定编码方式明文
		/// </summary>
		/// <param name="encrypted">密文</param>
		/// <param name="key">密钥</param>
		/// <param name="encoding">字符编码方案</param>
		/// <returns>明文</returns>
		public static string Decrypt(string encrypted, string key,Encoding encoding)  
		{       
			byte[] buff = Convert.FromBase64String(encrypted);  
			byte[] kb = System.Text.Encoding.Default.GetBytes(key);
			return encoding.GetString(Decrypt(buff,kb));      
		}  
		#endregion

		#region 使用 缺省密钥字符串 加密/解密/byte[]
		/// <summary>
		/// 使用缺省密钥字符串解密byte[]
		/// </summary>
		/// <param name="encrypted">密文</param>
		/// <param name="key">密钥</param>
		/// <returns>明文</returns>
		public static byte[] Decrypt(byte[] encrypted)  
		{  
			byte[] key = System.Text.Encoding.Default.GetBytes("MATICSOFT"); 
			return Decrypt(encrypted,key);     
		}
		/// <summary>
		/// 使用缺省密钥字符串加密
		/// </summary>
		/// <param name="original">原始数据</param>
		/// <param name="key">密钥</param>
		/// <returns>密文</returns>
		public static byte[] Encrypt(byte[] original)  
		{  
			byte[] key = System.Text.Encoding.Default.GetBytes("MATICSOFT"); 
			return Encrypt(original,key);     
		}  
		#endregion

		#region  使用 给定密钥 加密/解密/byte[]

		/// <summary>
		/// 生成MD5摘要
		/// </summary>
		/// <param name="original">数据源</param>
		/// <returns>摘要</returns>
		public static byte[] MakeMD5(byte[] original)
		{
			MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();   
			byte[] keyhash = hashmd5.ComputeHash(original);       
			hashmd5 = null;  
			return keyhash;
		}


		/// <summary>
		/// 使用给定密钥加密
		/// </summary>
		/// <param name="original">明文</param>
		/// <param name="key">密钥</param>
		/// <returns>密文</returns>
		public static byte[] Encrypt(byte[] original, byte[] key)  
		{  
			TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();       
			des.Key =  MakeMD5(key);
			des.Mode = CipherMode.ECB;  
     
			return des.CreateEncryptor().TransformFinalBlock(original, 0, original.Length);     
		}  

		/// <summary>
		/// 使用给定密钥解密数据
		/// </summary>
		/// <param name="encrypted">密文</param>
		/// <param name="key">密钥</param>
		/// <returns>明文</returns>
		public static byte[] Decrypt(byte[] encrypted, byte[] key)  
		{  
			TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();  
			des.Key =  MakeMD5(key);    
			des.Mode = CipherMode.ECB;  

			return des.CreateDecryptor().TransformFinalBlock(encrypted, 0, encrypted.Length);
		}  
  
		#endregion

		

		
	}
}
