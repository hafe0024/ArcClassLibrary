using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enbridge.DepthOfCover
{
    /// <summary>
    /// 
    /// </summary>
    public class DocSegment
    {
        public List<string> individualParams;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geomString"></param>
        /// <param name="startStn"></param>
        /// <param name="endStn"></param>
        /// <param name="seriesEventId"></param>
        /// <param name="pointMeasure"></param>
        /// <param name="pointEventId"></param>
        /// <param name="routeId"></param>
        public DocSegment(string geomString, double startStn, double endStn, string seriesEventId, double pointMeasurement, string pointEventId, string routeId)
        {
            individualParams = new List<string>();

            //geometry
            individualParams.Add(string.Format("geometry::STGeomFromText('{0}', 4326).MakeValid()", geomString));
            //start stationing       
            individualParams.Add(startStn.ToString());
            //start stationing       
            individualParams.Add(endStn.ToString());

            string guid = "'" + Guid.NewGuid().ToString().ToUpper() + "'";
            //eventid       
            individualParams.Add(guid);
            //origineventid       
            individualParams.Add(guid);

            //string dte = "'" + DateTime.Now.ToString("") + "'";
            ////created date
            //individualParams.Add(dte);
            ////modified date
            //individualParams.Add(dte);

            //createdby
            individualParams.Add("'SUPGIS01'");
            //modifiedby
            individualParams.Add("'SUPGIS01'");

            //series eventid
            individualParams.Add(string.Format("'{0}'", seriesEventId));

            //Measurement
            individualParams.Add(string.Format("{0}", pointMeasurement));

            //PointId
            individualParams.Add(string.Format("'{0}'", pointEventId));

            //route eventid
            individualParams.Add(string.Format("'{0}'", routeId));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "(" + string.Join(",", individualParams) + ")";
        }

    }
}
