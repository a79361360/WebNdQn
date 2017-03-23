using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace FrameWork.Common
{
    public static class Verifier
    {
        #region 正则表达式
        //邮政编码
        private static Regex RegPostCode = new Regex("^\\d{6}$");
        //中国身份证验证
        private static Regex RegCardID = new Regex("^\\d{17}[\\d|X]|\\d{15}|\\d{18}$");
        //数字
        private static Regex RegNumber = new Regex("^\\d+$");
        //固定电话
        private static Regex RegTel = new Regex("^\\d{3,4}-\\d{7,8}|\\d{7,8}$");
        //手机号
        private static Regex RegPhone = new Regex("^[1][3-8]\\d{9}$");
        //电话号码（包括固定电话和手机号）
        private static Regex RegTelePhone = new Regex("^(\\d{3,4}-\\d{7,8}|\\d{7,8})|([1][3-8]\\d{9})$");
        //邮箱
        private static Regex RegEmail = new Regex("^[\\w-]+@[\\w-]+\\.(com|net|org|edu|mil|tv|biz|info)$");
        //中文
        private static Regex RegCHZN = new Regex("[\u4e00-\u9fa5]");
        //IP地址
        private static Regex RegIP = new Regex("((25[0-5]|2[0-4]\\d|1?\\d?\\d)\\.){3}(25[0-5]|2[0-4]\\d|1?\\d?\\d)");
        #endregion

        
        /// <summary>
        ///  判断是否是数字
        /// </summary>
        /// <param name="inputValue">输入值</param>
        /// <returns></returns>
        public static bool IsInt(this object inputValue)
        {
            int num;
            return int.TryParse(inputValue.ToStringValue(), out num);
        }
        /// <summary>
        ///  判断是否是小数
        /// </summary>
        /// <param name="inputValue">输入值</param>
        /// <returns></returns>
        public static bool IsDouble(this object inputValue)
        {
            Double dValue;
            return Double.TryParse(inputValue.ToStringValue(), out dValue);
        }
        /// <summary>
        ///  判断是否是小数
        /// </summary>
        /// <param name="inputValue">输入值</param>
        /// <returns></returns>
        public static bool IsFloat(this object inputValue)
        {
            float fValue;
            return float.TryParse(inputValue.ToStringValue(), out fValue);
        }
        /// <summary>
        ///  判断字符串是否为空
        ///  空：true，不为空：false
        /// </summary>
        /// <param name="inputValue">输入值</param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this object inputValue)
        {
            if (string.IsNullOrEmpty(inputValue.ToStringValue()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        ///  判断字符串是否为Email
        /// </summary>
        /// <param name="inputValue">输入值</param>
        /// <returns></returns>
        public static bool IsEmail(this object inputValue)
        {
            System.Text.RegularExpressions.Match match = RegEmail.Match(inputValue.ToStringValue());
            return match.Success;
        }
        /// <summary>
        ///  判断字符串是否为固话
        /// </summary>
        /// <param name="inputValue">输入值</param>
        /// <returns></returns>
        public static bool IsTel(this object inputValue)
        {
            System.Text.RegularExpressions.Match match = RegTel.Match(inputValue.ToStringValue());
            return match.Success;
        }
        /// <summary>
        ///  判断字符串是否为手机号
        /// </summary>
        /// <param name="inputValue">输入值</param>
        /// <returns></returns>
        public static bool IsPhone(this object inputValue)
        {
            System.Text.RegularExpressions.Match match = RegPhone.Match(inputValue.ToStringValue());
            return match.Success;
        }
        /// <summary>
        ///  判断字符串是否为电话号码
        ///  （包含固定电话和手机号）
        /// </summary>
        /// <param name="inputValue">输入值</param>
        /// <returns></returns>
        public static bool IsTelePhone(this object inputValue)
        {
            System.Text.RegularExpressions.Match match = RegTelePhone.Match(inputValue.ToStringValue());
            return match.Success;
        }
        /// <summary>
        ///  判断字符串是否为IP地址
        /// </summary>
        /// <param name="inputValue">输入值</param>
        /// <returns></returns>
        public static bool IsIP(this object inputValue)
        {
            System.Text.RegularExpressions.Match match = RegIP.Match(inputValue.ToStringValue());
            return match.Success;
        }
        /// <summary>
        ///  判断字符串是否为邮编
        /// </summary>
        /// <param name="inputValue">输入值</param>
        /// <returns></returns>
        public static bool IsPostCode(this object inputValue)
        {
            System.Text.RegularExpressions.Match match = RegPostCode.Match(inputValue.ToStringValue());
            return match.Success;
        }
        /// <summary>
        ///  判断字符串是否为身份证
        /// </summary>
        /// <param name="inputValue">输入值</param>
        /// <returns></returns>
        public static bool IsCardID(this object inputValue)
        {
            System.Text.RegularExpressions.Match match = RegCardID.Match(inputValue.ToStringValue());
            return match.Success;
        }
        /// <summary>
        ///  判断字符串是否为中文
        /// </summary>
        /// <param name="inputValue">输入值</param>
        /// <returns></returns>
        public static bool IsCHZN(this object inputValue)
        {
            System.Text.RegularExpressions.Match match = RegCHZN.Match(inputValue.ToStringValue());
            return match.Success;
        }
        /// <summary>
        ///  判断字符串是否为数字
        /// </summary>
        /// <param name="inputValue">输入值</param>
        /// <returns></returns>
        public static bool IsNumber(this object inputValue)
        {
            System.Text.RegularExpressions.Match match = RegNumber.Match(inputValue.ToStringValue());
            return match.Success;
        }        
    }
}
