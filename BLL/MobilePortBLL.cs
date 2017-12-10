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
        [DllImport("com_aspire_ll800_util_ll800Key2.dll", EntryPoint = "WebToken", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern QueryECLLTFUsedResult QueryECLLTFUsed(AppInfo appinfo, QueryECLLTFUsed queryEClltfUsed) {

        }
    }
}
