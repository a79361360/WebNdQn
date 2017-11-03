using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.WxModel
{
    public class T_ActivityDrawLog
    {
        public int id { get; set; }
        public int cooperid { get; set; }
        public int type { get; set; }
        public string openid { get; set; }
        public string phone { get; set; }
        public int configlistid { get; set; }
        public string prizename { get; set; }
        public int state { get; set; }
        public string addtime { get; set; }
        public int score { get; set; }
        public int number { get; set; }

    }
}
