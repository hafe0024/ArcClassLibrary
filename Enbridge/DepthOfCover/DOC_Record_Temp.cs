using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enbridge.DepthOfCover
{
    public class docRecord_Temp
    {
        public string CreatedBy = null;
        public DateTime CreatedDate = DateTime.Now;
        public double POINT_X;
        public double POINT_Y;
        public double POINT_Z;
        public double? Measurement = null;
        public string Description = null;
        public int? EquipmentID = null;
        public double? Accuracy = null;
        public int? probe = null;
        public double? stn = null;
        public double? measure = null;
        public IPoint point = null;
        public string stnEventID = null;
        public string evtID = null;

        public docRecord_Temp(string CreatedBy, DateTime CreatedDate, string POINT_X, string POINT_Y, string POINT_Z, string Measurement,
            string Description, string EquipmentID, string Accuracy, string probe)
        {
            this.evtID = "{" + Guid.NewGuid().ToString().ToUpper() + "}";
            this.CreatedBy = CreatedBy;
            this.CreatedDate = CreatedDate;
            this.POINT_X = Convert.ToDouble(POINT_X);
            this.POINT_Y = Convert.ToDouble(POINT_Y);
            this.POINT_Z = (POINT_Z == "" ? 0 : Convert.ToDouble(POINT_Z));
            this.Measurement = Convert.ToDouble(Measurement);
            this.Description = (Description.Length > 50 ? Description.Substring(0, 50) : Description);
            this.EquipmentID = Convert.ToInt16(EquipmentID);
            this.Accuracy = Convert.ToDouble(Accuracy);
            this.probe = Convert.ToInt32(probe);
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9}",
                this.evtID, this.CreatedBy, this.POINT_X, this.POINT_Y, this.POINT_Z,
                this.Measurement, this.Description, this.EquipmentID, this.Accuracy, this.probe);
        }
    }
}
