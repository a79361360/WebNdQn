﻿using System;
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
        public int type { get; set; }
        public string title { get; set; }
        public int share { get; set; }
        public string explain { get; set; }
        public string bgurl { get; set; }
    }
}