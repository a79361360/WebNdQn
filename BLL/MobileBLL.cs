using Common;
using CsharpHttpHelper;
using CsharpHttpHelper.Enum;
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
using System.Threading.Tasks;

namespace BLL
{
    public class MobileBLL
    {
        CommonDAL dal = new CommonDAL();
        MobileDAL mdal = new MobileDAL();
        public static string czhost = "218.207.214.83:8080";   //充值页面的主机HOST
        #region one里面需要重新定义调用的
        /// <summary>
        /// 上传EXECL并且发送短信
        /// </summary>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        /// <returns></returns>
        public int one_plyhfp(int ctype,int issue) {
            czcachedto czdto = (czcachedto)FJSZ.OA.Common.CacheAccess.GetFromCache(ctype + "_czcache_" + issue);    //取得充值的CZCookie值
            if (czdto == null) return -1;
            int czresult = plyhfp(ctype, issue, czdto.czurl, czdto.cookie);     //上传EXECL并且发送短信
            if (czresult == 1)
                return 1;
            else
                return -2;
        }
        #endregion



        /// <summary>
        /// 是否存在待充值的记录
        /// </summary>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        /// <returns></returns>
        public int IsExistsCzList(int ctype,int issue) {
            //int num = mdal.IsExistsCzList(ctype, issue);
            //int num_1 = mdal.IsExistsActityList(ctype, issue);
            ////如果直充为0并且活动的充值也为0就返回0值.其他的情况返回9表示有值就可以了
            //if (num == 0 && num_1 == 0) {
            //    return 0;
            //}
            //return 9;
            int num = mdal.IsExistsCzList(ctype, issue);
            return num;
        } 





