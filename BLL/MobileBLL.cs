using Common;
using CsharpHttpHelper;
using DAL;
using Model.WxModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class MobileBLL
    {
        CommonDAL dal = new CommonDAL();
        public void GetHtmlByLoginCache(int ctype,int issue) {
            IList<T_LoginLogCache> list = DataTableToList.ModelConvertHelper<T_LoginLogCache>.ConvertToModel(dal.GetLoginCache(ctype, issue));
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
        }
    }
}
