using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameWork
{
    public class UIDataGrid
    {
        public UIDataGrid(int total, dynamic rows)
        {
            this.total = total;
            this.rows = rows;
        }
        /// <summary>
        /// 总数
        /// </summary>
        public int total;
        /// <summary>
        /// 行数据集
        /// </summary>
        public dynamic rows;
        /// <summary>
        /// 列数据集
        /// </summary>
        public dynamic cols;
    }
}
