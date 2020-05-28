using Common;
using Model.WishModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class WishDAL
    {
        SqlDal dal = new SqlDal();
        public DataTable WishList()
        {
            string sql = "SELECT [id],[wishid],[name],[sex],[age],[wishcontent],whishelperid FROM [dbo].[T_Wish] ";
            return dal.ExtSql(sql);
        }
        public int WishAdd(T_Wish o) {
            string sql = "INSERT INTO [dbo].[T_Wish]([wishid],[name],[sex],[age],[wishcontent])VALUES(@wishid,@name,@sex,@age,@wishcontent)";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@wishid",SqlDbType.Int),
                new SqlParameter("@name",SqlDbType.NVarChar,10),
                new SqlParameter("@sex",SqlDbType.Int),
                new SqlParameter("@age",SqlDbType.Int),
                new SqlParameter("@wishcontent",SqlDbType.NVarChar,250)
            };
            parameter[0].Value = o.wishid;
            parameter[1].Value = o.name;
            parameter[2].Value = o.sex;
            parameter[3].Value = o.age;
            parameter[4].Value = o.wishcontent;
            return dal.IntExtSql(sql, parameter);
        }
        public int WishHelperAdd(T_WishHelper o)
        {
            string sql = "INSERT INTO [dbo].[T_WishHelper]([helpername],[phone],[wishid],[sendtype],[sendmsg])VALUES(@helpername,@phone,@wishid,@sendtype,@sendmsg) SELECT @@IDENTITY";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@helpername",SqlDbType.NVarChar,50),
                new SqlParameter("@phone",SqlDbType.NVarChar,50),
                new SqlParameter("@wishid",SqlDbType.Int),
                new SqlParameter("@sendtype",SqlDbType.Int),
                new SqlParameter("@sendmsg",SqlDbType.NVarChar,250)
            };
            parameter[0].Value = o.helpername;
            parameter[1].Value = o.phone;
            parameter[2].Value = o.wishid;
            parameter[3].Value = o.sendtype;
            parameter[4].Value = o.sendmsg;

            int helperid = Convert.ToInt32(dal.ExtScalarSql(sql, parameter));
            if (o.wishid == 67) return helperid;                //67就可以直接返回了
            else return UpsetWishHelperId(helperid, o.wishid);  //非67去更新一下祝愿表的捐助人id
        }
        /// <summary>
        /// 使用捐助人的表来进行是否已被捐助的判断
        /// </summary>
        /// <param name="wishid"></param>
        /// <returns></returns>
        public int IsExistWish(int wishid) {
            string sql = "select top 1 id from [dbo].[T_WishHelper] where wishid=@wishid";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@wishid",SqlDbType.Int)
            };
            parameter[0].Value = wishid;
            return Convert.ToInt32(dal.ExtScalarSql(sql, parameter));
        }
        /// <summary>
        /// 更新祈愿表的捐助人id
        /// </summary>
        /// <param name="helperid"></param>
        /// <param name="wishid"></param>
        /// <returns></returns>
        public int UpsetWishHelperId(int helperid,int wishid) {
            string sql = "update T_Wish set whishelperid=@helperid where id=@wishid and whishelperid=0";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@helperid",SqlDbType.Int),
                new SqlParameter("@wishid",SqlDbType.Int)
            };
            parameter[0].Value = helperid;
            parameter[1].Value = wishid;
            return dal.IntExtSql(sql, parameter);
        }
    }
}
