using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enbridge.GeoJSON
{
    [Serializable]
    public class GeoJSON
    {
        List<JSONFeature> featureList;
        
        public GeoJSON()
        {
            featureList = new List<JSONFeature>();
        }

        public void addFeature(JSONFeature feat)
        {
            featureList.Add(feat);
        }

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
