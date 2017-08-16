using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class SqlDal : SqlConn, IDalInterFace
    {
        string logstr = "";
        /// <summary>
        /// SqlCommand对象
        /// </summary>
        private SqlCommand _mCommand;

        /// <summary>
        /// Sql数据库连接
        /// </summary>
        public SqlDal()
        {
            _mCommand = new SqlCommand();
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
        /// 执行存储过程，返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameter">存储过程参数</param>
        /// <returns>SqlDataReader</returns>
        public SqlDataReader RunProcedure(string procName, IDataParameter[] parameter)
        {
            SqlParameter[] sqlParameter = parameter as SqlParameter[];
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
                    if (sqlParameter != null)
                    {
                        foreach (SqlParameter temp in sqlParameter)
                        {
                            _mCommand.Parameters.Add(temp);
                            logstr += temp.ParameterName + "   " + temp.Value;
                        }
                    }
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/RunProcedure_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "ProName: " + procName + "logstr:" + logstr);
                    MSqlConn.Open();
                    return _mCommand.ExecuteReader(CommandBehavior.CloseConnection);
                }
                finally
                {
                    if (sqlParameter != null)
                    {
                        Array.Clear(sqlParameter, 0, sqlParameter.Length);
                    }
                }
            }
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
                        foreach (SqlParameter parameter in parameters)
                        {
                            if (parameter != null)
                            {
                                // 检查未分配值的输出参数,将其分配以DBNull.Value.
                                if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Output) &&
                                    (parameter.Value == null))
                                {
                                    parameter.Value = DBNull.Value;
                                }
                                logstr += parameter.ParameterName + "   " + parameter.Value;
                                _mCommand.Parameters.Add(parameter);
                            }
                        }
                    }
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/ExtProc_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "ProName: " + procName + "logstr:" + logstr);
                    SqlDataAdapter adpater = new SqlDataAdapter(_mCommand);
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
        /// 执行存储过程返回DataTabel对象
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataTable ExtProcRe(string procName, object[] parameters)
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
                        foreach (SqlParameter parameter in parameters)
                        {
                            if (parameter != null)
                            {
                                // 检查未分配值的输出参数,将其分配以DBNull.Value.
                                if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Output) &&
                                    (parameter.Value == null))
                                {
                                    parameter.Value = DBNull.Value;
                                }
                                logstr += parameter.ParameterName + "   " + parameter.Value;
                                _mCommand.Parameters.Add(parameter);
                            }
                        }
                    }
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/ExtProcRe_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "ProName: " + procName + "logstr:" + logstr);
                    SqlDataAdapter adpater = new SqlDataAdapter(_mCommand);
                    DataTable table = new DataTable();
                    adpater.Fill(table);
                    adpater.Dispose();
                    return table;
                }
                finally
                {
                    Restor();
                }
            }
        }

        /// <summary>
        /// 执行存储过程返回DataTabel对象
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="parameters"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public DataTable ExtProcPage(string procName, IDataParameter[] parameters, out int total)
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
                        foreach (SqlParameter parameter in parameters)
                        {
                            if (parameter != null)
                            {
                                // 检查未分配值的输出参数,将其分配以DBNull.Value.
                                if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Output) &&
                                    (parameter.Value == null))
                                {
                                    parameter.Value = DBNull.Value;
                                }
                                logstr += parameter.ParameterName + "   " + parameter.Value;
                                _mCommand.Parameters.Add(parameter);
                            }
                        }
                    }
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/ExtProcPage_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "ProName: " + procName + "logstr:" + logstr);
                    SqlDataAdapter adpater = new SqlDataAdapter(_mCommand);
                    DataTable table = new DataTable();
                    adpater.Fill(table);
                    adpater.Dispose();
                    total = Convert.ToInt32(parameters[8].Value);
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
        /// 执行存储过程返回DataTable对象的同时返回一个键值对对象
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="parameter"></param>
        /// <param name="retFiled"> </param>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public DataTable ExtProc(String procName, object[] parameter, string[] retFiled, out Dictionary<string, object> dictionary)
        {
            if (String.IsNullOrEmpty(procName)) { throw new Exception("存储过程的名字不能为空！"); }
            dictionary = new Dictionary<string, object>();
            SqlParameter[] sqlParameter = parameter as SqlParameter[];
            lock (this)
            {
                try
                {
                    _mCommand.CommandType = CommandType.StoredProcedure;
                    _mCommand.CommandText = procName;
                    _mCommand.Parameters.Clear();
                    if (sqlParameter != null)
                    {
                        foreach (SqlParameter temp in sqlParameter)
                        {
                            _mCommand.Parameters.Add(temp);
                            logstr += temp.ParameterName + "   " + temp.Value;
                        }
                    }
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/ExtProc_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "ProName: " + procName + "logstr:" + logstr);
                    SqlDataAdapter adapter = new SqlDataAdapter(_mCommand);
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    if (retFiled != null)
                    {
                        foreach (String key in retFiled)
                        {
                            dictionary.Add(key, _mCommand.Parameters[key].Value);
                        }
                    }
                    adapter.Dispose();
                    return table;
                }
                finally
                {
                    Restor();
                    if (sqlParameter != null)
                    {
                        Array.Clear(sqlParameter, 0, sqlParameter.Length);
                    }
                }
            }
        }

        /// <summary>
        /// 执行存储过程返回受影响的行数
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="parameter"></param>
        public int NoExtProc(string procName, object[] parameter)
        {
            SqlParameter[] sqlParameter = parameter as SqlParameter[];
            if (string.IsNullOrEmpty(procName)) { throw new Exception("存储过程名称不能为空！"); }
            lock (this)
            {
                try
                {
                    MSqlConn.Open();
                    _mCommand.CommandType = CommandType.StoredProcedure;
                    _mCommand.CommandText = procName;
                    _mCommand.Parameters.Clear();
                    if (sqlParameter != null)
                    {
                        foreach (SqlParameter temp in sqlParameter)
                        {
                            _mCommand.Parameters.Add(temp);
                            logstr += temp.ParameterName + "   " + temp.Value;
                        }
                    }
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/NoExtProc_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "ProName: " + procName + "logstr:" + logstr);
                    int retvalue = _mCommand.ExecuteNonQuery();
                    MSqlConn.Close();
                    return retvalue;
                }
                finally
                {
                    Restor();
                    if (sqlParameter != null)
                    {
                        Array.Clear(sqlParameter, 0, sqlParameter.Length);
                    }
                }
            }
        }

        /// <summary>
        /// 执行存储过程返回键值对对象
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameter">调用存储的参数</param>
        /// <param name="retFiled">要返回字段名</param>
        /// <returns></returns>
        public Dictionary<string, object> ExtProc(string procName, object[] parameter, string[] retFiled)
        {
            SqlParameter[] sqlParameter = parameter as SqlParameter[];
            Dictionary<string, object> retDictionary = null;
            if (string.IsNullOrEmpty(procName)) { throw new Exception("存储名字不能为空！"); }
            lock (this)
            {
                try
                {
                    MSqlConn.Open();
                    _mCommand.CommandType = CommandType.StoredProcedure;
                    _mCommand.CommandText = procName;
                    _mCommand.Parameters.Clear();
                    if (sqlParameter != null)
                    {
                        foreach (SqlParameter temp in sqlParameter)
                        {
                            _mCommand.Parameters.Add(temp);
                            logstr += temp.ParameterName + "   " + temp.Value;
                        }
                    }
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/ExtProc_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "ProName: " + procName + "logstr:" + logstr);
                    _mCommand.ExecuteNonQuery();
                    MSqlConn.Close();
                    if (retFiled != null)
                    {
                        retDictionary = new Dictionary<string, object>();
                        foreach (string key in retFiled)
                        {
                            retDictionary.Add(key, _mCommand.Parameters[key].Value);
                        }
                    }
                    return retDictionary;
                }
                finally
                {
                    Restor();
                    if (sqlParameter != null)
                    {
                        Array.Clear(sqlParameter, 0, sqlParameter.Length);
                    }
                }
            }
        }

        /// <summary>
        /// 执行查询语句返回表对象
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable ExtSql(String sql)
        {
            Common.Expend.LogTxtExpend.WriteLogs("/Logs/ExtSql_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", sql);
            if (String.IsNullOrEmpty(sql)) { throw new Exception("Sql结构化查询语句不能为空!"); }
            lock (this)
            {
                _mCommand.CommandType = CommandType.Text;
                _mCommand.CommandText = sql;
                SqlDataAdapter adapter = new SqlDataAdapter(_mCommand);
                DataTable table = new DataTable();
                adapter.Fill(table);
                adapter.Dispose();
                return table;
            }
        }
        /// <summary>
        /// 执行查询语句返回表对象
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public DataTable ExtSql(String sql, object[] parameter)
        {
            SqlParameter[] sqlParameter = parameter as SqlParameter[];
            if (String.IsNullOrEmpty(sql)) { throw new Exception("Sql结构化查询语句不能为空!"); }
            lock (this)
            {
                _mCommand.CommandType = CommandType.Text;
                _mCommand.CommandText = sql;
                _mCommand.Parameters.Clear();
                if (sqlParameter != null)
                {
                    foreach (SqlParameter temp in sqlParameter)
                    {
                        _mCommand.Parameters.Add(temp);
                        logstr += temp.ParameterName + "   " + temp.Value;
                    }
                }
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/ExtSqlParamter_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", sql + "Parameter: " + logstr);
                SqlDataAdapter adapter = new SqlDataAdapter(_mCommand);
                DataTable table = new DataTable();
                adapter.Fill(table);
                adapter.Dispose();
                return table;
            }
        }

        /// <summary>
        /// 执行查询语句返回受影响的行数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int IntExtSql(String sql)
        {
            Common.Expend.LogTxtExpend.WriteLogs("/Logs/IntExtSql_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", sql);
            if (String.IsNullOrEmpty(sql)) { throw new Exception("Sql结构化查询语句不能为空！"); }
            lock (this)
            {
                try
                {
                    MSqlConn.Open();
                    _mCommand.CommandType = CommandType.Text;
                    _mCommand.CommandText = sql;
                    int retValue = _mCommand.ExecuteNonQuery();

                    return retValue;
                }
                finally
                {
                    if (MSqlConn.State == ConnectionState.Open)
                    {
                        MSqlConn.Close();
                    }
                }
            }
        }
        /// <summary>
        /// 执行查询语句返回受影响的行数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int IntExtSql(String sql, object[] parameter)
        {
            SqlParameter[] sqlParameter = parameter as SqlParameter[];
            Common.Expend.LogTxtExpend.WriteLogs("/Logs/IntExtSqlParamter_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", sql);
            if (String.IsNullOrEmpty(sql)) { throw new Exception("Sql结构化查询语句不能为空！"); }
            lock (this)
            {
                try
                {
                    MSqlConn.Open();
                    _mCommand.CommandType = CommandType.Text;
                    _mCommand.CommandText = sql;
                    _mCommand.Parameters.Clear();
                    if (sqlParameter != null)
                    {
                        foreach (SqlParameter temp in sqlParameter)
                        {
                            _mCommand.Parameters.Add(temp);
                            logstr += temp.ParameterName + "   " + temp.Value;
                        }
                    }
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/IntExtSqlParamter_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", sql + "Parameter: " + logstr);
                    int retValue = _mCommand.ExecuteNonQuery();

                    return retValue;
                }
                finally
                {
                    if (MSqlConn.State == ConnectionState.Open)
                    {
                        MSqlConn.Close();
                    }
                }
            }
        }
        /// <summary>
        /// 单字段的查询结果
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public object ExtScalarSql(String sql)
        {
            if (String.IsNullOrEmpty(sql)) { throw new Exception("Sql结构化查询语句不能为空！"); }
            lock (this)
            {
                try
                {
                    MSqlConn.Open();
                    _mCommand.CommandType = CommandType.Text;
                    _mCommand.CommandText = sql;
                    object retValue = _mCommand.ExecuteScalar();

                    return retValue;
                }
                finally
                {
                    if (MSqlConn.State == ConnectionState.Open)
                    {
                        MSqlConn.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 单字段的查询结果带加密参数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public object ExtScalarSql(String sql, object[] parameter)
        {
            SqlParameter[] sqlParameter = parameter as SqlParameter[];
            lock (this)
            {
                try
                {
                    MSqlConn.Open();
                    _mCommand.CommandType = CommandType.Text;
                    _mCommand.CommandText = sql;
                    _mCommand.Parameters.Clear();
                    if (sqlParameter != null)
                    {
                        foreach (SqlParameter temp in sqlParameter)
                        {
                            _mCommand.Parameters.Add(temp);
                        }
                    }
                    object retValue = _mCommand.ExecuteScalar();

                    return retValue;
                }
                finally
                {
                    if (MSqlConn.State == ConnectionState.Open)
                    {
                        MSqlConn.Close();
                    }
                }
            }
        }
        /// <summary>
        /// 批量存储数据
        /// </summary>
        /// <param name="tabName"></param>
        /// <param name="dataTable"></param>
        public void SQLBulkCopy(string tabName, DataTable dataTable)
        {
            SQLBulkCopy(tabName, dataTable, 1000);
        }

        /// <summary>
        /// 批量存储数据
        /// </summary>
        /// <param name="tabName"></param>
        /// <param name="dataTable"></param>
        /// <param name="rowsCounts"> </param>
        public void SQLBulkCopy(string tabName, DataTable dataTable, int rowsCounts)
        {
            Common.Expend.LogTxtExpend.WriteLogs("/Logs/SQLBulkCopy_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", tabName);
            if (string.IsNullOrEmpty(tabName)) { throw new Exception("表明不能为空！"); }
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                lock (this)
                {
                    try
                    {
                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(MSqlConn))
                        {
                            MSqlConn.Open();
                            bulkCopy.DestinationTableName = tabName;
                            bulkCopy.NotifyAfter = rowsCounts;
                            foreach (DataColumn tempColumn in dataTable.Columns)
                            {
                                bulkCopy.ColumnMappings.Add(tempColumn.ColumnName, tempColumn.ColumnName);
                            }
                            bulkCopy.WriteToServer(dataTable);
                            bulkCopy.Close();
                        }
                    }
                    finally
                    {
                        if (MSqlConn.State == ConnectionState.Open)
                        {
                            MSqlConn.Close();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 批量更新数据源
        /// </summary>
        /// <param name="table"></param>
        /// <param name="tbName"> </param>
        public void UpdataSource(DataTable table, string tbName)
        {
            //if (table != null && table.Rows.Count > 0)
            //{
            //    SqlDataAdapter adapter = new SqlDataAdapter();
            //    try
            //    {
            //        BatchUpdate(adapter, table, tbName);
            //    }
            //    finally
            //    {
            //        adapter.Dispose();
            //    }
            //}
        }

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="adapter"></param>
        /// <param name="table"></param>
        /// <param name="tbName"></param>
        public virtual void BatchUpdate(SqlDataAdapter adapter, DataTable table, string tbName)
        {
            //adapter.UpdateCommand = new SqlCommand("UPDATE " + tbName + " SET UpdateTime=getdate() where IDentify=@IDentify", m_SqlConn);
            //adapter.UpdateCommand.Parameters.Add("@IDentify", SqlDbType.NVarChar, 50, "IDentify");
            //adapter.UpdateCommand.UpdatedRowSource = UpdateRowSource.None;

            //adapter.InsertCommand = new SqlCommand("Insert into " + tbName + " (IDentify,Account,UserID,tbName,GroupID,LoginDateTime,UpdateTime,ZBDTID,ZBZTID,IP) values(@IDentify,@Account,@UserID,@tbName,@GroupID,@LoginDateTime,@UpdateTime,@ZBDTID,@ZBZTID,@IP)", m_SqlConn);
            //adapter.InsertCommand.Parameters.Add("@IDentify", SqlDbType.NVarChar, 200, "IDentify");
            //adapter.InsertCommand.Parameters.Add("@Account", SqlDbType.NVarChar, 50, "Account");
            //adapter.InsertCommand.Parameters.Add("@UserID", SqlDbType.BigInt, 20, "UserID");
            //adapter.InsertCommand.Parameters.Add("@tbName", SqlDbType.NVarChar, 50, "tbName");
            //adapter.InsertCommand.Parameters.Add("@GroupID", SqlDbType.BigInt, 20, "GroupID");
            //adapter.InsertCommand.Parameters.Add("@LoginDateTime", SqlDbType.DateTime, 20, "LoginDateTime");
            //adapter.InsertCommand.Parameters.Add("@UpdateTime", SqlDbType.DateTime, 20, "UpdateTime");
            //adapter.InsertCommand.Parameters.Add("@ZBDTID", SqlDbType.BigInt, 20, "ZBDTID");
            //adapter.InsertCommand.Parameters.Add("@ZBZTID", SqlDbType.BigInt, 20, "ZBZTID");
            //adapter.InsertCommand.Parameters.Add("@IP", SqlDbType.NVarChar, 50, "IP");
            //adapter.InsertCommand.UpdatedRowSource = UpdateRowSource.None;
            //adapter.Update(table);
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
        public DataTable GetAllPage(string tableName, string primaryKey, string fields, int pageSize, int currentPage, string filter, string group, string order, out int total)
        {
            lock (this)
            {
                IDataParameter[] parameter = new[]{					
					new SqlParameter("@TableNames", SqlDbType.VarChar,1000){Value=tableName},
					new SqlParameter("@PrimaryKey", SqlDbType.VarChar,100){Value=primaryKey},
					new SqlParameter("@Fields", SqlDbType.VarChar,1000){Value=fields},
					new SqlParameter("@PageSize", SqlDbType.Int){Value=pageSize},
					new SqlParameter("@CurrentPage", SqlDbType.Int){Value=currentPage},
					new SqlParameter("@Filter", SqlDbType.VarChar,1000){Value=filter},
					new SqlParameter("@Group", SqlDbType.VarChar,1000){Value=group},
                    new SqlParameter("@Order", SqlDbType.VarChar,200){Value=order},
                    new SqlParameter("@Total", SqlDbType.Int){Direction=ParameterDirection.Output}};



                DataTable tables = ExtProcPage("PageList", parameter, out total);
                return tables;
            }
        }
        public DataTable PageResult(string tableName, string pk, string fields, int pagesize, int pageindex, string filter, string group, string order, ref int rowcount)
        {
            SqlParameter[] parameter = new[]{
                    new SqlParameter("@TableNames", SqlDbType.VarChar,1000){Value=tableName},
                    new SqlParameter("@PrimaryKey", SqlDbType.VarChar,100){Value=pk},
                    new SqlParameter("@Fields", SqlDbType.VarChar,1000){Value=fields},
                    new SqlParameter("@PageSize", SqlDbType.Int){Value=pagesize},
                    new SqlParameter("@CurrentPage", SqlDbType.Int){Value=pageindex},
                    new SqlParameter("@Filter", SqlDbType.VarChar,1000){Value=filter},
                    new SqlParameter("@Group", SqlDbType.VarChar,1000){Value=group},
                    new SqlParameter("@Order", SqlDbType.VarChar,200){Value=order},
                    new SqlParameter("@RecordCount", SqlDbType.Int){Direction=ParameterDirection.Output}};
            string spName = "usp_PagingLarge";
            //var ds = ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, parameter);
            DataTable dt = ExtProcRe(spName, parameter);
            rowcount = Convert.ToInt32(parameter[8].Value);
            return dt;
        }
        //通用存储过程
        public DataTable Select(string tablename, string Field, string Condition)
        {
            Common.Expend.LogTxtExpend.WriteLogs("/Logs/Select_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", tablename + Field + Condition);
            try
            {
                string sql = "Game_Select";
                SqlParameter[] parameter = new[]
            {
                new SqlParameter("@tabName",SqlDbType.VarChar,2000),
                new SqlParameter("@field",SqlDbType.VarChar,1000),
                new SqlParameter("@strCondition",SqlDbType.VarChar,2000),
            };
                parameter[0].Value = tablename;
                parameter[1].Value = Field;
                parameter[2].Value = Condition;

                return ExtProc(sql, parameter);
            }
            catch
            {
                return null;
            }
        }
        public DataSet getbysp2(string procName, SqlParameter[] parameters)
        {

            lock (this)
            {
                try
                {
                    _mCommand.CommandType = CommandType.StoredProcedure;
                    _mCommand.CommandText = procName;
                    _mCommand.Parameters.Clear();
                    if (parameters != null)
                    {
                        foreach (SqlParameter parameter in parameters)
                        {
                            if (parameter != null)
                            {
                                // 检查未分配值的输出参数,将其分配以DBNull.Value.
                                if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Output) &&
                                    (parameter.Value == null))
                                {
                                    parameter.Value = DBNull.Value;
                                }
                                logstr += parameter.ParameterName + "   " + parameter.Value;
                                _mCommand.Parameters.Add(parameter);
                            }
                        }
                    }
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/getbysp2_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "ProName: " + procName + "logstr:" + logstr);
                    SqlDataAdapter adpater = new SqlDataAdapter(_mCommand);
                    DataSet ds = new DataSet();
                    adpater.Fill(ds);
                    adpater.Dispose();
                    return ds;
                }
                finally
                {
                    Restor();
                }
            }
        }


        /// <summary>
        /// 释放资源
        /// </summary>
        public new void Dispose()
        {
            base.Dispose();
            if (_mCommand != null)
            {
                _mCommand.Dispose();
                _mCommand = null;
            }
        }
    }
}

