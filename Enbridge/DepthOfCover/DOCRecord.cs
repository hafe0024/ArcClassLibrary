using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enbridge.DepthOfCover
{
    /// <summary>
    /// Depth of cover record class
    /// </summary>
    [Serializable]
    public class DOCRecord
    {
        /// <summary>
        /// Created By
        /// </summary>
        public string createdBy;
        /// <summary>
        /// Created Date
        /// </summary>
        public DateTime createdDate;
        /// <summary>
        /// Longitude
        /// </summary>
        public double X;
        /// <summary>
        /// Latitude
        /// </summary>
        public double Y;
        /// <summary>
        /// Elevation
        /// </summary>
        public double? Z;
        /// <summary>
        /// Measurement in inches
        /// </summary>
        public double measurement;
        /// <summary>
        /// Text description
        /// </summary>
        public string description;
        /// <summary>
        /// Equipment id if available
        /// </summary>
        public int? equipmentid;
        /// <summary>
        /// Accuracy if available
        /// </summary>
        public double? accuracy;

        /// <summary>
        /// Depth of cover record constructor
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
        public DOCRecord(string createdBy, DateTime createdDate, double X, double Y, double? Z, 
            double measurement, string description, int? equipmentid, double? accuracy, bool probe)
        {
            this.createdBy = createdBy;
            this.createdDate = createdDate;
            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.measurement = measurement;
            this.description = description;
            this.equipmentid = equipmentid;
            this.accuracy = accuracy;
        }
    }


  
}
