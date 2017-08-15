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
        public DataTable PageResult(ref int Total, SqlPageParam Param)
        {
            DataTable dt = dal.PageResult(Param.TableName, Param.PrimaryKey, Param.Fields, Param.PageSize, Param.PageIndex, Param.Filter, Param.Group, Param.Order, ref Total);
            return dt;
        }
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
        public DataTable FindCooperList(string filter,string name,string value,int state) {
            string sql = "SELECT id,ctype,issue,title,eachflow,uplimit,state FROM dbo.T_CooperConfig " + filter;
            SqlParameter[] parameter = new[]
                                    {
                new SqlParameter("@name",SqlDbType.NVarChar,20),
                new SqlParameter("@value",SqlDbType.NVarChar,50),
                new SqlParameter("@state",SqlDbType.Int)
            };
            parameter[0].Value = name;
            parameter[1].Value = value;
            parameter[2].Value = state;
            return dal.ExtSql(sql, parameter);
        }
    }
}
