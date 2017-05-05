using Common.ExHelp;
using DAL;
using FJSZ.OA.Common.Web;
using Model.CommonModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class CommonBLL
    {
        CommonDAL dal = new CommonDAL();
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
        /// <summary>
        /// 添加领取流量的记录
        /// </summary>
        /// <param name="ctype">公司类型</param>
        /// <param name="phone">手机号码</param>
        /// <returns>返回影响行数</returns>
        public int TakeFlowLog(int ctype,string phone) {
            return dal.TakeFlowLog(ctype, phone);
        }
        /// <summary>
        /// 取得传送过来的验证码信息
        /// </summary>
        /// <param name="type">1登入验证码，2充值验证码</param>
        /// <param name="code">验证码</param>
        /// <returns></returns>
        public int TakeMsgCode(int type, int code) {
            return dal.TakeMsgCode(type, code);
        }
        /// <summary>
        /// 发送登入短信验证码
        /// </summary>
        /// <returns></returns>
        public bool SendLoginMsgCode() {
            WebHttp web = new WebHttp();
            string url = "";
            string data = "";
            try {
                web.Post(url, data);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 发送充值流量短信验证码
        /// </summary>
        /// <returns></returns>
        public bool SendFlowMsgCode()
        {
            WebHttp web = new WebHttp();
            string url = "";
            string data = "";
            try
            {
                web.Post(url, data);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 保存需要模拟POST提交需要的数据信息
        /// </summary>
        /// <returns></returns>
        public bool SaveLoginState(int code) {
            string corpid = "5913855431";               //企业代码
            string uname = "administrator";             //用户名
            string pwd = "nd123456";                    //密码
            WebHttp web = new WebHttp();
            string url = "";
            string data = "";
            try
            {
                string result = web.Post(url, data);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 模拟提交数据，完成给用户发送流量功能
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool SendFlowToUsered(int code) {
            WebHttp web = new WebHttp();
            string url = "";
            string data = "";
            try
            {
                string result = web.Post(url, data);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
