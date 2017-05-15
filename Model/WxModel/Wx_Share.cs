using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.WxModel
{
    public class Wx_Share
    {

    }
    public class Wx_kf_Config{
        public string appid { get; set; }
        public string appsecret { get; set; }
    }
    public static class Wx_config {
        public static string appid = "wx0d8924c9bc2c6e11";
        public static string appsecret = "9c0125be80b710c17e09124f13c82b24";

        //public static string appid = "wx905707332cae0c38";
        //public static string appsecret = "7561c3788343a7b3787e26cdc818ae37";
    }
    public class WxJsApi_token {
        public string access_token { get; set; }
        public string expires_in { get; set; }
        public string refresh_token { get; set; }
        public string openid { get; set; }
        public string scope { get; set; }
    }
    public class WxJsApi_ticket
    {
        public int errcode { get; set; }
        public string errmsg { get; set; }
        public string ticket { get; set; }
        public string expires_in { get; set; }
    }
    public class Wx_UserInfo {
        public int subscribe { get; set; }
        public string openid { get; set; }
        public string nickname { get; set; }
        public int sex { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string province { get; set; }
        public string language { get; set; }
        public string headimgurl { get; set; }
        public string subscribe_time { get; set; }
        public string unionid { get; set; }
        public string remark { get; set; }
        public string groupid { get; set; }
    }
}
