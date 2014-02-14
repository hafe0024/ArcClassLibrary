using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enbridge.Drawings.AlignmentSheets
{
    /// <summary>
    /// 
    /// </summary>
    public class PolarCoords
    {
        /// <summary>
        /// 
        /// </summary>
        public double distance
        {
            get;
            private set;
        }
        /// <summary>
        /// 
        /// </summary>
        public double heading
        {
            get;
            private set;
        }
        /// <summary>
        /// 
        /// </summary>
        private double centerX;
        /// <summary>
        /// 
        /// </summary>
        private double centerY;
        /// <summary>
        /// 
        /// </summary>
        public double cartesianX
        {
            get;
            private set;
        }
        /// <summary>
        /// 
        /// </summary>
        public double cartesianY
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="centerX"></param>
        /// <param name="centerY"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        public PolarCoords(double centerX, double centerY, double X, double Y)
        {
            this.centerX = centerX;
            this.centerY = centerY;
            heading = Math.Atan2(Y - centerY, X - centerX) * (180.0 / Math.PI);
            heading = (heading < 0 ? 360 + heading : heading);
            distance = distanceFormula(centerX, centerY, X, Y);
            cartesianX = this.centerX + X;
            cartesianY = this.centerY + Y;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scalar"></param>
        public void scaleDistance(double scalar)
        {
            distance *= scalar;
            this.updateCoordinates();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="angleDegress"></param>
        public void rotate(double angleDegress)
        {
            heading += angleDegress;
            heading = (heading < 0 ? 360 + heading : heading);
            this.updateCoordinates();
        }

        private void updateCoordinates()
        {
            double headingRadians = (heading / 180) * Math.PI;
            cartesianX = this.centerX + distance * Math.Cos(headingRadians);
            cartesianY = this.centerY + distance * Math.Sin(headingRadians);
        }

        private static double distanceFormula(double x1, double y1, double x2, double y2)
        {
            double a = x1 - x2;
            double b = y1 - y2;
            return Math.Sqrt(a * a + b * b);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Dist: {0}   Heading: {1}  X: {2}   Y: {3}", this.distance, this.heading, this.cartesianX, this.cartesianY);
        }
    }     
}
