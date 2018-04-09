using Common;
using Model.WxModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class ZxdtDAL
    {
        SqlDal dal = new SqlDal();
        public DataTable PageResult(ref int Total, SqlPageParam Param)
        {
            DataTable dt = dal.PageResult(Param.TableName, Param.PrimaryKey, Param.Fields, Param.PageSize, Param.PageIndex, Param.Filter, Param.Group, Param.Order, ref Total);
            return dt;
        }
        /// <summary>
        /// 根据ID取得T_TopicBank记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataTable GetTopicById(int id) {
            string sql = "SELECT [id],[cooperid],[topic],[answer],[keyanswer],[checkbox],[tips],[addtime] FROM [T_TopicBank] WHERE id=@id";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@id",SqlDbType.Int),
            };
            parameter[0].Value = id;
            return dal.ExtSql(sql, parameter);
        }
        /// <summary>
        /// 随机取得设置条数的题目
        /// </summary>
        /// <param name="cooperid"></param>
        /// <param name="tmts">题目条数</param>
        /// <param name="order">默认常规排序,0为随机</param>
        /// <returns></returns>
        public DataTable GetDttsTopic(int cooperid, int tmts, int order = 1){
            string orderby = " ORDER BY id";
            if (order == 0) orderby = " ORDER BY NEWID()";
            string sql = "SELECT TOP " + tmts + " [id],[cooperid],[topic],[answer],[keyanswer],[checkbox],[tips],[addtime] FROM T_TopicBank WHERE cooperid = @cooperid"+ orderby;
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@cooperid",SqlDbType.Int),
            };
            parameter[0].Value = cooperid;
            return dal.ExtSql(sql, parameter);
        }
        /// <summary>
        /// 返回影响行数
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public int SetZxdtTopic(T_TopicBank dto) {
            string sql = "INSERT INTO [T_TopicBank]([cooperid],[topic],[answer],[checkbox],[keyanswer],[tips])VALUES(@cooperid, @topic, @answer,@checkbox, @keyanswer,@tips)";
            if (dto.id != 0) {
                sql = "UPDATE [T_TopicBank] SET [cooperid] = @cooperid,[topic] = @topic,[answer] = @answer,[checkbox] = @checkbox,[keyanswer] = @keyanswer,[tips] = @tips WHERE id=@id";
            }
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@id",SqlDbType.Int),
                new SqlParameter("@cooperid",SqlDbType.Int),
                new SqlParameter("@topic",SqlDbType.NVarChar,300),
                new SqlParameter("@answer",SqlDbType.NVarChar,500),
                new SqlParameter("@checkbox",SqlDbType.Int),
                new SqlParameter("@keyanswer",SqlDbType.NVarChar,20),
                new SqlParameter("@tips",SqlDbType.NVarChar,500),
            };
            parameter[0].Value = dto.id;
            parameter[1].Value = dto.cooperid;
            parameter[2].Value = dto.topic;
            parameter[3].Value = dto.answer;
            parameter[4].Value = dto.checkbox;
            parameter[5].Value = dto.keyanswer;
            parameter[6].Value = dto.tips;
            int result = dal.IntExtSql(sql, parameter);
            return result;
        }
        /// <summary>
        /// 删除题库信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int TopicRemoveById(int id)
        {
            string sql = "DELETE FROM [T_TopicBank] WHERE id=@id";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@id",SqlDbType.Int)
            };
            parameter[0].Value = id;
            int result = dal.IntExtSql(sql, parameter);
            return result;
        }
        /// <summary>
        /// 设置在线答题流量配置
        /// </summary>
        /// <param name="id"></param>
        /// <param name="configid"></param>
        /// <param name="number"></param>
        /// <param name="lower"></param>
        /// <param name="upper"></param>
        /// <returns></returns>
        public int SetZxdtScore(int id, int configid, int number, int lower, int upper)
        {
            string sql = "INSERT INTO [T_ZxdtScore]([configid],[number],[lower],[upper])";
            sql += "VALUES(@configid, @number, @lower, @upper)";
            if (id != 0)
                sql = "UPDATE [T_ZxdtScore] SET [configid] = @configid,[number] = @number,[lower] = @lower,[upper] = @upper,[addtime] = GETDATE() WHERE id = @id";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@configid",SqlDbType.Int),
                new SqlParameter("@number",SqlDbType.NVarChar,50),
                new SqlParameter("@lower",SqlDbType.Int),
                new SqlParameter("@upper",SqlDbType.Int),
                new SqlParameter("@id",SqlDbType.Int)
            };
            parameter[0].Value = configid;
            parameter[1].Value = number;
            parameter[2].Value = lower;
            parameter[3].Value = upper;
            parameter[4].Value = id;
            int result = Convert.ToInt32(dal.IntExtSql(sql, parameter));
            return result;
        }
        /// <summary>
        /// 通过configid取得列表
        /// </summary>
        /// <param name="configid"></param>
        /// <returns></returns>
        public DataTable GetZxdtScore(int configid) {
            string sql = "SELECT [id],[configid],[number],[lower],[upper],[addtime] FROM [T_ZxdtScore] WHERE configid=@configid";
            SqlParameter[] parameter = new[]
{
                new SqlParameter("@configid",SqlDbType.Int)
            };
            parameter[0].Value = configid;
            return dal.ExtSql(sql, parameter);
        }
        /// <summary>
        /// 取得在线答题的列表
        /// </summary>
        /// <param name="cooperid"></param>
        /// <param name="type"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        public DataTable ZxdtDrawList(int cooperid, int type, string openid)
        {
            string sql = "SELECT configlistid score,Convert(nvarchar(19),addtime,120) addtime FROM T_ActivityDrawLog";
            sql += " WHERE cooperid = @cooperid and openid = @openid and type = @type";
            SqlParameter[] parameter = new[]
                        {
                new SqlParameter("@cooperid",SqlDbType.Int),
                new SqlParameter("@openid",SqlDbType.NVarChar,50),
                new SqlParameter("@type",SqlDbType.Int),
            };
            parameter[0].Value = cooperid;
            parameter[1].Value = openid;
            parameter[2].Value = type;
            return dal.ExtSql(sql, parameter);
        }
        public DataTable ZxdtDrawList_Search(string filter, int cooperid, string phone, int state)
        {
            string sql = "SELECT a.id,a.type,a.openid,a.phone,a.configlistid score,c.number,a.state,Convert(nvarchar(19),a.addtime,120) addtime";
            sql += " FROM T_ActivityDrawLog a";
            sql += " INNER JOIN T_ActivityConfig b ON a.cooperid = b.cooperid";
            sql += " INNER JOIN T_ZxdtScore c ON b.id = c.configid" + filter;
            SqlParameter[] parameter = new[]
                        {
                new SqlParameter("@cooperid",SqlDbType.Int),
                new SqlParameter("@phone",SqlDbType.NVarChar,20),
                new SqlParameter("@state",SqlDbType.Int)
            };
            parameter[0].Value = cooperid;
            parameter[1].Value = phone;
            parameter[2].Value = state;
            return dal.ExtSql(sql, parameter);
        }
        /// <summary>
        /// 取得在线用户已经参与人数的流量值
        /// </summary>
        /// <returns></returns>
        public int ZxdtDrawNumber(int cooperid) {
            string sql = "SELECT ISNULL(SUM(c.number),0) FROM T_ActivityDrawLog a";
            sql += " INNER JOIN T_ActivityConfig b ON a.cooperid = b.cooperid";
            sql += " INNER JOIN T_ZxdtScore c ON b.id = c.configid";
            sql += " WHERE a.type = 2 AND(a.configlistid > c.lower AND a.configlistid <= c.upper) AND a.cooperid = @cooperid ";
            SqlParameter[] parameter = new[]
                        {
                new SqlParameter("@cooperid",SqlDbType.Int)
            };
            parameter[0].Value = cooperid;
            int result = Convert.ToInt32(dal.ExtScalarSql(sql, parameter));
            return result;
        }
    }
}
