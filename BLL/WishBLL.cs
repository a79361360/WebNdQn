using Common;
using DAL;
using Fgly.Common.Expand;
using Model.WishModel;
using Model.WxModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class WishBLL
    {
        public readonly object obj = new object();
        WishDAL dal = new WishDAL();
        public IList<T_Wish> WishList()
        {
            IList<T_Wish> list = DataTableToList.ModelConvertHelper<T_Wish>.ConvertToModel(dal.WishList());
            return list;
        }
        public int WishAdd(T_Wish o) {
            var result = dal.WishAdd(o);
            return result;
        }
        public int WishHelperAdd(T_WishHelper o)
        {
            lock (obj)
            {
                //非67的情况下
                if (o.wishid != 67)
                {
                    if (dal.IsExistWish(o.wishid) > 0) return -10;  //已经存在捐助人
                }
                var result = dal.WishHelperAdd(o);
                return result;
            }
        }

        public T_WxShareWish TakeWxShareInfo(string url) {
            WeiXinBLL wxll = new WeiXinBLL();
            T_WxShareWish o = new T_WxShareWish();
            o.timestamp = DateTime.Now.ToUnixTimeStamp();                               //时间戳
            o.noncestr = TxtHelp.GetRandomString(16, true, true, true, false, "");      //随机字符串
            o.signatrue = wxll.Get_signatureurl(o.timestamp, o.noncestr, url);          //signatrue
            o.appid = Wx_config.appid;                                                  //分享到朋友，这里的appid用的是自己公司的，域名只能自己操作
            return o;
        }
    }
}
