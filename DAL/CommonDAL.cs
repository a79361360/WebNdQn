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
    public class CommonDAL
    {
        SqlDal dal = new SqlDal();
        public DataTable GetCooperConfig(int coopertype)
        {
            string sql = "SELECT [id],[coopertype],[title],[descride],[imgurl],[linkurl],[addtime] FROM [ndll_db].[dbo].[T_CooperConfig] WHERE coopertype=@coopertype";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@coopertype",SqlDbType.Int)
            };
            parameter[0].Value = coopertype;
            DataTable dt = dal.ExtSql(sql, parameter);
            return dt;
        }
    }
}
