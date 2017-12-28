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
        /// <param name="corpid"></param>
        /// <param name="phone"></param>
        /// <param name="csrf"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public int UpdateLogCacheInfo(int ctype, int issue, string corpid, string phone, string csrf,string cookie)
        {
            InitT_LogCacheState(); //先进行state初始化，设置为0
            string sql = "UPDATE [T_LogCache] SET [corpid] = @corpid,[phone] = @phone,[csrf] = @csrf,[dlcookie] = @dlcookie,[state] = 1 WHERE [ctype] = @ctype and [issue] = @issue";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@ctype",SqlDbType.Int),
                new SqlParameter("@issue",SqlDbType.Int),
                new SqlParameter("@corpid",SqlDbType.NVarChar,20),
                new SqlParameter("@phone",SqlDbType.NVarChar,20),
                new SqlParameter("@csrf",SqlDbType.NVarChar,1000),
                new SqlParameter("@dlcookie",SqlDbType.NVarChar,2000),
            };
            parameter[0].Value = ctype;
            parameter[1].Value = issue;
            parameter[2].Value = corpid;
            parameter[3].Value = phone;
            parameter[4].Value = csrf;
            parameter[5].Value = cookie;
            return dal.IntExtSql(sql, parameter);
        }
        /// <summary>
        /// 取得当前正在监控的项
        /// </summary>
        /// <returns></returns>
        public DataTable FindLogCacheState() {
            string sql = "SELECT ctype,issue,corpid,phone,csrf,dlyzm,czyzm,dlcookie,czcookie FROM T_LogCache WHERE state=1";
            return dal.ExtSql(sql);
        }
        /// <summary>
        /// 更新登入的cookie
        /// </summary>
        /// <param name="dlcookie"></param>
        /// <returns></returns>
        public int UpdateLogCacheDlCookie(string dlcookie) {
            string sql = "UPDATE T_LogCache SET dlcookie=@dlcookie WHERE state=1";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@dlcookie",SqlDbType.NVarChar,2000)
            };
            parameter[0].Value = dlcookie;
            return dal.IntExtSql(sql, parameter);
        }
    }
}
