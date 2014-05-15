using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enbridge.DepthOfCover
{
    
    /// <summary>
    /// Input table class validates input from string representation of input file
    /// </summary>
    [Serializable]
    public class InputDOCTable
    {
        /// <summary>
        /// validation error message, initialized to null
        /// </summary>
        public string validationError = null;

        /// <summary>
        /// List of depth of cover records
        /// </summary>
        public List<ESRI.ArcGIS.Client.Graphic> docGraphicList = new List<ESRI.ArcGIS.Client.Graphic>();

        public List<DocPoint> docPointList = new List<DocPoint>();
        /// <summary>
        /// The route or lineloop event id
        /// </summary>
        public string routeEventId;

        /// <summary>
        /// 
        /// </summary>
        public string pointGroupId;
        /// <summary>
        /// 
        /// </summary>
        public DateTime groupCreatedDate;
        /// <summary>
        /// 
        /// </summary>
        public string routeName;
        /// <summary>
        /// 
        /// </summary>
        public string groupDescription;

        public string username;

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, double> extent;

        /// <summary>
        /// Input Depth of Cover table object constructor
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="lineLoopEventId"></param>
        /// <param name="groupDescription"></param>
        public InputDOCTable(string inputString, string lineLoopEventId, string groupDescription, string username)
        {
            this.pointGroupId = "{" + Guid.NewGuid().ToString().ToUpper() + "}";

            this.routeEventId = "{" + lineLoopEventId.ToUpper() + "}";

            this.routeName = Enbridge.AppConstants.GetLineName(this.routeEventId);

            this.groupDescription = groupDescription;

            this.groupCreatedDate = DateTime.Now;

            this.username = username;

            

            extent = new Dictionary<string, double>{
                {"xMin", 10E100},
                {"xMax", -10E100},
                {"yMin", 10E100},
                {"yMax", -10E100}
            };
            
            string[] lines = inputString.Split('\n');

            for (int i = 1; i < lines.Length; i++)
            {
                string lineContent = lines[i];

                if (lineContent.Replace(",","").Trim() == "")
                {
                    //Empty row detected
                    break;
                }

                string[] rowCells = lineContent.Split(',');

                for (int j = 0; j < rowCells.Length; j++)
                {
                    rowCells[j] = rowCells[j].Trim();
                }

                //Get created by text
                string createdBy = rowCells[0];

                //get and validate the date
                DateTime createdDate;
                if (!DateTime.TryParse(rowCells[1], out createdDate)){
                    validationError = string.Format("Date format invalid, {0} in record# {1}", rowCells[1], i);
                    return;
                }

                //Get and validate the x, y and z values
                double X, Y;
                

                if (!Double.TryParse(rowCells[2], out X))
                {
                    validationError = string.Format("POINT_X format invalid, {0} in record# {1}", rowCells[2], i);
                    return;
                }

                if (!Double.TryParse(rowCells[3], out Y))
                {
                    validationError = string.Format("POINT_Y format invalid, {0} in record# {1}", rowCells[3], i);
                    return;
                }

                double? Z_null = null;
                double Z;

                if (rowCells[4] != "")
                {
                    if (!Double.TryParse(rowCells[4], out Z))
                    {
                        validationError = string.Format("POINT_Z format invalid, {0} in record# {1}", rowCells[4], i);
                        return;
                    }
                    Z_null = Z;
                }

                double measurement;
                if (!Double.TryParse(rowCells[5], out measurement))
                {
                    validationError = string.Format("Measurement format invalid, {0} in record# {1}", rowCells[5], i);
                    return;
                }

                int? equipIdNull = null;
                int equipId;

                string description = rowCells[6];

                if (rowCells[7] != "")
                {
                    if (!int.TryParse(rowCells[7], out equipId))
                    {
                        validationError = string.Format("Equipment Id format invalid, {0} in record# {1}", rowCells[7], i);
                        return;
                    }
                    equipIdNull = equipId;
                }

                double? accuracyNull = null;
                double accuracy;

                if (rowCells[8] != "")
                {
                    if (!Double.TryParse(rowCells[8], out accuracy))
                    {
                        validationError = string.Format("Accuracy format invalid, {0} in record# {1}", rowCells[8], i);
                        return;
                    }
                    accuracyNull = accuracy;
                }

                bool? probe = null;

                if (rowCells[9] != "")
                {
                    switch (rowCells[9])
                    {
                        case "0":
                            probe = false;
                            break;
                        case "1":
                            probe = true;
                            break;
                        default:
                            validationError = string.Format("Probe format invalid, {0} in record# {1}", rowCells[9], i);
                            return;
                    }
                }
                this.AddDOCRecord(createdBy, createdDate, X, Y, Z_null, measurement, description, equipIdNull, accuracyNull, probe);
            }

            if (this.validationError == null)
            {
                Enbridge.LinearReferencing.ContLineLocatorSQL locator = new Enbridge.LinearReferencing.ContLineLocatorSQL(lineLoopEventId);
                for (int i = 0; i < this.docPointList.Count; i++)
                {
                    double stn, meas, mp;
                    string stnSeriesEventId = locator.getLocation(docPointList[i].point_X, docPointList[i].point_Y, out stn, out meas, out mp);

                    docPointList[i].seriesEventId = stnSeriesEventId;
                    docPointList[i].station = stn;
                    docPointList[i].measure = meas;
                    docPointList[i].pointGroupId = this.pointGroupId;
                    docPointList[i].routeEventId = this.routeEventId;
                    docPointList[i].groupCreatedDate = this.groupCreatedDate;

                    this.docGraphicList.Add(docPointList[i].toGraphic());
                }
            }
        }

        private void AddDOCRecord(string createdBy, DateTime createdDate, double X, double Y, double? Z, 
            double measurement, string description, int? equipmentid, double? accuracy, bool? probe)
        {
            double xMerc, yMerc;
            Enbridge.Utilities.ProjectionConversion.toWebMercator(X, Y, out xMerc, out yMerc);

            //Set the extents
            this.extent["xMin"] = (xMerc < this.extent["xMin"] ? xMerc : this.extent["xMin"]);
            this.extent["xMax"] = (xMerc > this.extent["xMax"] ? xMerc : this.extent["xMax"]);
            this.extent["yMin"] = (yMerc < this.extent["yMin"] ? yMerc : this.extent["yMin"]);
            this.extent["yMax"] = (yMerc > this.extent["yMax"] ? yMerc : this.extent["yMax"]);

            DocPoint docPoint = new DocPoint(createdBy, createdDate, X, Y, Z, measurement, description, equipmentid, accuracy, probe, this.groupCreatedDate, this.username);
            docPoint.pointGroupId = this.pointGroupId;
            docPoint.routeEventId = this.routeEventId;
            docPoint.groupCreatedDate = this.groupCreatedDate;
            this.docPointList.Add(docPoint);
        }
    }
}
