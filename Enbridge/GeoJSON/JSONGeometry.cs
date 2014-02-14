//using ESRI.ArcGIS.Geometry;
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
    public class JSONGeometry
    {
        /// <summary>
        /// 
        /// </summary>
        protected string type;
        /// <summary>
        /// 
        /// </summary>
        protected string coordinates;

        /// <summary>
        /// 
        /// </summary>
        public JSONGeometry()
        {
            type = "null";
            coordinates = "null";
        }

        //public JSONGeometry(IGeometry inGeometry)
        //{
        //    if (inGeometry is IPoint)
        //    {
        //        IPoint point = (IPoint)inGeometry;
        //        type = "\"Point\"";
        //        coordinates = string.Format("[{0}, {1}]", point.X, point.Y);
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "\"geometry\":{\"type\":" + type + ",\"coordinates\":" + coordinates + "}";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class JSONPointGeometry : JSONGeometry
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometryString"></param>
        public JSONPointGeometry(string geometryString)
        {
            geometryString = geometryString.Replace("(", "").Replace(")", "");
            geometryString = geometryString.ToLower();
            string[] arr = geometryString.Split(null);
            this.type = "\"" + char.ToUpper(arr[0][0]) + arr[0].Substring(1) + "\"";
            
            coordinates = string.Format("[{0}, {1}]", arr[1], arr[2]);

        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class JSONMultiPointGeometry : JSONGeometry
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometryString"></param>
        public JSONMultiPointGeometry(string geometryString)
        {
            List<string> coordsList = new List<string>();

            geometryString = geometryString.Replace("(", "").Replace(")", "");
            geometryString = geometryString.ToLower();
            string[] arr = geometryString.Split(null);
            this.type = "\"" + char.ToUpper(arr[0][0]) + arr[0].Substring(1) + "\"";

            geometryString = geometryString.Replace(arr[0], "");
            string[] coordsPieces = geometryString.Split(',');

            for (int i = 0; i < coordsPieces.Length; i++)
            {
                coordsList.Add("[" + coordsPieces[i].Trim().Replace(" ", ", ") + "]");
            }

            coordinates = "[" + string.Join(", ", coordsList) + "]";
        }
    }
}    