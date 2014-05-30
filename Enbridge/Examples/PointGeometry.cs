using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enbridge.Examples
{
    [Serializable]
    public class PointGeometry
    {
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
        public double Z;
        /// <summary>
        /// Stationing, used as M (measurement) value in geometry
        /// </summary>
        public double stn_M;

        /// <summary>
        /// Constructor with no parameters
        /// </summary>
        public PointGeometry()
        {
            this.X = 0;
            this.Y = 0;
            this.Z = 0;
            this.stn_M = 0;
        }

        /// <summary>
        /// Constructor with parameters
        /// </summary>
        /// <param name="X">Longitude</param>
        /// <param name="Y">Latitude</param>
        /// <param name="Z">Elevation</param>
        /// <param name="stn_M">Stationing</param>
        public PointGeometry(double X, double Y, double Z, double stn_M)
        {
            //Case where the 'this' keyword is required
            //input parameters are provided with the same names as class properties
            //set the object property (example this.X) equal to the value of the input parameter (example X)
            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.stn_M = stn_M;
        }

        /// <summary>
        /// Return the Well Known Text (WKT) representation of the geometry
        /// </summary>
        /// <returns>Point Geometry WKT</returns>
        public string GetGeometryTextString()
        {
            return String.Format("POINT ({0} {1} {2} {3})", 
                Math.Round(this.X, 5), Math.Round(this.Y, 5), Math.Round(this.Z, 2), Math.Round(this.stn_M, 0));
        }
    }
}