        /// <summary>
        /// 完成用户列表充值
        /// </summary>
        /// <param name="phone">10657030登入，10657532190000761充值</param>
        /// <param name="xh">序列号</param>
        /// <param name="code">短信验证码</param>
        /// <returns></returns>
        public int OverCzWithMsgCode(string phone,string xh,string code) {
            Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "OverCzWithMsgCode 1 开始查询是否存在正在等接收短信的超端记录");
            //取得当前正在等待接收超端记录
            IList<ctypedto> list = DataTableToList.ModelConvertHelper<ctypedto>.ConvertToModel(mdal.FindCtypeIssueCache(phone, xh));
            if (list.Count == 1)
            {
                ctypedto dto = list[0];
                if (dto != null)
                {
                    czcachedto czdto = (czcachedto)FJSZ.OA.Common.CacheAccess.GetFromCache(dto.ctype + "_czcache_" + dto.issue);
                    if (czdto == null) {
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "OverCzWithMsgCode 2 充值的Cookie为空,5分钟内还没接收到短信,充值的cookie就失效了.");
                        return -2;
                    }
                    string url = "http://" + czhost + "/payflow/gm_fm/batchOrder.do?msgCode=" + code;
                    string param = dto.czparam;string cookie = czdto.cookie;    //从缓存中读取充值参数与cookie
                    HttpHelper helpweb = new HttpHelper();  //初始实例化HttpHelper
                    HttpResult result = new HttpResult();   //初始实例化HttpResult
                    HttpItem item = new HttpItem()          //初始实例化HttpItem
                    {
                        URL = url,//URL     必需项    
                        Method = "post",//URL     可选项 默认为Get   
                        ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值
                        Postdata = param,//Post要发送的数据
                        Cookie = cookie
                    };
                    try
                    {
                        //请求的返回值对象
                        result = helpweb.GetHtml(item);
                    }
                    catch (Exception er) {
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "OverCzWithMsgCode 3 接收短信去提交充值异常:" + er.Message);
                        return -3;
                    }
                    //获取请请求的Html
                    string html = result.Html;
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "OverCzWithMsgCode 3 充值结束,准备解析充值结果");
                    czoverdto msgdto = JsonConvert.DeserializeObject<czoverdto>(result.Html);
                    if (msgdto != null && msgdto.result == 1)
                    {
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "OverCzWithMsgCode 4 充值成功,修改待充值记录的状态为已充值.");
                        AfterOverCzSuccess(dto.ctype, dto.issue);
                        return 1;
                    }
                    else
                    {
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "OverCzWithMsgCode 4 充值提交返回结果为失败");
                        return -4;
                    }
                }
                return -5;
            }
            else {
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "OverCzWithMsgCode 2 不止返回了一条正在等待接收短信的超端记录，记录数为：" + list.Count);
                return -1;
            }
        }
        /// <summary>
        /// 将充值中的状态都修改成充值成功.
        /// </summary>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        public void AfterOverCzSuccess(int ctype,int issue) {
            mdal.UpdateFlowStateO(ctype, issue);        //充值完成后更新状态1为已完成
            mdal.UpdateActivityFlowState(ctype, issue);     //将活动的充值状态更新为1已完成
            //ExecuteCooperList();    
        }



        #region 已验证是需要的.




        /// <summary>
        /// 遍列需要充值的公司列表,将处于进行中的公司列表出来
        /// </summary>
        /// <returns></returns>
        public void ExecuteCooperList() {
            Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "ExecuteCooperList 1 开始读取待遍历的单子列表");
            IList<ctypedto> list = (IList<ctypedto>)FJSZ.OA.Common.CacheAccess.GetFromCache("CzCooperList");    //缓存是否存在
            if (list == null)
            {
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "ExecuteCooperList 2 待遍历的单子列表缓存为空,先进行列表数据库读取");
                //列表正在进行的单子
                list = DataTableToList.ModelConvertHelper<ctypedto>.ConvertToModel(dal.GetCooperConfigDrop(1));
                if (list.Count > 0)
                {
                    FJSZ.OA.Common.CacheAccess.InsertToCacheByTimeE("CzCooperList", list, 600);
                }
            }
            if (list.Count > 0)
            {
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "ExecuteCooperList 2 开始遍历需要充值的单子");
                int len = list.Count; ctypedto dto = null;
                for (int i = 0; i < len; len--) {
                    dto = list[0];  //取第0个位置的单子进行操作
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "ExecuteCooperList 3 遍历到类型" + dto.ctype + "期号" + dto.issue + "的单子");
                    int result = ToExeclSendMsgCode(dto.ctype, dto.issue);
                    if (result == 2)
                    {
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "ExecuteCooperList 4 类型" + dto.ctype + "期号" + dto.issue + "没有要充值的用户信息");
                        list.Remove(dto);
                        FJSZ.OA.Common.CacheAccess.SetCache("CzCooperList", list);
                    }
                    else if (result != 1)
                    {
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "ExecuteCooperList 4 类型" + dto.ctype + "期号" + dto.issue + "返回的值已经是异常了,result的值是"+ result);
                        list.Remove(dto);
                        FJSZ.OA.Common.CacheAccess.SetCache("CzCooperList", list);
                    }
                    else if (result == 1)
                    {
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "ExecuteCooperList 4 类型" + dto.ctype + "期号" + dto.issue + "充值列表上传成功");
                        list.Remove(dto);
                        FJSZ.OA.Common.CacheAccess.SetCache("CzCooperList", list);
                        return;
                    }
                }
            }
            else {
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "ExecuteCooperList 2 没有需要充值的单子在进行中");
                FJSZ.OA.Common.CacheAccess.RemoveCache("CzCooperList");
            }
        }
        /// <summary>
        /// 上传execl并调发送短信接口
        /// </summary>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        /// <returns>成功返回1</returns>
        public int ToExeclSendMsgCode(int ctype, int issue)
        {
            Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "ToExeclSendMsgCode 3_1 开始读取类型" + ctype + "期号" + issue + "的充值Cookie");
            czcachedto czdto = (czcachedto)FJSZ.OA.Common.CacheAccess.GetFromCache(ctype + "_czcache_" + issue);    //取得充值的CZCookie值
            if (czdto != null)
            {
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "ToExeclSendMsgCode 3_1 类型" + ctype + "期号" + issue + "的充值的URL:"+czdto.czurl+ "CZCookie:"+ czdto.cookie);
                int result = FindDtByCtype(ctype, issue);   //生成待充值execl,返回INT类型的生成结果
                if (result == 1)
                {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "ToExeclSendMsgCode 3_2 类型" + ctype + "期号" + issue + "的待充值EXECL表生成成功,准备开始上传");
                    int czresult = plyhfp(ctype, issue, czdto.czurl, czdto.cookie);     //上传EXECL并且发送短信
                    if (czresult == 1) 
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "ToExeclSendMsgCode 3_3 类型" + ctype + "期号" + issue + "的待充值EXECL表上传并发送短信成功");
                    else
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "ToExeclSendMsgCode 3_3 类型" + ctype + "期号" + issue + "的待充值EXECL表上传并发送短信失败");
                    return czresult;
                }
                return result;
            }
            else
            {
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "ToExeclSendMsgCode 3_1 类型" + ctype + "期号" + issue + "的充值的CZCookie为空,需要先进行获取");
                int result = GetHtmlByLoginCache(ctype, issue); //请求充值缓存
                if (result == 1)
                {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "ToExeclSendMsgCode 3_2 类型" + ctype + "期号" + issue + "生成充值的CZCookie成功");
                    result = ToExeclSendMsgCode(ctype, issue); //重新调用
                }
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "ToExeclSendMsgCode 3_2 类型" + ctype + "期号" + issue + "的充值result:"+ result);
                return result;
            }
        }
        /// <summary>
        /// 生成充值的CZCookie保存到缓存,方便后面的调用,如果成功返回1,如果失败返回值为 负数 
        /// </summary>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        /// <returns></returns>
        public int GetHtmlByLoginCache(int ctype, int issue)
        {
            try
            {
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "GetHtmlByLoginCache 3_1_1 开始读取类型" + ctype + "期号" + issue + "的登入Cookie");
                //取得登入的cookie值（这个需要手动登入一下）
                IList<T_LoginLogCache> list = DataTableToList.ModelConvertHelper<T_LoginLogCache>.ConvertToModel(mdal.GetLoginCache(ctype, issue));
                if (list.Count == 0)
                {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "GetHtmlByLoginCache 3_1_2 不存在当前类型: " + ctype + "期号" + issue + "的T_LoginLogCache的配置");
                    return -2;
                }
                T_LoginLogCache dto = list[0];
                //先判断登入Session是否有效,返回1为有效
                int result_1 = KeepSessionUsered(ctype, issue, 1);
                if (result_1 != 1) 
                    return -1010;
                string baseurl = "http://www.fj.10086.cn/power/NewGroupPortal/MYPower100/TranToOther.html?ConfigID=136";
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "GetHtmlByLoginCache 3_1_2 取得类型: " + ctype + "期号" + issue + "的T_LoginLogCache的配置");
                //访问流量统付自助服务
                HttpHelper helpweb = new HttpHelper();  //初始实例化HttpHelper
                HttpResult result = new HttpResult();   //初始实例化HttpResult
                HttpItem item = new HttpItem()          //初始实例化HttpItem
                {
                    URL = baseurl,//URL     必需项    
                    Method = "GET",//URL     可选项 默认为Get   
                    ProxyIp = "ieproxy",
                    Cookie = dto.cookie,
                    ContentType = "application/x-www-form-urlencoded",//ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
                };
                try
                {
                    result = helpweb.GetHtml(item);
                    if (result.StatusDescription != "OK")
                    {
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "GetHtmlByLoginCache 3_1_3 类型: " + ctype + " 期号：" + issue + "返回的流量统付自助服务状态非OK: 一般情况下为登入cookie已经失效了");
                        return -3;
                    }
                }
                catch (Exception er) {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "GetHtmlByLoginCache 3_1_3 类型: " + ctype + " 期号：" + issue + "访问流量统付自助服务异常" + er.Message);
                    return -4;
                }
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "GetHtmlByLoginCache 3_1_3 访问完流量统付自助服务");
                //得到带token的充值页面的URL
                NSoup.Nodes.Document doc = NSoup.NSoupClient.Parse(result.Html);
                string tokensrc = doc.Select("#setPassIframe").Attr("src");         //取得带token的URL
                if (string.IsNullOrEmpty(tokensrc))
                {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "GetHtmlByLoginCache 3_1_5 获取充值页面Iframe的URL为空");
                    return -5;
                }
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "GetHtmlByLoginCache 3_1_5 获取充值页面Iframe的URL成功,URL:  "+ tokensrc);
                try
                {
                    Uri u = new Uri(tokensrc); czhost = u.Authority;                    //取得iframe这个页面的主机host带端口
                    item = new HttpItem()
                    {
                        URL = tokensrc,//URL     必需项    
                        Method = "GET",//URL     可选项 默认为Get   
                        ProxyIp = "ieproxy",
                        Cookie = dto.cookie,
                        ContentType = "application/x-www-form-urlencoded",//ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
                    };
                    result = helpweb.GetHtml(item);
                    doc = NSoup.NSoupClient.Parse(result.Html);
                    string sf_index = doc.Select(".sf_index").Html();
                    if (string.IsNullOrEmpty(sf_index)|| string.IsNullOrEmpty(result.Cookie))
                    {
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "GetHtmlByLoginCache 3_1_7 访问IFrame页面失败result.Cookie: " + result.Cookie + "HTML:"+ result.Html);
                        return -6;
                    }
                }
                catch (Exception er) {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "GetHtmlByLoginCache 3_1_6 访问充值页面Iframe页面异常" + er.Message);
                    return -6;
                }
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "GetHtmlByLoginCache 3_1_6 访问完首页上的IFrame页面,得到充值的Cookie: " + result.Cookie);
                //访问完token页面后得到cookie值，并取得“立即办理”这个虚拟路径URL
                //dto.cookie += ";" + result.Cookie;  //
                string CzCookie = result.Cookie;  //保存一个临时
                string blhref = doc.Select(".sf_index a")[0].Attr("href");      //取得虚拟路径URL
                if (string.IsNullOrEmpty(czhost) && string.IsNullOrEmpty(blhref)) {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "GetHtmlByLoginCache 3_1_7 获取IFrame页面上'立即办理'的URL失败");
                    return -7;
                }
                tokensrc = "http://" + czhost + blhref;                         //与host拼接生成完整的路径URL
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "GetHtmlByLoginCache 3_1_7 获取IFrame页面上'立即办理'的URL:" + tokensrc);
                item = new HttpItem()
                {
                    URL = tokensrc,//URL     必需项    
                    Method = "GET",//URL     可选项 默认为Get   
                    ProxyIp = "ieproxy",
                    Cookie = CzCookie,
                    ContentType = "application/x-www-form-urlencoded",//ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
                };
                try
                {
                    result = helpweb.GetHtml(item);
                }
                catch (Exception er) {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "GetHtmlByLoginCache 3_1_8 访问'立即办理'页面异常:" + er.Message);
                    return -8;
                }
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "GetHtmlByLoginCache 3_1_8 访问'立即办理'页面完成");
                //流量池分配URL
                doc = NSoup.NSoupClient.Parse(result.Html);
                blhref = doc.Select("#tab2").Attr("href");
                if (string.IsNullOrEmpty(czhost) && string.IsNullOrEmpty(blhref)){
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "GetHtmlByLoginCache 3_1_9 获取流量池分配URL失败");
                    return -9;
                }
                tokensrc = "http://" + czhost + blhref;
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "GetHtmlByLoginCache 3_1_9 获取流量池分配URL:" + tokensrc);
                item = new HttpItem()
                {
                    URL = tokensrc,//URL     必需项    
                    Method = "GET",//URL     可选项 默认为Get   
                    ProxyIp = "ieproxy",
                    Cookie = CzCookie,
                    ContentType = "application/x-www-form-urlencoded",//ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
                };
                try
                {
                    result = helpweb.GetHtml(item);
                }
                catch (Exception er) {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "GetHtmlByLoginCache 3_1_10 访问流量池分配页面异常:" + er.Message);
                    return -10;
                }
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "GetHtmlByLoginCache 3_1_10 访问完流量池分配页面");
                //流量池分配-》批量用户分配
                doc = NSoup.NSoupClient.Parse(result.Html);
                var tabobj = doc.Select(".sf_tab");     //当前账号已经没有流量池的权限了
                if (tabobj.IsEmpty) {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "GetHtmlByLoginCache 3_1_11 类型: " + ctype + " 期号：" + issue + " 已经没有流量池的业务了");
                    return -11;
                }
                blhref = doc.Select(".sf_tab li")[1].Attr("onclick");
                string[] str1 = blhref.Split(new string[] { "','", "'" }, StringSplitOptions.RemoveEmptyEntries);
                if (string.IsNullOrEmpty(blhref) && str1.Length < 1) {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "GetHtmlByLoginCache 3_1_11 获取批量用户分配URL失败");
                    return -12;
                }
                tokensrc = "http://" + czhost + str1[1];
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "GetHtmlByLoginCache 3_1_11 获取批量用户分配URL:" + tokensrc);
                CreateCzCache(ctype, issue, tokensrc, CzCookie);      //设置CZCookie缓存
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "GetHtmlByLoginCache 3_1_12 类型: " + ctype + " 期号：" + issue + "缓存设置成功");
            }
            catch (Exception er)
            {
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "GetHtmlByLoginCache 3_1 全局异常：" + er.Message);
                return -1100;
            }
            return 1;
        }
        /// <summary>
        /// 将批量用户充值的URL和CZCookie保存到缓存里面,名称叫:ctype_czcache_issue,例如:26_czcache_1
        /// </summary>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        /// <param name="url"></param>
        /// <param name="cookies"></param>
        private void CreateCzCache(int ctype, int issue, string url, string cookies)
        {
            Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "CreateCzCache 3_1_11_1 类型: " + ctype + " 期号：" + issue + "缓存设置成功");
            czcachedto dto = new czcachedto();
            dto.czurl = url; dto.cookie = cookies;
            FJSZ.OA.Common.CacheAccess.InsertToCacheByTimeE(ctype + "_czcache_" + issue, dto, 300);
        }
        /// <summary>
        /// 生成要进行充值的用户列表，并生成execl
        /// </summary>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        /// <returns></returns>
        public int FindDtByCtype(int ctype, int issue)
        {
            Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "FindDtByCtype 3_1_1 开始读取类型" + ctype + "期号" + issue + "的配置,这里需要一个每人的金额和到期时间");
            IList<T_CooperConfig> list = DataTableToList.ModelConvertHelper<T_CooperConfig>.ConvertToModel(dal.GetCooperConfig(ctype, issue));
            if (list.Count > 0)
            {
                T_CooperConfig dto = list[0];
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "FindDtByCtype 3_1_2 成功读到" + ctype + "期号" + issue + "的配置信息,eachflow:"+dto.eachflow+ "cutdate:"+dto.cutdate);
                if (dto.eachflow == 0 || string.IsNullOrEmpty(dto.cutdate))
                {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "FindDtByCtype 3_1_3 类型ctype: " + ctype + "期号：" + issue + "未进行每个用户流量或者截止时间没有配置.T_CooperConfig");
                    return -2;
                }
                try
                {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "FindDtByCtype 3_1_3 开始读取" + ctype + "期号" + issue + "的直充待充值用户记录");
                    ////直充
                    //var dt = mdal.FindFlowLogToExecl(ctype, issue, dto.eachflow, dto.cutdate);  //将直充的用户查询到datatable返回,如果后面大转盘要自动充值,需要在这里做文章
                    ////活动
                    //var dt1 = mdal.FindActivityFlowLog(1, dto.id, dto.cutdate);
                    //var mergedt = mdal.MergeDt(dt, dt1);
                    var mergedt = mdal.FindFlowLogToExecl(ctype, issue, dto.eachflow, dto.cutdate);  //将直充的用户查询到datatable返回,如果后面大转盘要自动充值,需要在这里做文章
                    if (mergedt.Rows.Count > 0)
                    {
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "FindDtByCtype 3_1_4 读取到" + ctype + "期号" + issue + "的直充待充值用户记录数:" + mergedt.Rows.Count);
                        int result = Common.Helper.HmExcelAssist.DataTabletoExcel(mergedt, AppDomain.CurrentDomain.BaseDirectory + (@"Content\Txt\") + "flowPoolExcel.xls");
                        if (result == 1)
                            Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "FindDtByCtype 3_1_5 读取到" + ctype + "期号" + issue + "的待充值用户记录生成EXECL表成功");
                        else
                            Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "FindDtByCtype 3_1_5 读取到" + ctype + "期号" + issue + "的待充值用户记录生成EXECL表失败");
                        return result;
                    }
                    else
                    {
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "FindDtByCtype 3_1_4 类型ctype: " + ctype + "期号：" + issue + "未取得需要充值的用户");
                        return 2;
                    }
                }
                catch (Exception er)
                {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "FindDtByCtype 3_1_4 类型ctype: " + ctype + "期号：" + issue + "生成flowPoolExcel.xls异常了。" + er.Message);
                    return -3;
                }
            }
            return -1;
        }
        /// <summary>
        /// 上传Execl文件,并且发送充值短信
        /// </summary>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        /// <param name="url">流量池分配-》批量用户分配 页面的URL</param>
        /// <param name="cookie">CZCookie</param>
        /// <returns></returns>
        private int plyhfp(int ctype, int issue, string url, string cookie)
        {
            try
            {
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "plyhfp 3_2_1 开始上传类型: " + ctype + " 期号：" + issue + "url：" + url + " Cookie: " + cookie);
                HttpHelper helpweb = new HttpHelper();  //初始实例化HttpHelper
                HttpResult result = new HttpResult();   //初始实例化HttpResult
                HttpItem item = new HttpItem()
                {
                    URL = url,//URL     必需项    
                    Method = "GET",//URL     可选项 默认为Get   
                    ProxyIp = "ieproxy",
                    Cookie = cookie,
                    ContentType = "application/x-www-form-urlencoded",//ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
                };
                try
                {
                    result = helpweb.GetHtml(item);
                }
                catch (Exception er) {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "plyhfp 3_2_1 访问 流量池分配-》批量用户分配 页面异常:" + er.Message);
                    return -2;
                }
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "plyhfp 3_2_1 成功访问 流量池分配-》批量用户分配 页面");
                //上传预览xls文件
                NSoup.Nodes.Document doc = NSoup.NSoupClient.Parse(result.Html);
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "plyhfp 3_2_2 开始解析HTML" + result.Html);
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
                if (string.IsNullOrEmpty(abc)) {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "plyhfp 3_2_3 如果连abc的值都没有取到,说明这个页面不是我们要的批量充值页面");
                    return -3;
                }
                string param = "abc=" + abc + "&abc=" + abc1 + "&BeginDate=" + BeginDate + "&EndDate=" + EndDate + "&deal_Id=" + deal_Id + "&BUSINESS=" + BUSINESS + "&TF_MODE=" + TF_MODE + "&predent=C:\fakepath\flowPoolExcel.xls";
                string tokensrc = "http://" + czhost + "/payflow/gm_fm/importFlowPoolExcel.do?" + param;
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "plyhfp 3_2_4 生成上传execl文件的URL：" + tokensrc);
                CookieContainer ttainer = ToCookieContainer(cookie);
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "plyhfp 3_2_5 拼接生成Cookie容器");
                string resposeresult = HttpPostData(tokensrc, 100000, "upload", predent, null, ttainer);    //上传execl,100000毫秒=1分钟
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "plyhfp 3_2_6 execl文件上传结果：" + resposeresult);
                if (!string.IsNullOrEmpty(resposeresult))
                {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "plyhfp 3_2_7 开始解析文件上传返回空值");
                    dlcgdto dto = JsonConvert.DeserializeObject<dlcgdto>(resposeresult);
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "plyhfp 3_2_8 dto.model_1"+ dto.model_1);
                    //List<string> tlist = (List<string>)dto.model_1;
                    //if (dto.chAmount != null & dto.chAmount.IsNum())
                    if (dto.model_1.Length == 0)
                    {
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "plyhfp 3_2_8 解析文件上传返回结果正常");
                        IMPORT_CODE = dto.model_2;  //重新附值---只有导入成功以后才会有值
                        string paramstr = "abc=" + abc2 + "&IMPORT_CODE=" + IMPORT_CODE + "&PACKTYPE=" + PACKTYPE + "&DISCOUNT=" + DISCOUNT + "&ECORDERID=" + ECORDERID + "&dealId=" + dealId + "&grid=" + grid;
                        url = "http://" + czhost + "/payflow/msgCode/getReTime.do";   //发送短信前
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "plyhfp 3_2_9 开始调用充值的短信URL_1:" + url);
                        item = new HttpItem()
                        {
                            URL = url,//URL     必需项    
                            Method = "GET",//URL     可选项 默认为Get   
                            ProxyIp = "ieproxy",
                            Cookie = cookie,
                            ContentType = "application/x-www-form-urlencoded",//ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
                        };
                        result = helpweb.GetHtml(item);
                        url = "http://" + czhost + "/payflow/msgCode/get.do";   //发送短信
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "plyhfp 3_2_10 开始调用充值的短信URL_2:" + url+"URL_1的结果:"+ result.Html);
                        item = new HttpItem()
                        {
                            URL = url,//URL     必需项    
                            Method = "GET",//URL     可选项 默认为Get   
                            ProxyIp = "ieproxy",
                            Cookie = cookie,
                            ContentType = "application/x-www-form-urlencoded",//ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
                        };
                        result = helpweb.GetHtml(item);
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "plyhfp 3_2_11 开始调用充值的短信URL_2的结果:" + result.Html);
                        msgdto msgdto = JsonConvert.DeserializeObject<msgdto>(result.Html);
                        if (msgdto != null && msgdto.status == 0)
                        {
                            int xhresult = mdal.UpdateLogCacheXh(ctype, issue, "10657532190000761", msgdto.seq, paramstr);    //将短信的序号更新到数据库
                            return 1;
                        }
                        else
                        {
                            Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "plyhfp 3_2_12 短信发送失败：" + result.Html);
                            return -6;
                        }
                    }
                    else
                    {
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "plyhfp 3_2_8 execl文件上传返回异常值：" + resposeresult);
                        return -5;
                    }
                }
                else
                {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "plyhfp 3_2_7 execl文件上传返回空值");
                    return -4;
                }
            }
            catch (Exception er)
            {
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "plyhfp 3_2_1 全局执行出现异常" + er.Message);
            }
            return -1;
        }
        /// <summary>
        /// 拼接生成CookieContainer的容器
        /// </summary>
        /// <param name="cookies"></param>
        /// <returns></returns>
        public CookieContainer ToCookieContainer(string cookies)
        {
            NameValueCollection dic = new NameValueCollection();
            CookieContainer ner = new CookieContainer();
            string[] cookiestr = cookies.Split('=');
            for (int i = 0; i < cookiestr.Length; i++)
            {
                dic.Add(cookiestr[i], cookiestr[i + 1].Split(';')[0]);
                ner.Add(new Cookie(cookiestr[i], cookiestr[i + 1].Split(';')[0], "/", "218.207.214.83"));
                i++;
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
        /// 发送登入的短信,并将短信序列号更新到数据库,并生成ViewState和Cookie的缓存,返回int类型1为成功.负数为失败
        /// </summary>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        public int HelpWebSend(int ctype, int issue)
        {
            IList<T_CooperConfig> list = DataTableToList.ModelConvertHelper<T_CooperConfig>.ConvertToModel(dal.GetCooperConfig(ctype, issue));
            if (list.Count > 0)
            {
                T_CooperConfig dto = list[0];
                int result_1 = KeepSessionUsered(ctype, issue, 1);    //先判断登入Session是否有效,返回1为有效
                if (result_1 == 1)
                {
                    return 1;
                }
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "HelpWebSend 2 开始发送登入短信");
                string baseurl = "http://www.fj.10086.cn/power/ADCECPortal/PowerLogin.aspx?ReturnUrl=ADCQDLPortal&test=t";
                //访问登入页面
                HttpHelper helpweb = new HttpHelper();  //初始实例化HttpHelper
                HttpResult result = new HttpResult();   //初始实例化HttpResult
                HttpItem item = new HttpItem()
                {
                    URL = baseurl,//URL     必需项    
                    Method = "GET",//URL     可选项 默认为Get   
                    ProxyIp = "ieproxy",
                    ContentType = "application/x-www-form-urlencoded",//ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
                };
                try
                {
                    result = helpweb.GetHtml(item);
                }
                catch (Exception er)
                {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "HelpWebSend 3 登入首页异常" + er.Message);
                    return -1;
                }
                string viewstate = "";                      //定义ViewState变量
                string cookie = fhcookie(result.Cookie);    //合并Cookie
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "HelpWebSend 3 选择短信方式登入 Cookie: " + cookie);
                //选择短信登入
                helpweb = new HttpHelper();
                item = new HttpItem()
                {
                    URL = baseurl,//URL     必需项    
                    Method = "POST",//URL     可选项 默认为Get   
                    ProxyIp = "ieproxy",
                    Cookie = cookie.ToString(),
                    ContentType = "application/x-www-form-urlencoded",//ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
                    Postdata = "__EVENTTARGET=rbl_PType%241&__EVENTARGUMENT=&__LASTFOCUS=&__VIEWSTATE=" + System.Web.HttpUtility.UrlEncode(viewstate) + "&__VIEWSTATEGENERATOR=CC3279BD&__VIEWSTATEENCRYPTED=&LoginType=1&SMSTimes=90&SMSAliasTimes=90&txtCorpCode=&txtUserName=&rbl_PType=2&txtPd=&txtCheckCode=&txtQDLRegisterUrl=%2FADCQDLPortal%2FProduction%2FProductOrderControl.aspx"
                };
                try
                {
                    //请求的返回值对象
                    result = helpweb.GetHtml(item);
                }
                catch (Exception er)
                {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "HelpWebSend 4 选择短信方式登入异常：" + er.Message);
                    return -2;
                }
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "HelpWebSend 4 准备开始发送登入短信");
                //发送短信
                helpweb = new HttpHelper();
                item = new HttpItem()
                {
                    URL = baseurl,//URL     必需项    
                    Method = "POST",//URL     可选项 默认为Get   
                    ProxyIp = "ieproxy",
                    Cookie = cookie.ToString(),
                    ContentType = "application/x-www-form-urlencoded",//ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
                    Postdata = "__EVENTTARGET=lbtn_GetSMS&__EVENTARGUMENT=&__LASTFOCUS=&__VIEWSTATE=" + System.Web.HttpUtility.UrlEncode(viewstate) + "&__VIEWSTATEGENERATOR=CC3279BD&__VIEWSTATEENCRYPTED=&LoginType=1&SMSTimes=90&SMSAliasTimes=90&txtCorpCode=" + dto.corpid + "&txtUserName=" + dto.username + "&rbl_PType=2&SMSP=&txtCheckCode=&txtQDLRegisterUrl=%2FADCQDLPortal%2FProduction%2FProductOrderControl.aspx"
                };
                try
                {
                    result = helpweb.GetHtml(item);
                }
                catch (Exception er)
                {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "HelpWebSend 5 发送登入短信异常：" + er.Message);
                    return -3;
                }
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "HelpWebSend 6 成功发送登入短信,开始解析HMTL,取得序列号和ViewState");
                //获取请请求的Html
                string html = result.Html;
                NSoup.Nodes.Document doc = NSoup.NSoupClient.Parse(result.Html);
                string blhref = doc.Select("#lab_SMSIndex").Html();     //取得序列号字符串
                viewstate = doc.Select("#__VIEWSTATE").Val();           //取得ViewState的值
                string xlh = System.Text.RegularExpressions.Regex.Replace(blhref, @"[^0-9]+", "");  //序列号数字
                if (!string.IsNullOrEmpty(blhref) && !string.IsNullOrEmpty(xlh))
                {
                    FJSZ.OA.Common.CacheAccess.InsertToCacheByTime(dto.corpid + "_cookie", cookie.ToString(), 3600);
                    FJSZ.OA.Common.CacheAccess.InsertToCacheByTime(dto.corpid + "_viewstate", viewstate, 3600);
                    int result_2 = mdal.UpdateLogCacheDlXh(ctype, issue, "10657030", xlh);     //更新超端当前登入的单子的状态和序列号,以方便后面接收短信的时候使用
                    if (result_2 == 1)
                    {
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "HelpWebSend 7 生成Cookie,ViewState缓存Cache并更新保存到数据库");
                        return 1;
                    }
                    else
                    {
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "HelpWebSend 7 将序列号更新到数据为失败");
                        return -4;
                    }
                }
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "HelpWebSend 7 解析HTML失败");
                return -5;
            }
            else
            {
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "HelpWebSend 1 类型: " + ctype + " 期号：" + issue + "缺少超端直充配置");
                return -10;
            }
        }
        /// <summary>
        /// 接收登入短信,更新T_CooperConfig的密码字段,并且调用TakeLoginMsgForCache生成登入Session
        /// </summary>
        /// <param name="phone">发送登入短信的号码</param>
        /// <param name="xh">序列号</param>
        /// <param name="code">登入短信验证码</param>
        /// <returns></returns>
        public int UpdateConfigPwd(string phone, string xh, string code)
        {
            //通过发送短信手机号码和登入的序列号取得超端的记录
            IList<ctypedto> list = DataTableToList.ModelConvertHelper<ctypedto>.ConvertToModel(mdal.FindCtypeIssueForDl(phone, xh));
            if (list.Count == 1)
            {
                ctypedto dto = list[0];
                if (dto != null)
                {
                    int result = mdal.UpdateConfigPwd(dto.ctype, dto.issue, code);  //更新登入密码

                    int result_1 = TakeLoginMsgForCache(dto.ctype, dto.issue);

                    return result;
                }
            }
            return -1;
        }
        /// <summary>
        /// 尝试登入并获取登入Cookie保存到数据库超端记录表
        /// </summary>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        /// <returns></returns>
        public int TakeLoginMsgForCache(int ctype, int issue)
        {
            IList<T_CooperConfig> list = DataTableToList.ModelConvertHelper<T_CooperConfig>.ConvertToModel(dal.GetCooperConfig(ctype, issue));
            if (list.Count > 0)
            {
                T_CooperConfig dto = list[0];
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "TakeLoginMsgForCache 1 类型" + ctype + "期号" + issue + "的配置存在");
                HttpHelper helpweb = new HttpHelper();  //初始实例化HttpHelper
                HttpResult result = new HttpResult();   //初始实例化HttpResult
                string baseurl = "http://www.fj.10086.cn/power/ADCECPortal/PowerLogin.aspx?ReturnUrl=ADCQDLPortal&test=t";
                string cookie = (string)FJSZ.OA.Common.CacheAccess.GetFromCache(dto.corpid + "_cookie");
                string viewstate = (string)FJSZ.OA.Common.CacheAccess.GetFromCache(dto.corpid + "_viewstate");
                if (string.IsNullOrEmpty(cookie) || string.IsNullOrEmpty(viewstate))
                {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "TakeLoginMsgForCache 2 类型" + ctype + "期号" + issue + "的Cookie或者viewstate为空 strCookies: " + cookie + " viewstate: " + viewstate);
                    return -2;
                }
                HttpItem item = new HttpItem()
                {

                    URL = baseurl,//URL     必需项    
                    Method = "post",//URL     可选项 默认为Get
                    ProxyIp = "ieproxy",
                    ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值
                    Postdata = "__EVENTTARGET=&__EVENTARGUMENT=&__LASTFOCUS=&__VIEWSTATE=" + System.Web.HttpUtility.UrlEncode(viewstate) + "&__VIEWSTATEGENERATOR=CC3279BD&__VIEWSTATEENCRYPTED=&LoginType=1&SMSTimes=25&SMSAliasTimes=90&txtCorpCode=" + dto.corpid + "&txtUserName=" + dto.username + "&rbl_PType=2&SMSP=" + dto.userpwd + "&txtCheckCode=&button3=%E7%99%BB%E5%BD%95&txtQDLRegisterUrl=%2FADCQDLPortal%2FProduction%2FProductOrderControl.aspx",//Post要发送的数据
                    Cookie = cookie.ToString()
                };
                try
                {
                    //请求的返回值对象
                    result = helpweb.GetHtml(item);
                }
                catch (Exception er)
                {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "TakeLoginMsgForCache 3 类型" + ctype + "期号" + issue + "的接收短信发生异常: " + er.Message);
                    return -3;
                }
                cookie = fhcookie(result.Cookie);
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "TakeLoginMsgForCache 3 类型" + ctype + "期号" + issue + "的访问结果开始解析HTML");
                NSoup.Nodes.Document doc = NSoup.NSoupClient.Parse(result.Html);
                NSoup.Select.Elements elscript = doc.GetElementsByTag("script");        //取得跳转地址字符串
                if (elscript.Count > 1)
                {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "TakeLoginMsgForCache 4 类型" + ctype + "期号" + issue + "的解析结果不正常:" + result.Html);
                    return -4;
                }
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "TakeLoginMsgForCache 4 类型" + ctype + "期号" + issue + "的取得script字符串:" + elscript);
                baseurl = "http://www.fj.10086.cn" + elscript[0].Data.Split('\'')[1];     //拼接出正确的跳转地址
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "TakeLoginMsgForCache 5 类型" + ctype + "期号" + issue + "的取得跳转的URL:" + baseurl);
                item = new HttpItem()
                {

                    URL = baseurl,//URL     必需项    
                    ProxyIp = "ieproxy",
                    ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值
                    Cookie = cookie.ToString()
                };
                try
                {
                    result = helpweb.GetHtml(item);
                }
                catch (Exception er)
                {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "TakeLoginMsgForCache 6 类型" + ctype + "期号" + issue + "的最后一次授权跳转异常:" + er.Message);
                    return -5;
                }
                cookie = fhcookie(result.Cookie);
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "TakeLoginMsgForCache 6 类型" + ctype + "期号" + issue + "的最后一次授权结束cookie:" + cookie);
                baseurl = "http://www.fj.10086.cn/power/ADCECPortal/PowerLogin.aspx?ReturnUrl=ADCQDLPortal&test=t";
                item = new HttpItem()
                {
                    URL = baseurl,//URL     必需项    
                    Method = "GET",//URL     可选项 默认为Get   
                    ProxyIp = "ieproxy",
                    ContentType = "application/x-www-form-urlencoded",//ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
                    Cookie = cookie.ToString()
                };
                try
                {
                    result = helpweb.GetHtml(item);
                }
                catch (Exception er)
                {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "TakeLoginMsgForCache 7 类型" + ctype + "期号" + issue + "的访问登入页面确认有效性异常:" + er.Message);
                    return -6;
                }
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "TakeLoginMsgForCache 7 类型" + ctype + "期号" + issue + "的开始解析登入HTML,验证Cookie有效性");
                doc = NSoup.NSoupClient.Parse(result.Html);
                string text = doc.Select("#pnWelCome").Html();
                if (string.IsNullOrEmpty(text))
                {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "TakeLoginMsgForCache 8 类型" + ctype + "期号" + issue + "的访问登入页面确认有效性失败");
                    return -7;
                }
                int result_1 = UpdateDlCookie(ctype, issue, cookie);
                if (result_1 == 1)
                {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "TakeLoginMsgForCache 8 类型" + ctype + "期号" + issue + "的更新到超端记录成功.");
                    return 1;
                }
                else
                {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "TakeLoginMsgForCache 8 类型" + ctype + "期号" + issue + "的更新到超端记录成功.");
                    return -8;
                }
            }
            Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "TakeLoginMsgForCache 1 类型" + ctype + "期号" + issue + "的配置为空");
            return -1;
        }
        /// <summary>
        /// 将登入的cookie更新到超端记录表中
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
        /// 
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


        #endregion





        /// <summary>
        /// 保持平台Session活跃30分钟，保持充值Session活跃5分钟,也可以用来判断是否有效
        /// </summary>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        /// <param name="lx">1为登入Session,2为充值Session</param>
        public int KeepSessionUsered(int ctype,int issue,int lx) {
            //登入
            NSoup.Nodes.Document doc;
            if (lx == 1)
            {
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "KeepSessionUsered 1 类型: " + ctype + " 期号：" + issue + " 开始验证登入的有效性");
                IList<T_LoginLogCache> list = DataTableToList.ModelConvertHelper<T_LoginLogCache>.ConvertToModel(mdal.GetLoginCache(ctype, issue));
                string baseurl = "http://www.fj.10086.cn/power/ADCECPortal/PowerLogin.aspx?ReturnUrl=ADCQDLPortal&test=t";
                if (list.Count > 0)
                {
                    T_LoginLogCache dto = list[0];
                    HttpHelper helpweb = new HttpHelper();  //初始实例化HttpHelper
                    HttpResult result = new HttpResult();   //初始实例化HttpResult
                    HttpItem item = new HttpItem()          //初始实例化HttpItem
                    {
                        URL = baseurl,//URL     必需项    
                        Method = "GET",//URL     可选项 默认为Get   
                        ProxyIp = "ieproxy",
                        Cookie = dto.cookie,
                        ContentType = "application/x-www-form-urlencoded",//ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
                    };
                    try
                    {
                        result = helpweb.GetHtml(item);
                        doc = NSoup.NSoupClient.Parse(result.Html);
                        string text = doc.Select("#pnWelCome").Html();
                        if (string.IsNullOrEmpty(text))
                        {
                            Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "KeepSessionUsered 3  类型: " + ctype + " 期号：" + issue + " 解析登入HMTL,失败 ");
                            return -1;
                        }
                    }
                    catch (Exception er) {
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "KeepSessionUsered 2 类型: " + ctype + " 期号：" + issue + "访问异常:" + er.Message);
                        return -3;
                    }
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "KeepSessionUsered 3 类型: " + ctype + " 期号：" + issue + "登入Session有效");
                    return 1;
                }
                else {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "KeepSessionUsered 1 类型: " + ctype + " 期号：" + issue + " 超端记录异常 ");
                    return -4;
                }
            }
            else if (lx == 2) {
                czcachedto czdto = (czcachedto)FJSZ.OA.Common.CacheAccess.GetFromCache(ctype + "_czcache_" + issue);
                if (czdto != null)
                {
                    HttpHelper helpweb = new HttpHelper();  //初始实例化HttpHelper
                    HttpResult result = new HttpResult();   //初始实例化HttpResult
                    HttpItem item = new HttpItem()          //初始实例化HttpItem
                    {
                        URL = czdto.czurl,//URL     必需项    
                        Method = "GET",//URL     可选项 默认为Get   
                        ProxyIp = "ieproxy",
                        Cookie = czdto.cookie,
                        ContentType = "application/x-www-form-urlencoded",//ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
                    };
                    try
                    {
                        result = helpweb.GetHtml(item);
                        doc = NSoup.NSoupClient.Parse(result.Html);
                        string content = doc.Select("#tabContenth2").Html();     //取得序列号字符串
                        Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "KeepSessionUsered 1 类型: " + ctype + " 期号：" + issue + " HTML: " + result.Html);
                        if (result.StatusDescription == "OK" && !string.IsNullOrEmpty(content))
                        {
                            return 1;   //一旦他非空,那么这个CZCookie肯定是有效的
                        }
                        else
                        {
                            FJSZ.OA.Common.CacheAccess.RemoveCache(ctype + "_czcache_" + issue);
                            return -1;
                        }
                    }
                    catch (Exception er) {
                        FJSZ.OA.Common.CacheAccess.RemoveCache(ctype + "_czcache_" + issue);
                        return -1;
                    }
                }
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
        /// 添加超端记录,返回-1000表示已经存在超端记录
        /// </summary>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        /// <returns></returns>
        public int InsertLoginCache(int ctype,int issue) {
            int result = mdal.IsExistsT_LoginLogCache(ctype, issue);
            if (result > 0) return -1000;
            T_LoginLogCache dto = new T_LoginLogCache();
            dto.ctype = ctype;dto.issue = issue;
            result = mdal.InsertLoginCache(dto);
            return result;
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
                        int result = mdal.RemoveLoginCache(gid);
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

        


        /// <summary>
        /// 临时添加更新一下密码值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="issue"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public int testuppwd(int type, int issue, string code)
        {
            int result = mdal.UpdateConfigPwd(type, issue, code);
            return result;
        }
    }
    public class czcachedto {
        public string czurl { get; set; }
        public string cookie { get; set; }
    }
    public class dlcgdto {
        public int errorCode { get; set; }
        public string[] model_1 { get; set; }
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
        public int id { get; set; }
        public int ctype { get; set; }
        public int issue { get; set; }
        public string dlcookie { get; set; }
        public string cookie { get; set; }
        public string czparam { get; set; }
        public string czurl { get; set; }
    }
}
