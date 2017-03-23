using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Data.OleDb;
using System.Security.Cryptography.X509Certificates;

namespace FrameWork.Common
{
    /// <summary>
    /// Summary description for Util.
    /// </summary>
    public class Utils
    {
        public Utils()
        {
        }
        private static string[] ChineseNum = { "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖" };

        public static bool IsDate(string date)
        {
            try
            {
                DateTime.Parse(date);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsZipCode(string zip)
        {
            if (IsNaturalNumber(zip) && zip.Length == 6)
                return true;
            else
                return false;
        }

        public static bool IsMoney(string money)
        {
            try
            {
                Decimal.Parse(money);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool HasMoreRow(DataSet ds)
        {
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool HasMoreRow(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
                return false;
            else
                return true;
        }

        public static bool HasMoreRow(IDataReader dt)
        {
            if (dt == null)
                return false;
            else
                return true;
        }

        //整理字符串到安全格式
        public static string SafeFormat(string strInput)
        {
            return strInput.Trim().Replace("'", "''");
        }

        public static string ToSqlString(string paramStr)
        {
            return "'" + SafeFormat(paramStr) + "'";
        }

        public static string ToSqlLikeString(string paramStr)
        {
            return "'%" + SafeFormat(paramStr) + "%'";
        }
        public static string ToSqlLikeStringR(string paramStr)
        {
            return "'" + SafeFormat(paramStr) + "%'";
        }

        /// <summary>
        /// 左右模糊查询 使用SqlParameter时调用此方法
        /// </summary>
        /// <param name="paramStr"></param>
        /// <returns></returns>
        public static string ToSqlLikeStringForParamers(string paramStr)
        {
            return "%" + SafeFormat(paramStr) + "%";
        }
        /// <summary>
        /// 右模糊查询 使用SqlParameter时调用此方法
        /// </summary>
        /// <param name="paramStr"></param>
        /// <returns></returns>
        public static string ToSqlLikeStringRForParamers(string paramStr)
        {
            return SafeFormat(paramStr) + "%";
        }

        /// <summary>
        /// 就是一组数字或文字拼接到SQL文中的IN Clause中去。比如传入一个数组，得到拼接好的(a,b,c,d)之类的
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string ToSqlInString(ICollection list)
        {
            StringBuilder res = new StringBuilder();

            int i = 0;
            foreach (object o in list)
            {
                if (i != 0)
                    res.Append(',');

                res.Append(o.ToString());
                i++;
            }
            return "(" + res.ToString() + ")";
        }

        /// <summary>
        /// 传入的参数必须是'yyyy-MM-dd' 格式. 不另外检查
        /// </summary>
        /// <param name="paramDate"></param>
        /// <returns></returns>
        public static string ToSqlEndDate(string paramDate)
        {
            return ToSqlString(paramDate + " 23:59:59");
        }



        public static string GetMD5(String str)
        {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "md5").ToLower();
        }

        public static string MakeMD5(string str)
        {
            byte[] b = System.Text.Encoding.Default.GetBytes(str);
            b = new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(b);
            string ret = "";
            for (int i = 0; i < b.Length; i++)
            {
                ret += b[i].ToString("x").PadLeft(2, '0');
            }
            return ret;
        }

        // Function to test for Positive Integers.  
        public static bool IsNaturalNumber(String strNumber)
        {
            Regex objNotNaturalPattern = new Regex("[^0-9]");
            Regex objNaturalPattern = new Regex("0*[1-9][0-9]*");
            return !objNotNaturalPattern.IsMatch(strNumber) &&
                objNaturalPattern.IsMatch(strNumber);
        }

        // Function to test for Positive Integers with zero inclusive  

        public static bool IsWholeNumber(String strNumber)
        {
            Regex objNotWholePattern = new Regex("[^0-9]");
            return !objNotWholePattern.IsMatch(strNumber);
        }

        // Function to Test for Integers both Positive & Negative  

        public static bool IsInteger(String strNumber)
        {
            Regex objNotIntPattern = new Regex("[^0-9-]");
            Regex objIntPattern = new Regex("^-[0-9]+$|^[0-9]+$");
            return !objNotIntPattern.IsMatch(strNumber) && objIntPattern.IsMatch(strNumber);
        }

        // Function to Test for Positive Number both Integer & Real 

        public static bool IsPositiveNumber(String strNumber)
        {
            Regex objNotPositivePattern = new Regex("[^0-9.]");
            Regex objPositivePattern = new Regex("^[.][0-9]+$|[0-9]*[.]*[0-9]+$");
            Regex objTwoDotPattern = new Regex("[0-9]*[.][0-9]*[.][0-9]*");
            return !objNotPositivePattern.IsMatch(strNumber) &&
                objPositivePattern.IsMatch(strNumber) &&
                !objTwoDotPattern.IsMatch(strNumber);
        }

        // Function to test whether the string is valid number or not
        public static bool IsNumber(String strNumber)
        {
            Regex objNotNumberPattern = new Regex("[^0-9.-]");
            Regex objTwoDotPattern = new Regex("[0-9]*[.][0-9]*[.][0-9]*");
            Regex objTwoMinusPattern = new Regex("[0-9]*[-][0-9]*[-][0-9]*");
            String strValidRealPattern = "^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$";
            String strValidIntegerPattern = "^([-]|[0-9])[0-9]*$";
            Regex objNumberPattern = new Regex("(" + strValidRealPattern + ")|(" + strValidIntegerPattern + ")");
            return !objNotNumberPattern.IsMatch(strNumber) &&
                !objTwoDotPattern.IsMatch(strNumber) &&
                !objTwoMinusPattern.IsMatch(strNumber) &&
                objNumberPattern.IsMatch(strNumber);
        }

        // Function To test for Alphabets. 
        public static bool IsAlpha(String strToCheck)
        {
            Regex objAlphaPattern = new Regex("[^a-zA-Z]");
            return !objAlphaPattern.IsMatch(strToCheck);
        }
        public static bool IsChinese(String strToCheck)
        {
            Regex objAlphaPattern = new Regex("[\u4e00-\u9fa5]");
            return !objAlphaPattern.IsMatch(strToCheck);
        }
        // Function to Check for AlphaNumeric.
        public static bool IsAlphaNumeric(String strToCheck)
        {
            Regex objAlphaNumericPattern = new Regex("[^a-zA-Z0-9]");
            return !objAlphaNumericPattern.IsMatch(strToCheck);
        }
        public static bool IsEmailAddress(string strEmailAddress)
        {
            Regex objNotEmailAddress = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            return objNotEmailAddress.IsMatch(strEmailAddress);
        }
        // Function Format Money 
        public static decimal ToMoney(string moneyStr)
        {
            return decimal.Round(decimal.Parse(moneyStr), 2);
        }
        public static decimal ToMoney(decimal moneyValue)
        {
            return decimal.Round(moneyValue, 2);
        }
        /// <summary>
        /// 舍去金额的分,直接舍去,非四舍五入
        /// </summary>
        /// <param name="moneyValue"></param>
        /// <returns></returns>
        public static decimal TruncMoney(decimal moneyValue)
        {
            int tempAmt = Convert.ToInt32(moneyValue * 100) % 10;
            moneyValue = (decimal)((moneyValue * 100 - tempAmt) / 100m);
            return moneyValue;
        }

        /// <summary>
        /// 舍去小数位
        /// </summary>
        /// <param name="dec"></param>
        /// <returns></returns>
        public static int TruncDdecimal(decimal dec)
        {
            return Convert.ToInt32(Math.Floor(dec));
        }
        /// <summary>
        /// 正则验证
        /// </summary>
        /// <param name="Phone"></param>
        /// <returns></returns>
        public static bool IsRegularValidate(string str, string strRegex)
        {
            Regex objNotPhoneNumber = new Regex(strRegex);
            return objNotPhoneNumber.IsMatch(str);
        }
        /// <summary>
        /// 是否电话号码，区号-电话号码
        /// </summary>
        /// <param name="Phone"></param>
        /// <returns></returns>
        public static bool IsPhoneNumber(string Phone)
        {
            Regex objNotPhoneNumber = new Regex(@"^(0\d{2,3}-\d{7,8})");
            return objNotPhoneNumber.IsMatch(Phone);
        }

        /// <summary>
        /// 是否是小灵通，区号电话号码
        /// </summary>
        /// <param name="Phone"></param>
        /// <returns></returns>
        public static bool IsPasPhoneNumber(string Phone)
        {
            Regex objNotPhoneNumber = new Regex(@"^(0\d{3}\d{7,8})");
            return objNotPhoneNumber.IsMatch(Phone);
        }

        /// <summary>
        /// 判断是否手机号码
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public static bool IsCellNumber(string cell)
        {
            if (cell.Length != 11)
            {
                return false;
            }
            long number;
            try
            {
                number = Convert.ToInt64(cell);
                if (number < 13000000000)
                    return false;
                else if (number >= 19000000000)
                    return false;
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static void ToExcel(System.Web.UI.WebControls.DataGrid DataGrid2Excel, string FileName, string Title)
        {
            ToExcel(DataGrid2Excel, FileName, Title, "");
        }

        public static void ToExcel(DataSet ds, HttpResponse resp, string FileName)
        {
            resp.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            resp.AppendHeader("Content-Disposition", "attachment;filename=" + FileName);
            string colHeaders = "", ls_item = "";

            //定义表对象与行对象，同时用DataSet对其值进行初始化 
            DataTable dt = ds.Tables[0];
            DataRow[] myRow = dt.Select();//可以类似dt.Select("id>10")之形式达到数据筛选目的
            int i = 0;
            int cl = dt.Columns.Count;

            //取得数据表各列标题，各标题之间以t分割，最后一个列标题后加回车符 
            for (i = 0; i < cl; i++)
            {
                if (i == (cl - 1))//最后一列，加n
                {
                    colHeaders += dt.Columns[i].Caption.ToString() + "n";
                }
                else
                {
                    colHeaders += dt.Columns[i].Caption.ToString() + "t";
                }

            }
            resp.Write(colHeaders);
            //向HTTP输出流中写入取得的数据信息 

            //逐行处理数据   
            foreach (DataRow row in myRow)
            {
                //当前行数据写入HTTP输出流，并且置空ls_item以便下行数据     
                for (i = 0; i < cl; i++)
                {
                    if (i == (cl - 1))//最后一列，加n
                    {
                        ls_item += row[i].ToString() + "n";
                    }
                    else
                    {
                        ls_item += row[i].ToString() + "t";
                    }
                }
                resp.Write(ls_item);
                ls_item = "";
            }
            resp.End();
        }

        public static void ToExcel(System.Web.UI.WebControls.DataGrid DataGrid2Excel, string FileName, string Title, string Head)
        {
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment; filename=" + FileName + ".xls");  //filename=Report.xls		
            HttpContext.Current.Response.Charset = "GB2312"; //UTF-8 
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
            //msword;image/JPEG;text/HTML;image/GIF;vnd.ms-excel

            //HttpContext.Current.Application.Page.EnableViewState = false; //Turn off the view state.

            System.IO.StringWriter tw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);

            hw.WriteLine(@"<HTML>");
            hw.WriteLine(@"<BODY>");
            hw.WriteLine("<b>" + Title + "</b>");

            //string Head = @"<table border=1><tr><td>1</td><td>2</td></tr><tr><td>3</td><td>4</td></tr></table>";
            if (Head != "")
                hw.WriteLine(Head);
            DataGrid2Excel.RenderControl(hw); //Get the HTML for the control.
            hw.WriteLine(@"</BODY>");
            hw.WriteLine(@"</HTML>");
            hw.Flush();
            hw.Close();
            HttpContext.Current.Response.Write(tw.ToString()); //Write the HTML back to the browser.
            HttpContext.Current.Response.End();
        }
        public static void ToExcel(System.Web.UI.WebControls.GridView GridView2Excel, string FileName, string Title, string Head)
        {
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment; filename=" + FileName + ".xls");  //filename=Report.xls		
            HttpContext.Current.Response.Charset = "GB2312"; //UTF-8 
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
            //msword;image/JPEG;text/HTML;image/GIF;vnd.ms-excel

            //HttpContext.Current.Application.Page.EnableViewState = false; //Turn off the view state.

            System.IO.StringWriter tw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);

            hw.WriteLine(@"<HTML>");
            hw.WriteLine(@"<BODY>");
            hw.WriteLine("<b>" + Title + "</b>");

            //string Head = @"<table border=1><tr><td>1</td><td>2</td></tr><tr><td>3</td><td>4</td></tr></table>";
            if (Head != "")
                hw.WriteLine(Head);
            GridView2Excel.RenderControl(hw); //Get the HTML for the control.
            hw.WriteLine(@"</BODY>");
            hw.WriteLine(@"</HTML>");
            hw.Flush();
            hw.Close();
            HttpContext.Current.Response.Write(tw.ToString()); //Write the HTML back to the browser.
            HttpContext.Current.Response.End();
        }
        public static void ToExcelD(System.Web.UI.WebControls.DataGrid DataGrid2Excel, string FileName, string Title, string Head, string foot)
        {
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment; filename=" + FileName + ".xls");  //filename=Report.xls		
            HttpContext.Current.Response.Charset = "UTF-8"; //UTF-8 
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
            //msword;image/JPEG;text/HTML;image/GIF;vnd.ms-excel

            //HttpContext.Current.Application.Page.EnableViewState = false; //Turn off the view state.

            System.IO.StringWriter tw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);

            hw.WriteLine(@"<HTML>");
            hw.WriteLine(@"<BODY>");
            hw.WriteLine("<b>" + Title + "</b>");

            //string Head = @"<table border=1><tr><td>1</td><td>2</td></tr><tr><td>3</td><td>4</td></tr></table>";
            if (Head != "")
                hw.WriteLine(Head);

            DataGrid2Excel.RenderControl(hw); //Get the HTML for the control.
            if (foot != "")
                hw.WriteLine(foot);
            hw.WriteLine(@"</BODY>");
            hw.WriteLine(@"</HTML>");
            hw.Flush();
            hw.Close();
            HttpContext.Current.Response.Write(tw.ToString()); //Write the HTML back to the browser.
            HttpContext.Current.Response.End();
        }

        public static void ToExcelE(System.Web.UI.WebControls.DataGrid DataGrid2Excel, string FileName)
        {
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment; filename=" + FileName + ".xls");  //filename=Report.xls		
            HttpContext.Current.Response.Charset = "GB2312"; //UTF-8 
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";

            System.IO.StringWriter tw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);

            DataGrid2Excel.RenderControl(hw); //Get the HTML for the control.
            hw.Flush();
            hw.Close();
            HttpContext.Current.Response.Write(tw.ToString()); //Write the HTML back to the browser.
            HttpContext.Current.Response.End();
        }

        public static void ToExcelF(System.Web.UI.HtmlControls.HtmlTable Table1, string FileName, string Title, string Head, string foot)
        {
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment; filename=" + FileName + ".xls");  //filename=Report.xls		
            HttpContext.Current.Response.Charset = "UTF-8"; //UTF-8 
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
            //msword;image/JPEG;text/HTML;image/GIF;vnd.ms-excel

            //HttpContext.Current.Application.Page.EnableViewState = false; //Turn off the view state.

            System.IO.StringWriter tw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);

            hw.WriteLine(@"<HTML>");
            hw.WriteLine(@"<BODY>");
            hw.WriteLine("<b>" + Title + "</b>");

            //string Head = @"<table border=1><tr><td>1</td><td>2</td></tr><tr><td>3</td><td>4</td></tr></table>";
            if (Head != "")
                hw.WriteLine(Head);
            Table1.RenderControl(hw);

            if (foot != "")
                hw.WriteLine(foot);
            hw.WriteLine(@"</BODY>");
            hw.WriteLine(@"</HTML>");
            hw.Flush();
            hw.Close();
            HttpContext.Current.Response.Write(tw.ToString()); //Write the HTML back to the browser.
            HttpContext.Current.Response.End();
        }

        public static void ToExcelG(System.Web.UI.WebControls.DataGrid DataGrid2Excel, string FileName, string Title, string Head, string Foot)//add by zachary 2009-04-07 
        {
            System.IO.StringWriter sw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(sw);

            ToExcelFrontDecorator(hw);
            if (Title != "")
                hw.Write(Title + "<br>");
            if (Head != "")
                hw.Write(Head);

            DataGrid2Excel.EnableViewState = false;
            DataGrid2Excel.RenderControl(hw);

            if (Foot != "")
                hw.Write(Foot + "<br>");

            ToExelRearDecorator(hw);

            System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
            response.Clear();
            response.Buffer = true;
            response.ContentType = "application/vnd.ms-excel";
            response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(FileName) + ".xls");
            response.Charset = "UTF-8";
            response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            response.Write(sw.ToString());
            response.End();
        }

