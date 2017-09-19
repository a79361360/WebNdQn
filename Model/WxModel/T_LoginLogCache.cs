using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.WxModel
{
    public class T_LoginLogCache
    {
        public int id { get; set; }
        public int ctype { get; set; }
        public int issue { get; set; }
        public string cookie { get; set; }
        public string czparam { get; set; }
        public string czdxhm { get; set; }
        public string dxxh { get; set; }
        public int state { get; set; }
        public string dlxh { get; set; }
        public int dlstate { get; set; }
        public DateTime lasttime { get; set; }
    }
}
