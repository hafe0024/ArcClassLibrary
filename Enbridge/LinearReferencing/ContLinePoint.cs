using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enbridge.LinearReferencing
{
    [Serializable]
    public class ContLinePoint
    {
        public double X;
        public double Y;
        public double Z;
        public double stn;
        public double meas = 0;
        public string stnSeries;
        public string llid;
        public double MP = 0;
        public double diameter;
        public double wallThickness;
        public bool isDiversion;

        public ContLinePoint(string x, string y, string z, string m, string stnSeries, string llid, double diameter, double wallthickness, bool isDiversion)
        {
            this.X = double.Parse(x);
            this.Y = double.Parse(y);
            this.Z = double.Parse(z);
            this.stn = double.Parse(m);
            this.stnSeries = stnSeries;
            this.llid = llid;
            this.isDiversion = isDiversion;
            this.diameter = diameter;
            this.wallThickness = wallthickness;
            
        }

        public override string ToString()
        {
            return string.Format("X: {0}, Y: {1}, Stn: {2}, Meas: {3}, MP: {4}, Ser: {5}, LL: {6}, Div: {7}", 
                Math.Round(X, 5), Math.Round(Y, 5), Math.Round(stn, 0), Math.Round(meas, 0), Math.Round(MP, 2), stnSeries, llid, isDiversion);
        }
    }
}
