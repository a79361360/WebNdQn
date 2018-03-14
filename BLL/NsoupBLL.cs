using Common;
using CsharpHttpHelper;
using DAL;
using FJSZ.OA.Common.Web;
using Model.ViewModel;
using Model.WxModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BLL
{
    public class NsoupBLL
    {
        CommonDAL cdal = new CommonDAL();
        NsoupDAL ndal = new NsoupDAL();
        public static string dlmobile = "10657047";     //登入短信号码
        public static string czmobile = "10657047";     //充值短信号码
        /// <summary>
        /// 发送登入短信动态码
        /// </summary>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        /// <returns>1成功-1异常-2没有配置T_CooperConfig-3未取到_csrf的值-4未添加到监控列表或者添加过多-5更新corpid,phone,csrf,cookie值时失败-6短信发送失败</returns>
        public int SendLoginMsg(int ctype,int issue) {
            Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SendLoginMsg 类型: " + ctype + " 期号：" + issue + " 开始发送登入短信");
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
                    string cookie = fhcookie(result.Cookie);                //合并Cookie
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SendLoginMsg 类型: " + ctype + " 期号：" + issue + " Cookie值为:" + cookie);
                    doc = NSoup.NSoupClient.Parse(result.Html);
                    string csrf = doc.Select("#form2 input[name=_csrf]").Val();
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SendLoginMsg 类型: " + ctype + " 期号：" + issue + " csrf值为:" + csrf);
                    if (!string.IsNullOrEmpty(csrf))
                    {
                        int result_1 = ndal.IsExitsLogCache(ctype, issue);      //是否存在监控的项
                        if (result_1 == 1)
                        {
                            int result_2 = ndal.UpdateLogCacheInfo(ctype, issue, dto.corpid, dto.signphone, csrf, cookie);      //更新csrf值和临时cookie值(需要这个值才能发送短信)
                            if (result_2 == 1)
                            {
                                url = "http://www.fj.10086.cn/power/ll800/ht/login/checkInfo.do";
                                string data = "acount=" + dto.corpid + "&mobile=" + dto.signphone.MD5();
                                Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SendLoginMsg  类型: " + ctype + " 期号：" + issue + " 发短信的参数 " + data);
                                item = new HttpItem()
                                {
                                    URL = url,//URL     必需项    
                                    Method = "POST",//URL     可选项 默认为Get   
                                    ProxyIp = "ieproxy",
                                    Cookie = cookie,
                                    ContentType = "application/x-www-form-urlencoded",//ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
                                    Postdata = data
                                };
                                try
                                {
                                    result = helpweb.GetHtml(item);
                                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SendLoginMsg  类型: " + ctype + " 期号：" + issue + " 短信发送后的返回结果 " + result.Html);
                                    nsoupmsgdto msgdto = JsonConvert.DeserializeObject<nsoupmsgdto>(result.Html);
                                    if (msgdto.result == "ok")
                                        return 1;
                                    else
                                        return -6;
                                }
                                catch (Exception er)
                                {
                                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SendLoginMsg 短信发送异常:" + er.Message);
                                    return -1;
                                }
                            }
                            else {
                                Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SendLoginMsg  类型: " + ctype + " 期号：" + issue + " 更新corpid,phone,csrf,cookie值时失败 ");
                                return -5;
                            }
                        }
                        else {
                            Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SendLoginMsg  类型: " + ctype + " 期号：" + issue + " 未添加到监控列表或者添加过多 ");
                            return -4;
                        }
                    }
                    else {
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SendLoginMsg  类型: " + ctype + " 期号：" + issue + " 未取到_csrf的值 ");
                        return -3;
                    }
                }
                catch (Exception er)
                {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SendLoginMsg 类型: " + ctype + " 期号：" + issue + "访问异常:" + er.Message);
                    return -1;
                }
            }
            else
            {
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SendLoginMsg 类型: " + ctype + " 期号：" + issue + " 没有配置T_CooperConfig表:");
                return -2;
            }
        }
        /// <summary>
        /// 解析短信内容返回类型(登入|充值)和短信动态码,格式为: "1|000000"
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public string FilterContentTC(string mobile, string content)
        {
            string pstr = "本次登录的动态码为：";
            if (content.IndexOf(pstr) != -1)
            {
                string yzm = content.Substring(content.LastIndexOf(pstr) + 10, 6);
                return "1|" + yzm;
            }
            else if (content.IndexOf("本次动态码为：") != -1) {
                pstr = "本次动态码为：";
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
        /// <summary>
        /// 生成登入Cookie并保存到数据库,根据短信动态码
        /// </summary>
        /// <param name="code"></param>
        /// <returns>1成功-1异常-2校验失败-3不存在正在进行时的监控列-4登入的Cookie更新到数据库失败-5生成充值的Execl表失败-6提交充值的Execl表失败</returns>
        public int CreateLoginCookie(int code) {
            IList<T_LogCache> list = DataTableToList.ModelConvertHelper<T_LogCache>.ConvertToModel(ndal.FindLogCacheState());
            if (list.Count > 0)
            {
                T_LogCache dto = list[0];
                string url = "http://www.fj.10086.cn/power/ll800/ht/login/toLoginUp.do";
                string data = "acount=" + dto.corpid + "&vcode=" + code.ToString().MD5() + "&_csrf=" + dto.csrf;
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "CreateLoginCookie提交账号与短信动态码 data: " + data);
                HttpHelper helpweb = new HttpHelper();  //初始实例化HttpHelper
                HttpResult result = new HttpResult();   //初始实例化HttpResult
                HttpItem item = new HttpItem()
                {
                    URL = url,                                          //URL     必需项    
                    Method = "POST",                                    //URL     可选项 默认为Get   
                    ProxyIp = "ieproxy",
                    ContentType = "application/x-www-form-urlencoded",  //ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
                    Postdata = data,
                    Cookie = dto.dlcookie
                };
                try
                {
                    result = helpweb.GetHtml(item);
                    logindto dto_1 = JsonConvert.DeserializeObject<logindto>(result.Html);
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "CreateLoginCookie " + dto.dlcookie);
                    string cookie = fhcookie(result.Cookie);                //合并Cookie
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "CreateLoginCookie" + cookie);
                    int result_1 = ndal.UpdateLogCacheDlCookie(cookie);     //更新登入cookie
                    if (result_1 == 1)
                    {
                        int result_2 = SignDlCookie();      //校验一下是否数据库更新成功并且登入Cookie是否有效
                        if (result_2 == 1)
                        {
                            Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "CreateLoginCookie 校验登入cookie成功");
                            return 1;
                        }
                        else {
                            Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "CreateLoginCookie 校验登入cookie失败");
                            return -2;
                        }
                    }
                    else {
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "CreateLoginCookie 将登入的Cookie更新到数据库失败");
                        return -4;
                    }
                }
                catch (Exception er)
                {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "CreateLoginCookie 生成登入Cookie异常 " + er.Message);
                    return -1;
                }
            }
            else {
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "CreateLoginCookie 不存在正在进行时的监控列");
                return -3;
            }
        }
        /// <summary>
        /// 合并Cookie
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string fhcookie(string str)
        {
            string[] str1 = str.Split(','); string tempstr = "";
            foreach (var item in str1)
            {
                string[] str2 = item.Split(';');
                if (tempstr == "") tempstr = str2[0];
                else tempstr += ";" + str2[0];
            }
            return tempstr;
        }
        /// <summary>
        /// 校验登入的cookie是否有效
        /// </summary>
        /// <returns>1成功-1异常-2校验失败-3不存在进行时的监控项</returns>
        public int SignDlCookie() {
            IList<T_LogCache> list = DataTableToList.ModelConvertHelper<T_LogCache>.ConvertToModel(ndal.FindLogCacheState());
            if (list.Count > 0)
            {
                T_LogCache dto = list[0];
                string url = "http://www.fj.10086.cn/power/ll800/ht/home/main.do";
                HttpHelper helpweb = new HttpHelper();  //初始实例化HttpHelper
                HttpResult result = new HttpResult();   //初始实例化HttpResult
                HttpItem item = new HttpItem()
                {
                    URL = url,                                          //URL     必需项    
                    Method = "GET",                                    //URL     可选项 默认为Get   
                    ProxyIp = "ieproxy",
                    ContentType = "application/x-www-form-urlencoded",  //ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
                    Cookie = dto.dlcookie
                };
                try
                {
                    result = helpweb.GetHtml(item);
                    NSoup.Nodes.Document doc = NSoup.NSoupClient.Parse(result.Html);
                    string text = doc.Select(".current a")[0].Text();
                    if (text == "首页" & doc.ChildNodes.Count == 2)
                    {
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SignDlCookie 校验登入Cookie成功 ctype=" + dto.ctype + " issue=" + dto.issue);
                        return 1;
                    }
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SignDlCookie 校验登入Cookie失败 ctype=" + dto.ctype + " issue=" + dto.issue);
                    return -2;
                }
                catch (Exception er)
                {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SignDlCookie 校验登入Cookie异常 " + er.Message);
                    return -1;
                }
            }
            else {
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SignDlCookie 不存在正在进行时的监控列");
                return -3;
            }
        }
        /// <summary>
        /// 校验登入的cookie是否有效
        /// </summary>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        /// <returns>1成功-1异常-2校验失败-3不存在进行时的监控项</returns>
        public int SignDlCookie(int ctype,int issue)
        {
            IList<T_LogCache> list = DataTableToList.ModelConvertHelper<T_LogCache>.ConvertToModel(ndal.FindLogCacheState(ctype, issue));
            if (list.Count > 0)
            {
                T_LogCache dto = list[0];
                string url = "http://www.fj.10086.cn/power/ll800/ht/home/main.do";
                HttpHelper helpweb = new HttpHelper();  //初始实例化HttpHelper
                HttpResult result = new HttpResult();   //初始实例化HttpResult
                HttpItem item = new HttpItem()
                {
                    URL = url,                                          //URL     必需项    
                    Method = "GET",                                    //URL     可选项 默认为Get   
                    ProxyIp = "ieproxy",
                    ContentType = "application/x-www-form-urlencoded",  //ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
                    Cookie = dto.dlcookie
                };
                try
                {
                    result = helpweb.GetHtml(item);
                    NSoup.Nodes.Document doc = NSoup.NSoupClient.Parse(result.Html);
                    string text = doc.Select(".current a")[0].Text();
                    if (text == "首页" & doc.ChildNodes.Count == 2)
                    {
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SignDlCookie(int ctype,int issue) 校验登入Cookie成功 ctype=" + dto.ctype + " issue=" + dto.issue);
                        return 1;
                    }
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SignDlCookie(int ctype,int issue) 校验登入Cookie失败 ctype=" + dto.ctype + " issue=" + dto.issue);
                    return -2;
                }
                catch (Exception er)
                {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SignDlCookie(int ctype,int issue) 校验登入Cookie异常 " + er.Message);
                    return -1;
                }
            }
            else
            {
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SignDlCookie(int ctype,int issue) 不存在正在进行时的监控列");
                return -3;
            }
        }
        /// <summary>
        /// 是否存在待充值的记录
        /// </summary>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        /// <returns>1存在充值记录-2客户不存在-3没有充值记录</returns>
        public int IsExistsCzList(int ctype, int issue)
        {
            int num = ndal.IsExistsCzList(1, ctype, issue);
            return num;
        }
        /// <summary>
        /// 待充值的记录生成Execl文件
        /// </summary>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        /// <returns>1成功-1异常-3不存在进行时的监控项-4充值记录的表是空的</returns>
        public int CreateCzExecl(int ctype,int issue) {
            IList<T_CooperConfig> list = DataTableToList.ModelConvertHelper<T_CooperConfig>.ConvertToModel(cdal.GetCooperConfig(ctype, issue));
            if (list.Count > 0)
            {
                T_CooperConfig dto = list[0];
                if (dto.eachflow == 0 || string.IsNullOrEmpty(dto.cutdate))
                {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "CreateCzExecl 类型ctype: " + ctype + "期号：" + issue + "未进行每个用户流量或者截止时间没有配置.T_CooperConfig");
                    return -2;
                }
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "CreateCzExecl 类型ctype: " + ctype + "期号：" + issue + " 开始生成充值记录的Datatable表");
                var dt = ndal.CreatCzDTToExecl(2, ctype, issue);
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "CreateCzExecl 类型ctype: " + ctype + "期号：" + issue + " 生成充值记录的Datatable表结束,条数为" + dt.Rows.Count);
                if (dt.Rows.Count > 0)
                {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "CreateCzExecl 类型ctype: " + ctype + "期号：" + issue + " 开始将充值记录的Datatable表生成Execl表文件");
                    int result = Common.Helper.HmExcelAssist.DataTabletoExcel(dt, AppDomain.CurrentDomain.BaseDirectory + (@"Content\Txt\") + "flowPoolExcel.xls");
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "CreateCzExecl 类型ctype: " + ctype + "期号：" + issue + " 将充值记录的Datatable表生成Execl表文件的结果" + result);
                    return result;
                }
                else {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "CreateCzExecl 类型ctype: " + ctype + "期号：" + issue + " Datatable表的条数为0");
                    return -4;
                }
            }
            else {
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "CreateCzExecl 类型ctype: " + ctype + "期号：" + issue + " 不存在当前监控的客户");
                return -3;
            }
        }
        /// <summary>
        /// 生成execl文件，当自动充值出问题的时候，人工导出Execl
        /// </summary>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public int CreateCzExecl(int ctype, int issue,string filename) {
            IList<T_CooperConfig> list = DataTableToList.ModelConvertHelper<T_CooperConfig>.ConvertToModel(cdal.GetCooperConfig(ctype, issue));
            if (list.Count > 0)
            {
                T_CooperConfig dto = list[0];
                if (dto.eachflow == 0 || string.IsNullOrEmpty(dto.cutdate))
                {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "CreateCzExecl 类型ctype: " + ctype + "期号：" + issue + "未进行每个用户流量或者截止时间没有配置.T_CooperConfig");
                    return -2;
                }
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "CreateCzExecl 类型ctype: " + ctype + "期号：" + issue + " 开始生成充值记录的Datatable表");
                var dt = ndal.CreatCzDTToExecl(2, ctype, issue);
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "CreateCzExecl 类型ctype: " + ctype + "期号：" + issue + " 生成充值记录的Datatable表结束,条数为" + dt.Rows.Count);
                if (dt.Rows.Count > 0)
                {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "CreateCzExecl 类型ctype: " + ctype + "期号：" + issue + " 开始将充值记录的Datatable表生成Execl表文件");
                    int result = Common.Helper.HmExcelAssist.DataTabletoExcel(dt, AppDomain.CurrentDomain.BaseDirectory + (@"Content\Txt\") + filename);
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "CreateCzExecl 类型ctype: " + ctype + "期号：" + issue + " 将充值记录的Datatable表生成Execl表文件的结果" + result);
                    return result;
                }
                else {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "CreateCzExecl 类型ctype: " + ctype + "期号：" + issue + " Datatable表的条数为0");
                    return -4;
                }
            }
            else {
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "CreateCzExecl 类型ctype: " + ctype + "期号：" + issue + " 不存在当前监控的客户");
                return -3;
            }
        }
        /// <summary>
        /// 提交Cz的Execl表
        /// </summary>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        /// <returns>1成功-1异常-3不存在进行时的监控项-4Execl文件提交失败-5短信发送失败</returns>
        public int SubmitCzExecl(int ctype,int issue) {
            Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SubmitCzExecl 开始上传Execl并发送充值短信: " + ctype + " 期号：" + issue);
            IList<T_LogCache> list = DataTableToList.ModelConvertHelper<T_LogCache>.ConvertToModel(ndal.FindLogCacheState(ctype, issue));
            if (list.Count > 0)
            {
                T_LogCache dto = list[0];
                string url = "http://www.fj.10086.cn/power/ll800/ht/flow/upload.do";
                CookieContainer ttainer = ToCookieContainer(dto.dlcookie);
                string path = AppDomain.CurrentDomain.BaseDirectory + @"Content\Txt\flowPoolExcel.xls";
                string resposeresult = HttpPostData(url, 10000, "fileField", path, null, ttainer);
                NSoup.Nodes.Document doc = NSoup.NSoupClient.Parse(resposeresult);
                var tabobj = doc.Select("#table").Html();     //当前账号已经没有流量池的权限了
                if (!string.IsNullOrEmpty(tabobj))
                {
                    url = "http://www.fj.10086.cn/power/ll800/ht/activity/sendMSM.do";
                    HttpHelper helpweb = new HttpHelper();  //初始实例化HttpHelper
                    HttpResult result = new HttpResult();   //初始实例化HttpResult
                    HttpItem item = new HttpItem()
                    {
                        URL = url,//URL     必需项    
                        Method = "GET",//URL     可选项 默认为Get   
                        ProxyIp = "ieproxy",
                        Cookie = dto.dlcookie,
                        ContentType = "application/x-www-form-urlencoded",//ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
                    };
                    try
                    {
                        result = helpweb.GetHtml(item);
                        nsoupmsgdto msgdto = JsonConvert.DeserializeObject<nsoupmsgdto>(result.Html);
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SubmitCzExecl 发送短信返回的HTML ctype: " + ctype + "期号：" + issue + " result.Html: " + result.Html);
                        if (result.StatusCode == HttpStatusCode.OK & msgdto.result != "false")
                        {
                            Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SubmitCzExecl 发送短信成功类型ctype: " + ctype + "期号：" + issue);
                            //更新监控列表状态,为后面的
                            int result_1 = ndal.UpdateLogCacheState(dto.ctype, dto.issue);
                            if (result_1 == 1)
                            {
                                Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SubmitCzExecl 更新监控列表的状态成功ctype: " + ctype + "期号：" + issue);
                                return 1;
                            }
                            else {
                                Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SubmitCzExecl 更新监控列表的状态成功ctype: " + ctype + "期号：" + issue);
                                return -6;
                            }
                        }
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SubmitCzExecl 发送短信失败ctype: " + ctype + "期号：" + issue+ " result.Html: "+ result.Html);
                        return -5;
                    }
                    catch (Exception er)
                    {
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SubmitCzExecl ctype: " + ctype + "期号：" + issue + " 发送短信异常:" + er.Message);
                        return -1;
                    }
                }
                else {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SubmitCzExecl ctype: " + ctype + "期号：" + issue + " 提交充值Execl文件失败:" + doc.Html());
                    return -4;
                }
            }
            else {
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SubmitCzExecl 类型ctype: " + ctype + "期号：" + issue + " 不存在当前监控的客户");
                return -3;
            }
        }
        /// <summary>
        /// 提交充值短信
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public int SubmitCzMsg(int code) {
            IList<T_LogCache> list = DataTableToList.ModelConvertHelper<T_LogCache>.ConvertToModel(ndal.FindLogCacheState());
            if (list.Count > 0)
            {
                T_LogCache dto = list[0];
                string url = "http://www.fj.10086.cn/power/ll800/ht/flow/toFlowPoolList.do";
                string data = "vcode=" + code;
                HttpHelper helpweb = new HttpHelper();  //初始实例化HttpHelper
                HttpResult result = new HttpResult();   //初始实例化HttpResult
                HttpItem item = new HttpItem()
                {
                    URL = url,//URL     必需项    
                    Method = "POST",//URL     可选项 默认为Get   
                    ProxyIp = "ieproxy",
                    Cookie = dto.dlcookie,
                    ContentType = "application/x-www-form-urlencoded",//ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
                    Postdata = data,
                };
                try
                {
                    result = helpweb.GetHtml(item);
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SubmitCzMsg 提交充值短信类型ctype: " + dto.ctype + "期号：" + dto.issue + " result.Html: " + result.Html);
                    czdto czdto = JsonConvert.DeserializeObject<czdto>(result.Html);
                    if (result.StatusCode == HttpStatusCode.OK & czdto.Code == "100")
                    {
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SubmitCzMsg 提交充值短信成功类型ctype: " + dto.ctype + "期号：" + dto.issue + " result.Html: " + result.Html);
                        int result_1 = UpdateFlowLogState(dto.ctype, dto.issue);
                        if (result_1 == 1) return 1; else return -4;
                    }
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SubmitCzMsg 提交充值短信失败ctype: " + dto.ctype + "期号：" + dto.issue + " result.Html: " + result.Html);
                    return -5;
                }
                catch (Exception er)
                {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SubmitCzMsg 提交充值短信异常ctype: " + dto.ctype + "期号：" + dto.issue + " 异常:" + er.Message);
                    return -1;
                }
            }
            else {
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SubmitCzMsg 不存在正在进行监控的客户");
                return -3;
            }
        }
        /// <summary>
        /// 充值成功完成调用这个方法完成充值订单的状态
        /// </summary>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        /// <returns></returns>
        public int UpdateFlowLogState(int ctype, int issue)
        {
            Common.Expend.LogTxtExpend.WriteLogs("/Logs/NsoupBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "UpdateFlowLogState 更新充值记录状态类型ctype: " + ctype + "期号：" + issue);
            int result = ndal.UpdateFlowState(3, ctype, issue);
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cookies"></param>
        /// <returns></returns>
        public CookieContainer ToCookieContainer(string cookies)
        {
            CookieContainer ner = new CookieContainer();
            string[] cookiestr = cookies.Split(';');
            for (int i = 0; i < cookiestr.Length; i++)
            {
                string[] v = cookiestr[i].Split('=');
                ner.Add(new Cookie(v[0], v[1], "/", "www.fj.10086.cn"));
            }
            return ner;
        }
        /// <summary>
        /// 提交批量待充值的手机号码execl到服务器
        /// </summary>
        /// <param name="url"></param>
        /// <param name="timeOut">稁秒</param>
        /// <param name="fileKeyName">文件名称</param>
        /// <param name="filePath">文件路径</param>
        /// <param name="stringDict">参数放到body</param>
        /// <param name="container">cookie</param>
        /// <returns>返回结果</returns>
        public string HttpPostData(string url, int timeOut, string fileKeyName, string filePath, NameValueCollection stringDict, CookieContainer container)
        {
            string responseContent;
            var memStream = new MemoryStream();
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            // 边界符
            var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
            // 边界符
            var beginBoundary = Encoding.ASCII.GetBytes("--" + boundary + "\r\n");
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            // 最后的结束符
            var endBoundary = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");

            // 设置属性
            webRequest.Method = "POST";
            webRequest.Timeout = timeOut;
            webRequest.CookieContainer = container;
            webRequest.ContentType = "multipart/form-data; boundary=" + boundary;
            try
            {
                // 写入文件
                const string filePartHeader =
                    "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" +
                     "Content-Type: application/vnd.ms-excel\r\n\r\n";
                var header = string.Format(filePartHeader, fileKeyName, "flowPoolExcel.xls");
                var headerbytes = Encoding.UTF8.GetBytes(header);

                memStream.Write(beginBoundary, 0, beginBoundary.Length);
                memStream.Write(headerbytes, 0, headerbytes.Length);

                var buffer = new byte[1024];
                int bytesRead; // =0

                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    memStream.Write(buffer, 0, bytesRead);
                }

                // 写入字符串的Key
                //var stringKeyHeader = "\r\n--" + boundary +
                //                       "\r\nContent-Disposition: form-data; name=\"{0}\"" +
                //                       "\r\n\r\n{1}\r\n";

                //foreach (byte[] formitembytes in from string key in stringDict.Keys
                //                                 select string.Format(stringKeyHeader, key, stringDict[key])
                //                                     into formitem
                //                                 select Encoding.UTF8.GetBytes(formitem))
                //{
                //    memStream.Write(formitembytes, 0, formitembytes.Length);
                //}

                // 写入最后的结束边界符
                memStream.Write(endBoundary, 0, endBoundary.Length);

                webRequest.ContentLength = memStream.Length;

                var requestStream = webRequest.GetRequestStream();

                memStream.Position = 0;
                var tempBuffer = new byte[memStream.Length];
                memStream.Read(tempBuffer, 0, tempBuffer.Length);
                memStream.Close();

                requestStream.Write(tempBuffer, 0, tempBuffer.Length);
                requestStream.Close();

                var httpWebResponse = (HttpWebResponse)webRequest.GetResponse();
                using (var httpStreamReader = new StreamReader(httpWebResponse.GetResponseStream(),
                                                                Encoding.GetEncoding("utf-8")))
                {
                    responseContent = httpStreamReader.ReadToEnd();
                }
                fileStream.Close();
                httpWebResponse.Close();
                webRequest.Abort();
                return responseContent;
            }
            catch
            {
                fileStream.Close();
                webRequest.Abort();
                return "";
            }
        }
        /// <summary>
        /// 取得监控列表
        /// </summary>
        /// <returns></returns>
        public IList<T_LogCache> FindLogCacheList() {
            IList<T_LogCache> list = DataTableToList.ModelConvertHelper<T_LogCache>.ConvertToModel(ndal.FindLogCacheList());
            return list;
        }
        /// <summary>
        /// 添加超端记录,返回-1000表示已经存在超端记录
        /// </summary>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        /// <returns></returns>
        public int InsertLoginCache(int ctype, int issue)
        {
            int result = ndal.IsExitsLogCache(ctype, issue);
            if (result > 0) return -1000;
            IList<T_CooperConfig> list = DataTableToList.ModelConvertHelper<T_CooperConfig>.ConvertToModel(cdal.GetCooperConfig(ctype, issue));
            if (list.Count > 0) {
                T_CooperConfig dto = list[0];
                T_LogCache dto1 = new T_LogCache();
                dto1.ctype = ctype; dto1.issue = issue; dto1.corpid = dto.corpid; dto1.phone = dto.signphone;
                result = ndal.InsertT_LogCache(dto1);
                return result;
            }
            return -1;
        }
        /// <summary>
        /// 移除超端记录,返回成功数量
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public int RemoveLoginCache(IList<IdListDto> ids)
        {
            int sresult = 0;    //成功的数量
            if (ids.Count == 0)
                throw new ArgumentNullException();
            else
            {
                try
                {
                    foreach (var item in ids)
                    {
                        int gid = item.id;        //ID
                        int result = ndal.RemoveLoginCache(gid);
                        sresult = sresult + result;
                    }
                }
                catch
                {
                    return -1000;
                }
            }
            return sresult;
        }
    }
    //新发送短信返回结果实体类
    public class nsoupmsgdto {
        public string result { get; set; }
        public string message { get; set; }
    }
    //短信登入返回结果实体类
    public class logindto {
        public bool flag { get; set; }
    }
    //确认提交充值短信
    public class czdto {
        public string AllocateAmount { get; set; }
        public string BeginDate { get; set; }
        public string Code { get; set; }
        public string ECCode { get; set; }
        public string ECOrderID { get; set; }
        public string EndDate { get; set; }
        public string Mode { get; set; }
        public string Msg { get; set; }
        public string PackageAmount { get; set; }
        public string Parammsidn { get; set; }
        public string Piid { get; set; }
        public string Type { get; set; }
        public string mobielnum { get; set; }
    }
}
