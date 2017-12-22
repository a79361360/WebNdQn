using Common;
using CsharpHttpHelper;
using DAL;
using FJSZ.OA.Common.Web;
using Model.WxModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class NsoupBLL
    {
        CommonDAL cdal = new CommonDAL();
        NsoupDAL ndal = new NsoupDAL();
        public static string dlmobile = "10657047";     //登入短信号码
        public static string czmobile = "10657047";     //充值短信号码
        public int SendLoginMsg(int ctype,int issue) {
            Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SendLoginMsg 1 类型: " + ctype + " 期号：" + issue + " 开始发送登入短信");
            string url = "http://www.fj.10086.cn/power/ll800/ht/index.jsp";
            IList<T_CooperConfig> list = DataTableToList.ModelConvertHelper<T_CooperConfig>.ConvertToModel(cdal.GetCooperConfig(ctype, issue));
            if (list.Count > 0)
            {
                T_CooperConfig dto = list[0];
                NSoup.Nodes.Document doc;
                HttpHelper helpweb = new HttpHelper();  //初始实例化HttpHelper
                HttpResult result = new HttpResult();   //初始实例化HttpResult
                HttpItem item = new HttpItem()          //初始实例化HttpItem
                {
                    URL = url,//URL     必需项    
                    Method = "GET",//URL     可选项 默认为Get   
                    ProxyIp = "ieproxy",
                    ContentType = "application/x-www-form-urlencoded",//ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
                };
                try
                {
                    result = helpweb.GetHtml(item);
                    doc = NSoup.NSoupClient.Parse(result.Html);
                    string csrf = doc.Select("#form2 input[name=_csrf]").Val();
                    if (!string.IsNullOrEmpty(csrf))
                    {
                        int result_1 = ndal.IsExitsLogCache(ctype, issue);
                        if (result_1 == 1)
                        {
                            int result_2 = ndal.UpdateLogCacheInfo(ctype, issue, dto.username, dto.signphone, csrf);
                            if (result_2 == 1)
                            {
                                url = "http://www.fj.10086.cn/power/ll800/ht/login/checkInfo.do";
                                string data = "acount=" + dto.username + "&mobile=" + dto.signphone.MD5();
                                Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "KeepSessionUsered 3  类型: " + ctype + " 期号：" + issue + " 发短信的参数 "+ data);
                                WebHttp p = new WebHttp();
                                string result_3 = p.Post(url, data);
                                Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "KeepSessionUsered 3  类型: " + ctype + " 期号：" + issue + " 短信发送后的返回结果 "+ result_3);
                                nsoupmsgdto msgdto = JsonConvert.DeserializeObject<nsoupmsgdto>(result_3);
                                if (msgdto.result == "ok") {
                                    return 1;
                                }
                                return -6;
                            }
                            Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "KeepSessionUsered 3  类型: " + ctype + " 期号：" + issue + " 更新name,phone,csrf值时失败 ");
                            return -5;
                        }
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "KeepSessionUsered 3  类型: " + ctype + " 期号：" + issue + " 未添加到监控列表或者添加过多 ");
                        return -4;
                    }
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "KeepSessionUsered 3  类型: " + ctype + " 期号：" + issue + " 未取到_csrf的值 ");
                    return -1;
                }
                catch (Exception er)
                {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "KeepSessionUsered 2 类型: " + ctype + " 期号：" + issue + "访问异常:" + er.Message);
                    return -3;
                }
            }
            Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "KeepSessionUsered 2 类型: " + ctype + " 期号：" + issue + " 没有配置T_CooperConfig表:");
            return -2;
        }
        /// <summary>
        /// 解析短信内容返回类型(登入|充值)和短信动态码,格式为: "1|000000"
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public string FilterContentTC(string mobile, string content)
        {
            string pstr = "本次登录流量800平台的动态码为：";
            if (content.IndexOf(pstr) != -1)
            {
                string yzm = content.Substring(content.LastIndexOf(pstr) + 17, 6);
                return "1|" + yzm;
            }
            else if (content.IndexOf("本次动态码为：") != -1) {
                string yzm = content.Substring(content.LastIndexOf(pstr) + 7, 6);
                return "2|" + yzm;
            }
            return "0|0";
        }
        /// <summary>
        /// 将收到短信保存到数据库
        /// </summary>
        /// <param name="type">1为登入2为充值</param>
        /// <param name="phone">发送短信的号码</param>
        /// <param name="xh">序号,新版无序号默认给0</param>
        /// <param name="code">动态码(短信验证码)</param>
        /// <param name="content">完整的短信字符串</param>
        /// <returns></returns>
        public int TakeMsgCode(int type,string phone,string xh,string code,string content) {
            return cdal.TakeMsgCode(type, phone, xh, code, content);
        }

    }
    //新发送短信返回结果实体类
    public class nsoupmsgdto {
        public string result { get; set; }
        public string message { get; set; }
    }
}
