using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enbridge.LinearReferencing
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ContLineLocatorSQL
    {
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, StationSeriesSQL> stnSeriesDict = new Dictionary<string, StationSeriesSQL>();
        /// <summary>
        /// 
        /// </summary>
        public List<ContLinePoint> pointList = new List<ContLinePoint>();
        /// <summary>
        /// 
        /// </summary>
        public double startMP = 0;
        /// <summary>
        /// 
        /// </summary>
        public double minLat = 10E100;
        /// <summary>
        /// 
        /// </summary>
        public double minLon = 10E100;
        /// <summary>
        /// 
        /// </summary>
        public double maxLat = -10E100;
        /// <summary>
        /// 
        /// </summary>
        public double maxLon = -10E100;
        /// <summary>
        /// 
        /// </summary>
        public List<Valve> valveList = new List<Valve>();


        /// <summary>
        /// Generate a locator object
        /// </summary>
        /// <param name="llId">Line Loop Event ID</param>
        public ContLineLocatorSQL(string llId)
        {
            # region get the geometries and start mp
            using (SqlConnection conn = new SqlConnection(AppConstants.CONN_STRING_PODS))
            {
                //Get the station series
                SqlCommand comm = conn.CreateCommand();
                comm.CommandText = "SELECT Shape.ToString() as shp,* FROM sde.STATIONSERIES_EVW where LineLoopEventID = @llid;";
                comm.Parameters.Add(new SqlParameter("@llid", llId));
                conn.Open();

                try
                {
                    SqlDataReader reader = comm.ExecuteReader();

                    while (reader.Read())
                    {
                        stnSeriesDict.Add(reader["EventID"].ToString(), new StationSeriesSQL(reader["EventID"].ToString(), llId,
                            reader["StationSeriesName"].ToString(), reader["FromStationSeriesEventID"].ToString(),
                            reader["ToStationSeriesEventID"].ToString(), reader["shp"].ToString(),
                            reader["Diameter"].ToString(), reader["WallThickness"].ToString()));
                    }
                    reader.Close();
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    comm.Dispose();
                }


                //Get the Valves
                comm = conn.CreateCommand();
                comm.CommandText = "SELECT Shape.ToString() as shp,* FROM sde.VALVE_EVW where RouteEventID = @llid;";
                comm.Parameters.Add(new SqlParameter("@llid", llId));

                try
                {
                    SqlDataReader reader = comm.ExecuteReader();

                    while (reader.Read())
                    {
                        string shpString = reader["shp"].ToString();
                        shpString = shpString.Substring(shpString.IndexOf('(') + 1).Replace(')', ' ');
                        string[] coords = shpString.Split();
                        
                        double x = double.Parse(coords[0]);
                        double y = double.Parse(coords[1]);

                        string evtID = reader["EventID"].ToString();
                        string serID = reader["SeriesEventID"].ToString();

                        double meas = double.Parse(reader["Measure"].ToString());
                        double stn = double.Parse(reader["Station"].ToString());

                        string typeCL = reader["TypeCL"].ToString();
                        string functionCL = reader["FunctionCL"].ToString();

                        if (functionCL.ToLower() == "check")
                        {
                            continue;
                        }

                        this.valveList.Add(new Valve(evtID, serID, stn, meas, x, y, typeCL, functionCL));
                    }
                    reader.Close();
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    comm.Dispose();
                }

                //Get the route properties
                comm = conn.CreateCommand();
                comm.CommandText = "SELECT StartMP FROM sde.LINELOOP_EVW WHERE EventID = @llid;";
                comm.Parameters.Add(new SqlParameter("@llid", llId));
                try
                {
                    startMP = double.Parse(comm.ExecuteScalar().ToString());
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    comm.Dispose();
                    conn.Close();
                }
            }

            # endregion

            # region set up the point list

            StationSeriesSQL currentSeries = null;

            foreach (string key in stnSeriesDict.Keys)
            {
                minLat = (stnSeriesDict[key].minLat < minLat ? stnSeriesDict[key].minLat : minLat);
                minLon = (stnSeriesDict[key].minLon < minLon ? stnSeriesDict[key].minLon : minLon);
                maxLat = (stnSeriesDict[key].maxLat > maxLat ? stnSeriesDict[key].maxLat : maxLat);
                maxLon = (stnSeriesDict[key].maxLon > maxLon ? stnSeriesDict[key].maxLon : maxLon);
                if (string.IsNullOrEmpty(stnSeriesDict[key].fromStnSeries))
                {
                    currentSeries = stnSeriesDict[key];
                }
            }

            //Freak out if it wasn't found
            if (currentSeries == null)
            {
                throw new Exception();
            }

            double startStn = currentSeries.startM;
            double runningTotal = 0;


            while (true)
            {
                //trim off the last point after the first points have been added
                if (pointList.Count > 0)
                {
                    pointList.RemoveAt(pointList.Count - 1);
                }

                //Modify the points add add them
                for (int i = 0; i < currentSeries.pointList.Count; i++)
                {
                    //Calculate the measure value
                    currentSeries.pointList[i].meas = currentSeries.pointList[i].stn - currentSeries.startM + runningTotal;
                    //calculate the milepost
                    if (currentSeries.isDiv)
                    {
                        currentSeries.pointList[i].MP = startMP + (runningTotal + currentSeries.pointList[i].stn) / 5280;
                    }
                    else
                    {
                        currentSeries.pointList[i].MP = startMP + currentSeries.pointList[i].stn / 5280;
                    }

                    pointList.Add(currentSeries.pointList[i]);
                }

                //update the running total
                runningTotal = pointList[pointList.Count - 1].meas;

                //check for next station series, break if this is the last one
                if (string.IsNullOrEmpty(currentSeries.toStnSeries))
                {
                    break;
                }
                else
                {
                    currentSeries = stnSeriesDict[currentSeries.toStnSeries];
                }

            }
            # endregion



            this.updateValveMeasure();
            this.valveList.Sort();



        }

        /// <summary>
        /// gets location values given a point with latitude longitude
        /// </summary>
        /// <param name="X">point longitude</param>
        /// <param name="Y">point latitude</param>
        /// <param name="stn">stationing</param>
        /// <param name="meas">continuous stationing</param>
        /// <param name="MP">mile post</param>
        /// <returns>StationSeries event id, in caps with brackets</returns>
        public string getLocation(double X, double Y, out double stn, out double meas, out double MP)
        {
            double minDist = 10E40;
            int minIndex = 0;
            int secondIndex = 0;
            string stnSeries;

            //find the minimum distance
            for (int i = 0; i < pointList.Count; i++)
            {
                double newDist = Math.Sqrt(Math.Pow(pointList[i].X - X, 2) + Math.Pow(pointList[i].Y - Y, 2));
                if (newDist < minDist)
                {
                    minDist = newDist;
                    minIndex = i;
                }
            }

            double between;
            double second;
            //minimum distance at first point
            if (minIndex == 0)
            {
                between = Math.Sqrt(Math.Pow(pointList[0].X - pointList[1].X, 2) + Math.Pow(pointList[0].Y - pointList[1].Y, 2));
                second = Math.Sqrt(Math.Pow(pointList[1].X - X, 2) + Math.Pow(pointList[1].Y - Y, 2));
                if (Math.Pow(second, 2) > Math.Pow(between, 2) + Math.Pow(minDist, 2))
                {
                    stnSeries = "{" + pointList[0].stnSeries.ToUpper() + "}";
                    stn = pointList[0].stn;
                    meas = pointList[0].meas;
                    MP = pointList[0].MP;
                }
                else
                {
                    stnSeries = this.interpolateOut(0, 1, X, Y, out stn, out meas, out MP);
                }
            }
            //minimum distance at last point
            else if (minIndex == pointList.Count - 1)
            {
                between = Math.Sqrt(Math.Pow(pointList[pointList.Count - 1].X - pointList[pointList.Count - 2].X, 2) +
                    Math.Pow(pointList[pointList.Count - 1].Y - pointList[pointList.Count - 2].Y, 2));
                second = Math.Sqrt(Math.Pow(pointList[pointList.Count - 1].X - X, 2) + Math.Pow(pointList[pointList.Count - 2].Y - Y, 2));
                if (Math.Pow(second, 2) > Math.Pow(between, 2) + Math.Pow(minDist, 2))
                {
                    stnSeries = "{" + pointList[pointList.Count - 1].stnSeries.ToUpper() + "}";
                    stn = pointList[pointList.Count - 1].stn;
                    meas = pointList[pointList.Count - 1].meas;
                    MP = pointList[pointList.Count - 1].MP;
                }
                else
                {
                    stnSeries = this.interpolateOut(pointList.Count - 1, pointList.Count - 2, X, Y, out stn, out meas, out MP);
                }
            }
            //minimum distance somewhere in the middle
            else
            {
                double lowDist = Math.Sqrt(Math.Pow(pointList[minIndex - 1].X - X, 2) + Math.Pow(pointList[minIndex - 1].Y - Y, 2));
                double lowInterval = Math.Sqrt(Math.Pow(pointList[minIndex].X - pointList[minIndex - 1].X, 2) + 
                    Math.Pow(pointList[minIndex].Y - pointList[minIndex - 1].Y, 2));

                if (Math.Pow(lowDist, 2) < (Math.Pow(lowInterval, 2) + Math.Pow(minDist, 2)))
                    secondIndex = minIndex - 1;
                else
                    secondIndex = minIndex + 1;



                stnSeries = this.interpolateOut(minIndex, secondIndex, X, Y, out stn, out meas, out MP);

            }
            return stnSeries;
        }

        /// <summary>
        /// Helper method to interpolate values between two indices in the point list
        /// </summary>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        /// <param name="X">Lon</param>
        /// <param name="Y">Lat</param>
        /// <param name="stn"></param>
        /// <param name="meas"></param>
        /// <param name="mp"></param>
        /// <returns>Station Series event id</returns>
        private string interpolateOut(int index1, int index2, double X, double Y, out double stn, out double meas, out double mp)
        {
            //swap if necessary
            if (index1 > index2)
            {
                int temp = index1;
                index1 = index2;
                index2 = temp;
            }

            double pointDist1 = Math.Sqrt(Math.Pow(pointList[index1].X - X, 2) + Math.Pow(pointList[index1].Y - Y, 2));
            double pointDist2 = Math.Sqrt(Math.Pow(pointList[index2].X - X, 2) + Math.Pow(pointList[index2].Y - Y, 2));
            double degInterval = Math.Sqrt(Math.Pow(pointList[index1].X - pointList[index2].X, 2) + Math.Pow(pointList[index1].Y - pointList[index2].Y, 2));

            if (pointDist1 == 0)
            {
                stn = pointList[index1].stn;
                meas = pointList[index1].meas;
                mp = pointList[index1].MP;
                return pointList[index1].stnSeries;
            }

            if (pointDist2 == 0)
            {
                stn = pointList[index2].stn;
                meas = pointList[index2].meas;
                mp = pointList[index2].MP;
                return pointList[index2].stnSeries;
            }


            double cosB = (pointDist1 * pointDist1 + degInterval * degInterval - pointDist2 * pointDist2) / (2 * pointDist1 * degInterval);
            //Console.WriteLine(cosB);
            double d1 = pointDist1 * cosB;
            double d2 = degInterval - d1;

  

            //if the indices are on different station series, defer to the first
            if (pointList[index1].stnSeries != pointList[index2].stnSeries)
            {
                double tempStn = pointList[index1].stn + pointList[index2].meas - pointList[index1].meas;
                stn = ((degInterval - d1) / degInterval) * pointList[index1].stn + ((degInterval - d2) / degInterval) * tempStn;
                Console.WriteLine("different next");
            }
            else
            {
                stn = ((degInterval - d1) / degInterval) * pointList[index1].stn + ((degInterval - d2) / degInterval) * pointList[index2].stn;
            }

           

            meas = ((degInterval - d1) / degInterval) * pointList[index1].meas + ((degInterval - d2) / degInterval) * pointList[index2].meas;
            mp = ((degInterval - d2) / degInterval) * pointList[index1].MP + ((degInterval - d2) / degInterval) * pointList[index2].MP;
            return "{" + pointList[index1].stnSeries.ToUpper() + "}";
        }

        /// <summary>
        /// Update the valve measures
        /// </summary>
        private void updateValveMeasure()
        {
            for (int i = 1; i < this.valveList.Count; i++)
            {
                double stn;
                double meas;
                double mp;

                this.getLocation(this.valveList[i].x, this.valveList[i].y, out stn, out meas, out mp);
                this.valveList[i].measure = meas;
            }
            //Console.WriteLine(this.valveList.Count);
        }

        /// <summary>
        /// Get the stationing and location from the MP
        /// </summary>
        /// <param name="MP"></param>
        /// <param name="meas"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Z"></param>
        /// <returns></returns>
        public double getStnFromMP(double MP, out double meas, out double X, out double Y, out double Z)
        {
            int index1 = -1;
            int index2 = -1;
            double minDiff = 10E100;

            //return the low stationing if the MP is less than that of the first point
            if (MP < pointList[0].MP)
            {
                X = pointList[0].X;
                Y = pointList[0].Y;
                Z = pointList[0].Z;
                meas = pointList[0].meas;
                return pointList[0].MP;
            }

            //return the high stationing if the MP is greater than that of the last point in the list
            if (MP > pointList[pointList.Count - 1].MP)
            {
                X = pointList[pointList.Count - 1].X;
                Y = pointList[pointList.Count - 1].Y;
                Z = pointList[pointList.Count - 1].Z;
                meas = pointList[pointList.Count - 1].meas;
                return pointList[pointList.Count - 1].MP;
            }

            //iterate over the points
            for (int i = 0; i < pointList.Count; i++)
            {
                if (Math.Abs(pointList[i].MP - MP) < minDiff)
                {
                    minDiff = Math.Abs(pointList[i].MP - MP);
                    index1 = i;
                }
            }
            
            //get the index of the second closest point
            index2 = (MP > pointList[index1].MP ? index1 + 1 : index1 - 1);

            //swap the indices if necessary to be sure that index1 is less
            if (pointList[index1].MP > pointList[index2].MP)
            {
                index1--;
                index2++;
            }

            //get the mile post interval
            double interval = Math.Abs(pointList[index2].MP - pointList[index1].MP);

            //get stationing for the high and low index
            double stn1 = pointList[index1].stn;
            double stn2 = (pointList[index1].stnSeries == pointList[index2].stnSeries ?
                pointList[index2].stn :
                pointList[index1].stn + pointList[index2].meas - pointList[index1].meas);

            //interpolate the x and y values
            double interpValue = (MP - pointList[index1].MP) / (pointList[index2].MP - pointList[index1].MP);

            X = pointList[index1].X + interpValue * (pointList[index2].X - pointList[index1].X);
            Y = pointList[index1].Y + interpValue * (pointList[index2].Y - pointList[index1].Y);
            Z = pointList[index1].Z + interpValue * (pointList[index2].Z - pointList[index1].Z);
            meas = pointList[index1].meas + interpValue * (pointList[index2].meas - pointList[index1].meas); 
            
            //return the interpolated stationing
            return (1 - Math.Abs(pointList[index1].MP - MP) / interval) * stn1 +  (1 - Math.Abs(pointList[index2].MP - MP) / interval) * stn2;
        }
        
        /// <summary>
        /// Get an estimated mile post from startioning
        /// </summary>
        /// <param name="stn"></param>
        /// <param name="meas"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Z"></param>
        /// <returns>mile post</returns>
        public double getMPFromStn(double stn, out double meas, out double X, out double Y, out double Z)
        {
            double minDiff = 10E100;
            int minIndex = -1;
            X = 10;
            Y = 10;

            if (stn < pointList[0].stn)
            {
                X = pointList[0].X;
                Y = pointList[0].Y;
                Z = pointList[0].Z;
                meas = pointList[0].meas;
                return pointList[0].MP;
            }

            if (stn > pointList[pointList.Count - 1].stn)
            {
                X = pointList[pointList.Count - 1].X;
                Y = pointList[pointList.Count - 1].Y;
                Z = pointList[pointList.Count - 1].Z;
                meas = pointList[pointList.Count - 1].meas;
                return pointList[pointList.Count - 1].MP;
            }

            for (int i = 1; i < pointList.Count - 2; i++)
            {
                if (pointList[i].isDiversion)
                    continue;
                if (Math.Abs(pointList[i].stn - stn) < minDiff)
                {
                    minDiff = Math.Abs(pointList[i].stn - stn);
                    minIndex = i;
                }
            }

            int secondIndex = (stn < pointList[minIndex].stn ? minIndex - 1 : minIndex + 1);

            if (secondIndex < minIndex)
            {
                int tempInd = minIndex;
                minIndex = secondIndex;
                secondIndex = tempInd;
            }

            double multFactor = (stn - pointList[minIndex].stn) / (pointList[secondIndex].stn - pointList[minIndex].stn);

            X = pointList[minIndex].X + multFactor * (pointList[secondIndex].X - pointList[minIndex].X);
            Y = pointList[minIndex].Y + multFactor * (pointList[secondIndex].Y - pointList[minIndex].Y);
            Z = pointList[minIndex].Z + multFactor * (pointList[secondIndex].Z - pointList[minIndex].Z);
            meas = pointList[minIndex].meas + multFactor * (pointList[secondIndex].meas - pointList[minIndex].meas);
            return pointList[minIndex].MP + multFactor * (pointList[secondIndex].MP - pointList[minIndex].MP);
        }


        /// <summary>
        /// Return the stationing given a measure value
        /// </summary>
        /// <param name="measure"></param>
        /// <param name="MP"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Z"></param>
        /// <returns>stationing</returns>
        public double getStnMPFromMeasure(double measure, out double MP, out double X, out double Y, out double Z)
        {
            //if the measure is out of bounds, bail out
            if (measure < this.pointList[0].meas || measure > this.pointList[this.pointList.Count - 1].meas)
            {
                MP = -1;
                X = -1;
                Y = -1;
                Z = -1;
                return -1;
            }

            int lowIndex = -1;
            int highIndex = -1;
            
            for (int i = 1; i < this.pointList.Count; i++)
            {
                if (measure == this.pointList[i].meas)
                {
                    MP = this.pointList[i].MP;
                    X = this.pointList[i].X;
                    Y = this.pointList[i].Y;
                    Z = this.pointList[i].Z;
                    return this.pointList[i].stn;
                }

                if (this.pointList[i].meas > measure)
                {
                    lowIndex = i - 1;
                    highIndex = i;
                    break;
                }
            }

            double interpolateFactor = (measure - this.pointList[lowIndex].meas) / (this.pointList[highIndex].meas - this.pointList[lowIndex].meas);

            MP = this.pointList[lowIndex].MP + interpolateFactor * (this.pointList[highIndex].MP - this.pointList[lowIndex].MP);
            X = this.pointList[lowIndex].X + interpolateFactor * (this.pointList[highIndex].X - this.pointList[lowIndex].X);
            Y = this.pointList[lowIndex].Y + interpolateFactor * (this.pointList[highIndex].Y - this.pointList[lowIndex].Y);
            Z = this.pointList[lowIndex].Z + interpolateFactor * (this.pointList[highIndex].Z - this.pointList[lowIndex].Z);
            return this.pointList[lowIndex].stn + interpolateFactor * (this.pointList[highIndex].stn - this.pointList[lowIndex].stn);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="station"></param>
        /// <param name="mp"></param>
        /// <param name="throughPutbbld"></param>
        /// <param name="minutesToClose"></param>
        /// <param name="remoteOnly"></param>
        /// <returns></returns>
        public VolumeOutResult getVolumeOut(double? station = null, double? mp = null,
            double? throughPutbbld = null, double? minutesToClose = null, bool remoteOnly = false)
        {
            if (remoteOnly && (throughPutbbld == null || minutesToClose == null)){
                throw new Exception("if remote only is flagged, inputs must be provided for throughput and time to close");
            }

            double cubicFeetToBarrels = 0.178107619047619;
            Enbridge.LinearReferencing.VolumeOutResult result = new VolumeOutResult();

            result.dynamicContribution = 0;
            if (remoteOnly)
            {
                result.dynamicContribution = Math.Round((double)(throughPutbbld * (minutesToClose / (24 * 60))), 2);
            }

            //Process the valves according to inputs
            List<Valve> valvesToUseList = new List<Valve>();

            for (int i = 0; i < this.valveList.Count; i++)
            {
                //append all if not remote only flagged
                if (!remoteOnly)
                {
                    valvesToUseList.Add(this.valveList[i]);
                }
                //otherwise just use the remote valves
                else if (remoteOnly && this.valveList[i].functionCL == "Remote")
                {
                    valvesToUseList.Add(this.valveList[i]);
                }
            }

            double leak_stn;
            double leak_measure;
            double leak_mp;
            double leak_x;
            double leak_y;
            double leak_z;

            int highIndex = -1;
            int lowIndex = -1;

            //get the measure value based on stationing or milepost input
            if (station == null && mp == null)
            {
                return null;
            }

            if (station == null)
            {
                //Use the input MP
                if ((double)mp < this.pointList[0].MP || (double)mp > this.pointList[this.pointList.Count - 1].MP)
                {
                    return null;
                }

                leak_stn = this.getStnFromMP((double)mp, out leak_measure, out leak_x, out leak_y, out leak_z);
                result.ruptureLocation["MP"] = Math.Round((double)mp, 2);
                result.ruptureLocation["Station"] = Math.Round(leak_stn, 1);
                result.ruptureLocation["X"] = Math.Round(leak_x, 5);
                result.ruptureLocation["Y"] = Math.Round(leak_y, 5);
                result.ruptureLocation["Z"] = Math.Round(leak_z, 2);
            }
            else
            {
                //Use the input stationing
                if ((double)station < this.pointList[0].stn || 
                    (double)station > this.pointList[this.pointList.Count - 1].stn)
                {
                    return null;
                }

                leak_mp = this.getMPFromStn((double)station, out leak_measure, out leak_x, out leak_y, out leak_z);
                result.ruptureLocation["MP"] = Math.Round(leak_mp, 2);
                result.ruptureLocation["Station"] =  Math.Round((double)station, 1);
                result.ruptureLocation["X"] =  Math.Round(leak_x, 5);
                result.ruptureLocation["Y"] = Math.Round(leak_y, 5);
                result.ruptureLocation["Z"] = Math.Round(leak_z, 2);
            }

            //get lower and upper bounding indices
            for (int i = 0; i < this.pointList.Count; i++)
            {
                if (this.pointList[i].meas > leak_measure)
                {
                    highIndex = i;
                    lowIndex = i - 1;
                    break;
                }
            }

            //Get Bounds by end of line or valve
            double upstreamMeasureBound = 0;
            double downstreamMeasureBound = 0;

            //Get the lowLimit, start of line or previous section valve
            if (leak_measure < valvesToUseList[0].measure)
            {
                upstreamMeasureBound = pointList[0].meas;
            }
            else
            {
                for (int i = 1; i < valvesToUseList.Count; i++)
                {
                    if (valvesToUseList[i].measure > leak_measure)
                    {
                        upstreamMeasureBound = valvesToUseList[i - 1].measure;
                        break;
                    }
                }
            }

            //Get the highLimit, end of line or next section valve
            if (leak_measure > valvesToUseList[valvesToUseList.Count - 1].measure)
            {
                downstreamMeasureBound = pointList[pointList.Count - 1].meas;
            }
            else
            {
                for (int i = 0; i < valvesToUseList.Count; i++)
                {
                    if (valvesToUseList[i].measure > leak_measure)
                    {
                        downstreamMeasureBound = valvesToUseList[i].measure;
                        break;
                    }
                }
            }

            double upstreamMP, upstreamX, upstreamY, upstreamZ, upstreamBoundStn;
            upstreamBoundStn = this.getStnMPFromMeasure(upstreamMeasureBound, out upstreamMP, out upstreamX, out upstreamY, out upstreamZ);

            //Add upstream boundary properties to result object
            result.upstreamBound["MP"] = Math.Round(upstreamMP, 2);
            result.upstreamBound["Measure"] = Math.Round(upstreamMeasureBound, 1);
            result.upstreamBound["Station"] = Math.Round(upstreamBoundStn, 1);
            result.upstreamBound["X"] = Math.Round(upstreamX, 5);
            result.upstreamBound["Y"] = Math.Round(upstreamY, 5);
            result.upstreamBound["Z"] = Math.Round(upstreamZ, 2);

            double downstreamMP, downstreamX, downstreamY, downstreamZ, downstreamBoundStn;
            downstreamBoundStn = this.getStnMPFromMeasure(downstreamMeasureBound, out downstreamMP, out downstreamX, out downstreamY, out downstreamZ);

            result.downstreamBound["MP"] = Math.Round(downstreamMP, 2);
            result.downstreamBound["Measure"] = Math.Round(downstreamMeasureBound, 1);
            result.downstreamBound["Station"] = Math.Round(downstreamBoundStn, 1);
            result.downstreamBound["X"] = Math.Round(downstreamX, 5);
            result.downstreamBound["Y"] = Math.Round(downstreamY, 5);
            result.downstreamBound["Z"] = Math.Round(downstreamZ, 2);

            //estimate total upstream and downstream contributions assuming constant diameter, wall thickness
            double assumedFootRadius = (pointList[lowIndex].diameter - 2 * pointList[lowIndex].wallThickness) / (2 * 12);
            double assumedfootArea = Math.Pow(assumedFootRadius, 2) * Math.PI;

            double assumedUpstreamVolume = assumedfootArea * (leak_measure - upstreamMeasureBound);

            double assumedDownstreamVolume = assumedfootArea * (downstreamMeasureBound - leak_measure);

            result.upstreamContribution["total"] = Math.Round(assumedUpstreamVolume, 2);
            result.downstreamContribution["total"] = Math.Round(assumedDownstreamVolume, 2);


            result.vAssumed = assumedUpstreamVolume + assumedDownstreamVolume;

            //Get upstream contribution
            double upstreamContribution = 0;
            double upstreamStorage = 0;
            double trackUpstream = leak_measure;
            double trackUpstreamMaxElev = leak_z;
            double trackUpstreamBreak = leak_measure;
            int trackUpstreamLowIndex = lowIndex;
            int trackUpstreamHighIndex = highIndex;

            while (true)
            {
                //Trigger the break if decrementing by one is less than the low measure and the 
                //measure of the low bound index point is less than the low measure too
                if (trackUpstreamLowIndex < 0)
                {
                    break;
                }

                if (trackUpstream - 1 < upstreamMeasureBound && this.pointList[trackUpstreamLowIndex].meas < upstreamMeasureBound)
                {
                    break;
                }

                double incrementalVolume;
                double footradius = (pointList[trackUpstreamLowIndex].diameter - 2 * pointList[trackUpstreamLowIndex].wallThickness) / (2 * 12);
                double footArea = Math.Pow(footradius, 2) * Math.PI;

                if (trackUpstream - 1 >= pointList[trackUpstreamLowIndex].meas)
                {
                    trackUpstream = trackUpstream - 1;
                    incrementalVolume = footArea * 1; //incdicates one foot increment

                    double interpElev = pointList[trackUpstreamLowIndex].Z +
                       ((trackUpstream - pointList[trackUpstreamLowIndex].meas) / (pointList[trackUpstreamHighIndex].meas - pointList[trackUpstreamLowIndex].meas)) *
                       (pointList[trackUpstreamHighIndex].Z - pointList[trackUpstreamLowIndex].Z);


                    if (interpElev > trackUpstreamMaxElev)
                    {
                        trackUpstreamMaxElev = interpElev;
                        trackUpstreamBreak = trackUpstream;
                        upstreamContribution += incrementalVolume;
                    }
                    else
                    {
                        upstreamStorage += incrementalVolume;
                    }
                                       
                    
                }
                else
                {
                    incrementalVolume = (trackUpstream - pointList[trackUpstreamLowIndex].meas) * footArea;
                    if (pointList[trackUpstreamLowIndex].Z > trackUpstreamMaxElev)
                    {
                        trackUpstreamMaxElev = pointList[trackUpstreamLowIndex].Z;
                        trackUpstreamBreak = pointList[trackUpstreamLowIndex].meas;
                        upstreamContribution += incrementalVolume;
                    }
                    else
                    {
                        upstreamStorage += incrementalVolume;
                    }
                    trackUpstream = pointList[trackUpstreamLowIndex].meas;
                    trackUpstreamLowIndex--;
                    trackUpstreamHighIndex--;
                }
            }

            result.upstreamContribution["out"] = Math.Round(upstreamContribution, 2);
            result.upstreamContribution["storage"] = Math.Round(upstreamStorage, 2);

            //Get the upstream break properties
            double upBreakMP, upBreakX, upBreakY, upBreakZ, upBreakStn;
            upBreakStn = this.getStnMPFromMeasure(trackUpstreamBreak, out upBreakMP, out upBreakX, out upBreakY, out upBreakZ);

            result.upstreamBreak["MP"] = Math.Round(upBreakMP, 2);
            result.upstreamBreak["Station"] = Math.Round(upBreakStn, 1);
            result.upstreamBreak["Measure"] = Math.Round(trackUpstreamBreak, 1);
            result.upstreamBreak["X"] = Math.Round(upBreakX, 5);
            result.upstreamBreak["Y"] = Math.Round(upBreakY, 5);
            result.upstreamBreak["Z"] = Math.Round(upBreakZ, 2);

            //Get downstream contribution
            double downstreamContribution = 0;
            double downstreamStorage = 0;
            double trackDownstream = leak_measure;
            double trackDownstreamMaxElev = leak_z;
            double trackDownstreamBreak = leak_measure;
            int trackDownstreamLowIndex = lowIndex;
            int trackDownstreamHighIndex = highIndex;

            while (true)
            {
                //Trigger the break if decrementing by one is less than the low measure and the 
                //measure of the low bound index point is less than the low measure too
                if (trackDownstreamHighIndex > pointList.Count - 1)
                {
                    break;
                }

                if (trackDownstream + 1 > downstreamMeasureBound && pointList[trackDownstreamHighIndex].meas > downstreamMeasureBound)
                {
                    break;
                }

                double incrementalVolume;
                double footradius = (pointList[trackDownstreamHighIndex].diameter - 2 * pointList[trackDownstreamHighIndex].wallThickness) / (2 * 12);
                double footArea = Math.Pow(footradius, 2) * Math.PI;

                if (trackDownstream + 1 <= pointList[trackDownstreamHighIndex].meas)
                {
                    trackDownstream = trackDownstream + 1;
                    incrementalVolume = footArea * 1; //incdicates one foot increment

                    double interpElev = pointList[trackDownstreamLowIndex].Z +
                       ((trackDownstream - pointList[trackDownstreamLowIndex].meas) / (pointList[trackDownstreamHighIndex].meas - pointList[trackDownstreamLowIndex].meas)) *
                       (pointList[trackDownstreamHighIndex].Z - pointList[trackDownstreamLowIndex].Z);


                    if (interpElev > trackDownstreamMaxElev)
                    {
                        trackDownstreamMaxElev = interpElev;
                        trackDownstreamBreak = trackDownstream;
                        downstreamContribution += incrementalVolume;
                    }
                    else
                    {
                        downstreamStorage += incrementalVolume;
                    }
                }
                else
                {
                    incrementalVolume = (pointList[trackDownstreamHighIndex].meas - trackDownstream) * footArea;
                    if (pointList[trackDownstreamHighIndex].Z > trackDownstreamMaxElev)
                    {
                        trackDownstreamMaxElev = pointList[trackDownstreamHighIndex].Z;
                        trackDownstreamBreak = pointList[trackDownstreamHighIndex].meas;
                        downstreamContribution += incrementalVolume;
                    }
                    else
                    {
                        downstreamStorage += incrementalVolume;
                    }
                    trackDownstream = pointList[trackDownstreamHighIndex].meas;
                    trackDownstreamLowIndex++;
                    trackDownstreamHighIndex++;
                }
            }


            result.downstreamContribution["out"] = Math.Round(downstreamContribution, 2);
            result.downstreamContribution["storage"] = Math.Round(downstreamStorage, 2);

            double downBreakMP, downBreakX, downBreakY, downBreakZ, downBreakStn;
            downBreakStn = this.getStnMPFromMeasure(trackDownstreamBreak, out downBreakMP, out downBreakX, out downBreakY, out downBreakZ);

            result.downstreamBreak["MP"] = Math.Round(downBreakMP, 2);
            result.downstreamBreak["Station"] = Math.Round(downBreakStn, 1);
            result.downstreamBreak["Measure"] = Math.Round(trackDownstreamBreak, 1);
            result.downstreamBreak["X"] = Math.Round(downBreakX, 5);
            result.downstreamBreak["Y"] = Math.Round(downBreakY, 5);
            result.downstreamBreak["Z"] = Math.Round(downBreakZ, 2);

            result.vStorage = downstreamStorage + upstreamStorage;
            result.vOut = downstreamContribution + upstreamContribution;
            result.vTotal = downstreamStorage + upstreamStorage + downstreamContribution + upstreamContribution;
            result.percentAccounted = (result.vTotal / result.vAssumed) * 100;

            //Convert to bbls
            result.vAssumed = Math.Round(result.vAssumed * cubicFeetToBarrels, 2);
            result.vStorage = Math.Round(result.vStorage * cubicFeetToBarrels, 2);
            result.vOut = Math.Round(result.vOut * cubicFeetToBarrels, 2);
            result.vTotal = Math.Round(result.vTotal * cubicFeetToBarrels, 2);

            result.upstreamContribution["total"] = Math.Round(result.upstreamContribution["total"] * cubicFeetToBarrels, 2);
            result.upstreamContribution["out"] = Math.Round(result.upstreamContribution["out"] * cubicFeetToBarrels, 2);
            result.upstreamContribution["storage"] = Math.Round(result.upstreamContribution["storage"] * cubicFeetToBarrels, 2);
            result.downstreamContribution["total"] = Math.Round(result.downstreamContribution["total"] * cubicFeetToBarrels, 2);
            result.downstreamContribution["out"] = Math.Round(result.downstreamContribution["out"] * cubicFeetToBarrels, 2);
            result.downstreamContribution["storage"] = Math.Round(result.downstreamContribution["storage"] * cubicFeetToBarrels, 2);

            result.makeExtent();
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="startMeas"></param>
        /// <param name="endMeas"></param>
        /// <param name="stnStart"></param>
        /// <param name="stnEnd"></param>
        /// <returns>linestring representation of the geometry</returns>
        public string makeSegmentLineString(double startMeas, double endMeas, out double stnStart, out double stnEnd)
        {
            int lowIndex = -1;
            int highIndex = -1;

            for (int i = 1; i < this.pointList.Count; i++)
            {
                if (this.pointList[i].meas > startMeas)
                {
                    lowIndex = i - 1;
                    highIndex = i;
                    break;
                }
            }

            if (lowIndex == -1)
            {
                throw new Exception("out of range");
            }

            while (true)
            {
                //Console.WriteLine("here");
                if (this.pointList[highIndex].meas > endMeas)
                {
                    break;
                }
                highIndex++;
            }

            List<string> coordsList = new List<string>();

            double X, Y, MP, Z;
            stnStart = this.getStnMPFromMeasure(startMeas, out MP, out X, out Y, out Z);

            coordsList.Add(string.Format("{0} {1} {2} {3}", X, Y, Z, startMeas));


            for (int i = lowIndex + 1; i < highIndex; i++)
            {
                coordsList.Add(string.Format("{0} {1} {2} {3}", this.pointList[i].X, this.pointList[i].Y, this.pointList[i].Z, this.pointList[i].meas));
            }

            stnEnd = this.getStnMPFromMeasure(endMeas, out MP, out X, out Y, out Z);
            coordsList.Add(string.Format("{0} {1} {2} {3}", X, Y, Z, endMeas));
            return "LINESTRING (" + string.Join(",", coordsList) + ")";
        }
    }
}

