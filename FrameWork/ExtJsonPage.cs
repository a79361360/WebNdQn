using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameWork
{
    public class ExtJsonPage
    {
        public bool success { get; set; }
        public int code{ get; set; }
        public string msg { get; set; }
        public int pageindex { get; set; }
        public int pagesize { get; set; }
        public int total { get; set; }
        public object list { get; set; }
    }
}