        /// <summary>
        /// Renders the html text before the datagrid.
        /// </summary>
        /// <param name="writer">A HtmlTextWriter to write html to output stream</param>
        private static void ToExcelFrontDecorator(HtmlTextWriter writer)
        {
            writer.WriteFullBeginTag("HTML");
            writer.WriteFullBeginTag("Head");
            //			writer.RenderBeginTag(HtmlTextWriterTag.Style);
            //			writer.Write("<!--");
            //			
            //			StreamReader sr = File.OpenText(CurrentPage.MapPath(MY_CSS_FILE));
            //			String input;
            //			while ((input=sr.ReadLine())!=null) 
            //			{
            //				writer.WriteLine(input);
            //			}
            //			sr.Close();
            //			writer.Write("-->");
            //			writer.RenderEndTag();
            writer.WriteEndTag("Head");
            writer.WriteFullBeginTag("Body");
        }

        /// <summary>
        /// Renders the html text after the datagrid.
        /// </summary>
        /// <param name="writer">A HtmlTextWriter to write html to output stream</param>
        private static void ToExelRearDecorator(HtmlTextWriter writer)
        {
            writer.WriteEndTag("Body");
            writer.WriteEndTag("HTML");
        }

        public static void ToTxtFile(System.Web.UI.WebControls.DataGrid DataGrid2Txt, string FileName, string ColumnIDs, bool AddID, int POSysNo)
        {
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment; filename=" + FileName + ".txt");  //filename=Report.xls		
            HttpContext.Current.Response.Charset = "GB2312";
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            HttpContext.Current.Response.ContentType = "application/vnd.ms-txt";

            System.IO.StringWriter tw = new System.IO.StringWriter();

            string[] Columns = ColumnIDs.Split(';');

            string sLine = "";
            if (AddID)
            {
                sLine = "\"ID\",";
            }
            for (int i = 0; i < Columns.Length; i++)
            {
                sLine += "\"" + DataGrid2Txt.Columns[Int32.Parse(Columns[i])].HeaderText + "\",";
            }
            sLine = sLine.Substring(0, sLine.Length - 1);

            tw.WriteLine(sLine);

            foreach (System.Web.UI.WebControls.DataGridItem item in DataGrid2Txt.Items)
            {
                if (AddID)
                {
                    sLine = "\"" + Convert.ToString(item.ItemIndex + 1) + "\",";
                }
                else
                {
                    sLine = "";
                }
                for (int i = 0; i < Columns.Length; i++)
                {
                    if (item.Cells[Int32.Parse(Columns[i])].Text.Trim().Length > 0)
                    {
                        sLine += "\"" + item.Cells[Int32.Parse(Columns[i])].Text.Trim().Replace("&nbsp;", "");
                        //用于在商品编号后面增加下划线和采购单号
                        if (i == 0)
                        {
                            sLine += "_" + POSysNo.ToString();
                        }
                        sLine += "\",";
                    }
                    else
                    {
                        sLine += "\"\",";
                    }
                }
                sLine = sLine.Substring(0, sLine.Length - 1);

                tw.WriteLine(sLine);
            }

            tw.Flush();
            tw.Close();
            HttpContext.Current.Response.Write(tw.ToString()); //Write the HTML back to the browser.
            HttpContext.Current.Response.End();
        }


