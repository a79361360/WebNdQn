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
            string sql = "SELECT [id],[cooperid],[topic],[answer],[keyanswer],[addtime] FROM [T_TopicBank] WHERE id=@id";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@id",SqlDbType.Int),
            };
            parameter[0].Value = id;
            return dal.ExtSql(sql, parameter);
        }
        /// <summary>
        /// 返回影响行数
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public int SetZxdtTopic(T_TopicBank dto) {
            string sql = "INSERT INTO [T_TopicBank]([cooperid],[topic],[answer],[keyanswer])VALUES(@cooperid, @topic, @answer, @keyanswer)";
            if (dto.id != 0) {
                sql = "UPDATE [T_TopicBank] SET [cooperid] = @cooperid,[topic] = @topic,[answer] = @answer,[keyanswer] = @keyanswer WHERE id=@id";
            }
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@id",SqlDbType.Int),
                new SqlParameter("@cooperid",SqlDbType.Int),
                new SqlParameter("@topic",SqlDbType.NVarChar,300),
                new SqlParameter("@answer",SqlDbType.NVarChar,500),
                new SqlParameter("@keyanswer",SqlDbType.Int),
            };
            parameter[0].Value = dto.id;
            parameter[1].Value = dto.cooperid;
            parameter[2].Value = dto.topic;
            parameter[3].Value = dto.answer;
            parameter[4].Value = dto.keyanswer;
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
    }
}
