using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace Enbridge.Utilities
{
    [Serializable]
    public static class JSONHelper
    {
        public static string ToJSON(this object obj)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(obj);
        }

        public static string ToJSON(this object obj, int recursionDepth)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            jss.RecursionLimit = recursionDepth;
            return jss.Serialize(obj);
        }
    }
}



