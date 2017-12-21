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
    public class NsoupDAL
    {
        SqlDal dal = new SqlDal();
        public int IsExitsLogCache(int ctype,int issue) {
            string sql = "SELECT COUNT(id) FROM T_LogCache WHERE ctype=@ctype AND issue=@issue";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@ctype",SqlDbType.Int),
                new SqlParameter("@issue",SqlDbType.Int)
            };
            parameter[0].Value = ctype;
            parameter[1].Value = issue;
            int result = Convert.ToInt32(dal.ExtScalarSql(sql, parameter));
            return result;
        }
    }
}
