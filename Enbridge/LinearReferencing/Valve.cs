using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enbridge.LinearReferencing
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class Valve: IComparable
    {
        /// <summary>
        /// 
        /// </summary>
        public string eventID;
        /// <summary>
        /// 
        /// </summary>
        public string seriesEventID;
        /// <summary>
        /// 
        /// </summary>
        public double station;
        /// <summary>
        /// 
        /// </summary>
        public double measure;
        /// <summary>
        /// 
        /// </summary>
        public double x;
        /// <summary>
        /// 
        /// </summary>
        public double y;
        /// <summary>
        /// 
        /// </summary>
        public string typeCL;
        /// <summary>
        /// 
        /// </summary>
        public string functionCL;
        
        
        /// <summary>
        /// Valve Class
        /// </summary>
        /// <param name="eventID"></param>
        /// <param name="serId"></param>
        /// <param name="station"></param>
        /// <param name="measure"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="typeCL"></param>
        /// <param name="functionCL"></param>
        public Valve(string eventID, string serId, double station, double measure, double x, double y, string typeCL, string functionCL)
        {
            this.eventID = eventID;
            this.seriesEventID = serId;
            this.station = station;
            this.measure = measure;
            this.x = x;
            this.y = y;
            this.typeCL = typeCL;
            this.functionCL = functionCL;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            Valve v = obj as Valve;
            if (this.measure < v.measure)
            {
                return -1;
            }
            else if (this.measure == v.measure)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
    }
}
