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
        public DataTable PageResult(ref int Total, SqlPageParam Param)
        {
            DataTable dt = dal.PageResult(Param.TableName, Param.PrimaryKey, Param.Fields, Param.PageSize, Param.PageIndex, Param.Filter, Param.Group, Param.Order, ref Total);
            return dt;
        }
        /// <summary>
        /// 取得当前活动已经中奖的人数
        /// </summary>
        /// <param name="cooperid"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public int GetDrawCount(int cooperid,int type) {
            string sql = "SELECT COUNT(*) FROM T_ActivityDrawLog WHERE cooperid=@cooperid AND type=@type";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@cooperid",SqlDbType.Int),
                new SqlParameter("@type",SqlDbType.Int)
            };
            parameter[0].Value = cooperid;
            parameter[1].Value = type;
            int result = Convert.ToInt32(dal.ExtScalarSql(sql, parameter));
            return result;
        }
        public DataTable GetActivityConfigList(int configid)
        {
            string sql = "SELECT b.id,b.prizename,b.count,b.number,b.winprob,";
            sql += "(SELECT COUNT(*) FROM T_ActivityDrawLog WHERE configlistid=b.id) drowcount";
            sql += " FROM T_ActivityConfigList b WHERE b.configid=@configid ORDER BY b.id";

            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@configid",SqlDbType.Int)
            };
            parameter[0].Value = configid;
            DataTable dt = dal.ExtSql(sql, parameter);
            return dt;
        }
        /// <summary>
        /// 取得大转盘
        /// </summary>
        /// <param name="cooperid"></param>
        /// <param name="type">1表示大转盘</param>
        /// <returns></returns>
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
        /// 判断活动配置表是否存在
        /// </summary>
        /// <param name="cooperid"></param>
        /// <param name="type">1大转盘2在线答题</param>
        /// <returns></returns>
        public int IsExistActivity(int cooperid, int type)
        {
            string sql = "SELECT COUNT(*) FROM T_ActivityConfig WHERE cooperid=@cooperid and type=@type";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@cooperid",SqlDbType.Int),
                new SqlParameter("@type",SqlDbType.Int)
            };
            parameter[0].Value = cooperid;
            parameter[1].Value = type;
            int result = Convert.ToInt32(dal.ExtScalarSql(sql, parameter));
            return result;
        }
        /// <summary>
        /// 前三个都可以通用这个方法来进行调用
        /// </summary>
        /// <param name="cztype">1为添加(摇到奖品),2为更新手机号码,3当前用户可摇奖次数,4分享记录的添加</param>
        /// <param name="cooperid">T_CooperConfig的ID值</param>
        /// <param name="type">活动的类型1大转盘2答题</param>
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
        public DataTable GetActivityZb(int cooperid,int type) {
            string sql = "SELECT id,cooperid,type,title,share,explain,bgurl,wx_title,wx_descride,wx_imgurl,wx_linkurl,dt_fs,dt_tmts,sright,flowamount FROM T_ActivityConfig where cooperid=@cooperid and type=@type";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@cooperid",SqlDbType.Int),
                new SqlParameter("@type",SqlDbType.Int)
            };
            parameter[0].Value = cooperid;
            parameter[1].Value = type;
            DataTable dt = dal.ExtSql(sql, parameter);
            return dt;
        }
        public int AddConfig(int cooperid,int type,string title,int share,string explain,string bgurl, string wxtitle, string wxdescride, string wximgurl, string wxlinkurl, int tmfs, int tmts, int sright, int flowamount) {
            string sql = "INSERT INTO [T_ActivityConfig]([cooperid],[type],[title],[share],[explain],[bgurl],[wx_title],[wx_descride],[wx_imgurl],[wx_linkurl],[dt_fs],[dt_tmts],[sright],[flowamount])";
            sql += "VALUES(@cooperid,@type,@title,@share,@explain,@bgurl,@wxtitle,@wxdescride,@wximgurl,@wxlinkurl,@tmfs,@tmts,@sright,@flowamount) select SCOPE_IDENTITY() as id";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@cooperid",SqlDbType.Int),
                new SqlParameter("@type",SqlDbType.Int),
                new SqlParameter("@title",SqlDbType.NVarChar,20),
                new SqlParameter("@share",SqlDbType.Int),
                new SqlParameter("@explain",SqlDbType.NVarChar,400),
                new SqlParameter("@bgurl",SqlDbType.NVarChar,250),
                new SqlParameter("@wxtitle",SqlDbType.NVarChar,50),
                new SqlParameter("@wxdescride",SqlDbType.NVarChar,250),
                new SqlParameter("@wximgurl",SqlDbType.NVarChar,250),
                new SqlParameter("@wxlinkurl",SqlDbType.NVarChar,250),
                new SqlParameter("@tmfs",SqlDbType.Int),
                new SqlParameter("@tmts",SqlDbType.Int),
                new SqlParameter("@sright",SqlDbType.Int),
                new SqlParameter("@flowamount",SqlDbType.Int),
            };
            parameter[0].Value = cooperid;
            parameter[1].Value = type;
            parameter[2].Value = title;
            parameter[3].Value = share;
            parameter[4].Value = explain;
            parameter[5].Value = bgurl;
            parameter[6].Value = wxtitle;
            parameter[7].Value = wxdescride;
            parameter[8].Value = wximgurl;
            parameter[9].Value = wxlinkurl;
            parameter[10].Value = tmfs;
            parameter[11].Value = tmts;
            parameter[12].Value = sright;
            parameter[13].Value = flowamount;
            int result = 0;
            try {
                result = Convert.ToInt32(dal.ExtScalarSql(sql, parameter));
            }
            catch (Exception er) {
                return -1;
            }
            return result;
        }
        public int UpdateConfig(int configid, int cooperid, int type, string title, int share, string explain, string bgurl, string wxtitle, string wxdescride, string wximgurl, string wxlinkurl, int tmfs, int tmts, int sright,int flowamount) {
            string sql = "UPDATE [T_ActivityConfig] SET [cooperid] = @cooperid,[type] = @type,[title] = @title,[share] = @share,[explain] = @explain,[bgurl] = @bgurl ";
            sql += ",[wx_title] = @wxtitle,[wx_descride] = @wxdescride,[wx_imgurl] = @wximgurl,[wx_linkurl] = @wxlinkurl,[dt_fs] = @tmfs,[dt_tmts] = @tmts,[sright]=@sright,[flowamount]=@flowamount where id=@configid";
            SqlParameter[] parameter = new[]
              {
                new SqlParameter("@cooperid",SqlDbType.Int),
                new SqlParameter("@type",SqlDbType.Int),
                new SqlParameter("@title",SqlDbType.NVarChar,20),
                new SqlParameter("@share",SqlDbType.Int),
                new SqlParameter("@explain",SqlDbType.NVarChar,400),
                new SqlParameter("@bgurl",SqlDbType.NVarChar,250),
                new SqlParameter("@configid",SqlDbType.Int),
                new SqlParameter("@wxtitle",SqlDbType.NVarChar,50),
                new SqlParameter("@wxdescride",SqlDbType.NVarChar,250),
                new SqlParameter("@wximgurl",SqlDbType.NVarChar,250),
                new SqlParameter("@wxlinkurl",SqlDbType.NVarChar,250),
                new SqlParameter("@tmfs",SqlDbType.Int),
                new SqlParameter("@tmts",SqlDbType.Int),
                new SqlParameter("@sright",SqlDbType.Int),
                new SqlParameter("@flowamount",SqlDbType.Int),
            };
            parameter[0].Value = cooperid;
            parameter[1].Value = type;
            parameter[2].Value = title;
            parameter[3].Value = share;
            parameter[4].Value = explain;
            parameter[5].Value = bgurl;
            parameter[6].Value = configid;
            parameter[7].Value = wxtitle;
            parameter[8].Value = wxdescride;
            parameter[9].Value = wximgurl;
            parameter[10].Value = wxlinkurl;
            parameter[11].Value = tmfs;
            parameter[12].Value = tmts;
            parameter[13].Value = sright;
            parameter[14].Value = flowamount;
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
        /// <summary>
        /// 根据活动主表ID删除数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int ActivityRemoveById(int id) {
            string sql = "DELETE FROM [T_ActivityConfig] WHERE id=@id";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@id",SqlDbType.Int)
            };
            parameter[0].Value = id;
            int result = dal.IntExtSql(sql, parameter);
            return result;
        }
        //大转盘奖励列表删除
        public int ActivityListRemoveById(int configid)
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
        //在线答题流量配置删除
        public int ZxdtScoreRemoveById(int configid)
        {
            string sql = "DELETE FROM [T_ZxdtScore] WHERE configid=@configid";
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cooperid"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        public DataTable ActivityDrawList(int cooperid,int type, string openid)
        {
            string sql = "SELECT a.id,a.cooperid,a.type,a.openid,a.phone,a.configlistid,b.prizename,a.state,Convert(nvarchar(19),a.addtime,120) addtime FROM T_ActivityDrawLog a INNER JOIN T_ActivityConfigList b ON a.configlistid=b.id ";
                   sql+= "where a.cooperid=@cooperid and a.openid=@openid and a.type=@type";
            SqlParameter[] parameter = new[]
                        {
                new SqlParameter("@cooperid",SqlDbType.Int),
                new SqlParameter("@type",SqlDbType.Int),
                new SqlParameter("@openid",SqlDbType.NVarChar,50)
            };
            parameter[0].Value = cooperid;
            parameter[1].Value = type;
            parameter[2].Value = openid;
            return dal.ExtSql(sql, parameter);
        }
    }
}
