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
        /// <summary>
        /// 是否存在此列表中
        /// </summary>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 初始化,将状态设置为0
        /// </summary>
        /// <returns></returns>
        private int InitT_LogCacheState()
        {
            string sql = "UPDATE [T_LogCache] SET state = 0";
            return dal.IntExtSql(sql);
        }
        /// <summary>
        /// 更新监控列表的name,phone,csrf值
        /// </summary>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        /// <param name="username"></param>
        /// <param name="phone"></param>
        /// <param name="csrf"></param>
        /// <returns></returns>
        public int UpdateLogCacheInfo(int ctype, int issue, string username, string phone, string csrf)
        {
            InitT_LogCacheState(); //先进行state初始化，设置为0
            string sql = "UPDATE [T_LogCache] SET [username] = @username,[phone] = @phone,[csrf] = @csrf,[state] = 1 WHERE [ctype] = @ctype,[issue] = @issue";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@ctype",SqlDbType.Int),
                new SqlParameter("@issue",SqlDbType.Int),
                new SqlParameter("@username",SqlDbType.NVarChar,50),
                new SqlParameter("@phone",SqlDbType.Int),
                new SqlParameter("@csrf",SqlDbType.NVarChar,1000),
            };
            parameter[0].Value = ctype;
            parameter[1].Value = issue;
            parameter[2].Value = username;
            parameter[3].Value = phone;
            parameter[4].Value = csrf;
            return dal.IntExtSql(sql, parameter);
        }
    }
}
