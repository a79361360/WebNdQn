using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class MySqlConn : IDisposable
    {
        protected MySqlConnection MSqlConn;
        /// <summary>
        /// 构造函数
        /// </summary>
        public MySqlConn()
        {
            //string ll = FJSZ.OA.Common.DEncrypt.DEncrypt.Decrypt(System.Configuration.ConfigurationManager.ConnectionStrings["DbConnString"].ConnectionString, "adc9ee659ca881f1c3096688fff9fc58");
            MSqlConn = new MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DbConnString"].ConnectionString);
        }
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
