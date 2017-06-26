using Common;
using DAL;
using Model.WxModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class AdminBLL
    {
        AdminDAL dal = new AdminDAL();
        public IList<T_TakeFlowLog> GetFlowList_Search(int ctype,int issue,string phone) {
            string filter = "";
            if (ctype != -1)
                filter += " ctype=@ctype";
            if (issue != 0)
            {
                if (!string.IsNullOrEmpty(filter))
                    filter += " and issue=@issue";
                else
                    filter += " issue=@issue";
            }
            if (!string.IsNullOrEmpty(phone))
            {
                if (!string.IsNullOrEmpty(filter))
                    filter += " and phone=@phone";
                else
                    filter += " phone=@phone";
            }
            filter = "where" + filter;
            IList<T_TakeFlowLog> list = DataTableToList.ModelConvertHelper<T_TakeFlowLog>.ConvertToModel(dal.FlowList(filter, ctype, issue, phone));
            return list;
        }
    }
}
