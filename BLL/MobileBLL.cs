using Common;
using CsharpHttpHelper;
using CsharpHttpHelper.Enum;
using DAL;
using Model.WxModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class MobileBLL
    {
        CommonDAL dal = new CommonDAL();
        MobileDAL mdal = new MobileDAL();
        public static string czhost = "";   //充值页面的主机HOST
        public int GetHtmlByLoginCache(int ctype,int issue) {
            try {
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_耗时日志" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "生成充值缓存开始: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff"));
                //取得登入的cookie值（这个需要手动登入一下）
                IList<T_LoginLogCache> list = DataTableToList.ModelConvertHelper<T_LoginLogCache>.ConvertToModel(dal.GetLoginCache(ctype, issue));
                if (list.Count == 0)
                {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "GetHtmlByLoginCache失败：不存在当前类型: " + ctype + "的T_LoginLogCache的配置");
                    return -2;
                }
                T_LoginLogCache dto = list[0];
                string baseurl = "http://www.fj.10086.cn/power/NewGroupPortal/MYPower100/TranToOther.html?ConfigID=136";

                //访问首页
                HttpHelper helpweb = new HttpHelper();
                HttpItem item = new HttpItem()
                {
                    URL = baseurl,//URL     必需项    
                    Method = "GET",//URL     可选项 默认为Get   
                    ProxyIp = "ieproxy",
                    Cookie = dto.cookie,
                    ContentType = "application/x-www-form-urlencoded",//ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
                };
                HttpResult result = helpweb.GetHtml(item);
                if (result.StatusDescription != "OK") {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "GetHtmlByLoginCache失败：类型: " + ctype + " 期号：" + issue + "返回的首页状态非OK: 一般情况下为登入cookie已经失效了" + result.Html);
                    return -3;
                }
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "GetHtmlByLoginCache成功：访问完首页页面。"+ baseurl);
                //得到带token的充值页面的URL
                NSoup.Nodes.Document doc = NSoup.NSoupClient.Parse(result.Html);
                string tokensrc = doc.Select("#setPassIframe").Attr("src");     //取得带token的URL
                Uri u = new Uri(tokensrc); czhost = u.Authority;            //取得iframe这个页面的主机host带端口
                item = new HttpItem()
                {
                    URL = tokensrc,//URL     必需项    
                    Method = "GET",//URL     可选项 默认为Get   
                    ProxyIp = "ieproxy",
                    Cookie = dto.cookie,
                    ContentType = "application/x-www-form-urlencoded",//ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
                };
                result = helpweb.GetHtml(item);
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "GetHtmlByLoginCache成功：访问完首页上的IFrame页面。"+ tokensrc);
                //访问完token页面后得到cookie值，并取得“立即办理”这个虚拟路径URL
                dto.cookie += ";" + result.Cookie;
                string tempcookie = result.Cookie;  //保存一个临时
                doc = NSoup.NSoupClient.Parse(result.Html);
                string blhref = doc.Select(".sf_index a")[0].Attr("href");  //取得虚拟路径URL
                tokensrc = "http://" + czhost + blhref;                        //与host拼接生成完整的路径URL
                item = new HttpItem()
                {
                    URL = tokensrc,//URL     必需项    
                    Method = "GET",//URL     可选项 默认为Get   
                    ProxyIp = "ieproxy",
                    Cookie = dto.cookie,
                    ContentType = "application/x-www-form-urlencoded",//ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
                };
                result = helpweb.GetHtml(item);
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "GetHtmlByLoginCache成功：访问完“立即办理”这个地址，来到直充页面。"+ tokensrc);
                //流量池分配URL
                doc = NSoup.NSoupClient.Parse(result.Html);
                blhref = doc.Select("#tab2").Attr("href");
                tokensrc = "http://" + czhost + blhref;
                item = new HttpItem()
                {
                    URL = tokensrc,//URL     必需项    
                    Method = "GET",//URL     可选项 默认为Get   
                    ProxyIp = "ieproxy",
                    Cookie = dto.cookie,
                    ContentType = "application/x-www-form-urlencoded",//ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
                };
                result = helpweb.GetHtml(item);
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "GetHtmlByLoginCache成功：访问完流量池分配页面。"+ tokensrc);
                //流量池分配-》批量用户分配
                doc = NSoup.NSoupClient.Parse(result.Html);
                blhref = doc.Select(".sf_tab li")[1].Attr("onclick");
                string[] str1 = blhref.Split(new string[] { "','", "'" }, StringSplitOptions.RemoveEmptyEntries);
                tokensrc = "http://" + czhost + str1[1];
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "GetHtmlByLoginCache成功：取得URL：" + tokensrc + " Cookie: " + tempcookie);
                //plyhfp(ctype, issue, tokensrc, tempcookie);           //调用上传execl
                CreateCzCache(ctype, issue, tokensrc, tempcookie);      //设置缓存
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "GetHtmlByLoginCache成功：类型: " + ctype + " 期号：" + issue + "缓存设置成功");
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_耗时日志" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "生成充值缓存结束: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff"));
            }
            catch (Exception er) {
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "GetHtmlByLoginCache失败：出现异常：" + er.Message);
                return -1100;
            }
            return 1;
        }
        /// <summary>
        /// 上传execl并调发送短信接口
        /// </summary>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        /// <returns>成功返回1</returns>
        public int ToExeclSendMsgCode(int ctype, int issue)
        {
            czcachedto czdto = (czcachedto)FJSZ.OA.Common.CacheAccess.GetFromCache(ctype + "_czcache_" + issue);
            if (czdto != null)
            {
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_耗时日志" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "生成Execl耗时开始: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff"));
                int result = FindDtByCtype(ctype, issue);   //生成待充值execl
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_耗时日志" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "生成Execl耗时结束: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff"));
                if (result == 1)
                {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_耗时日志" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "上传Execl并发送短信验证码耗时开始: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff"));
                    int czresult = plyhfp(ctype, issue, czdto.czurl, czdto.cookie);
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_耗时日志" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "上传Execl并发送短信验证码耗时结束: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff"));
                    return czresult;
                }
                return result;
            }
            else {
                int result = GetHtmlByLoginCache(ctype, issue); //请求充值缓存
                if (result == 1)
                {
                   result = ToExeclSendMsgCode(ctype, issue); //重新调用
                }
                return result;
            }
        }
        /// <summary>
        /// 批量用户分配页面
        /// </summary>
        private int plyhfp(int ctype,int issue, string url,string cookie) {
            try {
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "plyhfp成功：刚开始进入类型: " + ctype + " 期号：" + issue + "url：" + url + " Cookie: " + cookie);
                HttpHelper helpweb = new HttpHelper();
                HttpItem item = new HttpItem()
                {
                    URL = url,//URL     必需项    
                    Method = "GET",//URL     可选项 默认为Get   
                    ProxyIp = "ieproxy",
                    Cookie = cookie,
                    ContentType = "application/x-www-form-urlencoded",//ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
                };
                HttpResult result = helpweb.GetHtml(item);
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "plyhfp成功：访问批量充值页面"+ result.Html);
                //上传预览xls文件
                NSoup.Nodes.Document doc = NSoup.NSoupClient.Parse(result.Html);
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "plyhfp成功：解析HTML" + result.Html);
                string abc = doc.Select("#abc")[0].Val();
                string abc1 = doc.Select("#abc")[1].Val();
                string BeginDate = doc.Select("#BeginDate").Val();
                string EndDate = doc.Select("#EndDate").Val();
                string deal_Id = doc.Select("#deal_Id").Val();
                string BUSINESS = doc.Select("input[name=BUSINESS]").Val();
                string TF_MODE = doc.Select("input[name=TF_MODE]").Val();
                string predent = AppDomain.CurrentDomain.BaseDirectory + @"Content\Txt\flowPoolExcel.xls";   //使用固定这个地址来存放execl文件//AppDomain.CurrentDomain.BaseDirectory + (@"Content\Txt\") + "flowPoolExcel.xls"
                string abc2 = doc.Select("#abc")[2].Val();
                string IMPORT_CODE = doc.Select("#IMPORT_CODE").Val();
                string PACKTYPE = "";
                string DISCOUNT = doc.Select("#Discount").Val();
                string ECORDERID = doc.Select("#ECOrderID").Val();
                string dealId = doc.Select("#dealId").Val();
                string grid = "";

                string param = "abc=" + abc + "&abc=" + abc1 + "&BeginDate=" + BeginDate + "&EndDate=" + EndDate + "&deal_Id=" + deal_Id + "&BUSINESS=" + BUSINESS + "&TF_MODE=" + TF_MODE + "&predent=C:\fakepath\flowPoolExcel.xls";
                string tokensrc = "http://" + czhost + "/payflow/gm_fm/importFlowPoolExcel.do?" + param;
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "plyhfp成功：生成上传execl文件的URL：" + tokensrc);
                CookieContainer ttainer = ToCookieContainer(cookie);
                string resposeresult = HttpPostData(tokensrc, 100000, "upload", predent, null, ttainer);    //上传execl,100000毫秒=1分钟
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "plyhfp成功：execl文件上动作结果：" + resposeresult);
                if (!string.IsNullOrEmpty(resposeresult))
                {
                    dlcgdto dto = JsonConvert.DeserializeObject<dlcgdto>(resposeresult);
                    if (dto.chAmount != null & dto.chAmount.IsNum())
                    {
                        IMPORT_CODE = dto.model_2;  //重新附值---只有导入成功以后才会有值
                        string paramstr = "abc=" + abc2 + "&IMPORT_CODE=" + IMPORT_CODE + "&PACKTYPE=" + PACKTYPE + "&DISCOUNT=" + DISCOUNT + "&ECORDERID=" + ECORDERID + "&dealId=" + dealId + "&grid=" + grid;
                        url = "http://" + czhost + "/payflow/msgCode/get.do";   //发送短信
                        item = new HttpItem()
                        {
                            URL = url,//URL     必需项    
                            Method = "GET",//URL     可选项 默认为Get   
                            ProxyIp = "ieproxy",
                            Cookie = cookie,
                            ContentType = "application/x-www-form-urlencoded",//ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
                        };
                        result = helpweb.GetHtml(item);
                        msgdto msgdto = JsonConvert.DeserializeObject<msgdto>(result.Html);
                        if (msgdto != null && msgdto.status == 0)
                        {
                            int xhresult = mdal.UpdateLogCacheXh(ctype, issue, "10657532190000624", msgdto.seq, paramstr);    //将短信的序号更新到数据库
                            return 1;
                        }
                        else {
                            Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "plyhfp失败：短信发送失败：" + result.Html);
                        }
                    }
                    else {
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "plyhfp失败：execl文件上传返回异常值：" + resposeresult);
                    }
                }
                else {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "plyhfp失败：execl文件上传返回空值");
                }
            }
            catch (Exception er) {
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "plyhfp失败：执行出现异常" + er.Message);
            }
            return -1;
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
        public CookieContainer ToCookieContainer(string cookies) {
            NameValueCollection dic = new NameValueCollection();
            CookieContainer ner = new CookieContainer();
            string[] cookiestr = cookies.Split('=');
            for (int i = 0; i < cookiestr.Length; i++) {
                dic.Add(cookiestr[i], cookiestr[i + 1].Split(';')[0]);
                ner.Add(new Cookie(cookiestr[i], cookiestr[i + 1].Split(';')[0], "/", "218.207.214.83"));
                i++;
            }
            return ner;
        }
        /// <summary>
        /// 生成要进行充值的用户列表，并生成execl
        /// </summary>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        /// <returns></returns>
        public int FindDtByCtype(int ctype,int issue) {
            IList<T_CooperConfig> list = DataTableToList.ModelConvertHelper<T_CooperConfig>.ConvertToModel(dal.GetCooperConfig(ctype, issue));
            if (list.Count > 0)
            {
                T_CooperConfig dto = list[0];
                if (dto.eachflow == 0 || string.IsNullOrEmpty(dto.cutdate))
                {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "FindDtByCtype 类型ctype: " + ctype + "期号：" + issue + "未进行每个用户流量或者截止时间没有配置.T_CooperConfig");
                    return -2;
                }
                try {
                    var dt = mdal.FindFlowLogToExecl(ctype, issue, dto.eachflow, dto.cutdate);
                    if (dt.Rows.Count > 0)
                    {
                        Common.Helper.HmExcelAssist.DataTabletoExcel(dt, AppDomain.CurrentDomain.BaseDirectory + (@"Content\Txt\") + "flowPoolExcel.xls");
                        return 1;
                    }
                    else {
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "FindDtByCtype 类型ctype: " + ctype + "期号：" + issue + "未取得需要充值的用户");
                        return 2;
                    }
                }
                catch (Exception er) {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "FindDtByCtype 类型ctype: " + ctype + "期号：" + issue + "生成flowPoolExcel.xls异常了。" + er.Message);
                    return -3;
                }
            }
            return -1;
        }
        /// <summary>
        /// 等短信发送结束以后再来做缓存
        /// </summary>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        /// <param name="url"></param>
        /// <param name="cookies"></param>
        private void CreateCzCache(int ctype, int issue, string url, string cookies)
        {
            czcachedto dto = new czcachedto();
            dto.czurl = url;dto.cookie = cookies;
            FJSZ.OA.Common.CacheAccess.InsertToCacheByTimeE(ctype + "_czcache_" + issue, dto, 300);
        }

        /// <summary>
        /// 完成用户列表充值
        /// </summary>
        /// <param name="phone">10657030登入，10657532190000624充值</param>
        /// <param name="xh">序列号</param>
        /// <param name="code">短信验证码</param>
        /// <returns></returns>
        public int OverCzWithMsgCode(string phone,string xh,string code) {
            IList<ctypedto> list = DataTableToList.ModelConvertHelper<ctypedto>.ConvertToModel(mdal.FindCtypeIssueCache(phone, xh));
            if (list.Count == 1)
            {
                ctypedto dto = list[0];
                if (dto != null)
                {
                    czcachedto czdto = (czcachedto)FJSZ.OA.Common.CacheAccess.GetFromCache(dto.ctype + "_czcache_" + dto.issue);
                    string url = "http://" + czhost + "/payflow/gm_fm/batchOrder.do?msgCode=" + code;
                    string param = dto.czparam;string cookie = czdto.cookie;
                    HttpHelper http = new HttpHelper();
                    //创建Httphelper参数对象
                    HttpItem item = new HttpItem()
                    {
                        URL = url,//URL     必需项    
                        Method = "post",//URL     可选项 默认为Get   
                        ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值
                        Postdata = param,//Post要发送的数据
                        Cookie = cookie
                    };
                    //请求的返回值对象
                    HttpResult result = http.GetHtml(item);
                    //获取请请求的Html
                    string html = result.Html;
                    czoverdto msgdto = JsonConvert.DeserializeObject<czoverdto>(result.Html);
                    if (msgdto != null && msgdto.result == 1)
                    {
                        AfterOverCzSuccess(dto.ctype, dto.issue);
                        return 1;
                    }
                    else
                    {
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "OverCzWithMsgCode 加上验证提交后，返回失败。");
                        return -2;
                    }
                }
                return -3;
            }
            else {
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "OverCzWithMsgCode 返回了过多的登入缓存数据T_LoginLogCache，state状态为1的数据太多了。");
            }
            return -1;
        }
        public void AfterOverCzSuccess(int ctype,int issue) {
            mdal.UpdateFlowStateO(ctype, issue);    //充值完成后更新状态1为已完成
            ExecuteCooperList();    
        }
        /// <summary>
        /// 遍列需要充值的公司列表
        /// </summary>
        /// <returns></returns>
        public void ExecuteCooperList() {
            IList<ctypedto> list = (IList<ctypedto>)FJSZ.OA.Common.CacheAccess.GetFromCache("CzCooperList");    //缓存是否存在
            if (list == null)
            {
                list = DataTableToList.ModelConvertHelper<ctypedto>.ConvertToModel(dal.GetCooperConfigDrop(1));
                if (list.Count > 0)
                {
                    FJSZ.OA.Common.CacheAccess.InsertToCacheByTimeE("CzCooperList", list, 600);
                }
            }
            if (list.Count > 0)
            {
                int len = list.Count; ctypedto dto = null;
                for (int i = 0; i < len; len--) {
                    dto = list[0];
                    int result = ToExeclSendMsgCode(dto.ctype, dto.issue);
                    if (result == 2)
                    {
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "ExecuteCooperList 类型" + dto.ctype + "期号" + dto.issue + "没有要充值的用户信息。");
                        list.Remove(dto);
                        FJSZ.OA.Common.CacheAccess.SetCache("CzCooperList", list);
                    }
                    else if (result != 1)
                    {
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "ExecuteCooperList 类型" + dto.ctype + "期号" + dto.issue + "返回的值已经是异常了。");
                        list.Remove(dto);
                        FJSZ.OA.Common.CacheAccess.SetCache("CzCooperList", list);
                    }
                    else if (result == 1)
                    {
                        list.Remove(dto);
                        FJSZ.OA.Common.CacheAccess.SetCache("CzCooperList", list);
                        return;
                    }
                }
            }
            else {
                FJSZ.OA.Common.CacheAccess.RemoveCache("CzCooperList");
            }
        }


        /// <summary>
        /// 保持平台Session活跃30分钟，保持充值Session活跃5分钟
        /// </summary>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        /// <param name="lx"></param>
        public int KeepSessionUsered(int ctype,int issue,int lx) {
            if (lx == 1)
            {
                IList<T_LoginLogCache> list = DataTableToList.ModelConvertHelper<T_LoginLogCache>.ConvertToModel(dal.GetLoginCache(ctype, issue));
            //string baseurl = "http://www.fj.10086.cn/power/ADCECPortal/PowerLogin.aspx?ReturnUrl=ADCQDLPortal&test=t";
                string baseurl = "http://www.fj.10086.cn/power/ADCECPortal/power/PowerCheckCookier.aspx";
            
                T_LoginLogCache dto = list[0];
                //访问首页
                HttpHelper helpweb = new HttpHelper();
                HttpItem item = new HttpItem()
                {
                    URL = baseurl,//URL     必需项    
                    Method = "GET",//URL     可选项 默认为Get   
                    ProxyIp = "ieproxy",
                    Cookie = dto.cookie,
                    ContentType = "application/x-www-form-urlencoded",//ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
                };
                HttpResult result = helpweb.GetHtml(item);
                if (result.StatusDescription == "OK" && result.RedirectUrl == "")
                    return 1;
                else {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "KeepSessionUsered失败：类型: " + ctype + " 期号：" + issue + "Session已经丢失，需要重新输入");
                    return -1;
                }
                //NSoup.Nodes.Document doc = NSoup.NSoupClient.Parse(result.Html);
                //var obj = doc.Select(".loginBox_Logined");
                //if (obj.Html() != "") return 1;
            }
            else if (lx == 2) {
                czcachedto czdto = (czcachedto)FJSZ.OA.Common.CacheAccess.GetFromCache(ctype + "_czcache_" + issue);
                HttpHelper helpweb = new HttpHelper();
                HttpItem item = new HttpItem()
                {
                    URL = czdto.czurl,//URL     必需项    
                    Method = "GET",//URL     可选项 默认为Get   
                    ProxyIp = "ieproxy",
                    Cookie = czdto.cookie,
                    ContentType = "application/x-www-form-urlencoded",//ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
                };
                HttpResult result = helpweb.GetHtml(item);
                return 1;
            }
            return -1;
        }
        /// <summary>
        /// 取得Cache列表
        /// </summary>
        /// <returns></returns>
        public IList<listcachedto> FindCacheList() {
            IList<listcachedto> list = DataTableToList.ModelConvertHelper<listcachedto>.ConvertToModel(mdal.FindCtypeIssueCache());
            foreach (var item in list) {
                czcachedto czdto = (czcachedto)FJSZ.OA.Common.CacheAccess.GetFromCache(item.ctype + "_czcache_" + item.issue);
                if (czdto != null) {
                    item.czurl = czdto.czurl;item.cookie = czdto.cookie;
                }
            }
            return list;
        }
        /// <summary>
        /// 更新登入的cookie
        /// </summary>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        /// <returns></returns>
        public int UpdateDlCookie(int ctype, int issue, string dlcookie)
        {
            int result = mdal.UpdateDlCookie(ctype, issue, dlcookie);
            return result;
        }
        /// <summary>
        /// 发送登入的短信
        /// </summary>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        public void HelpWebSend(int ctype, int issue)
        {
            IList<T_CooperConfig> list = DataTableToList.ModelConvertHelper<T_CooperConfig>.ConvertToModel(dal.GetCooperConfig(ctype, issue));
            if (list.Count > 0)
            {
                T_CooperConfig dto = list[0];
                string baseurl = "http://www.fj.10086.cn/power/ADCECPortal/PowerLogin.aspx?ReturnUrl=ADCQDLPortal&test=t";
                //访问首页
                HttpHelper helpweb = new HttpHelper();
                HttpItem item = new HttpItem()
                {
                    URL = baseurl,//URL     必需项    
                    Method = "GET",//URL     可选项 默认为Get   
                    ProxyIp = "ieproxy",
                    ContentType = "application/x-www-form-urlencoded",//ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
                };
                HttpResult result = helpweb.GetHtml(item);
                string cookie = fhcookie(result.Cookie);
                //选择短信登入
                helpweb = new HttpHelper();
                item = new HttpItem()
                {
                    URL = baseurl,//URL     必需项    
                    Method = "POST",//URL     可选项 默认为Get   
                    ProxyIp = "ieproxy",
                    Cookie = cookie.ToString(),
                    ContentType = "application/x-www-form-urlencoded",//ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
                    Postdata = "__EVENTTARGET=rbl_PType%241&__EVENTARGUMENT=&__LASTFOCUS=&__VIEWSTATE=&__VIEWSTATEGENERATOR=CC3279BD&__VIEWSTATEENCRYPTED=&LoginType=1&SMSTimes=90&SMSAliasTimes=90&txtCorpCode=&txtUserName=&rbl_PType=2&txtPd=&txtCheckCode=&txtQDLRegisterUrl=%2FADCQDLPortal%2FProduction%2FProductOrderControl.aspx"
                };
                //请求的返回值对象
                result = helpweb.GetHtml(item);
                //发送短信
                helpweb = new HttpHelper();
                item = new HttpItem()
                {
                    URL = baseurl,//URL     必需项    
                    Method = "POST",//URL     可选项 默认为Get   
                    ProxyIp = "ieproxy",
                    Cookie = cookie.ToString(),
                    ContentType = "application/x-www-form-urlencoded",//ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
                    Postdata = "__EVENTTARGET=lbtn_GetSMS&__EVENTARGUMENT=&__LASTFOCUS=&__VIEWSTATE=&__VIEWSTATEGENERATOR=CC3279BD&__VIEWSTATEENCRYPTED=&LoginType=1&SMSTimes=90&SMSAliasTimes=90&txtCorpCode=" + dto.corpid + "&txtUserName=" + dto.username + "&rbl_PType=2&SMSP=&txtCheckCode=&txtQDLRegisterUrl=%2FADCQDLPortal%2FProduction%2FProductOrderControl.aspx"
                };
                result = helpweb.GetHtml(item);
                //获取请请求的Html
                string html = result.Html;
                NSoup.Nodes.Document doc = NSoup.NSoupClient.Parse(result.Html);
                string blhref = doc.Select("#lab_SMSIndex").Html();  //取得虚拟路径URL
                string xlh = System.Text.RegularExpressions.Regex.Replace(blhref, @"[^0-9]+", "");
                if (!string.IsNullOrEmpty(blhref)&& !string.IsNullOrEmpty(xlh))
                {
                    FJSZ.OA.Common.CacheAccess.InsertToCacheByTime(dto.corpid + "_cookie", cookie.ToString(), 3600);
                    mdal.UpdateLogCacheDlXh(ctype, issue, "10657030", xlh);
                }
            }
        }
        public int UpdateConfigPwd(string phone, string xh, string code) {
            IList<ctypedto> list = DataTableToList.ModelConvertHelper<ctypedto>.ConvertToModel(mdal.FindCtypeIssueForDl(phone, xh));
            if (list.Count == 1)
            {
                ctypedto dto = list[0];
                if (dto != null)
                {
                    int result = mdal.UpdateConfigPwd(dto.ctype, dto.issue, code);

                    var list1 = DataTableToList.ModelConvertHelper<T_CooperConfig>.ConvertToModel(dal.GetCooperConfig(dto.ctype, dto.issue));
                    if (list.Count > 0)
                    {
                        T_CooperConfig dto1 = list1[0];
                        LoginByMobileCode(dto1.corpid, dto1.username, dto1.userpwd);
                    }

                    return result;
                }
            }
            return -1;
        }
        private void LoginByMobileCode(string corpid, string username, string code)
        {
            string url = "http://www.fj.10086.cn/power/ADCECPortal/PowerLogin.aspx?ReturnUrl=ADCQDLPortal&test=t";
            string strCookies = (string)FJSZ.OA.Common.CacheAccess.GetFromCache(corpid + "_cookie");
            //strCookies = "ASP.NET_SessionId=aqmt2e45sg40swnggwf12hm3; oN11SIYcct=web.12; cdnweb=web_2409";
            HttpHelper helpweb = new HttpHelper();
            helpweb = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = url,//URL     必需项    
                Method = "post",//URL     可选项 默认为Get
                ProxyIp = "ieproxy",
                ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值
                Postdata = "__EVENTTARGET=&__EVENTARGUMENT=&__LASTFOCUS=&__VIEWSTATE=&__VIEWSTATEGENERATOR=CC3279BD&__VIEWSTATEENCRYPTED=&LoginType=1&SMSTimes=90&SMSAliasTimes=90&txtCorpCode=" + corpid + "&txtUserName=" + username + "&rbl_PType=2&SMSP=" + code + "&txtCheckCode=&button3=%E7%99%BB%E5%BD%95&txtQDLRegisterUrl=%2FADCQDLPortal%2FProduction%2FProductOrderControl.aspx",//Post要发送的数据
                //Postdata = "__EVENTTARGET=&__EVENTARGUMENT=&__LASTFOCUS=&__VIEWSTATE=&__VIEWSTATEENCRYPTED=&LoginType=1&SMSTimes=21&SMSAliasTimes=90&txtCorpCode=5913000125&txtUserName=administrator&rbl_PType=2&SMSP=058759&txtCheckCode=&button3=%E7%99%BB%E5%BD%95&txtQDLRegisterUrl=%2FADCQDLPortal%2FProduction%2FProductOrderControl.aspx",
                Cookie = strCookies,
                //Allowautoredirect = true,//自动跳转
                //AutoRedirectCookie = true//是否自动处理Cookie 
            };
            //请求的返回值对象
            //HttpResult result = helpweb.FastRequest(item);
            HttpResult result = helpweb.GetHtml(item);

            //List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
            //paramList.Add(new KeyValuePair<string, string>("__EVENTTARGET", ""));
            //paramList.Add(new KeyValuePair<string, string>("__EVENTARGUMENT", ""));
            //paramList.Add(new KeyValuePair<string, string>("__LASTFOCUS", ""));
            //paramList.Add(new KeyValuePair<string, string>("__VIEWSTATE", ""));
            //paramList.Add(new KeyValuePair<string, string>("__VIEWSTATEGENERATOR", "CC3279BD"));
            //paramList.Add(new KeyValuePair<string, string>("__VIEWSTATEENCRYPTED", ""));

            //paramList.Add(new KeyValuePair<string, string>("LoginType", "1"));
            //paramList.Add(new KeyValuePair<string, string>("SMSTimes", "0"));
            //paramList.Add(new KeyValuePair<string, string>("SMSAliasTimes", "90"));
            //paramList.Add(new KeyValuePair<string, string>("txtCorpCode", corpid));
            //paramList.Add(new KeyValuePair<string, string>("txtUserName", username));
            //paramList.Add(new KeyValuePair<string, string>("rbl_PType", "2"));
            //paramList.Add(new KeyValuePair<string, string>("SMSP", code.ToString()));
            //paramList.Add(new KeyValuePair<string, string>("txtCheckCode", ""));
            //paramList.Add(new KeyValuePair<string, string>("button3", "登录"));
            //paramList.Add(new KeyValuePair<string, string>("txtQDLRegisterUrl", "/ADCQDLPortal/Production/ProductOrderControl.aspx"));

            //HttpResponseMessage response = httpClient.PostAsync(new Uri(url), new FormUrlEncodedContent(paramList)).Result;
            //string result = response.Content.ReadAsStringAsync().Result;
        }

        private string fhcookie(string str) {
            string[] str1 = str.Split(',');string tempstr = "";
            foreach (var item in str1) {
                 string[] str2 = item.Split(';');
                if (tempstr == "") tempstr = str2[0];
                else tempstr += ";" + str2[0];
            }
            return tempstr;
        }

    }
    public class czcachedto {
        public string czurl { get; set; }
        public string cookie { get; set; }
    }
    public class dlcgdto {
        public int errorCode { get; set; }
        public object model_1 { get; set; }
        public object model_0 { get; set; }
        public string model_2 { get; set; }
        public string chAmount { get; set; }
    }
    public class msgdto {
        public int status { get; set; }
        public string seq { get; set; }
    }
    public class ctypedto {
        public int ctype { get; set; }
        public int issue { get; set; }
        public string czparam { get; set; }
    }
    public class czoverdto {
        public string msg { get; set; }
        public int result { get; set; }
    }
    public class listcachedto {
        public int ctype { get; set; }
        public int issue { get; set; }
        public string dlcookie { get; set; }
        public string cookie { get; set; }
        public string czparam { get; set; }
        public string czurl { get; set; }
    }
}
