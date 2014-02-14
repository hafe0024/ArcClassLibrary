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
    public static class AlignmentSheetsJSON
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string getAlignmentJSON()
        {
            Dictionary<string, List<AlignmentSheetRecord>> aligmentSheetList = new Dictionary<string, List<AlignmentSheetRecord>>();

            using (SqlConnection conn = new SqlConnection(AppConstants.CONN_STRING_SUPSQL_ARCGIS))
            {
                conn.Open();
                SqlCommand comm = conn.CreateCommand();
                comm.CommandText = "SELECT LineLoopName, DrawingID, MP_KP_Start, MP_KP_End, MatchStartStn, MatchEndStn, ";
                comm.CommandText += "MatchStartPixX, MatchStartPixY, MatchEndPixX, MatchEndPixY, NeedsUpdate, Extension ";
                comm.CommandText += "FROM DRAWINGS "; 
                comm.CommandText += "WHERE LineLoopName IS NOT NULL AND NOT NeedsUpdate = 1;";
                SqlDataReader reader;
                try
                {
                    reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        string llName = reader["LineLoopName"].ToString();
                        if (!aligmentSheetList.ContainsKey(llName))
                        {
                            aligmentSheetList.Add(llName, new List<AlignmentSheetRecord>());
                        }
                        aligmentSheetList[llName].Add(new AlignmentSheetRecord(reader, false));

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

            List<string> keyArrayList = new List<string>();

            foreach (string key in aligmentSheetList.Keys)
            {
                Console.WriteLine(key);
                List<string> jsonStringList = new List<string>();
                for (int i = 0; i < aligmentSheetList[key].Count; i++)
                {
                    jsonStringList.Add(aligmentSheetList[key][i].ToJSON());

                }

                keyArrayList.Add(string.Format("\"{0}\":[{1}]", key, string.Join(",",jsonStringList)));

            }
            return "{" + string.Join(",",keyArrayList) + "}";

        }
    }

    
}
