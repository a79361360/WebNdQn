using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.WxModel
{
    public class T_ZxdtScore
    {
        public int id { get; set; }
        public int configid { get; set; }
        public int number { get; set; }
        public int lower { get; set; }
        public int upper { get; set; }
        public DateTime addtime { get; set; }
    }
}
