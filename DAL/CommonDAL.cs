using Common;
using Model.EnumModel;
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
            string sql = "SELECT [id],[ctype],[issue],[areatype],areatypen='',[gener],[title],[descride],[imgurl],[btnurl],[bgurl],[linkurl],[redirecturi],[corpid],[username],[userpwd],[signphone],[wx_appid],[wx_secret],[qrcode_url],[eachflow],[uplimit],[cutdate],[state],[addtime] FROM [dbo].[T_CooperConfig] WHERE ctype=@ctype and state in(1,2)";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@ctype",SqlDbType.Int),
                new SqlParameter("@issue",SqlDbType.Int)
            };
            parameter[0].Value = ctype;
            parameter[1].Value = issue;
            DataTable dt = dal.ExtSql(sql, parameter);
            foreach (DataRow item in dt.Rows) {
                item["areatypen"] = Enum.GetName(typeof(ConstDefine.AreaType), 1);
            }
            return dt;
        }
        /// <summary>
        /// 取得公司的活动配置信息
        /// </summary>
        /// <param name="phone">校验的手机号码</param>
        /// <returns></returns>
        //public DataTable GetCooperConfig(string phone) {
        //    string sql = "SELECT top 1 [id],[ctype],[issue],[areatype],[gener],[title],[descride],[imgurl],[linkurl],[redirecturi],[corpid],[username],[userpwd],[signphone],[state],[addtime] FROM [dbo].[T_CooperConfig] WHERE signphone=@phone and state=1 order by id desc";
        //    SqlParameter[] parameter = new[]
        //    {
        //        new SqlParameter("@phone",SqlDbType.NVarChar,20)
        //    };
        //    parameter[0].Value = phone;
        //    DataTable dt = dal.ExtSql(sql, parameter);
        //    return dt;
        //}
        /// <summary>
        /// 取得公司的活动配置信息，下拉列表
        /// </summary>
        /// <param name="state">状态</param>
        /// <returns></returns>
        public DataTable GetCooperConfigDrop(int state) {
            string sql = "SELECT id,[ctype],[issue],[title],state FROM [dbo].[T_CooperConfig] WHERE state=@state order by id desc";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@state",SqlDbType.Int)
            };
            parameter[0].Value = state;
            DataTable dt = dal.ExtSql(sql, parameter);
            return dt;
        }
        /// <summary>
        /// 用公司的ID取得公司的信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataTable GetCooperConfigById(int id) {
            string sql = "SELECT [id],[ctype],[issue],[title],[areatype],areatypen='',[gener],[descride],[imgurl],[btnurl],[bgurl],[linkurl],[redirecturi],[corpid],[username],[userpwd],[signphone],[wx_appid],[wx_secret],[qrcode_url],[eachflow],[uplimit],[cutdate],[state],[addtime] FROM [dbo].[T_CooperConfig] WHERE id=@id";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@id",SqlDbType.Int)
            };
            parameter[0].Value = id;
            DataTable dt = dal.ExtSql(sql, parameter);
            foreach (DataRow item in dt.Rows)
                item["areatypen"] = Enum.GetName(typeof(ConstDefine.AreaType), Convert.ToInt32(item["areatype"]));
            return dt;
        }
        public int SetCooper(int id,int ctype,int issue, int areatype, string gener, string title,string descride,string imgurl,string btnurl,string bgurl,string linkurl,string redirecturi,
            string corpid,string username,string userpwd,string signphone,string wx_appid,string wx_secret,string qrcode_url,
            int eachflow,int uplimit,string cutdate,int state) {

            string sql = "INSERT INTO [T_CooperConfig]([ctype],[issue],[areatype],[gener],[title],[descride],[imgurl],[btnurl],[bgurl],[linkurl],[redirecturi],[corpid],[username],[userpwd],[signphone],[wx_appid],[wx_secret],[qrcode_url],[eachflow],[uplimit],[cutdate],[state])";
            sql += "VALUES(@ctype,@issue,@areatype,@gener,@title,@descride,@imgurl,@btnurl,@bgurl,@linkurl,@redirecturi,@corpid,@username,@userpwd,@signphone,@wx_appid,@wx_secret,@qrcode_url,@eachflow,@uplimit,@cutdate,@state)";
            if (id != 0)
            {
                sql = "UPDATE [T_CooperConfig] SET [ctype] = @ctype,[title] = @title,[areatype] = @areatype,[gener] = @gener,[descride] = @descride,[imgurl] = @imgurl,[btnurl] = @btnurl,[bgurl] = @bgurl,[linkurl] = @linkurl,[redirecturi] = @redirecturi,[corpid] = @corpid,[username] = @username,[userpwd] = @userpwd";
                sql += ",[signphone] = @signphone,[wx_appid] = @wx_appid,[wx_secret] = @wx_secret,[qrcode_url] = @qrcode_url,[eachflow] = @eachflow,[uplimit] = @uplimit,[cutdate] = @cutdate,[state] = @state";
                sql += " where id=@id";
            }
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@id",SqlDbType.Int),
                new SqlParameter("@ctype",SqlDbType.Int),
                new SqlParameter("@issue",SqlDbType.Int),
                new SqlParameter("@areatype",SqlDbType.Int),
                new SqlParameter("@gener",SqlDbType.NVarChar,50),
                new SqlParameter("@title",SqlDbType.NVarChar,50),
                new SqlParameter("@descride",SqlDbType.NVarChar,250),
                new SqlParameter("@imgurl",SqlDbType.NVarChar,250),
                new SqlParameter("@btnurl",SqlDbType.NVarChar,250),
                new SqlParameter("@bgurl",SqlDbType.NVarChar,250),
                new SqlParameter("@linkurl",SqlDbType.NVarChar,250),
                new SqlParameter("@corpid",SqlDbType.NVarChar,20),
                new SqlParameter("@username",SqlDbType.NVarChar,50),
                new SqlParameter("@userpwd",SqlDbType.NVarChar,50),
                new SqlParameter("@signphone",SqlDbType.NVarChar,20),
                new SqlParameter("@wx_appid",SqlDbType.NVarChar,50),
                new SqlParameter("@wx_secret",SqlDbType.NVarChar,50),
                new SqlParameter("@qrcode_url",SqlDbType.NVarChar,150),
                new SqlParameter("@eachflow",SqlDbType.Int),
                new SqlParameter("@uplimit",SqlDbType.Int),
                new SqlParameter("@cutdate",SqlDbType.NVarChar,10),
                new SqlParameter("@state",SqlDbType.Int),
                new SqlParameter("@redirecturi",SqlDbType.NVarChar,250),
            };
            parameter[0].Value = id;
            parameter[1].Value = ctype;
            parameter[2].Value = issue;
            parameter[3].Value = areatype;
            parameter[4].Value = gener;
            parameter[5].Value = title;
            parameter[6].Value = descride;
            parameter[7].Value = imgurl;
            parameter[8].Value = btnurl;
            parameter[9].Value = bgurl;
            parameter[10].Value = linkurl;
            parameter[11].Value = corpid;
            parameter[12].Value = username;
            parameter[13].Value = userpwd;
            parameter[14].Value = signphone;
            parameter[15].Value = wx_appid;
            parameter[16].Value = wx_secret;
            parameter[17].Value = qrcode_url;
            parameter[18].Value = eachflow;
            parameter[19].Value = uplimit;
            parameter[20].Value = cutdate;
            parameter[21].Value = state;
            parameter[22].Value = redirecturi;
            int result = Convert.ToInt32(dal.IntExtSql(sql, parameter));
            return result;

        }
        /// <summary>
        /// 删除Cooper信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int CooperRemoveById(int id)
        {
            string sql = "DELETE FROM [T_CooperConfig] WHERE id=@id";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@id",SqlDbType.Int)
            };
            parameter[0].Value = id;
            int result = dal.IntExtSql(sql, parameter);
            return result;
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
        public int DecideOpenid(string openid, int ctype, int issue)
        {
            string sql = "SELECT COUNT(*) FROM [dbo].[T_TakeFlowLog] WHERE ctype=@ctype AND issue=@issue AND openid=@openid";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@ctype",SqlDbType.Int),
                new SqlParameter("@issue",SqlDbType.Int),
                new SqlParameter("@openid",SqlDbType.NVarChar,50)
            };
            parameter[0].Value = ctype;
            parameter[1].Value = issue;
            parameter[2].Value = openid;
            int result = Convert.ToInt32(dal.ExtScalarSql(sql, parameter));
            return result;
        }
        /// <summary>
        /// 取得公司活动的号码收集数
        /// </summary>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        /// <returns></returns>
        public int CtypeInt(int ctype,int issue) {
            string sql = "SELECT COUNT(*) FROM [dbo].[T_TakeFlowLog] where ctype=@ctype and issue=@issue";
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
        /// 添加领取流量的记录
        /// </summary>
        /// <param name="ctype">公司类型</param>
        /// <param name="phone">手机号码</param>
        /// <returns>返回影响行数</returns>
        public int TakeFlowLog(int ctype, int issue, string phone,string openid) {
            string sql = "INSERT INTO [T_TakeFlowLog]([ctype],[issue],[phone],[openid])VALUES(@ctype,@issue,@phone,@openid)";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@ctype",SqlDbType.Int),
                new SqlParameter("@issue",SqlDbType.Int),
                new SqlParameter("@phone", SqlDbType.NVarChar, 20),
                new SqlParameter("@openid", SqlDbType.NVarChar, 50)
            };
            parameter[0].Value = ctype;
            parameter[1].Value = issue;
            parameter[2].Value = phone;
            parameter[3].Value = openid;
            return dal.IntExtSql(sql, parameter);
        }
        /// <summary>
        /// 删除flow记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int RemoveFlowLog(int id) {
            string sql = "DELETE FROM [T_TakeFlowLog] WHERE id=@id";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@id",SqlDbType.Int)
            };
            parameter[0].Value = id;
            return dal.IntExtSql(sql, parameter);
        }
        public DataTable FindFlowLogByCtype(int ctype, int issue)
        {
            string sql = "SELECT [ctype],[issue],[phone],[state],[addtime] FROM [T_TakeFlowLog] Where ctype=@ctype and issue=@issue and state=0";
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
        /// 取得传送过来的验证码信息
        /// </summary>
        /// <param name="type">1登入验证码，2充值验证码</param>
        /// <param name="phone">手机号码</param>
        /// <param name="code">验证码</param>
        /// <returns></returns>
        public int TakeMsgCode(int type,string phone,string xh,string code,string content) {
            string sql = "INSERT INTO [T_MsgCode]([type],[phone],[xh],[code],[text])VALUES(@type,@phone,@xh,@code,@text)";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@type",SqlDbType.Int),
                new SqlParameter("@phone",SqlDbType.NVarChar,20),
                new SqlParameter("@xh",SqlDbType.NVarChar,4),
                new SqlParameter("@code",SqlDbType.NVarChar,6),
                new SqlParameter("@text",SqlDbType.NVarChar,250)
            };
            parameter[0].Value = type;
            parameter[1].Value = phone;
            parameter[2].Value = xh;
            parameter[3].Value = code;
            parameter[4].Value = content;
            return dal.IntExtSql(sql, parameter);
        }
    }
}
