using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.WxModel
{
    public class T_TakeFlowLog
    {
        public int id { get; set; }
        public int ctype { get; set; }
        public int issue { get; set; }
        public string openid { get; set; }
        public string phone { get; set; }
        public int state { get; set; }
        public string addtime { get; set; }
    }
}
