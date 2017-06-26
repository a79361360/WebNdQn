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
        /// <summary>
        /// 取得公司的活动配置信息
        /// </summary>
        /// <param name="ctype">公司类型</param>
        /// <param name="issue">活动期号</param>
        /// <returns></returns>
        public DataTable GetCooperConfig(int ctype,int issue)
        {
            string sql = "SELECT [id],[ctype],[issue],[title],[descride],[imgurl],[linkurl],[corpid],[username],[userpwd],[signphone],[wx_appid],[wx_secret],[qrcode_url],[state],[addtime] FROM [dbo].[T_CooperConfig] WHERE ctype=@ctype and state=1";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@ctype",SqlDbType.Int),
                new SqlParameter("@issue",SqlDbType.Int)
            };
            parameter[0].Value = ctype;
            parameter[1].Value = issue;
            DataTable dt = dal.ExtSql(sql, parameter);
            return dt;
        }
        /// <summary>
        /// 取得公司的活动配置信息
        /// </summary>
        /// <param name="phone">校验的手机号码</param>
        /// <returns></returns>
        public DataTable GetCooperConfig(string phone) {
            string sql = "SELECT top 1 [id],[ctype],[issue],[title],[descride],[imgurl],[linkurl],[corpid],[username],[userpwd],[signphone],[state],[addtime] FROM [dbo].[T_CooperConfig] WHERE signphone=@phone and state=1 order by id desc";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@phone",SqlDbType.NVarChar,20)
            };
            parameter[0].Value = phone;
            DataTable dt = dal.ExtSql(sql, parameter);
            return dt;
        }
        /// <summary>
        /// 取得公司的活动配置信息，下拉列表
        /// </summary>
        /// <param name="state">状态</param>
        /// <returns></returns>
        public DataTable GetCooperConfigDrop(int state) {
            string sql = "SELECT [ctype],[issue],[title],state FROM [dbo].[T_CooperConfig] WHERE state=@state order by id desc";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@state",SqlDbType.Int)
            };
            parameter[0].Value = state;
            DataTable dt = dal.ExtSql(sql, parameter);
            return dt;
        }
        /// <summary>
        /// 当前手机是否已经添加过这个公司的这一期活动
        /// </summary>
        /// <param name="phone">用户手机号码</param>
        /// <param name="ctype">公司类型</param>
        /// <param name="issue">活动期号</param>
        /// <returns></returns>
        public int DecidePhone(string phone, int ctype, int issue) {
            string sql = "SELECT COUNT(*) FROM [dbo].[T_TakeFlowLog] WHERE ctype=@ctype AND issue=@issue AND phone=@phone";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@ctype",SqlDbType.Int),
                new SqlParameter("@issue",SqlDbType.Int),
                new SqlParameter("@phone",SqlDbType.NVarChar,20)
            };
            parameter[0].Value = ctype;
            parameter[1].Value = issue;
            parameter[2].Value = phone;
            int result = Convert.ToInt32(dal.ExtScalarSql(sql, parameter));
            return result;
        }
        /// <summary>
        /// 添加领取流量的记录
        /// </summary>
        /// <param name="ctype">公司类型</param>
        /// <param name="phone">手机号码</param>
        /// <returns>返回影响行数</returns>
        public int TakeFlowLog(int ctype, int issue, string phone) {
            string sql = "INSERT INTO [T_TakeFlowLog]([ctype],[issue],[phone])VALUES(@ctype,@issue,@phone)";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@ctype",SqlDbType.Int),
                new SqlParameter("@issue",SqlDbType.Int),
                new SqlParameter("@phone", SqlDbType.NVarChar, 50)
            };
            parameter[0].Value = ctype;
            parameter[1].Value = issue;
            parameter[2].Value = phone;
            return dal.IntExtSql(sql, parameter);
        }
        /// <summary>
        /// 取得传送过来的验证码信息
        /// </summary>
        /// <param name="type">1登入验证码，2充值验证码</param>
        /// <param name="phone">手机号码</param>
        /// <param name="code">验证码</param>
        /// <returns></returns>
        public int TakeMsgCode(int type,string phone,int code,string content) {
            string sql = "INSERT INTO [T_MsgCode]([type],[phone],[code],[text])VALUES(@type,@phone,@code,@text)";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@type",SqlDbType.Int),
                new SqlParameter("@phone",SqlDbType.NVarChar,20),
                new SqlParameter("@code",SqlDbType.Int),
                new SqlParameter("@text",SqlDbType.NVarChar,250)
            };
            parameter[0].Value = type;
            parameter[1].Value = phone;
            parameter[2].Value = code;
            parameter[3].Value = content;
            return dal.IntExtSql(sql, parameter);
        }
    }
}
