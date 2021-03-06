﻿using Common;
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
    public class MobileDAL
    {
        SqlDal dal = new SqlDal();
        /// <summary>
        /// 将未充值的T_TakeFlowLog用户状态先修改为充值中。。。
        /// </summary>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        /// <returns></returns>
        private int UpdateFlowState(int ctype, int issue) {
            string sql = "update T_TakeFlowLog set state=2 where ctype=@ctype and issue=@issue and state = 0";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@ctype",SqlDbType.Int),
                new SqlParameter("@issue",SqlDbType.Int)
            };
            parameter[0].Value = ctype;
            parameter[1].Value = issue;
            return dal.IntExtSql(sql, parameter);
        }
        /// <summary>
        /// 将充值中的T_TakeFlowLog用户状态修改为已充值完成
        /// </summary>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        /// <returns></returns>
        public int UpdateFlowStateO(int ctype, int issue) {
            string sql = "update T_TakeFlowLog set state=1 where ctype=@ctype and issue=@issue and state = 2";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@ctype",SqlDbType.Int),
                new SqlParameter("@issue",SqlDbType.Int)
            };
            parameter[0].Value = ctype;
            parameter[1].Value = issue;
            return dal.IntExtSql(sql, parameter);
        }
        /// <summary>
        /// 批量导入execl生成SQL语句
        /// </summary>
        /// <param name="ctype">类型</param>
        /// <param name="issue">期号</param>
        /// <param name="ll">流量多少</param>
        /// <param name="jzsj">截止时间</param>
        /// <returns></returns>
        public DataTable FindFlowLogToExecl(int ctype, int issue, int ll, string jzsj)
        {
            UpdateFlowState(ctype, issue);  //先将未充值的用户状态修改为待充值
            string sql = "SELECT [phone] 号码,'" + ll + "' 成员指定流量,'立即生效' 生效模式," + jzsj + " 截止时间 FROM [T_TakeFlowLog] Where ctype=@ctype and issue=@issue and state = 2";
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
        /// 将充值中的T_ActivityDrawLog用户状态修改为已充值完成
        /// </summary>
        /// <param name="type"></param>
        /// <param name="cooperid"></param>
        /// <returns></returns>
        public int UpdateActivityFlowState(int ctype,int issue) {
            string sql = "UPDATE T_ActivityDrawLog SET state=1 WHERE cooperid in(SELECT id FROM T_CooperConfig WHERE ctype=@ctype AND issue=@issue) AND state=2";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@ctype",SqlDbType.Int),
                new SqlParameter("@issue",SqlDbType.Int)
            };
            parameter[0].Value = ctype;
            parameter[1].Value = issue;
            return dal.IntExtSql(sql, parameter);
        }
        /// <summary>
        /// 批量导入execl生成SQL语句
        /// </summary>
        /// <param name="type">1大转盘,2在线答题</param>
        /// <param name="cooperid">用户的cooperid</param>
        /// <param name="jzsj">截止时间</param>
        /// <returns></returns>
        public DataTable FindActivityFlowLog(int cztype,int cooperid,string jzsj) {
            string sql = "SP_ActivityFlowLog";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@CzType",SqlDbType.Int),
                new SqlParameter("@Cooperid",SqlDbType.Int),
                new SqlParameter("@jzsj",SqlDbType.NVarChar,50)
            };
            parameter[0].Value = cztype;
            parameter[1].Value = cooperid;
            parameter[2].Value = jzsj;
            return dal.ExtProc(sql, parameter);
        }
        /// <summary>
        /// 更新用户T_CooperConfig的短信密码字段
        /// </summary>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public int UpdateConfigPwd(int ctype, int issue, string code)
        {
            string sql = "UPDATE [T_CooperConfig] SET [userpwd] = @pwd WHERE ctype=@ctype AND issue=@issue";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@ctype",SqlDbType.Int),
                new SqlParameter("@issue",SqlDbType.Int),
                new SqlParameter("@pwd",SqlDbType.NVarChar,50),
            };
            parameter[0].Value = ctype;
            parameter[1].Value = issue;
            parameter[2].Value = code;
            return dal.IntExtSql(sql, parameter);
        }
        /// <summary>
        /// 更新短信的序列号,将状态设置为1
        /// </summary>
        /// <param name="ctype">类型</param>
        /// <param name="issue">期号</param>
        /// <param name="czdxhm">发送短信的号码</param>
        /// <param name="dxxh">短信序列号</param>
        /// <returns></returns>
        public int UpdateLogCacheXh(int ctype, int issue, string czdxhm, string dxxh,string czparam)
        {
            InitT_LoginLogCacheState(); //先进行state初始化，设置为0
            string sql = "UPDATE [T_LoginLogCache] SET [czdxhm] = @czdxhm,[dxxh] = @dxxh,[czparam] = @czparam,state = 1 WHERE ctype=@ctype and issue=@issue";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@ctype",SqlDbType.Int),
                new SqlParameter("@issue",SqlDbType.Int),
                new SqlParameter("@czdxhm",SqlDbType.NVarChar,20),
                new SqlParameter("@dxxh",SqlDbType.Int),
                new SqlParameter("@czparam",SqlDbType.NVarChar,1000),
            };
            parameter[0].Value = ctype;
            parameter[1].Value = issue;
            parameter[2].Value = czdxhm;
            parameter[3].Value = dxxh;
            parameter[4].Value = czparam;
            return dal.IntExtSql(sql, parameter);
        }
        /// <summary>
        /// 将缓存的状态都设置为0
        /// </summary>
        /// <returns></returns>
        private int InitT_LoginLogCacheState() {
            string sql = "UPDATE [T_LoginLogCache] SET state = 0";
            return dal.IntExtSql(sql);
        }
        /// <summary>
        /// 通过序列号取得当前正在充值的合作公司类型
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="xh">序列号</param>
        /// <returns></returns>
        public DataTable FindCtypeIssueCache(string phone,string xh) {
            string sql = "SELECT [ctype],[issue],[czparam],[czdxhm] FROM [dbo].[T_LoginLogCache] WHERE czdxhm=@czdxhm AND dxxh=@dxxh and state = 1";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@czdxhm",SqlDbType.NVarChar,20),
                new SqlParameter("@dxxh",SqlDbType.NVarChar,4)
            };
            parameter[0].Value = phone;
            parameter[1].Value = xh;
            return dal.ExtSql(sql, parameter);
        }
        /// <summary>
        /// 通过序列号取得当前正在登入的合作公司类型
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="xh">序列号</param>
        /// <returns></returns>
        public DataTable FindCtypeIssueForDl(string phone, string xh)
        {
            string sql = "SELECT [ctype],[issue],[czdxhm] FROM [dbo].[T_LoginLogCache] WHERE czdxhm=@dldxhm AND dlxh=@dlxh and dlstate = 1";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@dldxhm",SqlDbType.NVarChar,20),
                new SqlParameter("@dlxh",SqlDbType.NVarChar,4)
            };
            parameter[0].Value = phone;
            parameter[1].Value = xh;
            return dal.ExtSql(sql, parameter);
        }
        /// <summary>
        /// 取得所有Cache的列表
        /// </summary>
        /// <returns></returns>
        public DataTable FindCtypeIssueCache() {
            string sql = "SELECT id, [ctype],[issue],[cookie] dlcookie,[czparam],[czdxhm] FROM [dbo].[T_LoginLogCache]";
            return dal.ExtSql(sql);
        }

        /// <summary>
        /// 更新登入的cookie
        /// </summary>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        /// <returns></returns>
        public int UpdateDlCookie(int ctype,int issue,string cookie) {
            string sql = "UPDATE [T_LoginLogCache] SET [cookie] =@cookie WHERE ctype=@ctype AND issue=@issue";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@ctype",SqlDbType.Int),
                new SqlParameter("@issue",SqlDbType.Int),
                new SqlParameter("@cookie",SqlDbType.NVarChar,2000),
            };
            parameter[0].Value = ctype;
            parameter[1].Value = issue;
            parameter[2].Value = cookie;
            return dal.IntExtSql(sql, parameter);
        }
        /// <summary>
        /// 更新登入短信的序列号
        /// </summary>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        /// <param name="dldxhm"></param>
        /// <param name="dlxh"></param>
        /// <returns></returns>
        public int UpdateLogCacheDlXh(int ctype, int issue,string dldxhm, string dlxh)
        {
            InitT_LoginLogCacheDlState(); //先进行dlstate初始化，设置为0
            string sql = "UPDATE [T_LoginLogCache] SET [czdxhm] = @dldxhm,[dlxh] = @dlxh,dlstate = 1 WHERE ctype=@ctype and issue=@issue";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@ctype",SqlDbType.Int),
                new SqlParameter("@issue",SqlDbType.Int),
                new SqlParameter("@dldxhm",SqlDbType.NVarChar,20),
                new SqlParameter("@dlxh",SqlDbType.NVarChar,4)
            };
            parameter[0].Value = ctype;
            parameter[1].Value = issue;
            parameter[2].Value = dldxhm;
            parameter[3].Value = dlxh;
            return dal.IntExtSql(sql, parameter);
        }
        /// <summary>
        /// 将缓存的登入状态都设置为0
        /// </summary>
        /// <returns></returns>
        private int InitT_LoginLogCacheDlState()
        {
            string sql = "UPDATE [T_LoginLogCache] SET dlstate = 0";
            return dal.IntExtSql(sql);
        }
        /// <summary>
        /// 取得登入缓存的cookie
        /// </summary>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        /// <returns></returns>
        public DataTable GetLoginCache(int ctype, int issue)
        {
            string sql = "SELECT [id],[ctype],[issue],[cookie],[czparam],[czdxhm],[dxxh],[state],[dlxh],[dlstate],[lasttime] FROM [dbo].[T_LoginLogCache] WHERE ctype=@ctype and issue=@issue";
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
        /// 是否存在待直充的记录
        /// </summary>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        /// <returns></returns>
        public int IsExistsCzList(int ctype,int issue) {
            string sql = "select COUNT(*) from T_TakeFlowLog where ctype=@ctype and issue=@issue and state!=1";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@ctype",SqlDbType.Int),
                new SqlParameter("@issue",SqlDbType.Int)
            };
            parameter[0].Value = ctype;
            parameter[1].Value = issue;
            return Convert.ToInt32(dal.ExtScalarSql(sql, parameter));
        }
        /// <summary>
        /// 是否存在待充值的活动记录
        /// </summary>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        /// <returns></returns>
        public int IsExistsActityList(int ctype, int issue)
        {
            string sql = "SELECT COUNT(*) FROM T_ActivityDrawLog WHERE state!=1 AND cooperid IN(SELECT id FROM T_CooperConfig WHERE ctype=@ctype AND issue=@issue AND state=1)";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@ctype",SqlDbType.Int),
                new SqlParameter("@issue",SqlDbType.Int)
            };
            parameter[0].Value = ctype;
            parameter[1].Value = issue;
            return Convert.ToInt32(dal.ExtScalarSql(sql, parameter));
        }
        /// <summary>
        /// 添加超端控制记录
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public int InsertLoginCache(T_LoginLogCache dto) {
            string sql = "INSERT INTO [T_LoginLogCache]([ctype],[issue])VALUES(@ctype,@issue)";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@ctype",SqlDbType.Int),
                new SqlParameter("@issue",SqlDbType.Int)
            };
            parameter[0].Value = dto.ctype;
            parameter[1].Value = dto.issue;
            return dal.IntExtSql(sql, parameter);
        }
        /// <summary>
        /// 根据ID移除超端记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int RemoveLoginCache(int id) {
            string sql = "DELETE FROM [T_LoginLogCache] WHERE id=@id";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@id",SqlDbType.Int)
            };
            parameter[0].Value = id;
            return dal.IntExtSql(sql, parameter);
        }
        /// <summary>
        /// 是否存在超端记录
        /// </summary>
        /// <param name="ctype"></param>
        /// <param name="issue"></param>
        /// <returns></returns>
        public int IsExistsT_LoginLogCache(int ctype, int issue)
        {
            string sql = "SELECT COUNT(*) FROM [T_LoginLogCache] where ctype=@ctype and issue=@issue";
            SqlParameter[] parameter = new[]
            {
                new SqlParameter("@ctype",SqlDbType.Int),
                new SqlParameter("@issue",SqlDbType.Int)
            };
            parameter[0].Value = ctype;
            parameter[1].Value = issue;
            return Convert.ToInt32(dal.ExtScalarSql(sql, parameter));
        }


        /// <summary>
        /// 合并
        /// </summary>
        /// <param name="dt1"></param>
        /// <param name="dt2"></param>
        /// <returns></returns>
        public DataTable MergeDt(DataTable dt1,DataTable dt2) {
            DataTable newDataTable = dt1.Clone();
            object[] obj = new object[newDataTable.Columns.Count];
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                dt1.Rows[i].ItemArray.CopyTo(obj, 0);
                newDataTable.Rows.Add(obj);
            }

            for (int i = 0; i < dt2.Rows.Count; i++)
            {
                dt2.Rows[i].ItemArray.CopyTo(obj, 0);
                newDataTable.Rows.Add(obj);
            }
            return newDataTable;
        }
    }
}
