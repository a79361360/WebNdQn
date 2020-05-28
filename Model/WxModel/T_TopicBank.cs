using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.WxModel
{
    public class T_TopicBank
    {
        public int id { get; set; }
        public int cooperid { get; set; }
        public int checkbox { get; set; }
        public string topic { get; set; }
        public string answer { get; set; }
        public string keyanswer { get; set; }
        public string tips { get; set; }
        public DateTime addtime { get; set; }
    }
}
