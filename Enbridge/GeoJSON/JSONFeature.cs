//using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Enbridge.GeoJSON
{
    [Serializable]
    public class JSONFeature
    {
        JSONGeometry geom;
        List<JSONProperty> propertyList;

        public JSONFeature()
        {
            this.geom = new JSONGeometry();
            propertyList = new List<JSONProperty>();
        }

        ///// <summary>
        ///// Geometry from IGeometry
        ///// </summary>
        ///// <param name="geom"></param>
        //public void addGeom(IGeometry geom)
        //{
        //    this.geom = new JSONGeometry(geom);
        //}

        /// <summary>
        /// Geometry from string
        /// </summary>
        /// <param name="geomString"></param>
        public void addGeom(string geomString)
        {

            switch (geomString.Split(null)[0])
            {
                case "POINT":
                    this.geom = new JSONPointGeometry(geomString);
                    break;
                case "MULTIPOINT":
                     this.geom = new JSONMultiPointGeometry(geomString);
                    break;
                default:
                    break;
            }            
        }

        public void addProperty(string name, object property)
        {
            propertyList.Add(new JSONProperty(name, property));
        }

        /// <summary>
        /// JSON format output of the feature
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            List<string> propertyStringList = new List<string>();
            
            foreach (JSONProperty prop in propertyList)
            {
                propertyStringList.Add(prop.ToString());
            }

            return "{\"type\":\"Feature\"," + geom.ToString() + ",\"properties\":{" + string.Join(",", propertyStringList) + "}}";
        }
    }


}
