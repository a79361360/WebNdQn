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
    public class WeiXinDAL
    {
        SqlDal dal = new SqlDal();
        /// <summary>
        /// 设置微信用户信息,如果dto.id的值不为0,则是更新
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public int SetWxUserInfo(T_WxUserInfo dto) {
            string sql = "";
            if (dto.id == 0){
                sql = "INSERT INTO [T_WxUserInfo]([wx_appid],[wx_openid],[wx_nickname],[wx_sex],[wx_headurl],[wx_unionid])";
                sql += "VALUES(@wx_appid,@wx_openid,@wx_nickname,@wx_sex,@wx_headurl,@wx_unionid)";
            }
            else {
                sql = "UPDATE [T_WxUserInfo]SET [wx_appid] = @wx_appid,[wx_openid] = @wx_openid,[wx_nickname] = @wx_nickname,";
                sql +="[wx_sex] = @wx_sex,[wx_headurl] = @wx_headurl,[wx_unionid] = @wx_unionid WHERE [id] = @id";
            }
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@id",SqlDbType.Int),
                new SqlParameter("@wx_appid",SqlDbType.NVarChar,50),
                new SqlParameter("@wx_openid",SqlDbType.NVarChar,50),
                new SqlParameter("@wx_nickname",SqlDbType.NVarChar,50),
                new SqlParameter("@wx_sex",SqlDbType.Int),
                new SqlParameter("@wx_headurl",SqlDbType.NVarChar,255),
                new SqlParameter("@wx_unionid",SqlDbType.NVarChar,50)
            };
            parameter[0].Value = dto.id;
            parameter[1].Value = dto.wx_appid;
            parameter[2].Value = dto.wx_openid;
            parameter[3].Value = dto.wx_nickname;
            parameter[4].Value = dto.wx_sex;
            parameter[5].Value = dto.wx_headurl;
            parameter[6].Value = dto.wx_unionid;
            int result = dal.IntExtSql(sql, parameter);
            return result;
        }
        /// <summary>
        /// 是否已经存在当前appid的微信用户openid
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        public int IsExistsWxUser(string appid,string openid) {
            string sql = "SELECT COUNT(*) FROM [T_WxUserInfo] WHERE wx_appid=@wx_appid AND wx_openid=@wx_openid";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@wx_appid",SqlDbType.NVarChar,50),
                new SqlParameter("@wx_openid",SqlDbType.NVarChar,50),
            };
            parameter[0].Value = appid;
            parameter[1].Value = openid;
            int result = Convert.ToInt32(dal.ExtScalarSql(sql, parameter));
            return result;
        }
        /// <summary>
        /// 取得微信用户记录ID通过appid和openid
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        public int GetWxUserIdByAO(string appid, string openid) {
            string sql = "SELECT top 1 id FROM [T_WxUserInfo] WHERE wx_appid=@wx_appid AND wx_openid=@wx_openid";
            SqlParameter[] parameter = new[]
{
                new SqlParameter("@wx_appid",SqlDbType.NVarChar,50),
                new SqlParameter("@wx_openid",SqlDbType.NVarChar,50),
            };
            parameter[0].Value = appid;
            parameter[1].Value = openid;
            int result = Convert.ToInt32(dal.ExtScalarSql(sql, parameter));
            return result;
        }
    }
}
