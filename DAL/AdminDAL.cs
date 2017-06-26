using Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class AdminDAL
    {
        SqlDal dal = new SqlDal();
        /// <summary>
        /// 执行语句
        /// </summary>
        /// <param name="filter">BLL拼接的SQL语句</param>
        /// <param name="ctype">参数ctype</param>
        /// <param name="issue">参数issue</param>
        /// <returns></returns>
        public DataTable FlowList(string filter, int ctype, int issue, string phone)
        {
            string sql = "SELECT [id],[ctype],[issue],[phone],[state],Convert(nvarchar(19),addtime,120) addtime FROM [dbo].[T_TakeFlowLog] " + filter;
            SqlParameter[] parameter = new[]
                        {
                new SqlParameter("@ctype",SqlDbType.Int),
                new SqlParameter("@issue",SqlDbType.Int),
                new SqlParameter("@phone",SqlDbType.NVarChar,20)
            };
            parameter[0].Value = ctype;
            parameter[1].Value = issue;
            parameter[2].Value = phone;
            return dal.ExtSql(sql, parameter);
        }
    }
}
