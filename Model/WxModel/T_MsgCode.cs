using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.WxModel
{
    public class T_MsgCode
    {
        public int id { get; set; }
        public int type { get; set; }
        public string phone { get; set; }
        public int xh { get; set; }
        public string code { get; set; }
        public string text { get; set; }
        public int state { get; set; }
        public DateTime addtime { get; set; }
    }
}
