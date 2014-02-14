using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enbridge.Drawings.AlignmentSheets
{
    /// <summary>
    /// 
    /// </summary>
    public static class GenerateAlignmentSheet
    {

        /// <summary>
        /// Check the drawings that are flagged as alignment sheets
        /// Create the raw png for georef point location
        /// if the sheet is ready, perform the georeferencing process
        /// </summary>
        public static void generateAlignmentPNG()
        {
            /*Avoid a lengthy open datbase connection by loading all the records into
             * a list of AlignmentSheetRecord objects
             * These have the appropriate methods to complete the png generation
             * and georeferencing tasks */
            List<AlignmentSheetRecord> alignSheets = new List<AlignmentSheetRecord>();

            using (SqlConnection conn = new SqlConnection(AppConstants.CONN_STRING_SUPSQL_ARCGIS))
            {
                //Join the drawings table with the drawingslineloopref table that holds values common to all aligmnent sheets for the route
                SqlCommand comm = conn.CreateCommand();
                comm.CommandText = "SELECT draw.LineLoopName, draw.DrawingID, draw.Extension, draw.NeedsUpdate, ";
                comm.CommandText += "linereftable.RghtBorLeft, linereftable.RghtBorTop, linereftable.RghtBorRght, linereftable.RghtBorBot,  ";
                comm.CommandText += "linereftable.DetLeft, linereftable.DetTop, linereftable.DetRght, linereftable.DetBot, ";
                comm.CommandText += "linereftable.TrrnLeft, linereftable.TrrnTop, linereftable.TrrnRght, linereftable.TrrnBot, ";
                comm.CommandText += "linereftable.PlanTop, linereftable.PlanBot, ";
                comm.CommandText += "linereftable.LineLoopID, linereftable.TrimBorder, ";
                comm.CommandText += "draw.MatchStartPixX, draw.MatchStartPixY, draw.MatchEndPixX, draw.MatchEndPixY, ";
                comm.CommandText += "draw.MatchStartStn, draw.MatchEndStn, draw.MP_KP_Start, draw.MP_KP_End ";
                comm.CommandText += "FROM drawings AS draw  ";
                comm.CommandText += "INNER JOIN DRAWINGSLINELOOPREF AS linereftable ";
                comm.CommandText += "ON draw.LineLoopName = linereftable.LineLoopName ";
                comm.CommandText += "WHERE draw.LineLoopName IS NOT NULL; ";

                try
                {
                    conn.Open();
                    SqlDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        alignSheets.Add(new AlignmentSheetRecord(reader, true));
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

            /*processHelper object contains persistent reference to arcobjects
             * Avoids repeatedly creating new arcobjects */
            using (DrawingProcessorHelper processHelper = new DrawingProcessorHelper())
            {
                for (int i = 0; i < alignSheets.Count; i++)
                {
                    AlignmentSheetRecord theSheet = alignSheets[i];
                    Console.WriteLine(theSheet.DrawingID);
                    Console.WriteLine("Update Flag " + theSheet.NeedsUpdate);
                    if (theSheet.NeedsUpdate == 0)
                        continue;

                    //Make the directories to hold the pieces of the alignment sheet
                    theSheet.makeDirectories();
                    //if the update flag is set to 1 (new record) make the raw png
                    if (theSheet.NeedsUpdate == 1)
                    {
                        theSheet.generateRawPng();
                        //set update flag to 2, awaiting georeferencing
                        theSheet.setNeedsUpdate(2);
                        Console.WriteLine("Raw PNG Made");

                    }

                    //sheetReady property derived from the present of georeferencing point values
                    //skip to next sheet if not ready
                    if (!theSheet.sheetReady)
                        continue;

                    //crop out the pieces of the drawing
                    theSheet.splitDrawing();
                    Console.WriteLine("Drawing Split");
                    //Geoference the drawing based on input points, note the processHelper passed as a parameter
                    theSheet.georefDrawing(processHelper);
                    Console.WriteLine("Geo Ref");
                    //Make the footprint
                    theSheet.makeFootprint();
                    Console.WriteLine("Make Footprint");
                    //Set the update flag to 0, georeferenced and finished
                    theSheet.setNeedsUpdate(0);
                    Console.WriteLine("Set needs update to false");
                }
            }
        }
    }
}
