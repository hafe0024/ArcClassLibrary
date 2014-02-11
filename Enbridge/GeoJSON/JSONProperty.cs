using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Enbridge.GeoJSON
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class JSONProperty
    {
        public string name;
        public object property;

        /// <summary>
        /// create the object, tests for object type, adds '' for non numeric and dates
        /// </summary>
        /// <param name="name">name of property as string</param>
        /// <param name="value">property value as object</param>
        public JSONProperty(string name, object value)
        {
            this.name = name;

            if (value == null)
            {
                this.property = "null";
                return;
            }

            string objectString  = value.ToString();

            if (objectString == "")
            {
                this.property = "null";
                return;
            }

            DateTime dte;
            if (DateTime.TryParse(objectString, out dte))
            {
                this.property = string.Format("\"{0}\"", dte.ToString("MM/dd/yyyy"));
                return;
            }

            double doub;
            if (Double.TryParse(objectString, out doub))
            {
                this.property = doub.ToString();
                return;
            }

            objectString = objectString.Replace("\"", "").Replace("'", "").Replace(@"\", @"\\\\");
            this.property = string.Format("\"{0}\"", objectString);
        }

        /// <summary>
        /// Overridden to string method for ToString</value>
        /// </summary>
        /// <returns>appropriate format "name": '<value>'</returns>
        public override string ToString()
        {
            return "\"" + name + "\":" + property;
        }
    }
}