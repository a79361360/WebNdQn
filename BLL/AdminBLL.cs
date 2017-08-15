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
            if (!string.IsNullOrEmpty(filter))
                filter = "where" + filter;
            IList<T_TakeFlowLog> list = DataTableToList.ModelConvertHelper<T_TakeFlowLog>.ConvertToModel(dal.FlowList(filter, ctype, issue, phone));
            return list;
        }
        public IList<T_CooperConfig> GetCooper_Page(string name, string value, int state,int pageSize,int pageIndex, ref int Total)
        {
            string filter = "";
            if (name != "-1")
            {
                filter += name + " like '%" + value + "%'";
            }
            if (state != -1)
            {
                if (!string.IsNullOrEmpty(filter))
                    filter += " and state=" + state;
                else
                    filter += " state=" + state;
            }
            if (!string.IsNullOrEmpty(filter))
                filter = "where" + filter;

            SqlPageParam param = new SqlPageParam();
            param.TableName = "T_CooperConfig";
            param.PrimaryKey = "id";
            param.Fields = "id,ctype,issue,title,eachflow,uplimit,state";
            param.PageSize = pageSize;
            param.PageIndex = pageIndex;
            param.Filter = filter;
            param.Group = "";
            param.Order = "id";
            IList<T_CooperConfig> list = DataTableToList.ModelConvertHelper<T_CooperConfig>.ConvertToModel(dal.PageResult(ref Total, param));
            return list;
        }
    }
}
