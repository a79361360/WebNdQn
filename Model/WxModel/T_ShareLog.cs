using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.WxModel
{
    public class T_ShareLog
    {
        public int id { get; set; }
        public int cooperid { get; set; }
        public int atype { get; set; }
        public string openid { get; set; }
        public int sharetype { get; set; }
        public string addtime { get; set; }
    }
}
