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
        /// 仅更新当前监控的状态
        /// </summary>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        /// <returns></returns>
        public int UpdateLogCacheState(int ctype, int issue)
        {
            InitT_LogCacheState(); //先进行state初始化，设置为0
            string sql = "UPDATE [T_LogCache] SET [state] = 1 WHERE [ctype] = @ctype and [issue] = @issue";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@ctype",SqlDbType.Int),
                new SqlParameter("@issue",SqlDbType.Int),
            };
            parameter[0].Value = ctype;
            parameter[1].Value = issue;
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
        /// 取得当前正在监控的项
        /// </summary>
        /// <returns></returns>
        public DataTable FindLogCacheState(int ctype,int issue)
        {
            string sql = "SELECT ctype,issue,corpid,phone,csrf,dlyzm,czyzm,dlcookie,czcookie FROM T_LogCache WHERE ctype=@ctype AND issue=@issue";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@ctype",SqlDbType.Int),
                new SqlParameter("@issue",SqlDbType.Int)
            };
            parameter[0].Value = ctype;
            parameter[1].Value = issue;
            return dal.ExtSql(sql, parameter);
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
        /// <summary>
        /// 是否存在待直充的记录
        /// </summary>
        /// <param name="type">1判断是否存在充值记录,1存在,-2当前客户不存在-3没有充值记录</param>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        /// <returns></returns>
        public int IsExistsCzList(int type, int ctype, int issue)
        {
            string sql = "SP_ExecuteFlowLog";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@Type",SqlDbType.Int),
                new SqlParameter("@Ctype",SqlDbType.Int),
                new SqlParameter("@Issue",SqlDbType.Int),
                new SqlParameter("@ReturnValue",SqlDbType.Int)
            };
            parameter[0].Value = type;
            parameter[1].Value = ctype;
            parameter[2].Value = issue;
            parameter[3].Direction = ParameterDirection.ReturnValue;
            string[] str = new string[] { "@ReturnValue" };
            Dictionary<string, object> list = new Dictionary<string, object>();
            dal.ExtProc(sql, parameter, str, out list);
            return Convert.ToInt32(list["@ReturnValue"]);
        }
        /// <summary>
        /// 刚充值完成的单子修改他的状态为已完成
        /// </summary>
        /// <param name="type">3</param>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        /// <returns></returns>
        public int UpdateFlowState(int type,int ctype,int issue) {
            string sql = "SP_ExecuteFlowLog";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@Type",SqlDbType.Int),
                new SqlParameter("@Ctype",SqlDbType.Int),
                new SqlParameter("@Issue",SqlDbType.Int)
            };
            parameter[0].Value = type;
            parameter[1].Value = ctype;
            parameter[2].Value = issue;
            dal.ExtProc(sql, parameter);
            return 1;
        }
        /// <summary>
        /// 生成待充值的记录
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        /// <returns></returns>
        public DataTable CreatCzDTToExecl(int type, int ctype, int issue)
        {
            string sql = "SP_ExecuteFlowLog";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@Type",SqlDbType.Int),
                new SqlParameter("@Ctype",SqlDbType.Int),
                new SqlParameter("@Issue",SqlDbType.Int),
                new SqlParameter("@ReturnValue",SqlDbType.Int)
            };
            parameter[0].Value = type;
            parameter[1].Value = ctype;
            parameter[2].Value = issue;
            parameter[3].Direction = ParameterDirection.ReturnValue;
            string[] str = new string[] { "@ReturnValue" };
            Dictionary<string, object> list = new Dictionary<string, object>();
            DataTable dt = new DataTable();
            dt = dal.ExtProc(sql, parameter, str, out list);
            return dt;
        }
        /// <summary>
        /// 取得监控列表
        /// </summary>
        /// <returns></returns>
        public DataTable FindLogCacheList() {
            string sql = "SELECT id,ctype,issue,corpid,phone,csrf,dlyzm,czyzm,dlcookie,czcookie FROM T_LogCache";
            return dal.ExtSql(sql);
        }
        /// <summary>
        /// 移除超端记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int RemoveLoginCache(int id)
        {
            string sql = "DELETE FROM [T_LogCache] WHERE id=@id";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@id",SqlDbType.Int)
            };
            parameter[0].Value = id;
            return dal.IntExtSql(sql, parameter);
        }
        /// <summary>
        /// 添加到超端
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public int InsertT_LogCache(T_LogCache dto) {
            string sql = "INSERT INTO [T_LogCache]([ctype],[issue],[corpid],[phone])VALUES(@ctype,@issue,@corpid,@phone)";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@ctype",SqlDbType.Int),
                new SqlParameter("@issue",SqlDbType.Int),
                new SqlParameter("@corpid",SqlDbType.NVarChar,20),
                new SqlParameter("@phone",SqlDbType.NVarChar,50),
            };
            parameter[0].Value = dto.ctype;
            parameter[1].Value = dto.issue;
            parameter[2].Value = dto.corpid;
            parameter[3].Value = dto.phone;
            return dal.IntExtSql(sql, parameter);
        }
    }
}
