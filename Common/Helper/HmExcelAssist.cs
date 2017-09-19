using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace Common.Helper
{
    /// <summary>
    /// Excel操作类
    /// </summary>
    public class HmExcelAssist
    {
        public static int DataTabletoExcel(System.Data.DataTable tmpDataTable, string strFileName)
        {
            Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "DataTabletoExcel 3_1_4_1 开始批量上传待充值用户记录,数据条数：" + tmpDataTable.Rows.Count + "文件的URL：" + strFileName);
            if (tmpDataTable == null)
                return -2;
            if (System.IO.File.Exists(strFileName))                                //存在则删除  
            {
                System.IO.File.Delete(strFileName);
            }
            int rowNum = tmpDataTable.Rows.Count;
            int columnNum = tmpDataTable.Columns.Count;
            int rowIndex = 1;
            int columnIndex = 0;
            Excel.Application xlApp = new Excel.Application();
            xlApp.DefaultFilePath = "";
            xlApp.DisplayAlerts = true;
            xlApp.SheetsInNewWorkbook = 1;
            Excel.Workbook xlBook = xlApp.Workbooks.Add(true);
            //将DataTable的列名导入Excel表第一行
            foreach (DataColumn dc in tmpDataTable.Columns)
            {
                columnIndex++;
                xlApp.Cells[rowIndex, columnIndex] = dc.ColumnName;
            }
            //将DataTable中的数据导入Excel中
            for (int i = 0; i < rowNum; i++)
            {
                rowIndex++;
                columnIndex = 0;
                for (int j = 0; j < columnNum; j++)
                {
                    columnIndex++;
                    xlApp.Cells[rowIndex, columnIndex] = tmpDataTable.Rows[i][j].ToString();
                }
            }
            //xlBook.SaveCopyAs(HttpUtility.UrlDecode(strFileName, System.Text.Encoding.UTF8));
            //xlBook.SaveCopyAs(strFileName);
            int result = 0;
            try {
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "DataTabletoExcel 3_1_4_2 准备开始保存Execl文件");
                xlBook.SaveAs(strFileName, Excel.XlFileFormat.xlExcel8, null, null, false, false, Excel.XlSaveAsAccessMode.xlNoChange, null, null, null, null, null);
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "DataTabletoExcel 3_1_4_3 保存结束Execl文件");
                result = 1;
            }
            catch (Exception er) {
                Common.Expend.LogTxtExpend.WriteLogs("/Logs/MobileBll_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "DataTabletoExcel 3_1_4_4 保存过程出现了异常。" + er.Message);
                result = -1;
            }
            xlBook.Close();
            return result;
        }
    }
}
