using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.WxModel
{
    public class T_ActivityConfigList
    {
        public int id { get; set; }
        public int configid { get; set; }
        public string prizename { get; set; }
        public int count { get; set; }
        public int number { get; set; }
        public string winprob { get; set; }
        /// <summary>
        /// 已经开奖的次数
        /// </summary>
        public int drowcount { get; set; }
    }
}
