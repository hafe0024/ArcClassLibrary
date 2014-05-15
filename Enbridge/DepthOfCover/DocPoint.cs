using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enbridge.DepthOfCover
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class DocPoint
    {
        /// <summary>
        /// 
        /// </summary>
        public string eventID;
        /// <summary>
        /// 
        /// </summary>
        public string createdBy;
        /// <summary>
        /// 
        /// </summary>
        public DateTime createdDate;
        /// <summary>
        /// 
        /// </summary>
        public double point_X;
        /// <summary>
        /// 
        /// </summary>
        public double point_Y;
        /// <summary>
        /// 
        /// </summary>
        public double? point_Z;
        /// <summary>
        /// 
        /// </summary>
        public double measurement;
        /// <summary>
        /// 
        /// </summary>
        public string description;
        /// <summary>
        /// 
        /// </summary>
        public int? equipmentid;
        /// <summary>
        /// 
        /// </summary>
        public double? accuracy;
        /// <summary>
        /// 
        /// </summary>
        public bool? probe;

        public double measure;
        public double station;
        public string seriesEventId;
        /// <summary>
        /// 
        /// </summary>
        public double segmentStart;
        /// <summary>
        /// 
        /// </summary>
        public double segmentStop;

        public string pointGroupId;

        public string routeEventId;

        public DateTime? groupCreatedDate;

        public bool flagArchive = false;

        public bool toRemove = false;

        public string modifiedBy;




        /// <summary>
        /// 
        /// </summary>
        /// <param name="createdBy"></param>
        /// <param name="createdDate"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Z"></param>
        /// <param name="measurement"></param>
        /// <param name="description"></param>
        /// <param name="equipmentid"></param>
        /// <param name="accuracy"></param>
        /// <param name="probe"></param>
        /// <param name="groupCreatedDate"></param>
        /// <param name="username"></param>
        public DocPoint(string createdBy, DateTime createdDate, double X, double Y, double? Z,
            double measurement, string description, int? equipmentid, double? accuracy, bool? probe, DateTime groupCreatedDate, string username)
        {
            this.eventID = "{" + Guid.NewGuid().ToString().ToUpper() + "}";
            this.createdBy = (createdBy.Length > 45 ? createdBy.Substring(0, 45) : createdBy);
            this.createdDate = createdDate;
            this.point_X = X;
            this.point_Y = Y;
            this.point_Z = Z;
            this.measurement = measurement;
            this.description = (description.Length > 50 ? description.Substring(0, 50) : description);
            this.equipmentid = equipmentid;
            this.accuracy = accuracy;
            this.probe = probe;
            this.station = 0;
            this.measure = 0;
            this.groupCreatedDate = groupCreatedDate;
            this.modifiedBy = username;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        public DocPoint(SqlDataReader reader)
        {
            this.eventID = reader["EventID"].ToString();
            this.createdBy = reader["CreatedBy"].ToString();
            this.createdDate = DateTime.Parse(reader["CreatedDate"].ToString());
            this.point_X = Convert.ToDouble(reader["POINT_X"].ToString());
            this.point_Y = Convert.ToDouble(reader["POINT_Y"].ToString());;
            double Z;
            if (Double.TryParse(reader["POINT_Z"].ToString(), out Z))
                this.point_Z = Z;
            else
                this.point_Z = null;

            this.measurement = Convert.ToDouble(reader["Measurement"].ToString());
            this.description = reader["Description"].ToString();
            int equip;
            if (Int32.TryParse(reader["EquipmentID"].ToString(), out equip))
                this.equipmentid = equip;
            else
                this.equipmentid = null;

            double accuracy;
            if (Double.TryParse(reader["Accuracy"].ToString(), out accuracy))
                this.accuracy = accuracy;
            else
                this.accuracy = null;

            int probe;
            if (Int32.TryParse(reader["Probe"].ToString(), out probe))
                this.probe = (probe == 1);
            else
                this.probe = null;

            this.pointGroupId = reader["PointGroupID"].ToString();
            this.seriesEventId = reader["SeriesEventID"].ToString();

            this.station = Convert.ToDouble(reader["Station"].ToString());
            this.measure = Convert.ToDouble(reader["Measure"].ToString());

            this.routeEventId = reader["RouteEventID"].ToString();
            this.modifiedBy = reader["ModifiedBy"].ToString();
        }

        public override bool Equals(object point2)
        {

            if (Object.ReferenceEquals(point2, null) || this.GetType() != point2.GetType())
                return false;
            DocPoint p2 = (DocPoint)point2;


            double distance = this.getDistance(p2);
            return (distance == 0 && p2.createdDate == this.createdDate && p2.probe == this.probe);
        }




        public double getDistance(DocPoint otherPoint)
        {
            return Math.Sqrt(Math.Pow(this.point_X - otherPoint.point_X, 2) + Math.Pow(this.point_Y - otherPoint.point_Y, 2));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pointGroupId"></param>
        /// <param name="routeEventId"></param>
        /// <param name="groupCreatedDate"></param>
        /// <returns></returns>
        public ESRI.ArcGIS.Client.Graphic toGraphic()
        {
            ESRI.ArcGIS.Client.Graphic graphic = new ESRI.ArcGIS.Client.Graphic();
            graphic.Geometry = new ESRI.ArcGIS.Client.Geometry.MapPoint(point_X, point_Y, new ESRI.ArcGIS.Client.Geometry.SpatialReference(4326));

            graphic.Attributes.Add("EventID", this.eventID);
            graphic.Attributes.Add("OriginEventID", this.eventID);
            graphic.Attributes.Add("CreatedDate", createdDate);
            graphic.Attributes.Add("CreatedBy", createdBy);
            graphic.Attributes.Add("POINT_X", point_X);
            graphic.Attributes.Add("POINT_Y", point_Y);
            if (point_Z != null)
            {
                graphic.Attributes.Add("POINT_Z", (double)point_Z);
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

            graphic.Attributes.Add("PointGroupID", pointGroupId);
            graphic.Attributes.Add("RouteEventID", routeEventId);
            graphic.Attributes.Add("LastModified", groupCreatedDate);

            graphic.Attributes.Add("SeriesEventID", this.seriesEventId);
            graphic.Attributes.Add("Station", this.station);
            graphic.Attributes.Add("Measure", this.measure);
            graphic.Attributes.Add("ModifiedBy", this.modifiedBy);

            return graphic;
        }
    }
}
