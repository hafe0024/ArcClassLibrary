using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enbridge.LinearReferencing
{
    [Serializable]
    public class StationSeriesSQL
    {
        public string fromStnSeries = null;
        public string toStnSeries = null;
        public string evntId = null;
        public List<ContLinePoint> pointList = new List<ContLinePoint>();
        public bool isDiv;
        public string stnSeriesName;
        public string llid;
        public double startM;
        public double minLat = 10E100;
        public double minLon = 10E100;
        public double maxLat = -10E100;
        public double maxLon = -10E100;
        public double diameter;
        public double wallThickness;
        public string evntID;



        public StationSeriesSQL(string evntID, string llid, string stnSeriesName, string fromStnSeries, 
            string toStnSeries, string shpString, string diameter, string wallthickness)
        {
            this.evntID = evntID;
            this.stnSeriesName = stnSeriesName;
            this.fromStnSeries = fromStnSeries;
            this.toStnSeries = toStnSeries;
            this.diameter = double.Parse(diameter);
            this.wallThickness = double.Parse(wallthickness);

            if (stnSeriesName.ToUpper().IndexOf("DIV") >= 0)
            {
                this.isDiv = true;
            }
            else
            {
                this.isDiv = false;
            }

            //preprocess the geometry string
            shpString = shpString.Replace("LINESTRING (", "").Replace(")", "");
            string[] pointArray = shpString.Split(',');

            //Loop through the points, add to the member pointlist
            for (int i = 0; i < pointArray.Length; i++)
            {
                string[] coordArr = pointArray[i].Trim().Split(null);
                minLat = (double.Parse(coordArr[1]) < minLat ? double.Parse(coordArr[1]) : minLat);
                minLon = (double.Parse(coordArr[0]) < minLon ? double.Parse(coordArr[0]) : minLon);
                maxLat = (double.Parse(coordArr[1]) > maxLat ? double.Parse(coordArr[1]) : maxLat);
                maxLon = (double.Parse(coordArr[0]) > maxLon ? double.Parse(coordArr[0]) : maxLon);
                pointList.Add(new ContLinePoint(coordArr[0], coordArr[1], coordArr[2], coordArr[3], this.evntID, this.llid, this.diameter, this.wallThickness, this.isDiv));
            }
            //property with start m
            startM = pointList[0].stn;
        }
    }
}

