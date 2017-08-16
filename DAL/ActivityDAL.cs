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
    public class ActivityDAL
    {
        SqlDal dal = new SqlDal();
        public DataTable GetActivityConfigList(int configid)
        {
            string sql = "SELECT id,prizename,count,number,winprob FROM T_ActivityConfigList WHERE configid=@configid ORDER BY id";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@configid",SqlDbType.Int)
            };
            parameter[0].Value = configid;
            DataTable dt = dal.ExtSql(sql, parameter);
            return dt;
        }
        public int GetActivityConfigId(int cooperid,int type) {
            string sql = "SELECT top 1 id FROM T_ActivityConfig WHERE cooperid=@cooperid and type=@type";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@cooperid",SqlDbType.Int),
                new SqlParameter("@type",SqlDbType.Int)
            };
            parameter[0].Value = cooperid;
            parameter[1].Value = type;
            var result = dal.ExtScalarSql(sql, parameter);
            if (result == null) return 0;
            return Convert.ToInt32(result);
        }
        /// <summary>
        /// 前三个都可以通用这个方法来进行调用
        /// </summary>
        /// <param name="cztype">1为添加(摇到奖品),2为更新手机号码,3当前用户可摇奖次数</param>
        /// <param name="cooperid">T_CooperConfig的ID值</param>
        /// <param name="type">活动的类型1大转盘</param>
        /// <param name="openid">微信用户的openid</param>
        /// <param name="phone">手机号码</param>
        /// <param name="configid">活动奖品的配置ID</param>
        /// <returns></returns>
        public int HandleActivity(int cztype, int cooperid,int type,string openid,string phone,int configid) {
            string sql = "SP_HandleActivity";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@CzType",SqlDbType.Int),
                new SqlParameter("@Cooperid",SqlDbType.Int),
                new SqlParameter("@AType",SqlDbType.Int),
                new SqlParameter("@Openid",SqlDbType.NVarChar,50),
                new SqlParameter("@Phone",SqlDbType.NVarChar,16),
                new SqlParameter("@ConfigLid",SqlDbType.Int),
                new SqlParameter("@ReturnValue",SqlDbType.Int)
            };
            parameter[0].Value = cztype;
            parameter[1].Value = cooperid;
            parameter[2].Value = type;
            parameter[3].Value = openid;
            parameter[4].Value = phone;
            parameter[5].Value = configid;
            parameter[6].Direction = ParameterDirection.ReturnValue;
            string[] str = new string[] { "@ReturnValue" };
            Dictionary<string, object> list = new Dictionary<string, object>();
            dal.ExtProc(sql, parameter, str, out list);
            return Convert.ToInt32(list["@ReturnValue"]);
        }
        public DataTable GetActivityZb(int cooperid) {
            string sql = "SELECT id,cooperid,type,title,share,explain,bgurl FROM T_ActivityConfig where cooperid=@cooperid";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@cooperid",SqlDbType.Int)
            };
            parameter[0].Value = cooperid;
            DataTable dt = dal.ExtSql(sql, parameter);
            return dt;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cooperid"></param>
        /// <param name="virtualpath"></param>
        /// <returns></returns>
        public int UpdateDzpBgUrlByCooperid(int cooperid,string virtualpath) {
            string sql = "UPDATE T_ActivityConfig SET bgurl=@bgurl WHERE cooperid=@cooperid";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@bgurl",SqlDbType.NVarChar,250),
                new SqlParameter("@cooperid",SqlDbType.Int)
            };
            parameter[0].Value = virtualpath;
            parameter[1].Value = cooperid;
            int result = dal.IntExtSql(sql, parameter);
            return result;
        }
    }
}
