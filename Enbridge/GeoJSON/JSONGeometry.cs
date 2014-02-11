//using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enbridge.GeoJSON
{
    [Serializable]
    public class JSONGeometry
    {
        protected string type;
        protected string coordinates;

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

        public override string ToString()
        {
            return "\"geometry\":{\"type\":" + type + ",\"coordinates\":" + coordinates + "}";
        }
    }

    public class JSONPointGeometry : JSONGeometry
    {
        public JSONPointGeometry(string geometryString)
        {
            geometryString = geometryString.Replace("(", "").Replace(")", "");
            geometryString = geometryString.ToLower();
            string[] arr = geometryString.Split(null);
            this.type = "\"" + char.ToUpper(arr[0][0]) + arr[0].Substring(1) + "\"";
            
            coordinates = string.Format("[{0}, {1}]", arr[1], arr[2]);

        }
    }

    public class JSONMultiPointGeometry : JSONGeometry
    {
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