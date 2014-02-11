using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Data.SqlClient;
using Enbridge.LinearReferencing;


namespace Enbridge.DepthOfCover
{
    /// <summary>
    /// Class for validation of inputs read from a CSV File
    /// </summary>

    public class DOC_Validator
    {
        private Thread worker;
        public bool isValid = false;
        private double minLat, maxLat, minLon, maxLon, minMeas, maxMeas, minElev, maxElev, minAcc, maxAcc;
        private bool hasAccuracy = false;
        private bool hasElevation = false;

        public string creatorValid = "Point Creator Valid";
        public string dateValid = "Date Format Valid";
        public string latValid = "";
        public string lonValid = "";
        public string measureValid = "";
        public string elevValid = "";
        public string equipmentValid = "Equipment List Valid";
        public string accuracyValid = "Accuracy Attribute N/A";
        public string probeValid = "Probe Atribute N/A";

        public List<docRecord_Temp> docRecordList;
        private string lineLoopID;
        private bool fullUpload;
        private string loadedBy;

        /// <summary>
        /// Constructor, takes the csv path string as an input
        /// </summary>
        /// <param name="inputCSV"></param>
        public DOC_Validator(string loadedBy, string lineLoopID, bool fullUpload = false)
        {
            docRecordList = new List<docRecord_Temp>();
            minLat = minLon = minMeas = minElev = minAcc = 1000E100;
            maxLat = maxLon = maxMeas = maxElev = maxAcc = -1000E100;

            //Add the brackets if needed and convert to upper case
            lineLoopID = (lineLoopID.IndexOf('{') < 0 ? "{" + lineLoopID : lineLoopID);
            lineLoopID = (lineLoopID.IndexOf('}') < 0 ? lineLoopID + "}": lineLoopID);
            this.lineLoopID = lineLoopID.ToUpper();

            this.fullUpload = fullUpload;
            this.loadedBy = loadedBy;
        }

