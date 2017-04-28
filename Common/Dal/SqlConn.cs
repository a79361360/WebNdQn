using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FJSZ.OA.Common.DEncrypt;

namespace Common
{
    public class SqlConn : IDisposable
    {
        protected SqlConnection MSqlConn;

        /// <summary>
        /// 构造函数
        /// </summary>
        public SqlConn()
        {
            //string ll = FJSZ.OA.Common.DEncrypt.DEncrypt.Decrypt(System.Configuration.ConfigurationManager.ConnectionStrings["SQLConnString"].ConnectionString, "adc9ee659ca881f1c3096688fff9fc58");
            string ll = System.Configuration.ConfigurationManager.ConnectionStrings["SQLConnString"].ConnectionString;
            MSqlConn = new SqlConnection(ll);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (MSqlConn != null)
            {
                MSqlConn.Close();
                MSqlConn.Dispose();
            }
        }
    }
}
