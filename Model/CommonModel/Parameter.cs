using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.CommonModel
{
    public class Parameter
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public Parameter(string _name, string _value)
        {
            this.Name = _name;
            this.Value = _value;
        }
    }
}