        #region ChineseMoney
        private static string getSmallMoney(string moneyValue)
        {
            int intMoney = Convert.ToInt32(moneyValue);
            if (intMoney == 0)
            {
                return "";
            }
            string strMoney = intMoney.ToString();
            int temp;
            StringBuilder strBuf = new StringBuilder(10);
            if (strMoney.Length == 4)
            {
                temp = Convert.ToInt32(strMoney.Substring(0, 1));
                strMoney = strMoney.Substring(1, strMoney.Length - 1);
                strBuf.Append(ChineseNum[temp]);
                if (temp != 0)
                    strBuf.Append("仟");
            }
            if (strMoney.Length == 3)
            {
                temp = Convert.ToInt32(strMoney.Substring(0, 1));
                strMoney = strMoney.Substring(1, strMoney.Length - 1);
                strBuf.Append(ChineseNum[temp]);
                if (temp != 0)
                    strBuf.Append("佰");
            }
            if (strMoney.Length == 2)
            {
                temp = Convert.ToInt32(strMoney.Substring(0, 1));
                strMoney = strMoney.Substring(1, strMoney.Length - 1);
                strBuf.Append(ChineseNum[temp]);
                if (temp != 0)
                    strBuf.Append("拾");
            }
            if (strMoney.Length == 1)
            {
                temp = Convert.ToInt32(strMoney);
                strBuf.Append(ChineseNum[temp]);
            }
            return strBuf.ToString();
        }

