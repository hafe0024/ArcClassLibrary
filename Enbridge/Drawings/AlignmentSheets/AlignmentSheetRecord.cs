using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enbridge.Drawings.AlignmentSheets
{
    /// <summary>
    /// 
    /// </summary>
    public class AlignmentSheetRecord
    {
        /// <summary>
        /// 
        /// </summary>
        public string LLName;
        /// <summary>
        /// 
        /// </summary>
        public string LLId;
        /// <summary>
        /// 
        /// </summary>
        public string Extension;
        /// <summary>
        /// 
        /// </summary>
        public string DrawingID;
        /// <summary>
        /// 
        /// </summary>
        public int NeedsUpdate;
        /// <summary>
        /// 
        /// </summary>
        public regionBounds details;
        /// <summary>
        /// 
        /// </summary>
        public regionBounds plan;
        /// <summary>
        /// 
        /// </summary>
        public regionBounds terrain;
        /// <summary>
        /// 
        /// </summary>
        public regionBounds rightBorder;

        /// <summary>
        /// 
        /// </summary>
        public double MatchStartPixX;
        /// <summary>
        /// 
        /// </summary>
        public double MatchStartPixY;
        /// <summary>
        /// 
        /// </summary>
        public double MatchEndPixX;
        /// <summary>
        /// 
        /// </summary>
        public double MatchEndPixY;

        /// <summary>
        /// 
        /// </summary>
        public double MatchStartStn;
        /// <summary>
        /// 
        /// </summary>
        public double MatchEndStn;
        /// <summary>
        /// 
        /// </summary>
        public double MP_KP_Start;
        /// <summary>
        /// 
        /// </summary>
        public double MP_KP_End;

        /// <summary>
        /// 
        /// </summary>
        public string pngPlanPath = null;

        /// <summary>
        /// 
        /// </summary>
        Dictionary<string, double> planCoords = null;
        /// <summary>
        /// 
        /// </summary>
        Dictionary<string, double[]> planBounds = null;
        /// <summary>
        /// 
        /// </summary>
        Dictionary<string, double> mercCoords = null;
        /// <summary>
        /// 
        /// </summary>
        public bool trimBorder = false;

        /// <summary>
        /// 
        /// </summary>
        public bool sheetReady = true;

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> directoryDict = new Dictionary<string, string>();


        /// <summary>
        /// Generate the aligmnet sheet record based on values extracted from a SqlDataReader object
        /// Iteration is handled outside of the constructor
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="joinedTable"></param>
        public AlignmentSheetRecord(SqlDataReader reader, bool joinedTable)
        {
            if (joinedTable)
            {
                LLId = reader["LineLoopID"].ToString();
                this.trimBorder = Convert.ToInt32(reader["TrimBorder"]) == 1 ? true : false;
            }
            else
            {
                LLId = "";
                this.trimBorder = false;
            }

            LLName = reader["LineLoopName"].ToString();
            DrawingID = reader["DrawingID"].ToString();
            Extension = reader["Extension"].ToString();
            NeedsUpdate = Convert.ToInt32(reader["NeedsUpdate"]);
            

            //List of values to check for existance to determine if sheet is ready for georeferencing
            List<string> checkList = new List<string>() { "MatchStartPixX", "MatchStartPixY", "MatchEndPixX", "MatchEndPixY",
                "MatchStartStn", "MatchEndStn", "MP_KP_Start", "MP_KP_End" };

            //Loop through the keys, if any are null the sheet is flagged as sheetReady = false
            foreach (string prop in checkList)
            {
                if (reader[prop] == DBNull.Value)
                {
                    sheetReady = false;
                    break;
                }
            }
            
            //If the sheet is ready, populate the values necessary for georeferencing
            if (sheetReady)
            {
                MatchStartPixX = Convert.ToDouble(reader["MatchStartPixX"]);
                MatchStartPixY = Convert.ToDouble(reader["MatchStartPixY"]);
                MatchEndPixX = Convert.ToDouble(reader["MatchEndPixX"]);
                MatchEndPixY = Convert.ToDouble(reader["MatchEndPixY"]);

                MatchStartStn = Convert.ToDouble(reader["MatchStartStn"]);
                MatchEndStn = Convert.ToDouble(reader["MatchEndStn"]);

                MP_KP_Start = Convert.ToDouble(reader["MP_KP_Start"]);
                MP_KP_End = Convert.ToDouble(reader["MP_KP_End"]);


                if (joinedTable)
                {
                    //Make the region bounds for regions constant to the alignment sheet route
                    rightBorder = new regionBounds(Convert.ToDouble(reader["RghtBorLeft"]), Convert.ToDouble(reader["RghtBorTop"]),
                        Convert.ToDouble(reader["RghtBorRght"]), Convert.ToDouble(reader["RghtBorBot"]));

                    details = new regionBounds(Convert.ToDouble(reader["DetLeft"]), Convert.ToDouble(reader["DetTop"]),
                        Convert.ToDouble(reader["DetRght"]), Convert.ToDouble(reader["DetBot"]));

                    terrain = new regionBounds(Convert.ToDouble(reader["TrrnLeft"]), Convert.ToDouble(reader["TrrnTop"]),
                        Convert.ToDouble(reader["TrrnRght"]), Convert.ToDouble(reader["TrrnBot"]));

                    plan = new regionBounds(MatchStartPixX, Convert.ToDouble(reader["PlanTop"]),
                   MatchEndPixX, Convert.ToDouble(reader["PlanBot"]));

                    double imageHeight = Math.Abs(plan.top - plan.bottom);

                    /*plan coords, the match points in pixels of the plan 
                     * Adjustment necessary because unreferenced image assumed to 
                     * be located with upper left corner at 0, 0
                     * Therefore, initial Y values will be negative in ArcMap space */
                    planCoords = new Dictionary<string, double>()
                    {
                        {"X1", 0},
                        {"Y1", -1 * (MatchStartPixY - plan.top)},
                        {"X2", plan.right - plan.left},
                        {"Y2", -1 * (MatchEndPixY - plan.top)}
                    };

                    //Dictionary to hold coordinates of plan coordinates
                    //Used later to generate footprint
                    planBounds = new Dictionary<string, double[]>(){
                        {"LL", new double[2]{0,-1 * imageHeight}},
                        {"UL", new double[2]{0,0}},
                        {"UR", new double[2]{planCoords["X2"], 0}},
                        {"LR", new double[2]{planCoords["X2"], -1 * imageHeight}}
                    };
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="drawingId"></param>
        /// <param name="MP_KP_Start"></param>
        /// <param name="MP_KP_End"></param>
        /// <param name="MatchStartStn"></param>
        /// <param name="MatchEndStn"></param>
        /// <param name="MatchStartPixX"></param>
        /// <param name="MatchStartPixY"></param>
        /// <param name="MatchEndPixX"></param>
        /// <param name="MatchEndPixY"></param>
        public AlignmentSheetRecord(string drawingId, double MP_KP_Start, double MP_KP_End, double MatchStartStn, double MatchEndStn,
                            double MatchStartPixX, double MatchStartPixY, double MatchEndPixX, double MatchEndPixY)
        {
            this.DrawingID = drawingId;
            this.MP_KP_Start = MP_KP_Start;
            this.MP_KP_End = MP_KP_End;
            this.MatchStartStn = MatchStartStn;
            this.MatchEndStn = MatchEndStn;
            this.MatchStartPixX = MatchStartPixX;
            this.MatchStartPixY = MatchStartPixY;
            this.MatchEndPixX = MatchEndPixX;
            this.MatchEndPixY = MatchEndPixY;
            this.sheetReady = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="drawingId"></param>
        public AlignmentSheetRecord(string drawingId)
        {
            this.DrawingID = drawingId;
            this.sheetReady = true;
        }




        /// <summary>
        /// Create the necessary directories if they don't yet exist
        /// </summary>
        public void makeDirectories()
        {
            directoryDict["routeRoot"] = Path.Combine(AppConstants.ALIGNMENT_SHEETS_ROOT_DIRECTORY, "Sheets_" + this.LLName);
            if (!Directory.Exists(directoryDict["routeRoot"]))
                Directory.CreateDirectory(directoryDict["routeRoot"]);
            directoryDict["details"] = Path.Combine(directoryDict["routeRoot"], "Details");
            if (!Directory.Exists(directoryDict["details"]))
                Directory.CreateDirectory(directoryDict["details"]);
            directoryDict["drawingInfo"] = Path.Combine(directoryDict["routeRoot"], "DrawingInfo");
            if (!Directory.Exists(directoryDict["drawingInfo"]))
                Directory.CreateDirectory(directoryDict["drawingInfo"]);
            directoryDict["plan"] = Path.Combine(directoryDict["routeRoot"], "Plan");
            if (!Directory.Exists(directoryDict["plan"]))
                Directory.CreateDirectory(directoryDict["plan"]);
            directoryDict["rawPng"] = Path.Combine(directoryDict["routeRoot"], "RawPNG");
            if (!Directory.Exists(directoryDict["rawPng"]))
                Directory.CreateDirectory(directoryDict["rawPng"]);
            directoryDict["terrain"] = Path.Combine(directoryDict["routeRoot"], "Terrain");
            if (!Directory.Exists(directoryDict["terrain"]))
                Directory.CreateDirectory(directoryDict["terrain"]);
        }


        /// <summary>
        /// Generate the raw png
        /// </summary>
        public void generateRawPng()
        {
            //Get the file data from the database binary
            byte[] buff = null;
            string fileExt = null;

            using (SqlConnection conn = new SqlConnection(AppConstants.CONN_STRING_SUPSQL_ARCGIS))
            {
                conn.Open();
                SqlCommand comm = conn.CreateCommand();
                comm.CommandText = "SELECT FileData, Extension FROM drawings WHERE DrawingID = @drawingid;";
                comm.Parameters.AddWithValue("@drawingid", this.DrawingID);
                try
                {
                    SqlDataReader reader = comm.ExecuteReader();
                    reader.Read();
                    buff = (byte[])reader["FileData"];
                    fileExt = reader["Extension"].ToString().ToLower();
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Error here");
                }
                finally
                {
                    comm.Dispose();
                    conn.Close();
                }
            }

            //Create a new process
            Process process = new Process();

            // Stop the process from opening a new window
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = AppConstants.IMAGE_MAGICK_EXE;

            //Save the pdf buffer or convert if not yet in pdf format
            string originalCopy = @"C:\TEMP\TempOrig.pdf";
            if (fileExt == ".pdf")
            {
                File.WriteAllBytes(originalCopy, buff);
            }
            else
            {
                string tempNotPdf = @"C:\TEMP\TempNotPDF" + fileExt;
                File.WriteAllBytes(tempNotPdf, buff);
                process.StartInfo.Arguments = string.Format("{0} -size 7200x4800 -quality 95 -delete 1--1 {1}", tempNotPdf, originalCopy);
                process.Start();
                process.WaitForExit();
            }

            //Three step process to generate transparent pngs with background color images removed
            process.StartInfo.FileName = AppConstants.GHOST_SCRIPT_EXE;
            process.StartInfo.Arguments = string.Format("-dSAFER -dNOPAUSE -dBATCH -sDEVICE=png16m -sColorConversionStrategy=Gray -r200 -sOutputFile=C:\\TEMP\\conv1.png {0}", originalCopy);
            process.Start();
            process.WaitForExit();

            process.StartInfo.FileName = AppConstants.IMAGE_MAGICK_EXE;
            process.StartInfo.Arguments =
                string.Format(@"C:\TEMP\conv1.png -resize 7200x4800 -quality 100 -type GrayscaleMatte {0}C:\TEMP\conv2.png", (this.trimBorder ? "-trim " : ""));
            process.Start();
            process.WaitForExit();

            string colorThreshold = "-white-threshold 3%";
            if (fileExt == ".tif")
                colorThreshold = "-black-threshold 90%";

            string outputRawPNG = directoryDict["rawPng"] + string.Format(@"\{0}.png", this.DrawingID);
            process.StartInfo.Arguments = string.Format(@"C:\TEMP\conv2.png {0} -transparent white -depth 1 {1}", colorThreshold, outputRawPNG);
            process.Start();
            process.WaitForExit();
        }

        /// <summary>
        /// Split the drawing into its component parts
        /// </summary>
        public void splitDrawing()
        {
            //Delete all files in the plan directory with the same root name, image and auxilliary files
            string[] filesArray = Directory.GetFiles(directoryDict["plan"]);
            for (int i = 0; i < filesArray.Length; i++)
            {
                if (filesArray[i].IndexOf(this.DrawingID) > 0)
                {
                    File.Delete(Path.Combine(directoryDict["plan"], filesArray[i]));
                }
            }

            //shorthand reference to the pngPlan path, will be used later
            pngPlanPath = directoryDict["plan"] + string.Format(@"/{0}.png", this.DrawingID);

            //perform the split
            using (Bitmap bmp = new Bitmap(directoryDict["rawPng"] + string.Format(@"\{0}.png", this.DrawingID)))
            {
                System.Drawing.GraphicsUnit pixUnit = System.Drawing.GraphicsUnit.Pixel;
                RectangleF imgBounds = bmp.GetBounds(ref pixUnit);
                int imageWidth = Convert.ToInt32(imgBounds.Width);
                int imageHeight = Convert.ToInt32(imgBounds.Height);

                //plan
                int tryPlanWidth = Convert.ToInt32(this.plan.right - this.plan.left);
                int planWidth = (Convert.ToInt32(this.plan.left) + tryPlanWidth > imageWidth ? imageWidth - Convert.ToInt32(this.plan.left) : tryPlanWidth);

                int tryPlanHeight = Convert.ToInt32(this.plan.bottom - this.plan.top);
                int planHeight = (Convert.ToInt32(this.plan.top) + tryPlanHeight > imageHeight ? imageHeight - Convert.ToInt32(this.plan.top) : tryPlanHeight);

                Rectangle planBorderBounds = new Rectangle(Convert.ToInt32(this.plan.left), Convert.ToInt32(this.plan.top), planWidth, planHeight);

                bmp.Clone(planBorderBounds, bmp.PixelFormat).Save(pngPlanPath);

                //right border
                int tryBorderWidth = Convert.ToInt32(this.rightBorder.right - this.rightBorder.left);
                int borderWidth = (Convert.ToInt32(this.rightBorder.left) + tryBorderWidth > imageWidth ? imageWidth - Convert.ToInt32(this.rightBorder.left) : tryBorderWidth);

                int tryBorderHeight = Convert.ToInt32(this.rightBorder.bottom - this.rightBorder.top);
                int borderHeight = (Convert.ToInt32(this.rightBorder.top) + tryBorderHeight > imageHeight ?  imageHeight - Convert.ToInt32(this.rightBorder.top) : tryBorderHeight);

                Rectangle rightBorderBounds = new Rectangle(Convert.ToInt32(this.rightBorder.left), Convert.ToInt32(this.rightBorder.top), borderWidth, borderHeight);

                bmp.Clone(rightBorderBounds, bmp.PixelFormat).Save(directoryDict["drawingInfo"] + string.Format(@"/{0}.png", this.DrawingID));

                //details
                int tryDetailsWidth = Convert.ToInt32(this.details.right - this.details.left);
                int detailsWidth = (Convert.ToInt32(this.details.left) + tryDetailsWidth > imageWidth ? imageWidth - Convert.ToInt32(this.details.left) : tryDetailsWidth);

                int tryDetailsHeight = Convert.ToInt32(this.details.bottom - this.details.top);
                int detailsHeight = (Convert.ToInt32(this.details.top) + tryDetailsHeight > imageHeight ? imageHeight - Convert.ToInt32(this.details.top) : tryDetailsHeight);

                Rectangle detailsBounds = new Rectangle(Convert.ToInt32(this.details.left), Convert.ToInt32(this.details.top), detailsWidth, detailsHeight);

                bmp.Clone(detailsBounds, bmp.PixelFormat).Save(directoryDict["details"] + string.Format(@"/{0}.png", this.DrawingID));

                //terrain
                int tryTerrainWidth = Convert.ToInt32(this.terrain.right - this.terrain.left);
                int terrainWidth = (Convert.ToInt32(this.terrain.left) + tryTerrainWidth > imageWidth ? imageWidth - Convert.ToInt32(this.terrain.left) : tryTerrainWidth);

                int tryTerrainHeight = Convert.ToInt32(this.terrain.bottom - this.terrain.top);
                int terrainHeight = (Convert.ToInt32(this.terrain.top) + tryTerrainHeight > imageHeight ? imageHeight - Convert.ToInt32(this.terrain.top) : tryTerrainHeight);

                Rectangle terrainBounds = new Rectangle(Convert.ToInt32(this.terrain.left), Convert.ToInt32(this.terrain.top), terrainWidth, terrainHeight);

                bmp.Clone(terrainBounds, bmp.PixelFormat).Save(directoryDict["terrain"] + string.Format(@"/{0}.png", this.DrawingID));
            }
        }

        /// <summary>
        /// Georeference the plan
        /// </summary>
        /// <param name="processHelper">reference to persistant arcobjects</param>
        public void georefDrawing(DrawingProcessorHelper processHelper)
        {
            //Locate the start and end match points by stationing
            Enbridge.LinearReferencing.ContLineLocatorSQL locator = new Enbridge.LinearReferencing.ContLineLocatorSQL(this.LLId);
            double startX, startY, startZ, endX, endY, endZ, meas;
            locator.getMPFromStn(this.MatchStartStn, out meas, out startX, out startY, out startZ);
            locator.getMPFromStn(this.MatchEndStn, out meas, out endX, out endY, out endZ);

            //convert long lat coordinates to web mercator
            double startMercX, startMercY, endMercX, endMercY;
            Enbridge.Utilities.ProjectionConversion.toWebMercator(startX, startY, out startMercX, out startMercY);
            Enbridge.Utilities.ProjectionConversion.toWebMercator(endX, endY, out endMercX, out endMercY);

            //dictionary to hold the mercator xy values
            mercCoords = new Dictionary<string, double>() { { "startMercX", startMercX }, { "startMercY", startMercY }, { "endMercX", endMercX }, { "endMercY", endMercY } };

            //define the projection of the plan to 3857
            processHelper.defineProj.in_dataset = pngPlanPath;
            processHelper.gp.Execute(processHelper.defineProj, null);


            //open the raster workspace
            processHelper.rastWorkspace =
                (ESRI.ArcGIS.DataSourcesRaster.IRasterWorkspace)processHelper.workspaceFact.OpenFromFile(Path.GetDirectoryName(pngPlanPath), 0);

            //open the raster dataset
            processHelper.rasDataset = processHelper.rastWorkspace.OpenRasterDataset(Path.GetFileName(pngPlanPath));
            //open the raster data
            processHelper.rast = processHelper.rasDataset.CreateDefaultRaster();

            //Set location of image source points
            ESRI.ArcGIS.Geometry.IPoint sourcePoint1 = new ESRI.ArcGIS.Geometry.Point();
            sourcePoint1.X = planCoords["X1"];
            sourcePoint1.Y = planCoords["Y1"];
            processHelper.sourcePoints.UpdatePoint(0, sourcePoint1);
            ESRI.ArcGIS.Geometry.IPoint sourcePoint2 = new ESRI.ArcGIS.Geometry.Point();
            sourcePoint2.X = planCoords["X2"];
            sourcePoint2.Y = planCoords["Y2"];
            processHelper.sourcePoints.UpdatePoint(1, sourcePoint2);

            //Set location of target points, that of the stationing match locations
            ESRI.ArcGIS.Geometry.IPoint targetPoint1 = new ESRI.ArcGIS.Geometry.Point();
            targetPoint1.X = mercCoords["startMercX"];
            targetPoint1.Y = mercCoords["startMercY"];
            processHelper.targetPoints.UpdatePoint(0, targetPoint1);
            ESRI.ArcGIS.Geometry.IPoint targetPoint2 = new ESRI.ArcGIS.Geometry.Point();
            targetPoint2.X = mercCoords["endMercX"];
            targetPoint2.Y = mercCoords["endMercY"];
            processHelper.targetPoints.UpdatePoint(1, targetPoint2);

            //complete the georeferencing
            processHelper.rasterPropc.TwoPointsAdjust(processHelper.sourcePoints, processHelper.targetPoints, processHelper.rast);
            //save the georeferencing to disk
            processHelper.rasterPropc.Register(processHelper.rast);
        }


        /// <summary>
        /// Make the raster footprint
        /// </summary>
        public void makeFootprint()
        {
            //Calculate the shift and scale factor
            double xShift = mercCoords["startMercX"] - planCoords["X1"];
            double yShift = mercCoords["startMercY"] - planCoords["Y1"];
            double scaleFactor = this.distanceFormula(mercCoords["startMercX"], mercCoords["startMercY"], mercCoords["endMercX"], mercCoords["endMercY"]) /
                    this.distanceFormula(planCoords["X1"], planCoords["Y1"], planCoords["X2"], planCoords["Y2"]);

            //Update the plan bounds coordinates
            planBounds["LL"][0] += xShift;
            planBounds["LR"][0] += xShift;
            planBounds["UL"][0] += xShift;
            planBounds["UR"][0] += xShift;
            planBounds["LL"][1] += yShift;
            planBounds["LR"][1] += yShift;
            planBounds["UL"][1] += yShift;
            planBounds["UR"][1] += yShift;

            //Make polar coordinates of the second match point relative to the first match point for both the plan
            //coordinates and the map match location points
            PolarCoords polarPlan1_2 = new PolarCoords(planCoords["X1"], planCoords["Y1"], planCoords["X2"], planCoords["Y2"]);
            PolarCoords polarMap1_2 = new PolarCoords(mercCoords["startMercX"], mercCoords["startMercY"], mercCoords["endMercX"], mercCoords["endMercY"]);

            //Calculate the roatation angle, the difference in the map points rotation angle and the plan points angle
            double totalRotate = polarMap1_2.heading - polarPlan1_2.heading;
            totalRotate = (totalRotate > 360 ? totalRotate -= 360 : totalRotate);
            totalRotate = (totalRotate < 0 ? totalRotate += 360 : totalRotate);

            //Make a list of the corner point keys
            List<string> cornerList = new List<string>() { "LL", "UL", "UR", "LR" };

            //initialize the well known text geometry string
            string geometryString = "POLYGON((";
            string closePoint = "";

            //Loop through the corner points
            for (int i = 0; i < cornerList.Count; i++)
            {
                //Create a polar coordinate object of the corner point relative to the rotation point, match point 1 in web mercator
                PolarCoords corner = new PolarCoords(mercCoords["startMercX"], mercCoords["startMercY"], planBounds[cornerList[i]][0], planBounds[cornerList[i]][1]);
                //Scale the distance
                corner.scaleDistance(scaleFactor);
                //rotate
                corner.rotate(totalRotate);
                //Add the coordinates to the wkt string
                geometryString += corner.cartesianX + " " + corner.cartesianY + ",";
                //keep a reference to the first point to close the polygon
                if (i == 0)
                    closePoint = corner.cartesianX + " " + corner.cartesianY + "))";
            }
            //Close the polygon with the same coordinates as that of the first point
            geometryString += closePoint;

            //Open a database connection to write the footprint and associated information
            using (SqlConnection conn2 = new SqlConnection(AppConstants.CONN_STRING_SUPSQL_ARCGIS))
            {
                conn2.Open();
                using (SqlCommand comm2 = conn2.CreateCommand())
                {
                    //Get the count of the records that have this same drawing identifier
                    comm2.CommandText = "SELECT COUNT(*) FROM AlignmentFootprint WHERE DrawingName = @drawingName;";
                    comm2.Parameters.AddWithValue("@drawingName", this.DrawingID);
                    try
                    {
                        int? recordCount = comm2.ExecuteScalar() as int?;
                        comm2.Parameters.Clear();


                        //if the count is 0, this is a new record to be inserted
                        if (recordCount == 0)
                        {
                            comm2.CommandText = "DECLARE @Object_ID int;";
                            comm2.CommandText += "DECLARE @Num_IDs int;";
                            comm2.CommandText += "EXEC supgis_data.i1984_get_ids 2, 1, @Object_ID OUTPUT, @Num_IDs OUTPUT;";
                            comm2.CommandText += "INSERT INTO AlignmentFootprint  ";
                            comm2.CommandText += "(OBJECTID, ROW_Name, LineLoopEventID, MP_KP_Start, MP_KP_End, StnStart, StnEnd, DrawingName, SHAPE)";
                            comm2.CommandText += "VALUES ";
                            comm2.CommandText += "(@Object_ID, @rowName, @lineLoopId, @MPKPStart, @MPKPEnd, @stnStart, @stnEnd, @drawingName, ";
                            comm2.CommandText += string.Format("geometry::STGeomFromText('{0}', 3857));", geometryString);
                        }
                        //The count is greater than 0, should be 1, update the existing record
                        else
                        {
                            comm2.CommandText = "UPDATE AlignmentFootprint SET ";
                            comm2.CommandText += "ROW_Name = @rowName, ";
                            comm2.CommandText += "LineLoopEventID = @lineLoopId, ";
                            comm2.CommandText += "MP_KP_Start = @MPKPStart, ";
                            comm2.CommandText += "MP_KP_End = @MPKPEnd, ";
                            comm2.CommandText += "StnStart = @stnStart, ";
                            comm2.CommandText += "StnEnd = @stnEnd, ";
                            comm2.CommandText += "SHAPE = " + string.Format("geometry::STGeomFromText('{0}', 3857) ", geometryString);
                            comm2.CommandText += "WHERE DrawingName = @drawingName;";
                        }
                        //Add relevent values
                        comm2.Parameters.AddWithValue("@rowName", this.LLName);
                        comm2.Parameters.AddWithValue("@lineLoopId", this.LLId);
                        comm2.Parameters.AddWithValue("@drawingName", this.DrawingID);
                        comm2.Parameters.AddWithValue("@MPKPStart", this.MP_KP_Start);
                        comm2.Parameters.AddWithValue("@MPKPEnd", this.MP_KP_End);
                        comm2.Parameters.AddWithValue("@stnStart", this.MatchStartStn);
                        comm2.Parameters.AddWithValue("@stnEnd", this.MatchEndStn);
                        //Execute the query
                        comm2.ExecuteNonQuery();
                    }
                    catch (SqlException sqlExcep)
                    {
                        Console.WriteLine(sqlExcep.Message);
                    }
                    finally
                    {
                        conn2.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Helper method to set the update flag of the drawing
        /// </summary>
        /// <param name="val">0 drawing finished, 2 raw png created</param>
        public void setNeedsUpdate(int val)
        {
            this.NeedsUpdate = val;
            using (SqlConnection conn2 = new SqlConnection(AppConstants.CONN_STRING_SUPSQL_ARCGIS))
            {
                conn2.Open();
                using (SqlCommand comm2 = conn2.CreateCommand())
                {
                    comm2.CommandText = "UPDATE drawings SET NeedsUpdate = @val WHERE drawingID = @drawingID;";
                    comm2.Parameters.AddWithValue("@drawingID", this.DrawingID);
                    comm2.Parameters.AddWithValue("@val", val);
                    try
                    {
                        comm2.ExecuteNonQuery();
                    }
                    catch (SqlException sqlExcep)
                    {
                        Console.WriteLine(sqlExcep.Message);
                    }
                    finally
                    {
                        conn2.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToJSON()
        {
            string outString = "{";

            if (MP_KP_Start.ToString() + MP_KP_End.ToString() + MatchStartStn.ToString() + MatchEndStn.ToString() +
                MatchStartPixX.ToString() + MatchStartPixY.ToString() + MatchEndPixX.ToString() + MatchEndPixY.ToString() == "00000000")
            {
                outString += string.Format("\"drawingId\":\"{0}\"", this.DrawingID);
            }
            else
            {
                outString += string.Format("\"drawingId\":\"{0}\",", this.DrawingID);
                outString += string.Format("\"MP_KP_Start\":{0},", MP_KP_Start.ToString());
                outString += string.Format("\"MP_KP_End\":{0},", MP_KP_End.ToString());
                outString += string.Format("\"MatchStartStn\":{0},", MatchStartStn.ToString());
                outString += string.Format("\"MatchEndStn\":{0},", MatchEndStn.ToString());
                outString += string.Format("\"MatchStartPixX\":{0},", MatchStartPixX.ToString());
                outString += string.Format("\"MatchStartPixY\":{0},", MatchStartPixY.ToString());
                outString += string.Format("\"MatchEndPixX\":{0},", MatchEndPixX.ToString());
                outString += string.Format("\"MatchEndPixY\":{0}", MatchEndPixY.ToString());
            }
            outString += "}";
            return outString;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clearValues"></param>
        /// <returns></returns>
        public string saveAlignmentLocationInfo(bool clearValues)
        {
            string returnString = "";

            using (SqlConnection conn = new SqlConnection(AppConstants.CONN_STRING_SUPSQL_ARCGIS))
            {
                conn.Open();
                SqlCommand comm = conn.CreateCommand();

                comm.CommandText = "UPDATE DRAWINGS SET NeedsUpdate = 2, ";

                if (clearValues)
                {
                    comm.CommandText += "MP_KP_Start = NULL, MP_KP_End = NULL, ";
                    comm.CommandText += "MatchStartStn = NULL, MatchEndStn = NULL, ";
                    comm.CommandText += "MatchStartPixX = NULL, MatchStartPixY = NULL, ";
                    comm.CommandText += "MatchEndPixX = NULL, MatchEndPixY = NULL ";
                    comm.CommandText += "WHERE drawingId = @drawingId;";
                    returnString = "Clear Values";
                }
                else
                {
                    //throw new Exception();
                    comm.CommandText += "MP_KP_Start = @MP_KP_Start, MP_KP_End = @MP_KP_End, ";
                    comm.CommandText += "MatchStartStn = @MatchStartStn, MatchEndStn = @MatchEndStn, ";
                    comm.CommandText += "MatchStartPixX = @MatchStartPixX, MatchStartPixY = @MatchStartPixY, ";
                    comm.CommandText += "MatchEndPixX = @MatchEndPixX, MatchEndPixY = @MatchEndPixY ";
                    comm.CommandText += "WHERE drawingId = @drawingId;";

                    comm.Parameters.AddWithValue("@MP_KP_Start", this.MP_KP_Start);
                    comm.Parameters.AddWithValue("@MP_KP_End", this.MP_KP_End);
                    comm.Parameters.AddWithValue("@MatchStartStn", this.MatchStartStn);
                    comm.Parameters.AddWithValue("@MatchEndStn", this.MatchEndStn);
                    comm.Parameters.AddWithValue("@MatchStartPixX", this.MatchStartPixX);
                    comm.Parameters.AddWithValue("@MatchStartPixY", this.MatchStartPixY);
                    comm.Parameters.AddWithValue("@MatchEndPixX", this.MatchEndPixX);
                    comm.Parameters.AddWithValue("@MatchEndPixY", this.MatchEndPixY);
                    returnString = "Update Values";
                }
                comm.Parameters.AddWithValue("@drawingId", this.DrawingID);

                try
                {
                    comm.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                    returnString = ex.Message;
                }
                finally
                {
                    comm.Dispose();
                    conn.Close();
                }
            }
            return returnString;
        }

        /// <summary>
        /// Simple distance calculator
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        private double distanceFormula(double x1, double y1, double x2, double y2)
        {
            double a = x1 - x2;
            double b = y1 - y2;
            return Math.Sqrt(a * a + b * b);
        }
    }

    /// <summary>
    /// Struct to hold the left, top, right, and bottom values in pixels of the bounds of a drawing region
    /// </summary>
    public struct regionBounds
    {
        /// <summary>
        /// 
        /// </summary>
        public double left;
        /// <summary>
        /// 
        /// </summary>
        public double top;
        /// <summary>
        /// 
        /// </summary>
        public double right;
        /// <summary>
        /// 
        /// </summary>
        public double bottom;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        public regionBounds(double left, double top, double right, double bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }
    }
}