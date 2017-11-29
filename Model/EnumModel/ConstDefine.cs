using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.EnumModel
{
    public class ConstDefine
    {
        public enum AwardType
        {
            [Description("Attribute")]
            实物奖励 = 0,
            [Description("这是一个描述")]
            乐豆 = 1,
            奖券 = 2,
            道具 = 3,
            元宝 = 4
        }
        public enum AreaType
        {
            宁德地区 = 1,
            莆田地区 = 2,
            福建地区 = 3
        }
    }
}
