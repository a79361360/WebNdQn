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
    public static class Wx_config{
        public static string appid = "wx0d8924c9bc2c6e11";
        public static string appsecret = "9c0125be80b710c17e09124f13c82b24";
    }
    public class WxJsApi_token {
        public string access_token { get; set; }
        public string expires_in { get; set; }
    }
    public class WxJsApi_ticket
    {
        public int errcode { get; set; }
        public string errmsg { get; set; }
        public string ticket { get; set; }
        public string expires_in { get; set; }
    }
}
