using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class MySqlDal : MySqlConn
    {
        /// <summary>
        /// SqlCommand对象
        /// </summary>
        private MySqlCommand _mCommand;
                /// <summary>
        /// Sql数据库连接
        /// </summary>
        public MySqlDal()
        {
            _mCommand = new MySqlCommand();
            _mCommand.Connection = MSqlConn;
            _mCommand.CommandTimeout = 15;
        }
        /// <summary>
        /// 还原对象，对操作后的对象进行必要清理
        /// </summary>
        private void Restor()
        {
            if (MSqlConn.State == ConnectionState.Open) { MSqlConn.Close(); }
            if (_mCommand != null && _mCommand.Parameters.Count > 0) { _mCommand.Parameters.Clear(); }
        }
        /// <summary>
        /// 执行存储过程返回DataTabel对象
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataTable ExtProc(string procName, object[] parameters)
        {
            if (string.IsNullOrEmpty(procName))
            {
                throw new Exception("存储过程名称不能为空！");
            }
            lock (this)
            {
                try
                {
                    _mCommand.CommandType = CommandType.StoredProcedure;
                    _mCommand.CommandText = procName;
                    _mCommand.Parameters.Clear();
                    if (parameters != null)
                    {
                        foreach (MySqlParameter parameter in parameters)
                        {
                            if (parameter != null)
                            {
                                // 检查未分配值的输出参数,将其分配以DBNull.Value.
                                if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Output) &&
                                    (parameter.Value == null))
                                {
                                    parameter.Value = DBNull.Value;
                                }
                                _mCommand.Parameters.Add(parameter);
                            }
                        }
                    }
                    MySqlDataAdapter adpater = new MySqlDataAdapter(_mCommand);
                    DataTable table = new DataTable();
                    adpater.Fill(table);
                    adpater.Dispose();
                    return table;
                }
                finally
                {
                    Restor();
                    if (parameters != null)
                    {
                        Array.Clear(parameters, 0, parameters.Length);
                    }
                }
            }
        }
        /// <summary>
        /// 多表存储分页
        /// </summary>
        /// <param name="tableName">表名,多表请使用 tA a inner join tB b On a.AID = b.AID</param>
        /// <param name="primaryKey">主键，可以带表头 a.AID</param>
        /// <param name="fields">读取字段</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="currentPage">开始页码</param>
        /// <param name="filter">Where条件</param>
        /// <param name="group">分组</param>
        /// <param name="order">排序字段</param>
        /// <param name="total">总记录数</param>
        /// <returns></returns>
        //public DataTable GetAllPage(string tableName, string primaryKey, string fields, int pageSize, int currentPage, string filter, string group, string order, out int total)
        //{
        //    lock (this)
        //    {
        //        IDataParameter[] parameter = new[]{
        //            CreateInParam("@TableNames",MySqlDbType.VarChar,1000,tableName),
        //            CreateInParam("@PrimaryKey",MySqlDbType.VarChar,100,primaryKey),
        //            CreateInParam("@Fields",MySqlDbType.VarChar,1000,fields),
        //            CreateInParam("@PageSize",MySqlDbType.Int32,4,pageSize),
        //            CreateInParam("@CurrentPage",MySqlDbType.Int32,4,currentPage),
        //            CreateInParam("@Filter",MySqlDbType.VarChar,1000,filter),
        //            CreateInParam("@Group",MySqlDbType.VarChar,1000,group),
        //            CreateInParam("@Order",MySqlDbType.VarChar,200,order),
        //            CreateOutParam("@Total",MySqlDbType.Int32,4),

        //        DataTable tables = ExtProcPage("PageList", parameter, out total);
        //        return tables;
        //    }
        //}
        /// <summary>
        /// 创建MySqlParameter方法
        /// </summary>
        /// <param name="ParamName">参数的名称</param>
        /// <param name="DbType">参数类型</param>
        /// <param name="Size">参数的大小</param>
        /// <param name="Direction">是输入参数还是输出参数input or output</param>
        /// <param name="Value">参数的值</param>
        /// <returns>Return parameters that has been assigned</returns>
        public static MySqlParameter CreateParam(string ParamName, MySqlDbType DbType, Int32 Size, ParameterDirection Direction, object Value)
        {
            MySqlParameter param;
            if (Size > 0)
            {
                param = new MySqlParameter(ParamName, DbType, Size);
            }
            else
            {
                param = new MySqlParameter(ParamName, DbType);
            }
            param.Direction = Direction;
            if (!(Direction == ParameterDirection.Output && Value == null))
            {
                param.Value = Value;
            }
            return param;
        }
        public static MySqlParameter CreateReturnParam(string ParamName, MySqlDbType DbType, int Size)
        {
            return CreateParam(ParamName, DbType, Size, ParameterDirection.ReturnValue, null);
        }
        public static MySqlParameter CreateOutParam(string ParamName, MySqlDbType DbType, int Size)
        {
            return CreateParam(ParamName, DbType, Size, ParameterDirection.Output, null);
        }
        public static MySqlParameter CreateInParam(string ParamName, MySqlDbType DbType, int Size, object Value)
        {
            return CreateParam(ParamName, DbType, Size, ParameterDirection.Input, Value);
        }
    }
}
