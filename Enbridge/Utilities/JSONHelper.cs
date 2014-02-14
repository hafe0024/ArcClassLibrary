using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace Enbridge.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public static class JSONHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJSON(this object obj)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="recursionDepth"></param>
        /// <returns></returns>
        public static string ToJSON(this object obj, int recursionDepth)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            jss.RecursionLimit = recursionDepth;
            return jss.Serialize(obj);
        }
    }
}



