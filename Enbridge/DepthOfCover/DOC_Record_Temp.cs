using ESRI.ArcGIS.Geometry;
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
    public class docRecord_Temp
    {
        /// <summary>
        /// 
        /// </summary>
        public string CreatedBy = null;
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreatedDate = DateTime.Now;
        /// <summary>
        /// 
        /// </summary>
        public double POINT_X;
        /// <summary>
        /// 
        /// </summary>
        public double POINT_Y;
        /// <summary>
        /// 
        /// </summary>
        public double POINT_Z;
        /// <summary>
        /// 
        /// </summary>
        public double? Measurement = null;
        /// <summary>
        /// 
        /// </summary>
        public string Description = null;
        /// <summary>
        /// 
        /// </summary>
        public int? EquipmentID = null;
        /// <summary>
        /// 
        /// </summary>
        public double? Accuracy = null;
        /// <summary>
        /// 
        /// </summary>
        public int? probe = null;
        /// <summary>
        /// 
        /// </summary>
        public double? stn = null;
        /// <summary>
        /// 
        /// </summary>
        public double? measure = null;
        /// <summary>
        /// 
        /// </summary>
        public IPoint point = null;
        /// <summary>
        /// 
        /// </summary>
        public string stnEventID = null;
        /// <summary>
        /// 
        /// </summary>
        public string evtID = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CreatedBy"></param>
        /// <param name="CreatedDate"></param>
        /// <param name="POINT_X"></param>
        /// <param name="POINT_Y"></param>
        /// <param name="POINT_Z"></param>
        /// <param name="Measurement"></param>
        /// <param name="Description"></param>
        /// <param name="EquipmentID"></param>
        /// <param name="Accuracy"></param>
        /// <param name="probe"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9}",
                this.evtID, this.CreatedBy, this.POINT_X, this.POINT_Y, this.POINT_Z,
                this.Measurement, this.Description, this.EquipmentID, this.Accuracy, this.probe);
        }
    }
}
