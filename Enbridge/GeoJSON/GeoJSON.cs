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
    public class GeoJSON
    {
        List<JSONFeature> featureList;
        
        /// <summary>
        /// 
        /// </summary>
        public GeoJSON()
        {
            featureList = new List<JSONFeature>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="feat"></param>
        public void addFeature(JSONFeature feat)
        {
            featureList.Add(feat);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            List<string> featStringList = new List<string>();

            foreach (JSONFeature feat in featureList)
            {
                featStringList.Add(feat.ToString());
            }

            return "{\"type\":\"FeatureCollection\",\"features\":[" + string.Join(",", featStringList) + "]}";
        }

    }
}
