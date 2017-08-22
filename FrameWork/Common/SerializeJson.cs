using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameWork.Common
{
    public static class SerializeJson<T>
    {
        public static IList<T> JSONStringToList(string JsonStr)
        {
            IList<T> objs = JsonConvert.DeserializeObject<IList<T>>(JsonStr);
            return objs;
        }
    }
}
