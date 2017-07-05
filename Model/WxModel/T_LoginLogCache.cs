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
        public int issue { get; set; }
        public string cookie { get; set; }
        public DateTime lasttime { get; set; }
    }
}
