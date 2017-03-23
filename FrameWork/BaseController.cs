using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace FrameWork
{
    public class BaseController : Controller
    {
        public ExtendActionResult JsonFormat(object data)
        {
            ExtendActionResult result = new ExtendActionResult();
            result.Data = data;
            return result;
        }
    }
}
