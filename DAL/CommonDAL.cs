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
        /// <summary>
        /// 添加领取流量的记录
        /// </summary>
        /// <param name="ctype">公司类型</param>
        /// <param name="phone">手机号码</param>
        /// <returns>返回影响行数</returns>
        public int TakeFlowLog(int ctype,string phone) {
            string sql = "INSERT INTO [T_TakeFlowLog]([ctype],[phone])VALUES(@ctype,@phone)";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@ctype",SqlDbType.Int),
                new SqlParameter("@ctype",SqlDbType.Int)
            };
            parameter[0].Value = ctype;
            parameter[1].Value = phone;
            return dal.IntExtSql(sql, parameter);
        }
    }
}
