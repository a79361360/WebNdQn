using CsharpHttpHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class NsoupBLL
    {
        public int SendLoginMsg(int ctype,int issue) {
            Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SendLoginMsg 1 类型: " + ctype + " 期号：" + issue + " 开始发送登入短信");
            string url = "http://www.fj.10086.cn/power/ll800/ht/index.jsp";
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
                var obj = doc.Select("input");
                var obj1 = doc.Select("#form2 input[name=_csrf]");
                string text = doc.Select("#form2 input[name=_csrf]").Val();
                if (string.IsNullOrEmpty(text))
                {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "KeepSessionUsered 3  类型: " + ctype + " 期号：" + issue + " 解析登入HMTL,失败 ");
                    return -1;
                }
            }
            catch (Exception er)
            {
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "KeepSessionUsered 2 类型: " + ctype + " 期号：" + issue + "访问异常:" + er.Message);
                return -3;
            }
            return 1;
        }
    }
}
