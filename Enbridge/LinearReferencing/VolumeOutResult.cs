using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Enbridge.Utilities;

namespace Enbridge.LinearReferencing
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class VolumeOutResult
    {
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, double> upstreamBound;
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, double> downstreamBound;
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, double> upstreamBreak;
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, double> downstreamBreak;
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, double> upstreamContribution;
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, double> downstreamContribution;
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, double> ruptureLocation;
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, double> extent;
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, double> extentMercator;
        /// <summary>
        /// 
        /// </summary>
        public double vOut;
        /// <summary>
        /// 
        /// </summary>
        public double vStorage;
        /// <summary>
        /// 
        /// </summary>
        public double vAssumed;
        /// <summary>
        /// 
        /// </summary>
        public double vTotal;
        /// <summary>
        /// 
        /// </summary>
        public double percentAccounted;
        /// <summary>
        /// 
        /// </summary>
        public double dynamicContribution;

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string getJSON()
        {
            return this.ToJSON();
        }

    }
}
