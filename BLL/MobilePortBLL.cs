using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class MobilePortBLL
    {
        [DllImport("com_aspire_ll800_util_ll800Key2.dll", EntryPoint = "WebToken", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern IntPtr WebToken(string appid, string appkey, string apptype);//与C++声明不太一致
        [DllImport("com_aspire_ll800_util_ll800Key2.dll", EntryPoint = "TokenALLSign", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern IntPtr TokenALLSign(string appid, string apptype, string sign, string timeamp, string token);//

        /// <param name="appid"></param>//appid
        /// <param name="apptype"></param>//应用类型
        ///  <param name="seq"></param> 序列号（定购接口必须唯一值）
        ///  <param name="token"></param> 初始化加密值
        ///  <param name="type"></param> 接口类型，每个接口类型值不同
        /// <param name="ECCode"></param> 集团编码
        [DllImport("com_aspire_ll800_util_ll800Key2.dll", EntryPoint = "OtherSign", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern IntPtr OtherSign(string appid, string apptype, string seq, string token, string type, string ECCode);//

        public void MobileBLL()
        {
            //string appid = "*****";
            //string appkey = "******";
            //string apptype = "1";//默认为1
            //string ECCode = "******";
            //string publicKey = "*****";

            ////初始化(只要初始化一次)
            //BLL.TestSDK.PayFlowServicePortTypeClient client = new BLL.TestSDK.PayFlowServicePortTypeClient();
            //BLL.TestSDK.AppInfo appinfo = new BLL.TestSDK.AppInfo();
            //appinfo.appid = appid;
            //appinfo.apptype = apptype;
            //IntPtr initStr = WebToken(appinfo.appid, appkey, appinfo.apptype);//appid,appkey,apptype  其中apptype默认为1 
            //appinfo.sign = Marshal.PtrToStringAnsi(initStr);
            //System.Diagnostics.Debug.WriteLine(appinfo.sign);
            //appinfo.sign = Marshal.PtrToStringAnsi(initStr);
            //BLL.TestSDK.InitBean initbean = client.Init(appinfo);//初始化结果
            //IntPtr lasttoken = TokenALLSign(appinfo.appid, appinfo.apptype, appinfo.sign, initbean.timeamp, initbean.token);//appid,apptype,sign,timeamp,token
            //string lasttokenStr = Marshal.PtrToStringAnsi(lasttoken);
            //System.Diagnostics.Debug.WriteLine(lasttokenStr);
            //if (lasttokenStr.Length < 100)
            //{
            //    System.Diagnostics.Debug.WriteLine("初始化失败");
            //    return;
            //}
            //System.Diagnostics.Debug.WriteLine("初始化完成");
        }
    }
}
