using BLL;
using FrameWork;
using Model.WishModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebNdQn.Controllers
{
    public class WishController : BaseController
    {
        // GET: Wish
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult wishlist() {
            WishBLL bll = new WishBLL();
            var result = bll.WishList();
            return JsonFormat(new ExtJson { success = true, code = 1000, msg = "查询成功", jsonresult = result });
        }
        public ActionResult addwishhelper(string name,string phone,int wishid,int sendtype,string sendmsg="") {
            if (string.IsNullOrEmpty(name))
                return JsonFormat(new ExtJson { success = false, code = -1000, msg = "请输入你的姓名", jsonresult = null });
            if (string.IsNullOrEmpty(phone))
                return JsonFormat(new ExtJson { success = false, code = -1000, msg = "请输入你的联系方式", jsonresult = null });
            if (wishid > 67 || wishid < 0)
                return JsonFormat(new ExtJson { success = false, code = -1000, msg = "捐助的非法编号", jsonresult = null });

            WishBLL bll = new WishBLL();
            T_WishHelper o = new T_WishHelper();
            o.helpername = name;
            o.phone = phone;
            o.wishid = wishid;
            o.sendtype = sendtype;
            o.sendmsg = sendmsg;
            var result = bll.WishHelperAdd(o);
            if (result > 0)
                return JsonFormat(new ExtJson { success = true, code = 1000, msg = "认捐成功，感谢！<br/> 转发朋友圈，让更多小伙伴加入吧！", jsonresult = result });
            else if (result == -10)
                return JsonFormat(new ExtJson { success = true, code = 1000, msg = "当前捐助编号已经存在捐助人", jsonresult = "" });
            return JsonFormat(new ExtJson { success = false, code = -1000, msg = "认捐失败", jsonresult = "" });
        }
        public ActionResult te() {
            WishBLL bll = new WishBLL();
            FileStream fileStream = new FileStream(@"C:\Users\Administrator\Desktop\1111.txt", FileMode.Open);
            using (StreamReader reader = new StreamReader(fileStream))
            {
                string nextLine; T_Wish o;
                //string line = reader.ReadLine();
                while ((nextLine = reader.ReadLine()) != null)
                {
                    o = new T_Wish();
                    o.wishid = Convert.ToInt32(nextLine.Split('-')[0]);
                    o.name = nextLine.Split('-')[1];
                    o.sex = Convert.ToInt32(nextLine.Split('-')[2]);
                    o.age = Convert.ToInt32(nextLine.Split('-')[3]);
                    o.wishcontent = nextLine.Split('-')[4];
                    bll.WishAdd(o);
                }
            }
            return JsonFormat(new ExtJson { success = true, code = 1000, msg = "查询成功", jsonresult = null });
            //StreamReader sR = File.OpenText(@"C:\Users\Administrator\Desktop\1111.txt");
            //string nextLine;
            //while ((nextLine = fileStream.ReadLine()) != null)
            //{
            //    Console.WriteLine(nextLine);
            //}
            //sR.Close();
        }


        public ActionResult Wx_Share(string url) {
            WishBLL bll = new WishBLL();
            var result = bll.TakeWxShareInfo(url);
            return JsonFormat(new ExtJson { success = true, code = 1000, msg = "查询成功", jsonresult = result });
        }
    }
}