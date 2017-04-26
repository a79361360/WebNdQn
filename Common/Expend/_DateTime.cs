using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fgly.Common.Expand
{
    public static class _DateTime
    {
        /// <summary>
        /// 转换成星座
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToConstellAtion(this System.DateTime dt)
        {
            string[] constellations = { "水瓶座", "双鱼座", "白羊座", "金牛座", "双子座", "巨蟹座", "狮子座", "处女座", "天秤座", "天蝎座", "射手座", "摩羯座" };
            int[] minDay = { 21, 20, 21, 21, 22, 22, 23, 24, 24, 24, 23, 22 };

            if (dt.Day >= minDay[dt.Month - 1])
                return constellations[dt.Month - 1];
            else
                return constellations[(dt.Month - 2) < 0 ? 11 : (dt.Month - 2)];
        }

        /// <summary>
        /// 转换成Unix时间戳
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static long ToUnixTimeStamp(this System.DateTime dt)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return (long)DateTime.Now.Subtract(dtStart).TotalSeconds;
        }
    }
}