        public static string GetChineseMoney(decimal moneyValue)
        {
            if (moneyValue < 0)
            {
                moneyValue *= -1;
            }
            int intMoney = Convert.ToInt32(TruncMoney(moneyValue) * 100);
            string strMoney = intMoney.ToString();
            int moneyLength = strMoney.Length;
            StringBuilder strBuf = new StringBuilder(100);
            if (moneyLength > 14)
            {
                throw new Exception("金额过大，无法转换大写");
            }

            //处理亿部分
            if (moneyLength > 10 && moneyLength <= 14)
            {
                strBuf.Append(getSmallMoney(strMoney.Substring(0, strMoney.Length - 10)));
                strMoney = strMoney.Substring(strMoney.Length - 10, 10);
                strBuf.Append("亿");
            }

            //处理万部分
            if (moneyLength > 6)
            {
                strBuf.Append(getSmallMoney(strMoney.Substring(0, strMoney.Length - 6)));
                strMoney = strMoney.Substring(strMoney.Length - 6, 6);
                strBuf.Append("万");
            }

            //处理元部分
            if (moneyLength > 2)
            {
                strBuf.Append(getSmallMoney(strMoney.Substring(0, strMoney.Length - 2)));
                strMoney = strMoney.Substring(strMoney.Length - 2, 2);
                strBuf.Append("元");
            }

            //处理角、分处理分
            if (Convert.ToInt32(strMoney) == 0)
            {
                strBuf.Append("整");
            }
            else
            {
                int intJiao = Convert.ToInt32(strMoney.Substring(0, 1));
                strBuf.Append(ChineseNum[intJiao]);
                if (intJiao != 0)
                {
                    strBuf.Append("角");
                }
                int intFen = Convert.ToInt32(strMoney.Substring(1, 1));
                if (intFen != 0)
                {
                    strBuf.Append(ChineseNum[intFen]);
                    strBuf.Append("分");
                }
            }
            string temp = strBuf.ToString();
            while (temp.IndexOf("零零") != -1)
            {
                strBuf.Replace("零零", "零");
                temp = strBuf.ToString();
            }

            strBuf.Replace("零亿", "亿");
            strBuf.Replace("零万", "万");
            strBuf.Replace("亿万", "亿");
            return strBuf.ToString();
        }
        #endregion

        public static string RemoveHtmlTag(string str)
        {
            Regex reg = new Regex(@"<\/*[^<>]*>");
            return reg.Replace(str, "");
        }

