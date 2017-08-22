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
        public int AddConfig(int cooperid,int type,string title,int share,string explain,string bgurl) {
            string sql = "INSERT INTO [T_ActivityConfig]([cooperid],[type],[title],[share],[explain],[bgurl])";
            sql += "VALUES(@cooperid,@type,@title,@share,@explain,@bgurl) select SCOPE_IDENTITY() as id";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@cooperid",SqlDbType.Int),
                new SqlParameter("@type",SqlDbType.Int),
                new SqlParameter("@title",SqlDbType.NVarChar,20),
                new SqlParameter("@share",SqlDbType.Int),
                new SqlParameter("@explain",SqlDbType.NVarChar,400),
                new SqlParameter("@bgurl",SqlDbType.NVarChar,250),
            };
            parameter[0].Value = cooperid;
            parameter[1].Value = type;
            parameter[2].Value = title;
            parameter[3].Value = share;
            parameter[4].Value = explain;
            parameter[5].Value = bgurl;
            int result = 0;
            try {
                result = Convert.ToInt32(dal.ExtScalarSql(sql, parameter));
            }
            catch (Exception er) {
                return -1;
            }
            return result;
        }
        public int UpdateConfig(int configid, int cooperid, int type, string title, int share, string explain, string bgurl) {
            string sql = "UPDATE [T_ActivityConfig] SET [cooperid] = @cooperid,[type] = @type,[title] = @title,[share] = @share,[explain] = @explain,[bgurl] = @bgurl where id=@configid";
            SqlParameter[] parameter = new[]
              {
                new SqlParameter("@cooperid",SqlDbType.Int),
                new SqlParameter("@type",SqlDbType.Int),
                new SqlParameter("@title",SqlDbType.NVarChar,20),
                new SqlParameter("@share",SqlDbType.Int),
                new SqlParameter("@explain",SqlDbType.NVarChar,400),
                new SqlParameter("@bgurl",SqlDbType.NVarChar,250),
                new SqlParameter("@configid",SqlDbType.Int),
            };
            parameter[0].Value = cooperid;
            parameter[1].Value = type;
            parameter[2].Value = title;
            parameter[3].Value = share;
            parameter[4].Value = explain;
            parameter[5].Value = bgurl;
            parameter[6].Value = configid;
            int result = Convert.ToInt32(dal.IntExtSql(sql, parameter));
            return result;
        }
        /// <summary>
        /// 不管是插入还是更新，都可以用。主要是用configid是否为0来进行判断
        /// </summary>
        /// <param name="configid">主表的ID值</param>
        /// <param name="prizename"></param>
        /// <param name="count"></param>
        /// <param name="number"></param>
        /// <param name="winprob"></param>
        /// <returns></returns>
        public int SetConfigList(int id,int configid,string prizename,int count,int number,string winprob) {
            string sql = "INSERT INTO [T_ActivityConfigList]([configid],[prizename],[count],[number],[winprob])";
            sql += "VALUES(@configid,@prizename,@count,@number,@winprob)";
            if (id != 0)
                sql = "UPDATE [T_ActivityConfigList] SET [configid] = @configid,[prizename] = @prizename,[count] = @count,[number] = @number,[winprob] = @winprob where id=@id";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@configid",SqlDbType.Int),
                new SqlParameter("@prizename",SqlDbType.NVarChar,50),
                new SqlParameter("@count",SqlDbType.Int),
                new SqlParameter("@number",SqlDbType.Int),
                new SqlParameter("@winprob",SqlDbType.NVarChar,10),
                new SqlParameter("@id",SqlDbType.Int)
            };
            parameter[0].Value = configid;
            parameter[1].Value = prizename;
            parameter[2].Value = count;
            parameter[3].Value = number;
            parameter[4].Value = winprob;
            parameter[5].Value = id;
            int result = Convert.ToInt32(dal.IntExtSql(sql, parameter));
            return result;
        }
        public int ActivityRemoveById(int id) {
            string sql = "DELETE FROM [T_ActivityConfig] WHERE id=@id";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@id",SqlDbType.Int)
            };
            parameter[0].Value = id;
            int result = dal.IntExtSql(sql, parameter);
            if (result > 0) ActivityListRemoveById(id);
            return result;
        }
        private int ActivityListRemoveById(int configid)
        {
            string sql = "DELETE FROM [T_ActivityConfigList] WHERE configid=@configid";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@configid",SqlDbType.Int)
            };
            parameter[0].Value = configid;
            int result = dal.IntExtSql(sql, parameter);
            return result;
        }
        /// <summary>
        /// 取得大转盘配置的机率
        /// </summary>
        /// <param name="configid"></param>
        /// <returns></returns>
        public float GetWinProb(int configid) {
            string sql = "select SUM(CAST(winprob AS FLOAT)) FROM [T_ActivityConfigList] WHERE configid=@configid";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@configid",SqlDbType.Int)
            };
            parameter[0].Value = configid;
            float result = Convert.ToSingle(dal.ExtScalarSql(sql, parameter));
            return result;
        }
        /// <summary>
        /// 取得中奖信息中已经存在的手机号码
        /// </summary>
        /// <param name="cooperid"></param>
        /// <param name="type"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        public string GetActivityPhone(int cooperid, int type, string openid) {
            string sql = "SELECT top 1 phone FROM T_ActivityDrawLog WHERE cooperid=@cooperid AND type=@type AND openid=@openid and phone<>''";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@cooperid",SqlDbType.Int),
                new SqlParameter("@type",SqlDbType.Int),
                new SqlParameter("@openid",SqlDbType.NVarChar,50)
            };
            parameter[0].Value = cooperid;
            parameter[1].Value = type;
            parameter[2].Value = openid;
            var result = dal.ExtScalarSql(sql, parameter);
            if (result == null|| result.ToString() == "") return "";
            return result.ToString();
        }
    }
}
