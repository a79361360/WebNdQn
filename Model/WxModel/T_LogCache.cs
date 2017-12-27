using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.WxModel
{
    public class T_LogCache
    {
        public int id { get; set; }
        public int ctype { get; set; }
        public int issue { get; set; }
        public string corpid { get; set; }
        public string phone { get; set; }
        public string csrf { get; set; }
        public int dlyzm { get; set; }
        public int czyzm { get; set; }
        public string dlcookie { get; set; }
        public string czcookie { get; set; }
        public int state { get; set; }
    }
}
