using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Enbridge.DepthOfCover
{
    /// <summary>
    /// 
    /// </summary>
    public static class UpdateDocPointsSegments
    {
        /// <summary>
        /// updates segments based on all points
        /// </summary>
        public static void MakeSegments()
        {
            List<string> routeEventIds = new List<string>();

            using (SqlConnection conn = new SqlConnection(AppConstants.CONN_STRING_DOC))
            {
                conn.Open();
                SqlCommand comm = conn.CreateCommand();
                comm.CommandText = "EXEC sde.set_current_version 'sde.WORKING';";
                comm.CommandText += "Select Distinct RouteEventID from sde.DEPTHOFCOVER_EVW WHERE Status='Active';";

                try
                {
                    SqlDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        routeEventIds.Add(reader[0].ToString());
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }

            foreach (string id in routeEventIds)
            {
                List<double> pointMeasureList = new List<double>();
                List<double> pointMeasurementList = new List<double>();
                List<string> pointEventIdList = new List<string>();
                List<string> seriesEventIdList = new List<string>();

                string routeId = id.ToUpper();
                Console.WriteLine(routeId);

                UpdatePoints(routeId);
                continue;

                using (SqlConnection conn = new SqlConnection(AppConstants.CONN_STRING_DOC))
                {
                    conn.Open();
                    SqlCommand comm = conn.CreateCommand();
                    comm.CommandText = "EXEC sde.set_current_version 'sde.WORKING';";
                    comm.CommandText += "Select EventID, SeriesEventID, Measure, Measurement from sde.DEPTHOFCOVER_EVW ";
                    comm.CommandText += "WHERE Status='Active' AND RouteEventID = @routeId ";
                    comm.CommandText += "Order by Measure ASC;";
                    comm.Parameters.AddWithValue("routeId", routeId);

                    try
                    {
                        SqlDataReader reader = comm.ExecuteReader();
                        while (reader.Read())
                        {
                            double measure;
                            double measurement;

                            if (!Double.TryParse(reader["Measure"].ToString(), out measure))
                            {
                                continue;
                            }
                            if (!Double.TryParse(reader["Measurement"].ToString(), out measurement))
                            {
                                continue;
                            }

                            pointEventIdList.Add(reader["EventID"].ToString());
                            seriesEventIdList.Add(reader["SeriesEventID"].ToString());
                            pointMeasureList.Add(measure);
                            pointMeasurementList.Add(measurement);
                        }
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }

                List<double[]> segList = new List<double[]>();

                double firstStart = pointMeasureList[0] - 300;
                double firstEnd = (pointMeasureList[0] + pointMeasureList[1]) / 2;

                firstEnd = (firstEnd - pointMeasureList[0] > 300 ? pointMeasureList[0] + 300 : firstEnd);

                //Console.WriteLine("{0} {1} {2} {3} {4}", pointMeasureList[0], firstStart, firstEnd, pointMeasureList[0] - firstStart, firstEnd - pointMeasureList[0]);

                segList.Add(new double[3] { firstStart, firstEnd, pointMeasurementList[0] });

                for (int i = 1; i < pointMeasurementList.Count - 1; i++)
                {
                    double startMeasure = (pointMeasureList[i - 1] + pointMeasureList[i]) / 2;
                    startMeasure = (pointMeasureList[i] - startMeasure > 300 ? pointMeasureList[i] - 300 : startMeasure);

                    double endMeasure = (pointMeasureList[i] + pointMeasureList[i + 1]) / 2;
                    endMeasure = (endMeasure - pointMeasureList[i] > 300 ? pointMeasureList[i] + 300 : endMeasure);

                    //Console.WriteLine("{0} {1} {2} {3} {4}", pointMeasureList[i], startMeasure, endMeasure, pointMeasureList[i] - startMeasure, endMeasure - pointMeasureList[i]);
                    //return;
                    segList.Add(new double[3] {startMeasure, endMeasure, pointMeasurementList[i]});
                }

                double lastStart = (pointMeasureList[pointMeasureList.Count - 1] + pointMeasureList[pointMeasureList.Count - 2]) / 2;
                lastStart = (pointMeasureList[pointMeasureList.Count - 1] - lastStart > 300 ? pointMeasureList[pointMeasureList.Count - 1] - 300 : lastStart);
                double lastEnd = pointMeasureList[pointMeasureList.Count - 1] + 300;

                segList.Add(new double[3] { lastStart, lastEnd, pointMeasurementList[pointMeasurementList.Count - 1] });

                Enbridge.LinearReferencing.ContLineLocatorSQL locator = new Enbridge.LinearReferencing.ContLineLocatorSQL(routeId);

                double minMeasure = locator.pointList[0].meas;
                double maxMeasure = locator.pointList[locator.pointList.Count - 1].meas;

                List<string> valuesList = new List<string>();

                for (int i = 0; i < segList.Count; i++)
                {
                    List<string> individualParams = new List<string>();
                    //Console.WriteLine("{0} {1} {2}", segList[i][0], segList[i][1], segList[i][2]);
                    segList[i][0] = (segList[i][0] < minMeasure ? minMeasure : segList[i][0]);
                    segList[i][1] = (segList[i][1] > maxMeasure ? maxMeasure : segList[i][1]);
                    double startStn, endStn;
                    string geomString = locator.makeSegmentLineString(segList[i][0], segList[i][1], out startStn, out endStn);
                    //geometry
                    individualParams.Add(string.Format("geometry::STGeomFromText('{0}', 4326)", geomString));
                    //start stationing       
                    individualParams.Add(startStn.ToString());
                    //start stationing       
                    individualParams.Add(endStn.ToString());
                    string guid = "'" + Guid.NewGuid().ToString().ToUpper() + "'";
                    //eventid       
                    individualParams.Add(guid);
                    //origineventid       
                    individualParams.Add(guid);

                    string dte = "'" + DateTime.Now.ToString("yyyyddmm") + "'";
                    //created date
                    individualParams.Add(dte);
                    //modified date
                    individualParams.Add(dte);

                    //createdby
                    individualParams.Add("'SUPGIS01'");
                    //modifiedby
                    individualParams.Add("'SUPGIS01'");

                    //series eventid
                    individualParams.Add(string.Format("'{0}'", seriesEventIdList[i]));

                    //Measurement
                    individualParams.Add(string.Format("{0}", pointMeasurementList[i]));

                    //PointId
                    individualParams.Add(string.Format("'{0}'", pointEventIdList[i]));

                    //route eventid
                    individualParams.Add(string.Format("'{0}'", routeId));

                    valuesList.Add("(" + string.Join(",", individualParams) + ")");
                }

                using (SqlConnection conn = new SqlConnection(AppConstants.CONN_STRING_DOC))
                {
                    conn.Open();
                    SqlCommand comm = conn.CreateCommand();
                    comm.CommandText = "EXEC sde.set_current_version 'sde.WORKING';";
                    comm.CommandText += "EXEC sde.edit_version 'sde.WORKING', 1;";
                    comm.CommandText += "BEGIN TRANSACTION;";
                    comm.CommandText += "DELETE FROM sde.DEPTHOFCOVERSEGMENTS_EVW WHERE RouteEventID = @routeId OR RouteEventID IS NULL;";

                    comm.CommandText += "INSERT INTO sde.DEPTHOFCOVERSEGMENTS_EVW ";
                    comm.CommandText += "(";
                    comm.CommandText += "SHAPE, StartStationing, EndStationing, EventID, OriginEventID, CreatedDate, LastModified, ";
                    comm.CommandText += "CreatedBy, ModifiedBy, SeriesEventID, Measurement, PointEventID, RouteEventID";
                    comm.CommandText += ") ";
                    comm.CommandText += "VALUES ";
                    comm.CommandText += string.Join(",", valuesList) + ";";
                    comm.CommandText += "COMMIT;";

                    comm.CommandText += "EXEC sde.edit_version 'sde.WORKING', 2;";
                    comm.Parameters.AddWithValue("@routeId", routeId);
                    try
                    {
                        comm.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        comm.Dispose();
                        conn.Close();
                        Console.WriteLine("exited");
                    }
                }
            }
        }

        private static void UpdatePoints(string routeId)
        {
            using (SqlConnection conn = new SqlConnection(AppConstants.CONN_STRING_DOC))
            {
                conn.Open();
                SqlCommand comm = conn.CreateCommand();
                comm.CommandText = "EXEC sde.set_current_version 'sde.WORKING';";
                comm.CommandText += "Select EventID, SeriesEventID, Measure, Measurement from sde.DEPTHOFCOVER_EVW ";
                comm.CommandText += "WHERE Status='Active' AND RouteEventID = @routeId ";
                comm.CommandText += "Order by Measure ASC;";
                comm.Parameters.AddWithValue("routeId", routeId);

                try
                {
                    SqlDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        double measure;
                        double measurement;
                        Console.WriteLine(reader["Measure"]);
                        //if (!Double.TryParse(reader["Measure"].ToString(), out measure))
                        //{
                        //    continue;
                        //}
                        //if (!Double.TryParse(reader["Measurement"].ToString(), out measurement))
                        //{
                        //    continue;
                        //}

                        //pointEventIdList.Add(reader["EventID"].ToString());
                        //seriesEventIdList.Add(reader["SeriesEventID"].ToString());
                        //pointMeasureList.Add(measure);
                        //pointMeasurementList.Add(measurement);
                    }
                    Console.WriteLine("here");
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }


        }
    }



}