        /// <summary>
        /// 计算两个日期的时间间隔,返回天数
        /// </summary>
        /// <param name="DateTime1">第一个日期和时间</param>
        /// <param name="DateTime2">第二个日期和时间</param>
        /// <returns></returns>
        public static int DateDiff(DateTime DateTime1, DateTime DateTime2)
        {
            DateTime1 = Convert.ToDateTime(DateTime1.ToString("yyyy-MM-dd"));
            DateTime2 = Convert.ToDateTime(DateTime2.ToString("yyyy-MM-dd"));

            TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
            TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
            TimeSpan ts = ts1.Subtract(ts2).Duration();

            return ts.Days;
        }


        //通过nslookup程序查询MX记录，获取域名对应的mail服务器
        private static string GetMailServer(string strEmail)
        {
            string strDomain = strEmail.Split('@')[1];
            ProcessStartInfo info = new ProcessStartInfo();
            info.UseShellExecute = false;
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = true;
            info.FileName = "nslookup";
            info.CreateNoWindow = true;
            info.Arguments = "-type=mx " + strDomain;
            Process ns = Process.Start(info);
            StreamReader sout = ns.StandardOutput;
            Regex reg = new Regex("mail exchanger = (?<mailServer>[^\\s]+)");
            string strResponse = "";
            while ((strResponse = sout.ReadLine()) != null)
            {
                Match amatch = reg.Match(strResponse);
                if (reg.Match(strResponse).Success)
                    return amatch.Groups["mailServer"].Value;
            }
            return null;
        }

        //连接邮件服务器，确认服务器的可用性和用户是否存在
        /// <summary>
        /// return 200 = valid email address
        /// </summary>
        /// <param name="mailAddress"></param>
        /// <returns></returns>
        public static int CheckEmail(string mailAddress)
        {
            Regex reg = new Regex("^[a-zA-Z0-9_-]+@([a-zA-Z0-9-]+\\.){1,}(com|net|edu|miz|biz|cn|cc)$");

            if (!reg.IsMatch(mailAddress))
                return 405;//Email地址形式上就不对

            string mailServer = GetMailServer(mailAddress);
            if (mailServer == null)
            {
                return 404; //邮件服务器探测错误
            }
            TcpClient tcpc = new TcpClient();
            tcpc.NoDelay = true;
            tcpc.ReceiveTimeout = 3000;
            tcpc.SendTimeout = 3000;
            try
            {
                tcpc.Connect(mailServer, 25);
                NetworkStream s = tcpc.GetStream();
                StreamReader sr = new StreamReader(s, Encoding.Default);
                StreamWriter sw = new StreamWriter(s, Encoding.Default);
                string strResponse = "";
                string strTestFrom = "service@baby1one.com.cn";
                sw.WriteLine("helo " + mailServer);
                sw.WriteLine("mail from:<" + mailAddress + ">");
                sw.WriteLine("rcpt to:<" + strTestFrom + ">");
                strResponse = sr.ReadLine();
                if (!strResponse.StartsWith("2")) return 403; //用户名有误
                sw.WriteLine("quit");
                return 200; //Email地址检查无误
            }
            catch
            {
                return 403;//发生错误或邮件服务器不可达
            }
        }

        public static string GetWeekName(int id)
        {
            string name = "";
            switch (id)
            {
                case 1:
                    name = "星期一";
                    break;
                case 2:
                    name = "星期二";
                    break;
                case 3:
                    name = "星期三";
                    break;
                case 4:
                    name = "星期四";
                    break;
                case 5:
                    name = "星期五";
                    break;
                case 6:
                    name = "星期六";
                    break;
                case 7:
                    name = "星期日";
                    break;
            }
            return name;
        }

        public static int GetWeekID(DayOfWeek week)
        {
            int id = 0;
            switch (week)
            {
                case DayOfWeek.Monday:
                    id = 1;
                    break;
                case DayOfWeek.Tuesday:
                    id = 2;
                    break;
                case DayOfWeek.Wednesday:
                    id = 3;
                    break;
                case DayOfWeek.Thursday:
                    id = 4;
                    break;
                case DayOfWeek.Friday:
                    id = 5;
                    break;
                case DayOfWeek.Saturday:
                    id = 6;
                    break;
                case DayOfWeek.Sunday:
                    id = 7;
                    break;
            }
            return id;
        }

        /// <summary>
        /// 计算文件的绝对路径, 在类库中调用，一般是不能使用Server.MapPath的时候
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetAbsoluteFilePath(string filePath)
        {
            string file = filePath;
            if (!filePath.Substring(1, 1).Equals(":")
                && !filePath.StartsWith("\\"))
            {
                file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);
            }

            return file.Replace("/", "\\");
        }



        /*/// <summary>
        /// 返回1，说明发送成功
        /// </summary>
        /// <returns></returns>
        public static int SendSMSMessage(string CellNumber,string SMSContent )
        {
            //通过移通网络短信平台发送
            string command = "MT_REQUEST";
            string spid = "0000"; 
            string sppassword = ""; 
            string da = "86" + CellNumber;
            string dc = "15"; //GBK编码
            string sm = "";

            Encoding gb = Encoding.GetEncoding("gbk");
            byte[] bytes = gb.GetBytes(SMSContent);
            for (int i = 0; i < bytes.Length; i++)
            {
                sm += Convert.ToString(bytes[i], 16);
            }

            string url = "http://esms1.etonenet.com/sms/mt?command=" + command + "&spid=" + spid + "&sppassword=" + sppassword + "&da=" + da + "&dc=" + dc + "&sm=" + sm;
            System.Net.WebClient wc = new System.Net.WebClient();
            Stream stream = wc.OpenRead(url);

            StreamReader reader = new StreamReader(stream);
            string result = reader.ReadToEnd();
            stream.Close();
            reader.Close();
            wc.Dispose();
            if (result.IndexOf("mterrcode=000") > 0)
                return 1;
            else
                return 0;
        }*/