        public int checkInputs(string inputCSV, List<int> equipmentList = null)
        {
            isValid = true;
            double Num;
            int EquipNum;
            int counter = 0;
            using (Enbridge.Utilities.CsvFileReader reader = new Enbridge.Utilities.CsvFileReader(inputCSV))
            {
                Enbridge.Utilities.CsvRow row = new Enbridge.Utilities.CsvRow();


                //Skip first row
                reader.ReadRow(row);
                while (reader.ReadRow(row))
                {
                    counter++;
                    # region check for empty row
                    string allRow = "";
                    for (int i = 0; i < row.Count; i++)
                    {
                        allRow += row[i];
                    }
                    if (allRow == "")
                    {
                        break;
                    }
                    # endregion

                    # region verify creator
                    string createdBy = row[0].ToString();
                    if (createdBy == "")
                    {
                        isValid = false;
                        creatorValid = "Empty Created By, Line " + counter;
                        break;
                    }
                    # endregion

                    # region verifydate
                    DateTime date;
                    string collectionDate = row[1];
                    {
                        if (collectionDate == "")
                        {

                            dateValid = "Empty Date Field, Line " + counter;
                            break;
                        }
                        if (!DateTime.TryParse(collectionDate, out date))
                        {
                            isValid = false;
                            dateValid = "Invalid Date Format, Line " + counter;
                            break;
                        }
                    }
                    # endregion

                    # region verifylon
                    string x = row[2];
                    if (double.TryParse(x, out Num))
                    {
                        minLon = (Num < minLon ? Num : minLon);
                        maxLon = (Num > maxLon ? Num : maxLon);
                        if (Num < -180 || Num > 180)
                        {
                            isValid = false;
                            lonValid = "Longitude out of range, Line " + counter;
                            break;
                        }
                    }
                    else
                    {
                        isValid = false;
                        lonValid = "Non Numeric longitude, Line " + counter;
                        break;
                    }
                    # endregion

                    # region verfiylat
                    string y = row[3];
                    if (double.TryParse(y, out Num))
                    {
                        minLat = (Num < minLat ? Num : minLat);
                        maxLat = (Num > maxLat ? Num : maxLat);
                        if (Num < -90 || Num > 90)
                        {
                            isValid = false;
                            latValid = "Latitude out of range, Line " + counter;
                            break;
                        }
                    }
                    else
                    {
                        isValid = false;
                        latValid = "Non Numeric latitude, Line " + counter;
                        break;
                    }
                    # endregion

                    # region checkElev
                    string z = row[4];
                    if (double.TryParse(z, out Num))
                    {
                        hasElevation = true;
                        minElev = (Num < minElev ? Num : minElev);
                        maxElev = (Num > maxElev ? Num : maxElev);
                    }
                    else
                    {
                        if (z != "")
                        {
                            isValid = false;
                            elevValid = "Non Numeric longitude, Line " + counter;
                            break;
                        }
                    }
                    # endregion

                    # region check meas
                    string meas = row[5];
                    if (double.TryParse(meas, out Num))
                    {
                        minMeas = (Num < minMeas ? Num : minMeas);
                        maxMeas = (Num > maxMeas ? Num : maxMeas);
                    }
                    else
                    {
                        isValid = false;
                        measureValid = "Non Numeric measurement, Line " + counter;
                        break;
                    }
                    # endregion

                    string desc = row[6];

                    # region checkEquip
                    string equip = row[7];
                    if (equipmentList == null)
                    {
                        equip = "-1";
                        equipmentValid = "Equipment List Not Applicable";
                    }
                    else
                    {
                        if (Int32.TryParse(equip, out EquipNum))
                        {
                            if (!equipmentList.Contains(EquipNum))
                            {
                                isValid = false;
                                equipmentValid = "Equipment id not valid for vendor, Line " + counter;
                                break;
                            }
                        }
                        else
                        {
                            isValid = false;
                            equipmentValid = "Equipment value not valid, Line " + counter;
                            break;
                        }

                    }
                    # endregion

                    #region check accruacy
                    string accuracy = (row.Count > 8 ? row[8] : "");
                    if (accuracy == "")
                    {
                        accuracy = "-1";
                    }
                    else
                    {
                        if (!Double.TryParse(accuracy, out Num))
                        {
                            isValid = false;
                            measureValid = "Non Numeric accuracy, Line " + counter;
                            break;
                        }
                        else
                        {
                            hasAccuracy = true;
                            minAcc = (Num < minAcc ? Num : minAcc);
                            maxAcc = (Num > maxAcc ? Num : maxAcc);
                        }
                    }
                    # endregion

                    # region check probe
                    int outNum;
                    string strProbe = (row.Count > 9 ? row[9] : "");
                    if (strProbe == "")
                    {
                        strProbe = "-1";
                    }
                    else
                    {
                        if (Int32.TryParse(strProbe, out outNum))
                        {
                            if (outNum == 0 || outNum == 1)
                            {
                                probeValid = "Probe attribute valid";
                            }
                            else
                            {
                                isValid = false;
                                probeValid = "Probe attribute must be 0, 1 or blank, Line " + counter;
                                break;

                            }
                        }
                        else
                        {
                            isValid = false;
                            probeValid = "Invalid probe attribute, Line " + counter;
                            break;
                        }
                    }

                    # endregion

                    docRecordList.Add(new docRecord_Temp(createdBy, date, x, y, z, meas, desc, equip, accuracy, strProbe));
                }
            }

            if (this.isValid)
            {
                latValid = String.Format("Latitude Min {0} Max {1}", minLat, maxLat);
                lonValid = String.Format("Longitude Min {0} Max {1}", minLon, maxLon);
                measureValid = String.Format("Measurement Min {0} Max {1}", minMeas, maxMeas);
                elevValid = (hasElevation ? String.Format("Elevation Min {0} Max {1}", minElev, maxElev) : elevValid);
                accuracyValid = (hasAccuracy ? String.Format("Accuracy Min {0} Max {1}", minAcc, maxAcc) : accuracyValid);
            }
            return counter;
        }


        /// <summary>
        /// Trigger the loadfile method in a new thread
        /// </summary>
        public void loadFileTrigger()
        {
            //Spawn a new thread to handle insertion and locating
            worker = new Thread(() => loadFile());
            worker.IsBackground = true;
            worker.Start();
            Console.WriteLine("Trigger Finished");
        }

