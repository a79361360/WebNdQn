using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fgly.Common.Expand
{
    public static class _Enum
    {
        /// <summary>
        /// 获取枚举名称
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToName<T>(this Enum e)
        {
            return Enum.GetName(typeof(T), e);
        }
    }
}
