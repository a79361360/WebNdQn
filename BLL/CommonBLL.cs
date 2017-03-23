using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class CommonBLL
    {
        public bool ReadPhoneFliter(string phone, string path) {
            List<string> list = (List<string>)FJSZ.OA.Common.CacheAccess.GetFromCache("MoneyList");
            Common.Expend.LogTxtExpend.WriteLogs("/Logs/ExtProc_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "开始时间:" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fffff"));
            if (list == null)
            {
                list = new List<string>();
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/ExtProc_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "开始读txt时间:" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fffff"));
                //按行读取为字符串数组
                string[] lines = System.IO.File.ReadAllLines(path);
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/ExtProc_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "结束读txt时间:" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fffff"));
                foreach (string line in lines)
                {
                    list.Add(line);
                }
                FJSZ.OA.Common.CacheAccess.InsertToCacheByTimeE("MoneyList", list, 30);
            }
            Common.Expend.LogTxtExpend.WriteLogs("/Logs/ExtProc_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "list时间:" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fffff"));
            if (list.Contains(phone)) {
                return true;
            }
            return false;
        }
    }
}
