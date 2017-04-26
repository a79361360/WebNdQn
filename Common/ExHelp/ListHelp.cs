using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.ExHelp
{
    public static class ListHelp
    {
        //将健值对列表通过字典类进行ASCII排序
        public static SortedDictionary<string, string> GetSortedDictionary(NameValueCollection collection, Func<string, bool> filter = null)
        {
            //获取排序的键值对  
            SortedDictionary<string, string> dic = new SortedDictionary<string, string>();
            if (collection != null && collection.Count > 0)
            {
                foreach (var k in collection.AllKeys)
                {
                    if (filter == null || !filter(k))
                    {//如果没设置过滤条件或者无需过滤  
                        dic.Add(k, collection[k]);
                    }
                }
            }
            return dic;
        }
        //字典序拼接成字符串
        public static void FillStringBuilder(StringBuilder builder, SortedDictionary<string, string> dic)
        {
            foreach (var kv in dic)
            {
                if (builder.Length != 0)
                    builder.Append('&');
                builder.Append(kv.Key);
                builder.Append('=');
                builder.Append(kv.Value);
            }//按key顺序组织字符串  
        }
    }
}
