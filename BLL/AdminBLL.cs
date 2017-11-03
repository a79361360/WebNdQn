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
        ActivityDAL adal = new ActivityDAL();
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
            filter += " order by addtime";
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
        public IList<T_ActivityDrawLog> GetActivityDrawList_Search(int cooperid, string phone, int state)
        {
            string filter = "";
            if (cooperid != -1)
                filter += " cooperid=@cooperid";
            if (!string.IsNullOrEmpty(phone))
            {
                if (!string.IsNullOrEmpty(filter))
                    filter += " and phone=@phone";
                else
                    filter += " phone=@phone";
            }
            if (state != -1)
            {
                if (!string.IsNullOrEmpty(filter))
                    filter += " and state=@state";
                else
                    filter += " state=@state";
            }
            if (!string.IsNullOrEmpty(filter))
                filter = "where" + filter;
            IList<T_ActivityDrawLog> list = DataTableToList.ModelConvertHelper<T_ActivityDrawLog>.ConvertToModel(dal.ActivityDrawList(filter, cooperid, phone, state));
            return list;
        }
        public IList<T_ShareLog> GetActivityShareList_Search(int cooperid, int atype, int sharetype) {
            string filter = "";
            if (cooperid != -1)
                filter += " cooperid=@cooperid";
            if (atype != -1)
            {
                if (!string.IsNullOrEmpty(filter))
                    filter += " and atype=@atype";
                else
                    filter += " atype=@atype";
            }
            if (sharetype != -1)
            {
                if (!string.IsNullOrEmpty(filter))
                    filter += " and sharetype=@sharetype";
                else
                    filter += " sharetype=@sharetype";
            }
            if (!string.IsNullOrEmpty(filter))
                filter = "where" + filter;
            IList<T_ShareLog> list = DataTableToList.ModelConvertHelper<T_ShareLog>.ConvertToModel(dal.ActivityShareList(filter, cooperid, atype, sharetype));
            return list;
        }
        public IList<T_MsgCode> GetMsgCodeList(int type,string phone) {
            string filter = "";
            if (type != -1)
                filter += " type=@type";
            if (!string.IsNullOrEmpty(phone))
            {
                if (!string.IsNullOrEmpty(filter))
                    filter += " and phone=@phone";
                else
                    filter += " phone=@phone";
            }
            if (!string.IsNullOrEmpty(filter))
                filter = "where" + filter;
            filter += " order by addtime desc";
            IList<T_MsgCode> list = DataTableToList.ModelConvertHelper<T_MsgCode>.ConvertToModel(dal.MsgCodeList(filter, type, phone));
            return list;
        }
    }
}
