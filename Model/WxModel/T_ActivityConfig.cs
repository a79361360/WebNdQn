using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.WxModel
{
    public class T_ActivityConfig
    {
        public int id { get; set; }
        public int cooperid { get; set; }
        /// <summary>
        /// 用视图V_ActivityConfig
        /// </summary>
        public int ctype { get; set; }
        public int type { get; set; }
        public string title { get; set; }
        public int share { get; set; }
        public string explain { get; set; }
        public string bgurl { get; set; }

        public string wx_title { get; set; }
        public string wx_descride { get; set; }
        public string wx_imgurl { get; set; }
        public string wx_linkurl { get; set; }
        public int dt_fs { get; set; }
        public int dt_tmts { get; set; }
        public int sright { get; set; }
        public int flowamount { get; set; }

    }
}
