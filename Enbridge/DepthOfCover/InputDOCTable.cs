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
        public InputDOCTable(string inputString, string lineLoopEventId, string groupDescription)
        {
            this.pointGroupId = "{" + Guid.NewGuid().ToString().ToUpper() + "}";

            this.routeEventId = "{" + lineLoopEventId.ToUpper() + "}";

            this.routeName = Enbridge.AppConstants.GetLineName(this.routeEventId);

            this.groupDescription = groupDescription;

            this.groupCreatedDate = DateTime.Now;

            

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
                for (int i = 0; i < this.docGraphicList.Count; i++)
                {
                    double stn, meas, mp;
                    string stnSeriesEventId = locator.getLocation(
                        (double)this.docGraphicList[i].Attributes["POINT_X"], 
                        (double)this.docGraphicList[i].Attributes["POINT_Y"], 
                        out stn, 
                        out meas, 
                        out mp);
                    this.docGraphicList[i].Attributes.Add("SeriesEventID", stnSeriesEventId);
                    this.docGraphicList[i].Attributes.Add("Station", stn);
                    this.docGraphicList[i].Attributes.Add("Measure", meas);
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

            ESRI.ArcGIS.Client.Graphic graphic = new ESRI.ArcGIS.Client.Graphic();
            graphic.Geometry = new ESRI.ArcGIS.Client.Geometry.MapPoint(X, Y, new ESRI.ArcGIS.Client.Geometry.SpatialReference(4326));
            string guid = "{" + Guid.NewGuid().ToString().ToUpper() + "}";
            graphic.Attributes.Add("EventID", guid);
            graphic.Attributes.Add("OriginEventID", guid);
            graphic.Attributes.Add("CreatedDate", createdDate);
            graphic.Attributes.Add("CreatedBy", createdBy);
            graphic.Attributes.Add("POINT_X", X);
            graphic.Attributes.Add("POINT_Y", Y);
            if (Z != null)
            {
                graphic.Attributes.Add("POINT_Z", (double)Z);
            }
            graphic.Attributes.Add("Measurement", measurement);
            graphic.Attributes.Add("Description", description);
            if (equipmentid != null)
            {
                graphic.Attributes.Add("EquipmentID", (int)equipmentid);
            }
            if (accuracy != null)
            {
                graphic.Attributes.Add("Accuracy", (double)accuracy);
            }
            if (probe != null)
            {
                graphic.Attributes.Add("Probe", ((bool)probe ? 1 : 0));
            }
            
            graphic.Attributes.Add("PointGroupID", this.pointGroupId);
            graphic.Attributes.Add("RouteEventID", this.routeEventId);
            graphic.Attributes.Add("LastModified", this.groupCreatedDate);

            this.docGraphicList.Add(graphic);
        }
    }
}
