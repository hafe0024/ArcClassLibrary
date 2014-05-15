using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Drawing;
using System.Threading;


namespace Enbridge.Drawings
{
    /// <summary>
    /// 
    /// </summary>
    public static class DrawingsSearch
    {
        /// <summary>
        /// Clear records where the file no longer exists
        /// </summary>
        public static void clearRemoved()
        {
            List<string> removeList = new List<string>();

            using (SqlConnection conn = new SqlConnection(AppConstants.CONN_STRING_SUPSQL_ARCGIS))
            {
                SqlCommand comm = conn.CreateCommand();
                conn.Open();
                comm.CommandText = "SELECT DrawingID, Path FROM drawings;";

                try
                {
                    SqlDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        string theFilePath = reader["Path"].ToString();
                        string drawid = reader["DrawingID"].ToString();
                        Console.WriteLine(drawid);
                        
                        if (!File.Exists(theFilePath))
                        {
                            removeList.Add(drawid);
                            Console.WriteLine("## remove ##");
                        }
                    }

                    reader.Close();
                    
                    foreach (string dwgid in removeList)
                    {
                        Console.WriteLine("Remove {0}", dwgid);
                        comm.Parameters.Clear();
                        comm.CommandText = "DELETE FROM drawings WHERE DrawingID=@drawid;";
                        comm.Parameters.AddWithValue("@drawid", dwgid);
                        comm.ExecuteNonQuery();
                        clearAlignmentFilesAndFootprint(dwgid);
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
        }

        /// <summary>
        /// Recursively search through the direcotories and add to database
        /// </summary>
        /// <param name="sDir"></param>
        public static void dirSearch(string sDir)
        {
            string[] useTheseExtensions = new string[4] { ".PDF", ".TIF", ".JPG", ".PNG" };

            //process each file the directory
            foreach (string fullFilePath in Directory.GetFiles(sDir))
            {
                //Get the file info
                FileInfo theFileInfo = new FileInfo(fullFilePath);

                string fullFileName = theFileInfo.Name.ToUpper();
                string fileExtension = theFileInfo.Extension.ToUpper();
                fullFileName = fullFileName.Replace("(", "").Replace(")", "");

                try
                {
                    fullFileName = fullFileName.Replace(fileExtension, "");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    continue;
                }

                //Skip if not in the desired extensions
                if (Array.IndexOf(useTheseExtensions, fileExtension) < 0)
                {
                    continue;
                }

                //Skip if there is a reference to issued for construction
                if (fullFilePath.ToUpper().IndexOf("IFC") > 0)
                {
                    Console.WriteLine("Skip issued for construction");
                    continue;
                }

                //Check for the version number, this step will skip some file names that are mile post specific
                Regex reg = new Regex(@"-[0-9](\.|_)[0-9]*-[A-Z0-9]*-[0-9]*");
                Match match = reg.Match(fullFileName);

                string[] dwgNamePieces = match.Value.Split('-');
                int dwgVersion;
                if (!int.TryParse(dwgNamePieces[dwgNamePieces.Length - 1], out dwgVersion))
                {
                    Console.WriteLine(fullFilePath);
                    Console.WriteLine("Skipped - Version Name Invalid");
                    continue;
                }

                Console.WriteLine(fullFileName);
                Console.WriteLine("\t##Version: {0}", dwgVersion);

                //process flag values 
                Dictionary<int, string> processFlagDict = new Dictionary<int, string>(){
                    {0, "Current"},
                    {1, "New drawing"},
                    {2, "Update drawing"}
                };

                //check if the version or modified date has been changed
                //Preserve any georeferencing
                Regex regCheckName = new Regex(@"[-A-Z0-9]*(\.|_)[0-9]*-[A-Z0-9]*");
                Match matchName = regCheckName.Match(fullFileName);

                int processFlag = -1;
                using (SqlConnection conn = new SqlConnection(AppConstants.CONN_STRING_SUPSQL_ARCGIS))
                {
                    conn.Open();
                    SqlCommand comm = conn.CreateCommand();
                    comm.CommandText = "SELECT DrawingID, Version FROM Drawings WHERE DrawingID LIKE @likeValue";
                    comm.Parameters.AddWithValue("@likeValue", "%" + matchName.Value + "%");
                    try
                    {
                        SqlDataReader reader = comm.ExecuteReader();
                        if (reader.Read())
                        {
                            int existingVersion = (int)reader["Version"];
                            if (dwgVersion == existingVersion)
                                processFlag = 0;
                            else
                                processFlag = 2;
                        }
                        else
                        {
                            processFlag = 1;
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

                Console.WriteLine("\t##Drawing Status: {0}", processFlagDict[processFlag]);

                //throw an exeption if the flag wasn't set
                if (processFlag == -1)
                {
                    throw new Exception();
                }

                if (processFlag == 0)
                {
                    //Already up to date
                    continue;
                }

                //Get the created and modified dates
                DateTime createdDate = theFileInfo.CreationTime;
                DateTime modifiedDate = theFileInfo.LastWriteTime;

                // Setup executable and parameters
                if (!Directory.Exists(@"C:\thumbnailOut"))
                    Directory.CreateDirectory(@"C:\thumbnailOut");

                //Start a process to make the thumbnail
                Process process = new Process();

                // Stop the process from opening a new window
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.FileName = AppConstants.IMAGE_MAGICK_EXE;

                string fullPdfCopypath = (@"C:\thumbnailOut\" + fullFileName + ".pdf").Replace(" ","");
                string fullThumbPath = (@"C:\thumbnailOut\" + fullFileName + ".png").Replace(" ", "");

                if (fileExtension != ".PDF")
                {

                    process.StartInfo.Arguments = string.Format("\"{0}\" \"{1}\"", fullFilePath, fullPdfCopypath);
                    process.Start();
                    process.WaitForExit();
                    process.StartInfo.Arguments = string.Format("\"{0}[0]\" -resize 300x300 \"{1}\"", fullPdfCopypath, fullThumbPath);
                    process.Start();
                    process.WaitForExit();
                }
                else
                {
                    process.StartInfo.Arguments = string.Format("\"{0}[0]\" -resize 300x300 {1}", fullFilePath, fullThumbPath);
                    process.Start();
                    process.WaitForExit();
                }

                //Get the file data
                byte[] fileData = null;
                byte[] thumbnailData = null;
                try
                {
                    fileData = File.ReadAllBytes(fullFilePath);
                    thumbnailData = File.ReadAllBytes(fullThumbPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    continue;
                }

                using (SqlConnection conn = new SqlConnection(AppConstants.CONN_STRING_SUPSQL_ARCGIS))
                {
                    conn.Open();

                    SqlCommand comm = conn.CreateCommand();
                    
                    //New record
                    if (processFlag == 1)
                    {
                        comm.CommandText = "DECLARE @Object_ID int;";
                        comm.CommandText += "DECLARE @Num_IDs int;";
                        comm.CommandText += "EXEC supgis_data.i1981_get_ids 2, 1, @Object_ID OUTPUT, @Num_IDs OUTPUT;";
                        comm.CommandText += "INSERT INTO DRAWINGS ";
                        comm.CommandText += "(OBJECTID, path, DrawingID, Created, Modified, Version, FileData, Thumbnail, Extension, NeedsUpdate) ";
                        comm.CommandText += "VALUES ";
                        comm.CommandText += "(@Object_ID, @path, @DrawingID, @Created, @Modified, @Version, @FileData, @Thumb, @ext, 1);";
                    }
                    else
                    {
                        clearAlignmentFilesAndFootprint(matchName.Value);
                        comm.CommandText = "UPDATE DRAWINGS SET ";
                        comm.CommandText += "path = @path, ";
                        comm.CommandText += "DrawingID = @DrawingID, ";
                        comm.CommandText += "Created = @Created, ";
                        comm.CommandText += "Modified = @Modified, ";
                        comm.CommandText += "Version = @Version, ";
                        comm.CommandText += "FileData = @FileData, ";
                        comm.CommandText += "Thumbnail = @Thumb, ";
                        comm.CommandText += "Extension = @ext ,";
                        comm.CommandText += "NeedsUpdate = 1 ";
                        comm.CommandText += "WHERE DrawingID LIKE '%" + matchName.Value + "%';";
                    }

                    comm.Parameters.AddWithValue("@path", fullFilePath);
                    comm.Parameters.AddWithValue("@DrawingID", fullFileName);
                    comm.Parameters.AddWithValue("@Created", createdDate);
                    comm.Parameters.AddWithValue("@Modified", modifiedDate);
                    comm.Parameters.AddWithValue("@Version", dwgVersion);
                    comm.Parameters.AddWithValue("@FileData", fileData);
                    comm.Parameters.AddWithValue("@Thumb", thumbnailData);
                    comm.Parameters.AddWithValue("@ext", fileExtension);

                    try
                    {
                        comm.ExecuteNonQuery();
                        Console.WriteLine("\t$$New Drawing Name: {0}", fullFileName);

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

            //recurse through the directories
            foreach (string d in Directory.GetDirectories(sDir))
            {
                dirSearch(d);
            }
        }


        /// <summary>
        /// Helper to delete alignment sheet files and footprint for updated files
        /// </summary>
        /// <param name="matchId"></param>
        private static bool clearAlignmentFilesAndFootprint(string matchId)
        {
            bool returnVal = true;
            string drawId = null;
            string llid = null;

            using (SqlConnection conn = new SqlConnection(AppConstants.CONN_STRING_SUPSQL_ARCGIS))
            {
                conn.Open();
                SqlCommand comm = conn.CreateCommand();
                comm.CommandText = "Select DrawingID, LineLoopName From DRAWINGS WHERE DrawingID LIKE '%" + matchId + "%';";
                try
                {
                    SqlDataReader reader = comm.ExecuteReader();
                    
                    if (reader.Read())
                    {
                        drawId = (reader["DrawingID"] == DBNull.Value ? null : reader["DrawingID"].ToString());
                        llid = (reader["LineLoopName"] == DBNull.Value ? null : reader["LineLoopName"].ToString());
                    }
                    reader.Close();

                    if (!string.IsNullOrEmpty(drawId))
                    {
                        comm.CommandText = "Delete from AlignmentFootprint where DrawingName = @drawName;";
                        comm.Parameters.AddWithValue("@drawName", drawId);

                        comm.ExecuteNonQuery();

                        if (!string.IsNullOrEmpty(llid))
                        {
                            foreach (string d in Directory.GetDirectories(Path.Combine(AppConstants.ALIGNMENT_SHEETS_ROOT_DIRECTORY, "Sheets_" + llid)))
                            {
                                Console.WriteLine(d);
                                foreach (string f in Directory.GetFiles(d))
                                {
                                    if (f.IndexOf(drawId) > 0)
                                    {
                                        File.Delete(f);
                                    }
                                }
                            }
                        }
                    }

                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                    returnVal = false;
                }
                finally
                {
                    comm.Dispose();
                    conn.Close();
                }              
            }
            return returnVal;
        }


        /// <summary>
        /// Get the json representation of the file folder structure
        /// </summary>
        /// <returns>JSON tree data store</returns>
        public static string getDrawingsJSON()
        {
            /*Create the root object, name set to match that on the V drive 
             * for continuity of naming but this is arbitrary */
            FileFolder fileFolder = new FileFolder("Drawings - Field Access");

            using (SqlConnection conn = new SqlConnection(AppConstants.CONN_STRING_SUPSQL_ARCGIS))
            {
                conn.Open();
                SqlCommand comm = conn.CreateCommand();
                comm.CommandText = "SELECT Path, DrawingID, Version, Shape.ToString() AS shp, Zoom ";
                comm.CommandText += "FROM supgis_data.Drawings ";
                comm.CommandText += "ORDER BY Path, DrawingID";
                try
                {
                    SqlDataReader reader = comm.ExecuteReader();

                    while (reader.Read())
                    {
                        fileFolder.addFile(reader["Path"].ToString(), int.Parse(reader["Version"].ToString()), reader["shp"].ToString(), reader["Zoom"].ToString());
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
            return fileFolder.ToString();
        }

        /// <summary>
        /// Get the geojson representation of all the drawings at the extent at this zoom level
        /// </summary>
        /// <param name="xmin"></param>
        /// <param name="ymin"></param>
        /// <param name="xmax"></param>
        /// <param name="ymax"></param>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public static string getDrawingsByExtent(double xmin, double ymin, double xmax, double ymax, int zoom)
        {
            string outputGeoJson = "{\"type\": \"FeatureCollection\",\"features\": []}";

            //Create the geojson object
            GeoJSON.GeoJSON geoJSON = new GeoJSON.GeoJSON();

            using (SqlConnection conn = new SqlConnection(AppConstants.CONN_STRING_SUPSQL_ARCGIS))
            {
                conn.Open();
                SqlCommand comm = conn.CreateCommand();
                comm.CommandText = "Select Shape.ToString() as shp, Thumbnail, Path, DrawingID, Version, Lat, Lon, Zoom from DRAWINGS ";
                comm.CommandText += "WHERE Lon Between @minLon AND @maxLon AND Lat BETWEEN @minLat AND @maxLat AND Zoom = @zoomLevel;";
                comm.Parameters.AddWithValue("@minLon", xmin);
                comm.Parameters.AddWithValue("@maxLon", xmax);
                comm.Parameters.AddWithValue("@minLat", ymin);
                comm.Parameters.AddWithValue("@maxLat", ymax);
                comm.Parameters.AddWithValue("@zoomLevel", zoom);
                try
                {
                    SqlDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        GeoJSON.JSONFeature newFeature = new GeoJSON.JSONFeature();
                        newFeature.addGeom(reader["shp"].ToString());
                        newFeature.addProperty("Path", reader["Path"].ToString());
                        newFeature.addProperty("DrawingID", reader["DrawingID"].ToString());
                        newFeature.addProperty("Version", (int)reader["Version"]);
                        newFeature.addProperty("Lat", double.Parse(reader["Lat"].ToString()));
                        newFeature.addProperty("Lon", double.Parse(reader["Lon"].ToString()));
                        byte[] thumbnailBuffer = (byte[])reader["Thumbnail"];
                        string base64String = Convert.ToBase64String(thumbnailBuffer);
                        newFeature.addProperty("Thumbnail", base64String);

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
        /// Get the base64 representation of the thumbnail
        /// </summary>
        /// <param name="dwgID"></param>
        /// <returns></returns>
        public static string getDwgThumb(string dwgID)
        {
            string outputString = "";
            using (SqlConnection conn = new SqlConnection(AppConstants.CONN_STRING_SUPSQL_ARCGIS))
            {
                conn.Open();
                SqlCommand comm = conn.CreateCommand();
                comm.CommandText = "Select Thumbnail from DRAWINGS WHERE DrawingID = @drawingID;";
                comm.Parameters.AddWithValue("@drawingID", dwgID);
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


        /// <summary>
        /// Set the location based on the user input clicked point
        /// </summary>
        /// <param name="dwgID"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="zoom"></param>
        public static void addGeoRefPoint(string dwgID, double x, double y, int zoom)
        {
            using (SqlConnection conn = new SqlConnection(AppConstants.CONN_STRING_SUPSQL_ARCGIS))
            {
                conn.Open();
                SqlCommand comm = conn.CreateCommand();
                comm.CommandText = "DECLARE @GEOM VARCHAR(50);";
                comm.CommandText += "SET @GEOM = 'POINT(' + CONVERT(varchar(20),@LON) + ' ' + CONVERT(varchar(20),@LAT) + ')';";
                comm.CommandText += "UPDATE DRAWINGS SET Lat=@LAT, Lon=@LON, Zoom=@ZOOM, SHAPE=geometry::STGeomFromText(@GEOM, 4326) ";
                comm.CommandText += "WHERE DrawingID = @dwgID;";
                comm.CommandText += "Select Shape.ToString() as shp, Path, DrawingID, Version, Lat, Lon, Zoom from DRAWINGS;";
                comm.Parameters.AddWithValue("@dwgID", dwgID);
                comm.Parameters.AddWithValue("@LAT", y);
                comm.Parameters.AddWithValue("@LON", x);
                comm.Parameters.AddWithValue("@ZOOM", zoom);
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

        /// <summary>
        /// Clear the geographic reference for the drawing
        /// </summary>
        /// <param name="dwgID"></param>
        /// <returns></returns>
        public static string removeGeoRefPoint(string dwgID)
        {
            string outputString = "";
            using (SqlConnection conn = new SqlConnection(AppConstants.CONN_STRING_SUPSQL_ARCGIS))
            {
                conn.Open();
                SqlCommand comm = conn.CreateCommand();
                comm.CommandText = "UPDATE DRAWINGS SET Lat=NULL,Lon=NULL,Zoom=NULL,SHAPE=NULL WHERE DrawingID = @drawingID;";
                comm.Parameters.AddWithValue("@drawingID", dwgID);
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
            return outputString;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="drawingID"></param>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static byte[] getDwgFileData(string drawingID, out string ext)
        {
            byte[] fileData = null;
            ext = "";

            using (SqlConnection conn = new SqlConnection(AppConstants.CONN_STRING_SUPSQL_ARCGIS))
            {
                conn.Open();
                SqlCommand comm = conn.CreateCommand();
                comm.CommandText = "Select FileData, Extension from DRAWINGS WHERE DrawingID = @drawingID;";
                comm.Parameters.AddWithValue("@drawingID", drawingID);
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