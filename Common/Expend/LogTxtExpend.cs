using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Common.Expend
{
    public static class LogTxtExpend
    {
        public static string PhysicalPath(string virtualPath)
        {
            try
            {
                return HttpContext.Current.Server.MapPath(virtualPath);
            }
            catch {
                throw new Exception("错误的虚拟路径");
            }
        }
        /// <summary>
        /// 写日志，自动创建文件
        /// </summary>
        /// <param name="filePath">文件虚拟地址</param>
        /// <param name="log">内容</param>
        public static void WriteLogs(string filePath, string log)
        {
            using (StreamWriter sw = new StreamWriter(PhysicalPath(filePath), true, Encoding.UTF8))
            {
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "：" + log);
            }
        }
    }
}