        /// <summary>
        /// Do the work of loading the features
        /// </summary>
        /// <param name="docDataConnection"></param>
        /// <param name="featureClassName"></param>
        /// <param name="versionName"></param>
        /// <param name="lineLoopID"></param>
        /// <param name="fullUpload"></param>
        /// <returns></returns>
        private bool loadFile()
        {
            
            string pointGroupID = "{" + Guid.NewGuid().ToString().ToUpper() + "}";

            //Add the features to the incremental feature class if it is not the full version
            string tableName = (fullUpload ? AppConstants.DOC_VIEW : AppConstants.DOC_INCREM_VIEW);
            bool success = true;

            //Create the locator
            ContLineLocatorSQL stnLocator = new ContLineLocatorSQL(lineLoopID);

            using (SqlConnection conn = new SqlConnection(AppConstants.CONN_STRING_DOC))
            {
                conn.Open();
                SqlCommand comm = conn.CreateCommand();

                try
                {


                    
                    //loop over the points
                    int docCounter = 0;
                    for(int i = 0; i < docRecordList.Count; i++)
                    {
                        docRecord_Temp rec = docRecordList[i];
                        
                        //continue;
                        docCounter += 1;
                        double stn, meas, MP;
                        string serEventID = stnLocator.getLocation(rec.POINT_X, rec.POINT_Y, out stn, out meas, out MP);

                        //generate the update query
                        comm.CommandText = string.Format("EXEC sde.set_current_version '{0}'", AppConstants.DOC_WORKING_VERSION);
                        comm.CommandText += string.Format("EXEC sde.edit_version '{0}', 1", AppConstants.DOC_WORKING_VERSION);

                        comm.CommandText += "Insert into " + tableName + " ";
                        comm.CommandText += "(";
                        comm.CommandText += "   Shape,";
                        comm.CommandText += "   GlobalID,";
                        comm.CommandText += "   CreatedBy, ";
                        comm.CommandText += "   CreatedDate,";
                        comm.CommandText += "   EventID,";
                        comm.CommandText += "   OriginEventID,";
                        comm.CommandText += "   LastModified,";
                        comm.CommandText += "   ModifiedBy,";
                        comm.CommandText += "   HistoricalState,";
                        comm.CommandText += "   RouteEventID,";
                        comm.CommandText += "   Measure,";
                        comm.CommandText += "   SeriesEventID,";
                        comm.CommandText += "   Station,";
                        comm.CommandText += "   Status,";
                        comm.CommandText += "   POINT_X,";
                        comm.CommandText += "   POINT_Y,";
                        comm.CommandText += "   POINT_Z,";
                        comm.CommandText += "   Measurement,";
                        comm.CommandText += "   Description,";
                        comm.CommandText += "   PointGroupID,";
                        comm.CommandText += "   EquipmentID,";
                        comm.CommandText += "   Accuracy,";
                        comm.CommandText += "   Probe";
                        comm.CommandText += ") ";
                        comm.CommandText += "VALUES ";
                        comm.CommandText += "(";
                        comm.CommandText += string.Format("    geometry::STGeomFromText('POINT ({0} {1} {2} {3})', 4326), ", rec.POINT_X, rec.POINT_Y, rec.POINT_Z, rec.stn);
                        comm.CommandText += "    NEWID(), ";
                        comm.CommandText += "    @CreatedBy, ";
                        comm.CommandText += "    @CreatedDate, ";
                        comm.CommandText += "    @EventID, ";
                        comm.CommandText += "    @OriginEventID,";
                        comm.CommandText += "    GETDATE(),";
                        comm.CommandText += "    @ModifiedBy,";
                        comm.CommandText += "    @HistoricalState,";
                        comm.CommandText += "    @RouteEventID,";
                        comm.CommandText += "    @Measure,";
                        comm.CommandText += "    @SeriesEventID,";
                        comm.CommandText += "    @Station,";
                        comm.CommandText += "    @Status,";
                        comm.CommandText += "    @POINT_X,";
                        comm.CommandText += "    @POINT_Y,";
                        comm.CommandText += "    @POINT_Z,";
                        comm.CommandText += "    @Measurement,";
                        comm.CommandText += "    @Description,";
                        comm.CommandText += "    @PointGroupID,";
                        comm.CommandText += "    @EquipmentID,";
                        comm.CommandText += "    @Accuracy,";
                        comm.CommandText += "    @Probe";
                        comm.CommandText += ");";
                        
                        comm.Parameters.Clear();
                        
                        comm.Parameters.AddWithValue("@CreatedBy", rec.CreatedBy);
                        comm.Parameters.AddWithValue("@CreatedDate", rec.CreatedDate);
                        comm.Parameters.AddWithValue("@EventID", rec.evtID);
                        comm.Parameters.AddWithValue("@OriginEventID", rec.evtID);
                        comm.Parameters.AddWithValue("@ModifiedBy", loadedBy);
                        comm.Parameters.AddWithValue("@HistoricalState", "Unknown (Verified)");
                        comm.Parameters.AddWithValue("@RouteEventID", lineLoopID);
                        comm.Parameters.AddWithValue("@Measure", meas);
                        comm.Parameters.AddWithValue("@SeriesEventID", serEventID);
                        comm.Parameters.AddWithValue("@Station", stn);
                        comm.Parameters.AddWithValue("@Status", "Proposed");
                        comm.Parameters.AddWithValue("@POINT_X", rec.POINT_X);
                        comm.Parameters.AddWithValue("@POINT_Y", rec.POINT_Y);
                        comm.Parameters.AddWithValue("@POINT_Z", rec.POINT_Z);
                        comm.Parameters.AddWithValue("@Measurement", rec.Measurement);
                        comm.Parameters.AddWithValue("@Description", rec.Description);
                        comm.Parameters.AddWithValue("@PointGroupID", pointGroupID);
                        comm.Parameters.AddWithValue("@EquipmentID", rec.EquipmentID);
                        comm.Parameters.AddWithValue("@Accuracy", rec.Accuracy);
                        comm.Parameters.AddWithValue("@Probe", rec.probe);

                        comm.CommandText += "COMMIT";
                        comm.CommandText += string.Format("EXEC sde.edit_version '{0}', 2", AppConstants.DOC_WORKING_VERSION);
                        comm.ExecuteNonQuery();
                        
                    }
                    //generate the update query for point group table
                    comm.CommandText = string.Format("EXEC sde.set_current_version '{0}'", AppConstants.DOC_WORKING_VERSION);
                    comm.CommandText += string.Format("EXEC sde.edit_version '{0}', 1", AppConstants.DOC_WORKING_VERSION);

                    comm.CommandText += "Insert into " + AppConstants.DOC_POINT_GROUP_VIEW + " ";
                    comm.CommandText += "(";
                    comm.CommandText += "   PointGroup,";
                    comm.CommandText += "   FullUpload,";
                    comm.CommandText += "   LoadedBy, ";
                    comm.CommandText += "   LoadedDate,";
                    comm.CommandText += "   ApprovedBy,";
                    comm.CommandText += "   LineLoopEventID,";
                    comm.CommandText += "   NumPoints";
                    comm.CommandText += ") ";
                    comm.CommandText += "VALUES ";
                    comm.CommandText += "(";
                    comm.CommandText += "    @PointGroup,";
                    comm.CommandText += "    @FullUpload,";
                    comm.CommandText += "    @LoadedBy,";
                    comm.CommandText += "    GETDATE(),";
                    comm.CommandText += "    'PENDING',";
                    comm.CommandText += "    @LineLoopEventID,";
                    comm.CommandText += "    @NumPoints";
                    comm.CommandText += ");";
                    

                    comm.Parameters.Clear();
                    
                    comm.Parameters.AddWithValue("@PointGroup", pointGroupID);
                    comm.Parameters.AddWithValue("@FullUpload", (fullUpload ? 1 : 0));
                    comm.Parameters.AddWithValue("@LoadedBy", loadedBy);
                    comm.Parameters.AddWithValue("@LineLoopEventID", lineLoopID);
                    comm.Parameters.AddWithValue("@NumPoints", docCounter);

                    comm.CommandText += "COMMIT";
                    comm.CommandText += string.Format("EXEC sde.edit_version '{0}', 2", AppConstants.DOC_WORKING_VERSION);
                    comm.ExecuteNonQuery();
                    
                    Console.WriteLine("All records are written to database.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Commit Exception Type: {0}", ex.GetType());
                    Console.WriteLine("  Message: {0}", ex.Message);

                    // Attempt to roll back the transaction. 
                    try
                    {
                        //transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        // This catch block will handle any errors that may have occurred 
                        // on the server that would cause the rollback to fail, such as 
                        // a closed connection.
                        Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                        Console.WriteLine("  Message: {0}", ex2.Message);
                    }
                }
                finally
                {
                    comm.Dispose();
                    conn.Close();
                    Console.WriteLine("finished successfully");
                }
            }

            return success;
        }
    }
}