        public static string FilterCompetitorKeyword(string input)
        {
            return input.Replace("京东", " xx ").Replace("新蛋", " xx ").Replace("某东", " xx ").Replace("某蛋", " xx ").Replace("*东", " xx ").Replace("*蛋", " xx ");
        }

        public decimal Round(decimal x, int len)
        {
            return Decimal.Round(x + 0.000001m, len);
        }

        //显示提示页面
        public static string ShowAlertStr(string paramUrl, string paramBackTitle, string paramTitle, string paramContent, bool paramHaveBtn, string InfoType)
        {
            string _return = "";
            string _strBool = "0";

            if (InfoType == "error")
                paramContent = "<font color=red>" + paramContent + "</font>";

            if (paramHaveBtn) _strBool = "1";
            //_return = "<script language='javascript' type='text/javascript'>parent.document.all.titleFrame.src='SiteMapPath.aspx?url="+paramTitle+"';window.open('../basic/SaveOK.aspx?Url=" + paramUrl.Trim() + "&Content=" + paramContent.Trim() + "&HaveBtn=" + _strBool + "&BackTitle=" + paramBackTitle + "&time=" + DateTime.Now.ToString() + "','_self')</script>";
            _return = "<script language='javascript' type='text/javascript'>window.open('../basic/SaveOK.aspx?Url=" + paramUrl.Trim() + "&Content=" + paramContent.Trim() + "&HaveBtn=" + _strBool + "&BackTitle=" + paramBackTitle + "&time=" + DateTime.Now.ToString() + "','_self')</script>";
            return _return;
        }


        /// <summary>
        /// 获取IP信息
        /// </summary>
        /// <returns></returns>
        public static string GetIP()
        {
            string result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(result))
                result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            if (string.IsNullOrEmpty(result))
                result = HttpContext.Current.Request.UserHostAddress;

            if (string.IsNullOrEmpty(result))
                return "127.0.0.1";

