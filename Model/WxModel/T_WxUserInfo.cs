using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.WxModel
{
    public class T_WxUserInfo
    {
        public int id { get; set; }
        public string wx_appid { get; set; }
        public string wx_openid { get; set; }
        public string wx_nickname { get; set; }
        public int wx_sex { get; set; }
        public string wx_headurl { get; set; }
        [DefaultValue("")]
        public string wx_unionid { get; set; }

    }
}
