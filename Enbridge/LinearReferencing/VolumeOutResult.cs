using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Enbridge.Utilities;

namespace Enbridge.LinearReferencing
{
    [Serializable]
    public class VolumeOutResult
    {
        public Dictionary<string, double> upstreamBound;
        public Dictionary<string, double> downstreamBound;
        public Dictionary<string, double> upstreamBreak;
        public Dictionary<string, double> downstreamBreak;
        public Dictionary<string, double> upstreamContribution;
        public Dictionary<string, double> downstreamContribution;
        public Dictionary<string, double> ruptureLocation;
        public Dictionary<string, double> extent;
        public Dictionary<string, double> extentMercator;
        public double vOut;
        public double vStorage;
        public double vAssumed;
        public double vTotal;
        public double percentAccounted;
        public double dynamicContribution;

        
        public VolumeOutResult()
        {
            upstreamBound = new Dictionary<string, double>();
            downstreamBound = new Dictionary<string, double>();
            upstreamBreak = new Dictionary<string, double>();
            downstreamBreak = new Dictionary<string, double>();
            upstreamContribution = new Dictionary<string, double>();
            downstreamContribution = new Dictionary<string, double>();
            ruptureLocation = new Dictionary<string, double>();
            extent = new Dictionary<string, double>();
            extentMercator = new Dictionary<string, double>();
            vOut = 0;
            vStorage = 0;
            vAssumed = 0;
            vTotal = 0;
            percentAccounted = 0;
            dynamicContribution = 0;

        }


        public void makeExtent()
        {
            if (upstreamBound["X"] <= downstreamBound["X"])
            {
                extent["xmin"] = upstreamBound["X"];
                extent["xmax"] = downstreamBound["X"];
            }
            else
            {
                extent["xmin"] = downstreamBound["X"];
                extent["xmax"] = upstreamBound["X"];
            }

            if (upstreamBound["Y"] <= downstreamBound["Y"])
            {
                extent["ymin"] = upstreamBound["Y"];
                extent["ymax"] = downstreamBound["Y"];
            }
            else
            {
                extent["ymin"] = downstreamBound["Y"];
                extent["ymax"] = upstreamBound["Y"];
            }

            double xMinMerc, yMinMerc, xMaxMerc, yMaxMerc;
            Enbridge.Utilities.ProjectionConversion.toWebMercator(extent["xmin"], extent["ymin"], out xMinMerc, out yMinMerc);
            extentMercator["xmin"] = xMinMerc;
            extentMercator["ymin"] = yMinMerc;

            Enbridge.Utilities.ProjectionConversion.toWebMercator(extent["xmax"], extent["ymax"], out xMaxMerc, out yMaxMerc);
            extentMercator["xmax"] = xMaxMerc;
            extentMercator["ymax"] = yMaxMerc;
        }

        public string getJSON()
        {
            return this.ToJSON();
        }

    }
}
