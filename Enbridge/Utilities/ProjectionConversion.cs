using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enbridge.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public static class ProjectionConversion
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="xGeo"></param>
        /// <param name="yGeo"></param>
        public static void toGeographic(double x, double y, out double xGeo, out double yGeo)
        {
            if (Math.Abs(x) < 180 && Math.Abs(y) < 90)
            {
                xGeo = -1;
                yGeo = -1;
                return;
            }
            if ((Math.Abs(x) > 20037508.3427892) || (Math.Abs(y) > 20037508.3427892))
            {
                xGeo = -1;
                yGeo = -1;
                return;
            }
            double num3 = x / 6378137.0;
            double num4 = num3 * 57.295779513082323;
            double num5 = Math.Floor((num4 + 180.0) / 360.0);
            double num6 = num4 - (num5 * 360.0);
            double num7 = 1.5707963267948966 - (2.0 * Math.Atan(Math.Exp((-1.0 * y) / 6378137.0)));

            xGeo = num6;
            yGeo = num7 * 57.295779513082323;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="xMerc"></param>
        /// <param name="yMerc"></param>
        public static void toWebMercator(double x, double y, out double xMerc, out double yMerc)
        {
            if ((Math.Abs(x) > 180 || Math.Abs(y) > 90))
            {
                xMerc = 0;
                yMerc = 0;
                return;
            }

            double num = x * 0.017453292519943295;
            xMerc = 6378137.0 * num;
            double a = y * 0.017453292519943295;

            yMerc = 3189068.5 * Math.Log((1.0 + Math.Sin(a)) / (1.0 - Math.Sin(a)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static string trial(double x, double y)
        {
            return string.Format("{0} {1}", x, y);
        }
    


    }
}
