using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.WishModel
{
    public class Wish_dto
    {
    }
    public class T_Wish {
        public int id { get; set; }
        public int wishid { get; set; }
        public string name { get; set; }
        public int sex { get; set; }
        public int age { get; set; }
        public string wishcontent { get; set; }
        public int whishelperid { get; set; }
    }
    public class T_WishHelper
    {
        public int id { get; set; }
        public string helpername { get; set; }
        public string phone { get; set; }
        public int wishid { get; set; }
        public int sendtype { get; set; }
        public string sendmsg { get; set; }
    }
    public class T_WxShareWish {
        public long timestamp { get; set; }
        public string noncestr { get; set; }
        public string signatrue { get; set; }
        public string appid { get; set; }
    }
}
