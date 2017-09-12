using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.WxModel
{
    public class T_CooperConfig
    {
        public int id {get;set;}
        public int ctype { get;set;}
        public int issue { get; set; }
        public int areatype { get; set; }
        public string areatypen { get; set; }
        public string gener { get; set; }
        public string title {get;set;}
        public string descride {get;set;}
        public string imgurl {get;set;}
        public string btnurl { get; set; }
        public string bgurl { get; set; }
        public string linkurl {get;set;}
        public string redirecturi { get; set; }
        public string corpid { get; set; }
        public string username { get; set; }
        public string userpwd { get; set; }
        public string signphone { get; set; }
        public string wx_appid { get; set; }
        public string wx_secret { get; set; }
        public string qrcode_url { get; set; }
        public int eachflow { get; set; }
        public int uplimit { get; set; }
        public string cutdate { get; set; }
        public int state { get; set; }
        public DateTime addtime {get;set;}
    }
}