            return result;
        }
        public static int DecimalToU_Int32(decimal argument)
        {
            object Int32Value;
            object UInt32Value;

            // Convert the argument to an int value.
            try
            {
                Int32Value = (int)argument;
            }
            catch (Exception ex)
            {
                Int32Value = GetExceptionType(ex);
            }

            // Convert the argument to a uint value.
            try
            {
                UInt32Value = (uint)argument;
            }
            catch (Exception ex)
            {
                UInt32Value = GetExceptionType(ex);
            }

            return Int32.Parse(UInt32Value.ToString());
        }

        private static string GetExceptionType(Exception ex)
        {
            string exceptionType = ex.GetType().ToString();
            return exceptionType.Substring(exceptionType.LastIndexOf('.') + 1);
        }

        public static string TrimString(string s, int len, string strLen)
        {
            string _s;
            if (s.Length > len)
            {
                _s = s.Substring(0, len - 1) + strLen;
            }
            else
            {
                _s = s;
            }

            return _s;
        }

        /// <summary>
        /// decimal >= 0。空返回false
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsDecimalNoLessThanZero(string str)
        {
            if (str == String.Empty)
                return false;
            try
            {
                decimal tmp = Decimal.Parse(str);
                if (tmp >= 0)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }

        }

        /// <summary>
        /// decimal > 0。空返回false
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsDecimalGreaterThanZero(string str)
        {
            if (str == String.Empty)
                return false;
            try
            {
                decimal tmp = Decimal.Parse(str);
                if (tmp > 0)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }

        }

        /// <summary>
        /// Int > 0 ，空返回false
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsIntGreaterThanZero(string str)
        {
            if (str == string.Empty)
                return false;
            try
            {
                int tmp = Convert.ToInt32(str);
                if (tmp > 0)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Int >= 0。空返回false
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsIntNoLessThanZero(string str)
        {
            if (str == String.Empty)
                return false;
            try
            {
                int tmp = Convert.ToInt32(str);
                if (tmp >= 0)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        public static string getShowLoginPopFrame()
        {
            string js = "<script type=\"text/javascript\">\n";
            js += "javascript:setPopFrameUrl('PopLogin.aspx?timestamp=" + DateTime.Now.ToString() + "',480,360,'登录');";
            js += "\n";
            js += "</script>";
            return js;
        }

        #region Base64 加解密
        public static string EncodeBase64(string source)
        {
            try
            {
                byte[] bytes_1 = Encoding.Default.GetBytes(source);
                return Convert.ToBase64String(bytes_1).Replace("+", " ");
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string DecodeBase64(string result)
        {
            try
            {
                byte[] bytes_2 = Convert.FromBase64String(result.Replace(" ", "+"));
                return Encoding.Default.GetString(bytes_2);
            }
            catch
            {
                return string.Empty;
            }
        }
        #endregion

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="page"></param>
        /// <param name="filePath"></param>
        public static void DownloadExcel(Page page, string filePath)
        {
            filePath = page.Server.MapPath(filePath);//路径
            FileInfo fileInfo = new FileInfo(filePath);

            if (fileInfo.Exists)
            {
                page.Response.Clear();
                page.Response.ClearContent();
                page.Response.ClearHeaders();
                page.Response.AddHeader("Content-Disposition", "attachment;filename=" + fileInfo.Name);
                page.Response.AddHeader("Content-Length", fileInfo.Length.ToString());
                page.Response.AddHeader("Content-Transfer-Encoding", "binary");
                page.Response.ContentType = "application/octet-stream";
                page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
                page.Response.WriteFile(fileInfo.FullName);
                page.Response.Flush();
                page.Response.End();
            }
            else
            {
                throw new ApplicationException("下载文件失败，没有找到下载文件！请联系管理员！");
            }
        }
        /// <summary>
        /// 消除是否有Sql危险字符
        /// </summary>
        /// <returns>True:存在</returns>
        public static string SafeReplace(string strInput)
        {
            return strInput.Trim().Replace("-", "—").Replace(",", "，").Replace("'", "’"); ;
        }


        /// <summary>
        /// 获得当前绝对路径
        /// </summary>
        /// <param name="strPath">指定的路径</param>
        /// <returns>绝对路径</returns>
        public static string GetMapPath(string strPath)
        {
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Server.MapPath(strPath);
            }
            else //非web程序引用
            {
                strPath = strPath.Replace("/", "\\");
                if (strPath.StartsWith("\\"))
                {
                    strPath = strPath.Substring(strPath.IndexOf('\\', 1)).TrimStart('\\');
                }
                return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strPath);
            }
        }
        /// <summary>
        /// 把Excel转换成DataSet
        /// </summary>
        /// <param name="Path">Excel上传服务器的路径</param>
        /// <returns>Excel转换DataSet</returns>
        public static DataSet ExcelToDS(string Path)
        {
            string strConn = "Provider=Microsoft.Jet.OleDb.4.0;Data Source='" + Path + "';Extended Properties='Excel 8.0;HDR=YES'";
            OleDbConnection conn = new OleDbConnection(strConn);
            conn.Open();
            string strExcel = "";
            OleDbDataAdapter da = null;
            DataSet ds = null;
            strExcel = "select * from [sheet1$]";
            da = new OleDbDataAdapter(strExcel, strConn);
            ds = new DataSet();
            da.Fill(ds);
            conn.Close();
            return ds;
        }
        public static string GetSub(string str,int length,string lenstr)
        {
            if (!string.IsNullOrEmpty(str))
            {
                if (str.Length > length)
                {
                    str = str.Substring(0, length) + lenstr;
                }
            }
            return str;
        }
        #region 显示分页
        /// <summary>
        /// 返回分页页码
        /// </summary>
        /// <param name="pageSize">页面大小</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="totalCount">总记录数</param>
        /// <param name="function">js分页方法</param>
        /// <param name="centSize">中间页码数量</param>
        /// <returns></returns>
        public static string OutPageList(int pageSize, int pageIndex, int totalCount, string function, int centSize)
        {
            //计算页数
            if (totalCount < 1 || pageSize < 1)
            {
                return "";
            }
            int pageCount = totalCount / pageSize;
            if (pageCount < 1)
            {
                return "";
            }
            if (totalCount % pageSize > 0)
            {
                pageCount += 1;
            }
            if (pageCount <= 1)
            {
                return "";
            }
            StringBuilder pageStr = new StringBuilder();
            string indexBtn = "<a href=\"javascript:" + function + (pageIndex - 1).ToString() + ")\" class='pagerindex'></a>\n";
            string firstBtn = "<a href=\"javascript:" + function + (pageIndex - 1).ToString() + ")\" class='pagerpre'></a>\n";
            string lastBtn = "<a href=\"javascript:" + function + (pageIndex + 1).ToString() + ")\" class='pagernext'></a>\n";
            string endBtn = "<a href=\"javascript:" + function + (pageIndex + 1).ToString() + ")\" class='pagerlast'></a>\n";
            string firstStr = "<a href=\"javascript:" + function + "1)\">1</a>\n";
            string lastStr = "<a href=\"javascript:" + function + pageCount + ")\">" + pageCount.ToString() + "</a>\n";
            if (pageIndex <= 1)
            {
                indexBtn = "<a disabled=\"disabled\" class='pagerindex'></a>\n";
                firstBtn = "<a disabled=\"disabled\" class=\"pagerpre\"></a>\n";
            }
            if (pageIndex >= pageCount)
            {
                lastBtn = "<a disabled=\"disabled\" class=\"pagernext\"></a>\n";
                endBtn = "<a disabled=\"disabled\" class=\"pagerlast\"></a>\n";
            }
            if (pageIndex == 1)
            {
                firstStr = "<span>1</span>\n";
            }
            if (pageIndex == pageCount)
            {
                lastStr = "<span>" + pageCount.ToString() + "</span>\n";
            }
            int firstNum = pageIndex - (centSize / 2); //中间开始的页码
            if (pageIndex < centSize)
                firstNum = 2;
            int lastNum = pageIndex + centSize - ((centSize / 2) + 1); //中间结束的页码
            if (lastNum >= pageCount)
                lastNum = pageCount - 1;
            pageStr.Append(indexBtn + firstBtn + firstStr);
            if (pageIndex >= centSize)
            {
                pageStr.Append("<span>...</span>\n");
            }
            for (int i = firstNum; i <= lastNum; i++)
            {
                if (i == pageIndex)
                {
                    pageStr.Append("<span>" + i + "</span>\n");
                }
                else
                {
                    pageStr.Append("<a href=\"javascript:" + function + i + ")\">" + i + "</a>\n");
                }
            }
            if (pageCount - pageIndex > centSize - ((centSize / 2)))
            {
                pageStr.Append("<span>...</span>\n");
            }
            pageStr.Append(lastStr + lastBtn + endBtn);
            return pageStr.ToString();
        }
        public static string OutPageTotal(int pageSize, int PageIndex, int totalCount)
        {
            int PageTotal = totalCount / pageSize; // 页数合计
            if (totalCount%pageSize!=0)
            {
                PageTotal = PageTotal + 1;
            }
            PageTotal = PageTotal <= 0 ? 1 : PageTotal;
            string str = "共找到" + totalCount + "条记录  当前第" + PageIndex + "页[共" + PageTotal + "页]";
            if (totalCount==0)
            {
                str = "";
            }
            return str;
        }
        public static string OutPageListUrl(int pageSize, int pageIndex, int totalCount, string linkUrl, int centSize)
        {
            //计算页数
            if (totalCount < 1 || pageSize < 1)
            {
                return "";
            }
            int pageCount = totalCount / pageSize;
            if (pageCount < 1)
            {
                return "";
            }
            if (totalCount % pageSize > 0)
            {
                pageCount += 1;
            }
            if (pageCount <= 1)
            {
                return "";
            }
            StringBuilder pageStr = new StringBuilder();
            string pageId = "__id__";
            string firstBtn = "<li><a href=\"" + ReplaceStr(linkUrl, pageId, (pageIndex - 1).ToString()) + "\">上一页</a></li>";
            string lastBtn = "<li><a href=\"" + ReplaceStr(linkUrl, pageId, (pageIndex + 1).ToString()) + "\">下一页</a></li>";
            string firstStr = "<li><a href=\"" + ReplaceStr(linkUrl, pageId, "1") + "\">1</a>";
            string lastStr = "<li><a href=\"" + ReplaceStr(linkUrl, pageId, pageCount.ToString()) + "\">" + pageCount.ToString() + "</a></li>";
            if (pageIndex <= 1)
            {
                firstBtn = "<li class=\"disabled\"><a href=\"javascript:;\">上一页</a></li>";
            }
            if (pageIndex >= pageCount)
            {
                lastBtn = "<li class=\"disabled\"><a href=\"javascript:;\">下一页</a></li>";
            }
            if (pageIndex == 1)
            {
                firstStr = "<li class=\"active\"> <a href=\"javascript:;\">1</a> </li>";
            }
            if (pageIndex == pageCount)
            {
                lastStr = "<li class=\"active\"> <a href=\"javascript:;\">" + pageCount.ToString() + "</a> </li>";
            }
            int firstNum = pageIndex - (centSize / 2); //中间开始的页码
            if (pageIndex < centSize)
                firstNum = 2;
            int lastNum = pageIndex + centSize - ((centSize / 2) + 1); //中间结束的页码
            if (lastNum >= pageCount)
                lastNum = pageCount - 1;
            pageStr.Append("<ul>");
            pageStr.Append(firstBtn + firstStr);
            //if (pageIndex >= centSize)
            //{
            //    pageStr.Append("<span>...</span>\n");
            //}
            //<ul>
            //                    <li class="disabled"><a href="#">上一页</a></li>
            //                    <li class="active"> <a href="#">1</a> </li>
            //                    <li><a href="#">2</a></li>
            //                    <li><a href="#">3</a></li>
            //                    <li><a href="#">4</a></li>
            //                    <li><a href="#">下一页</a></li>
            //                </ul>
            for (int i = firstNum; i <= lastNum; i++)
            {
                if (i == pageIndex)
                {
                    pageStr.Append("<li class=\"disabled\"><a href=\"javascript:;\">" + i + "</a></li>");
                }
                else
                {
                    pageStr.Append("<li><a href=\"" + ReplaceStr(linkUrl, pageId, i.ToString()) + "\">" + i + "</a></li>");
                }
            }
            //if (pageCount - pageIndex > centSize - ((centSize / 2)))
            //{
            //    pageStr.Append("<span>...</span>");
            //}
            pageStr.Append(lastStr + lastBtn);
            pageStr.Append("</ul>");
            return pageStr.ToString();
        }
        #endregion
        #region 替换指定的字符串
        /// <summary>
        /// 替换指定的字符串
        /// </summary>
        /// <param name="originalStr">原字符串</param>
        /// <param name="oldStr">旧字符串</param>
        /// <param name="newStr">新字符串</param>
        /// <returns></returns>
        public static string ReplaceStr(string originalStr, string oldStr, string newStr)
        {
            if (string.IsNullOrEmpty(oldStr))
            {
                return "";
            }
            return originalStr.Replace(oldStr, newStr);
        }
        #endregion
        #region URL处理
        /// <summary>
        /// URL字符编码
        /// </summary>
        public static string UrlEncode(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }
            str = str.Replace("'", "");
            return HttpContext.Current.Server.UrlEncode(str);
        }

        /// <summary>
        /// URL字符解码
        /// </summary>
        public static string UrlDecode(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }
            return HttpContext.Current.Server.UrlDecode(str);
        }
        /// <summary>
        /// 组合URL参数
        /// </summary>
        /// <param name="_url">页面地址</param>
        /// <param name="_keys">参数名称</param>
        /// <param name="_values">参数值</param>
        /// <returns>String</returns>
        public static string CombUrlTxt(string _url, string _keys, params string[] _values)
        {
            StringBuilder urlParams = new StringBuilder();
            try
            {
                string[] keyArr = _keys.Split(new char[] { '&' });
                for (int i = 0; i < keyArr.Length; i++)
                {
                    if (!string.IsNullOrEmpty(_values[i]))
                    {
                        _values[i] = UrlEncode(_values[i]);
                        urlParams.Append(string.Format(keyArr[i], _values) + "&");
                    }
                }
                if (!string.IsNullOrEmpty(urlParams.ToString()) && _url.IndexOf("?") == -1)
                    urlParams.Insert(0, "?");
            }
            catch
            {
                return _url;
            }
            return _url + DelLastChar(urlParams.ToString(), "&");
        }
        #endregion
        #region 删除最后结尾的指定字符后的字符
        /// <summary>
        /// 删除最后结尾的指定字符后的字符
        /// </summary>
        public static string DelLastChar(string str, string strchar)
        {
            if (string.IsNullOrEmpty(str))
                return "";
            if (str.LastIndexOf(strchar) >= 0 && str.LastIndexOf(strchar) == str.Length - 1)
            {
                return str.Substring(0, str.LastIndexOf(strchar));
            }
            return str;
        }
        #endregion

        public static bool ValidateServerCertificate(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        /// <summary>
        /// 验证是否为数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNumeric(string str)
        {
            if (str == null || str.Length == 0)
                return false;
            foreach (char c in str)
            {
                if (!Char.IsNumber(c))
                {
                    return false;
                }
            }
            return true; 
        }
        /// <summary>
        /// 获取支付宝POST过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        public static SortedDictionary<string, string> GetRequestPost()
        {
            int i = 0;
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
            NameValueCollection coll;
            coll = HttpContext.Current.Request.Form;
            String[] requestItem = coll.AllKeys;
            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], HttpContext.Current.Request.Form[requestItem[i]]);
            }
            return sArray;
        }
        /// <summary>
        /// 获取支付宝GET过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        public static SortedDictionary<string, string> GetRequestGet()
        {
            int i = 0;
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
            NameValueCollection coll;
            //Load Form variables into NameValueCollection variable.
            coll = HttpContext.Current.Request.QueryString;

            // Get names of all forms into a string array.
            String[] requestItem = coll.AllKeys;

            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], HttpContext.Current.Request.QueryString[requestItem[i]]);
            }

            return sArray;
        }

    }
}