using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL
{
    public class SqlPageParam
    {
        public string TableName { get; set; }
        public string PrimaryKey { get; set; }
        public string Fields { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public string Filter { get; set; }
        public string Group { get; set; }
        public string Order { get; set; }
        public int Total { get; set; }
    }
}
