using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enbridge.LinearReferencing
{
    [Serializable]
    public class Valve: IComparable
    {
        public string eventID;
        public string seriesEventID;
        public double station;
        public double measure;
        public double x;
        public double y;
        public string typeCL;
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
