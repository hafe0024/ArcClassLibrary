using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enbridge.DepthOfCover
{
    /// <summary>
    /// 
    /// </summary>
    public static class SelectDOC
    {
        /// <summary>
        /// Get records by minimum value
        /// </summary>
        /// <param name="minDOC"></param>
        /// <param name="LLList"></param>
        /// <returns></returns>
        public static string getDOCByMin(double minDOC, string LLList)
        {    
            string outputGeoJson = "{\"type\": \"FeatureCollection\",\"features\": []}";
            //Create the geojson object
            GeoJSON.GeoJSON geoJSON = new GeoJSON.GeoJSON();

            using (SqlConnection conn = new SqlConnection(AppConstants.CONN_STRING_DOC))
            {
                conn.Open();
                SqlCommand comm = conn.CreateCommand();
                comm.CommandText = "EXEC dbo.set_current_version 'working';";
                comm.CommandText += "SELECT Shape.ToString() as shp, * FROM DepthOfCover_mv ";
                comm.CommandText += "WHERE Measurement <= @minMeas ";
                comm.CommandText += "AND HistoricalState = 'Current' ";
                comm.CommandText += "AND RouteEventID in (" + LLList + ") ORDER BY RouteEventID, Measure;";
                comm.Parameters.AddWithValue("@minMeas", minDOC);

                try
                {
                    SqlDataReader reader = comm.ExecuteReader();

                    while (reader.Read())
                    {
                        GeoJSON.JSONFeature newFeature = new GeoJSON.JSONFeature();
                        newFeature.addGeom(reader["shp"].ToString());
                        newFeature.addProperty("LineName", Enbridge.AppConstants.GetLineName("{" + reader["RouteEventID"].ToString() + "}"));

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string fieldName = reader.GetName(i);
                            if (fieldName.ToLower() == "shp" || fieldName.ToLower() == "shape")
                            {
                                continue;
                            }
                            newFeature.addProperty(fieldName, reader[fieldName]);
                        }
                        geoJSON.addFeature(newFeature);
                    }
                    outputGeoJson = geoJSON.ToString();
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
            return outputGeoJson;
        }

        /// <summary>
        /// Get records by bounds
        /// </summary>
        /// <param name="minx"></param>
        /// <param name="miny"></param>
        /// <param name="maxx"></param>
        /// <param name="maxy"></param>
        /// <param name="LLList"></param>
        /// <returns></returns>
        public static string getDOCByBounds(double minx, double miny, double maxx, double maxy, string LLList)
        {
            string outputGeoJson = "{\"type\": \"FeatureCollection\",\"features\": []}";
            //Create the geojson object
            GeoJSON.GeoJSON geoJSON = new GeoJSON.GeoJSON();

            using (SqlConnection conn = new SqlConnection(AppConstants.CONN_STRING_DOC))
            {
                conn.Open();
                SqlCommand comm = conn.CreateCommand();
                comm.CommandText = "EXEC dbo.set_current_version 'working';";
                comm.CommandText += "SELECT Shape.ToString() as shp, * FROM DepthOfCover_mv ";
                comm.CommandText += "WHERE ";
                comm.CommandText += "	POINT_X BETWEEN @minX AND @maxX ";
                comm.CommandText += "	AND POINT_Y BETWEEN @minY AND @maxY ";
                comm.CommandText += "	AND RouteEventID in (" + LLList + ") ";
                comm.CommandText += "	AND HistoricalState = 'Current' ";
                comm.CommandText += "ORDER BY RouteEventID, Measure;";
                comm.Parameters.AddWithValue("@minX", minx);
                comm.Parameters.AddWithValue("@minY", miny);
                comm.Parameters.AddWithValue("@maxX", maxx);
                comm.Parameters.AddWithValue("@maxY", maxy);

                try
                {
                    SqlDataReader reader = comm.ExecuteReader();

                    while (reader.Read())
                    {
                        GeoJSON.JSONFeature newFeature = new GeoJSON.JSONFeature();
                        newFeature.addGeom(reader["shp"].ToString());
                        newFeature.addProperty("LineName", Enbridge.AppConstants.GetLineName("{" + reader["RouteEventID"].ToString() + "}"));

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string fieldName = reader.GetName(i);
                            if (fieldName.ToLower() == "shp" || fieldName.ToLower() == "shape")
                            {
                                continue;
                            }
                            newFeature.addProperty(fieldName, reader[fieldName]);
                        }
                        geoJSON.addFeature(newFeature);
                    }
                    outputGeoJson = geoJSON.ToString();
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
            return outputGeoJson;
        }
    }
}