using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface IDalInterFace : IDisposable
    {
        /// <summary>
        /// 执行存储过程返回DataTabel对象
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameter">调用存储过程参数对象</param>
        /// <returns></returns>
        DataTable ExtProc(String procName, object[] parameter);

        /// <summary>
        /// 执行存储过程返回DataTable对象的同时返回一个键值对对象
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="parameter"></param>
        /// <param name="retFiled"> </param>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        DataTable ExtProc(String procName, object[] parameter, string[] retFiled, out Dictionary<string, object> dictionary);

        /// <summary>
        /// 执行存储过程返回受影响的行数
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameter">调用存储过程参数对象 </param>
        int NoExtProc(String procName, object[] parameter);

        /// <summary>
        /// 执行存储过程返回键值对对象
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameter">调用存储的参数</param>
        /// <param name="retFiled">要返回字段名</param>
        /// <returns></returns>
        Dictionary<string, object> ExtProc(String procName, object[] parameter, string[] retFiled);

        /// <summary>
        /// 执行查询语句返回表对象
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        DataTable ExtSql(String sql);

        /// <summary>
        /// 执行查询语句返回受影响的行数
        /// </summary>
        /// <param name="Sql"></param>
        /// <returns></returns>
        int IntExtSql(String Sql);

        /// <summary>
        /// 批量存储数据
        /// </summary>
        /// <param name="tabName"></param>
        /// <param name="dataTable"></param>
        void SQLBulkCopy(string tabName, DataTable dataTable);

        /// <summary>
        /// 批量存储数据
        /// </summary>
        /// <param name="tabName"></param>
        /// <param name="dataTable"></param>
        /// <param name="rowsCounts"></param>
        void SQLBulkCopy(string tabName, DataTable dataTable, int rowsCounts);

        /// <summary>
        /// 批量更新数据
        /// </summary>
        /// <param name="table"></param>
        /// <param name="tbName"> </param>
        void UpdataSource(DataTable table, string tbName);

        /// <summary>
        /// 更新表
        /// </summary>
        /// <param name="adapter"></param>
        /// <param name="table"></param>
        /// <param name="tbName"> </param>
        void BatchUpdate(SqlDataAdapter adapter, DataTable table, string tbName);
    }
}
