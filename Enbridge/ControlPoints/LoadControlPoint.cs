using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Data.SqlClient;

namespace Enbridge.ControlPoints
{
    public static class LoadControlPoint
    {
        public static bool loadMap(string loadedBy, string mapName, string coordString, string tempFilePath)
        {
            string tempDirectory = Path.GetDirectoryName(tempFilePath);

            string pdfWriteLocation = Path.Combine(tempDirectory, "pdfWrite.pdf");
            string thumbLocation = Path.Combine(tempDirectory, "thumb.png");
            Console.WriteLine(thumbLocation);

            //Start a process to make the thumbnail
            Process process = new Process();
            // Stop the process from opening a new window
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = AppConstants.GHOST_SCRIPT_EXE;
            process.StartInfo.Arguments = "-dSAFER -dNOPAUSE -dBATCH -sDEVICE=pdfwrite -r400 ";
            process.StartInfo.Arguments += string.Format(" -sOutputFile={0} {1}", pdfWriteLocation, tempFilePath);
            process.Start();
            process.WaitForExit();

            process.StartInfo.FileName = AppConstants.IMAGE_MAGICK_EXE;
            process.StartInfo.Arguments = string.Format("\"{0}[0]\" -resize 300x300 {1}", pdfWriteLocation, thumbLocation);
            process.Start();
            process.WaitForExit();

            //Get the file data
            byte[] fileData = null;
            byte[] thumbnailData = null;

            fileData = File.ReadAllBytes(pdfWriteLocation);
            thumbnailData = File.ReadAllBytes(thumbLocation);

            List<double> lats = new List<double>();
            List<double> longs = new List<double>();

            string[] coordsList = coordString.Split('\n');

            for (int i = 0; i < coordsList.Length; i++)
            {
                string[] pieces = coordsList[i].Split(null);
                if (pieces.Length < 2)
                {
                    continue;
                }

                lats.Add(double.Parse(pieces[0]));
                longs.Add(double.Parse(pieces[1]));
            }

            List<string> coordsStrings = new List<string>();
            for (int i = 0; i < lats.Count; i++)
            {
                coordsStrings.Add(string.Format("({0} {1})",longs[i], lats[i]));
            }

            string geomWKT = string.Format("MULTIPOINT({0})", string.Join(",",coordsStrings));

            using (SqlConnection conn = new SqlConnection(AppConstants.CONN_STRING_SUPSQL_ARCGIS))
            {
                conn.Open();
                SqlCommand comm = conn.CreateCommand();
                comm.CommandText = "DECLARE @Object_ID int;";
                comm.CommandText += "DECLARE @Num_IDs int;";
                comm.CommandText += "EXEC supgis_data.i1982_get_ids 2, 1, @Object_ID OUTPUT, @Num_IDs OUTPUT;";
                comm.CommandText += "INSERT INTO CONTROLPOINTS ";
                comm.CommandText += "(OBJECTID, EventID, Name, LoadedBy, LoadedDate, FileData, Thumbnail, Shape, Extension) ";
                comm.CommandText += "VALUES ";
                comm.CommandText += "(@Object_ID, @eventID, @name, @loadedBy, GETDATE(), @fileData, @thumbData, ";
                comm.CommandText += string.Format("geometry::STGeomFromText('{0}', 4326), ", geomWKT);
                comm.CommandText += "'.pdf');";
                comm.Parameters.AddWithValue("@eventID", "{" + Guid.NewGuid().ToString().ToUpper() + "}");
                comm.Parameters.AddWithValue("@name", mapName);
                comm.Parameters.AddWithValue("@loadedBy", loadedBy);
                comm.Parameters.AddWithValue("@fileData", fileData);
                comm.Parameters.AddWithValue("@thumbData", thumbnailData);

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
                }
            }
            return true;
        }

