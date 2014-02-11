using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enbridge.Geometry
{
    /// <summary>
    /// Simple point class
    /// </summary>
    [Serializable]
    public class SimplePoint
    {
        /// <summary>
        /// X coordinate
        /// </summary>
        double X;
        /// <summary>
        ///  Y coordinate
        /// </summary>
        double Y;

        /// <summary>
        /// Constructor for simple point
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        public SimplePoint(double X, double Y)
        {
            this.X = X;
            this.Y = Y;
        }

        /// <summary>
        /// Convert point coordinates in web mercator to geographic
        /// </summary>
        public void toGeographic(){
            double xGeo, yGeo;
            Enbridge.Utilities.ProjectionConversion.toGeographic(this.X, this.Y, out xGeo, out yGeo);
            this.X = xGeo;
            this.Y = yGeo;
        }

        /// <summary>
        /// Convert point coordinates in geographuc to web mercator
        /// </summary>
        public void toMercator()
        {
            double xMerc, yMerc;
            Enbridge.Utilities.ProjectionConversion.toWebMercator(this.X, this.Y, out xMerc, out yMerc);
            this.X = xMerc;
            this.Y = yMerc;
        }

        public void addOne(double add)
        {
            this.X += add;
        }
    }
}
