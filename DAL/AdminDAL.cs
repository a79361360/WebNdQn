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
    public class AdminDAL
    {
        SqlDal dal = new SqlDal();
        public DataTable PageResult(ref int Total, SqlPageParam Param)
        {
            DataTable dt = dal.PageResult(Param.TableName, Param.PrimaryKey, Param.Fields, Param.PageSize, Param.PageIndex, Param.Filter, Param.Group, Param.Order, ref Total);
            return dt;
        }
        /// <summary>
        /// 执行语句
        /// </summary>
        /// <param name="filter">BLL拼接的SQL语句</param>
        /// <param name="ctype">参数ctype</param>
        /// <param name="issue">参数issue</param>
        /// <returns></returns>
        public DataTable FlowList(string filter, int ctype, int issue, string phone)
        {
            string sql = "SELECT [id],[ctype],[issue],[phone],[limitflow],[state],Convert(nvarchar(19),addtime,120) addtime FROM [dbo].[T_TakeFlowLog] " + filter;
            SqlParameter[] parameter = new[]
                        {
                new SqlParameter("@ctype",SqlDbType.Int),
                new SqlParameter("@issue",SqlDbType.Int),
                new SqlParameter("@phone",SqlDbType.NVarChar,20)
            };
            parameter[0].Value = ctype;
            parameter[1].Value = issue;
            parameter[2].Value = phone;
            return dal.ExtSql(sql, parameter);
        }
        public DataTable FindCooperList(string filter,string name,string value,int state) {
            string sql = "SELECT id,ctype,issue,title,eachflow,uplimit,state FROM dbo.T_CooperConfig " + filter;
            SqlParameter[] parameter = new[]
                                    {
                new SqlParameter("@name",SqlDbType.NVarChar,20),
                new SqlParameter("@value",SqlDbType.NVarChar,50),
                new SqlParameter("@state",SqlDbType.Int)
            };
            parameter[0].Value = name;
            parameter[1].Value = value;
            parameter[2].Value = state;
            return dal.ExtSql(sql, parameter);
        }
        public DataTable ActivityDrawList(string filter, int cooperid, string phone, int state)
        {
            string sql = "SELECT a.id,a.cooperid,a.type,a.openid,a.phone,a.configlistid,b.prizename,b.number,a.state,Convert(nvarchar(19),a.addtime,120) addtime FROM T_ActivityDrawLog a INNER JOIN T_ActivityConfigList b ON a.configlistid=b.id " + filter;
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
        public DataTable ActivityShareList(string filter, int cooperid, int atype, int sharetype)
        {
            string sql = "SELECT id,cooperid,atype,openid,sharetype,Convert(nvarchar(19),addtime,120) addtime FROM T_ShareLog " + filter;
            SqlParameter[] parameter = new[]
                        {
                new SqlParameter("@cooperid",SqlDbType.Int),
                new SqlParameter("@atype",SqlDbType.Int),
                new SqlParameter("@sharetype",SqlDbType.Int)
            };
            parameter[0].Value = cooperid;
            parameter[1].Value = atype;
            parameter[2].Value = sharetype;
            return dal.ExtSql(sql, parameter);
        }
        /// <summary>
        /// 取得短信验证码列表
        /// </summary>
        /// <param name="filter">条件</param>
        /// <param name="type">1登入/2充值</param>
        /// <param name="phone">号码</param>
        /// <returns></returns>
        public DataTable MsgCodeList(string filter, int type, string phone)
        {
            string sql = "SELECT [id],[type],[phone],[xh],[code],[text],[state],[addtime] FROM T_MsgCode " + filter;
            SqlParameter[] parameter = new[]
                        {
                new SqlParameter("@type",SqlDbType.Int),
                new SqlParameter("@phone",SqlDbType.NVarChar,20)
            };
            parameter[0].Value = type;
            parameter[1].Value = phone;
            return dal.ExtSql(sql, parameter);
        }
        /// <summary>
        /// 删除管理员账号
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int RemoveTadmin(int id) {
            string sql = "DELETE T_AdminManager WHERE id=@id";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@id",SqlDbType.Int)
            };
            parameter[0].Value = id;
            return dal.IntExtSql(sql, parameter);
        }
        /// <summary>
        /// 新增和更新
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pwd"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public int UpdateTadmin(string username,string pwd, int id) {
            string sql = "INSERT T_AdminManager(username,userpwd)VALUES(@username,@userpwd)";
            if (id != 0)
                sql = "UPDATE T_AdminManager SET username=@username,userpwd=@userpwd WHERE id=@id";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@username",SqlDbType.NVarChar,50),
                new SqlParameter("@userpwd",SqlDbType.NVarChar,50),
                new SqlParameter("@id",SqlDbType.Int)
            };
            parameter[0].Value = username;
            parameter[1].Value = pwd;
            parameter[2].Value = id;
            return dal.IntExtSql(sql, parameter);
        }
        /// <summary>
        /// 取得管理员信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataTable GetTadminByid(int id) {
            string sql = "SELECT id,username,userpwd,system,CONVERT(VARCHAR(19),addtime,120) addtime FROM T_AdminManager WHERE id=@id";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@id",SqlDbType.Int)
            };
            parameter[0].Value = id;
            return dal.ExtSql(sql, parameter);
        }
        /// <summary>
        /// 根据用户账户取得用户ID
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public int GetTadminByUserName(string username)
        {
            string sql = "SELECT top 1 id FROM T_AdminManager WHERE username=@username";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@username",SqlDbType.NVarChar,50)
            };
            parameter[0].Value = username;
            var result = dal.ExtScalarSql(sql, parameter);
            if (result == null) return -1;
            else return Convert.ToInt32(result);
        }
    }
}