        public static string getControlPoints(string extents = null)
        {
            string outputGeoJson = "{\"type\": \"FeatureCollection\",\"features\": []}";
            //Create the geojson object
            Enbridge.GeoJSON.GeoJSON geoJSON = new Enbridge.GeoJSON.GeoJSON();

            using (SqlConnection conn = new SqlConnection(AppConstants.CONN_STRING_SUPSQL_ARCGIS))
            {
                conn.Open();
                SqlCommand comm = conn.CreateCommand();
                comm.CommandText = "SELECT EventID, Name, ApprovedBy, ApprovedDate, LoadedBy, LoadedDate, SHAPE.ToString() as shp from CONTROLPOINTS ";
                comm.CommandText += "WHERE ApprovedDate IS NOT NULL";
                if (string.IsNullOrEmpty(extents))
                {
                    comm.CommandText += ";";
                }
                else
                {
                    string[] extentStrings = extents.Split(null);
                    string polygonString = string.Format("POLYGON(({0} {1}, {0} {3}, {2} {3}, {2} {1}, {0} {1}))",
                        extentStrings[0], extentStrings[1], extentStrings[2], extentStrings[3]);
                    comm.CommandText += " AND SHAPE.STIntersects(geometry::STGeomFromText(@geomString, 4326)) = 1;";
                    comm.Parameters.AddWithValue("@geomString", polygonString);
                }

                try
                {
                    SqlDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        GeoJSON.JSONFeature newFeature = new GeoJSON.JSONFeature();

                        string geomString = reader["shp"] as string ?? "null";
                        Console.WriteLine(geomString);
                        newFeature.addGeom(geomString);

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string fieldName = reader.GetName(i);
                            if (fieldName.ToLower() == "shp" || fieldName.ToLower() == "shape" || fieldName.ToLower() == "thumbnail")
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

        public static string getControlPointsThumb(string controlPointEventID)
        {
            string outputString = "";
            using (SqlConnection conn = new SqlConnection(AppConstants.CONN_STRING_SUPSQL_ARCGIS))
            {
                conn.Open();
                SqlCommand comm = conn.CreateCommand();
                comm.CommandText = "SELECT Thumbnail from CONTROLPOINTS WHERE EventID = @controlPointId;";
                comm.Parameters.AddWithValue("@controlPointId", controlPointEventID);

                try
                {
                    byte[] thumbnailBuffer = (byte[])comm.ExecuteScalar();
                    outputString = Convert.ToBase64String(thumbnailBuffer);
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
            return outputString;
        }


        public static string getControlPointMapThumb(string mapEventID)
        {
            mapEventID = mapEventID.ToUpper();
            string outputString = "";

            using (SqlConnection conn = new SqlConnection(AppConstants.CONN_STRING_SUPSQL_ARCGIS))
            {
                conn.Open();
                SqlCommand comm = conn.CreateCommand();
                comm.CommandText = "Select Thumbnail from CONTROLPOINTS WHERE EventID = @mapEventID;";
                comm.Parameters.AddWithValue("@mapEventID", mapEventID);
                try
                {
                    byte[] thumbBuff = (byte[])comm.ExecuteScalar();
                    outputString = Convert.ToBase64String(thumbBuff);
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
            return outputString;
        }

        public static void approveControlPointMap(string username, string mapEventID)
        {
            mapEventID = mapEventID.ToUpper();

            using (SqlConnection conn = new SqlConnection(AppConstants.CONN_STRING_SUPSQL_ARCGIS))
            {
                conn.Open();
                SqlCommand comm = conn.CreateCommand();
                comm.CommandText = "UPDATE CONTROLPOINTS SET ApprovedBy = @username, ApprovedDate = GETDATE() WHERE EventID = @mapEventID;";
                comm.Parameters.AddWithValue("@username", username);
                comm.Parameters.AddWithValue("@mapEventID", mapEventID);
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
                }
            }
        }

        public static void discardControlPointMap(string mapEventID)
        {
            mapEventID = mapEventID.ToUpper();

            using (SqlConnection conn = new SqlConnection(AppConstants.CONN_STRING_SUPSQL_ARCGIS))
            {
                conn.Open();
                SqlCommand comm = conn.CreateCommand();
                comm.CommandText = "DELETE FROM CONTROLPOINTS WHERE EventID = @mapEventID;";
                comm.Parameters.AddWithValue("@mapEventID", mapEventID);
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
                }
            }
        }

        public static byte[] getControlPointFileData(string controlPointEvtId, out string ext)
        {
            byte[] fileData = null;
            ext = "";

            using (SqlConnection conn = new SqlConnection(AppConstants.CONN_STRING_SUPSQL_ARCGIS))
            {
                conn.Open();
                SqlCommand comm = conn.CreateCommand();
                comm.CommandText = "Select FileData, Extension from CONTROLPOINTS WHERE EventID = @contPointEvtID;";
                comm.Parameters.AddWithValue("@contPointEvtID", controlPointEvtId);
                try
                {
                    SqlDataReader reader = comm.ExecuteReader();
                    if (reader.Read())
                    {
                        fileData = (byte[])reader["FileData"];
                        ext = (string)reader["Extension"];
                    }
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
            return fileData;
        }
    }
}
