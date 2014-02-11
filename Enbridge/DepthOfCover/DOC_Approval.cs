using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enbridge.DepthOfCover
{
    public class DOC_Approval
    {
        //initialize class members
        public Dictionary<string, DOC_PointGroup_Records> toApproveDict = new Dictionary<string, DOC_PointGroup_Records>();
        public string pointGroupRecordsJSON;

        /// <summary>
        /// Approval object generates a json string and a dictionary for the individual records
        /// </summary>
        public DOC_Approval()
        {
            pointGroupRecordsJSON = "";

            using (SqlConnection conn = new SqlConnection(AppConstants.CONN_STRING_DOC))
            {
                conn.Open();
                SqlCommand comm = conn.CreateCommand();
                comm.CommandText += "EXEC dbo.set_current_version 'working';";
                comm.CommandText += "SELECT * FROM DOC_Point_Group_mv ORDER BY LoadedDate DESC;";
                try
                {
                    SqlDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        string pointGroupID = reader["PointGroup"].ToString();

                        //Add the point group to the list
                        toApproveDict.Add(pointGroupID, new DepthOfCover.DOC_PointGroup_Records(reader["PointGroup"].ToString()));
                    }
                }
                //catch error if necessary and finalize
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
            this.populateRecordsList();
        }

        /// <summary>
        /// helper function to get the record list
        /// </summary>
        /// <returns></returns>
        private string populateRecordsList()
        {
            //create the string lists to hold the temporary project properties strings
            List<string> DOCJSONList = new List<string>();
            using (SqlConnection conn = new SqlConnection(AppConstants.CONN_STRING_DOC))
            {
                conn.Open();
                SqlCommand comm = conn.CreateCommand();
                comm.CommandText += "EXEC dbo.set_current_version 'working';";
                comm.CommandText += "SELECT * FROM DOC_Point_Group_mv ORDER BY LoadedDate DESC;";
                try
                {
                    SqlDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        string pointGroupID = reader["PointGroup"].ToString();

                        //get the properties
                        string llEventID = reader["LineLoopEventID"].ToString();
                        DateTime dateLoaded = (DateTime)reader["LoadedDate"];
                        string loadedBy = reader["LoadedBy"].ToString();
                        int tempFullUpload = int.Parse(reader["FullUpload"].ToString());
                        string approvedBy = reader["ApprovedBy"].ToString();
                        string approvedDate = reader["ApprovedDate"].ToString();
                        DateTime dte;
                        if (DateTime.TryParse(approvedDate, out dte))
                        {
                            approvedDate = "\"" + dte.ToString("MM/dd/yyyy") + "\"";
                        }
                        else
                        {
                            approvedDate = "null";
                        }

                        //concatenate the properties
                        string pointGroupProps = "{\"pointGroupID\":\"" + pointGroupID + "\",";
                        pointGroupProps += "\"LoadedBy\":\"" + loadedBy.Replace(@"\", @"\\\\") + "\",";
                        pointGroupProps += "\"LoadedDate\":\"" + dateLoaded.ToString("MM/dd/yyyy") + "\",";
                        pointGroupProps += "\"FullUpload\":" + tempFullUpload + ",";
                        pointGroupProps += "\"ApprovedBy\":\"" + approvedBy.Replace(@"\", @"\\\\") + "\",";
                        pointGroupProps += "\"ApprovedDate\":" + approvedDate + ",";
                        pointGroupProps += "\"Line\":\"" + Enbridge.AppConstants.GetLineName(llEventID) + "\"}";

                        DOCJSONList.Add(pointGroupProps);
                    }
                }
                //catch error if necessary and finalize
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
            //put together the final string
            pointGroupRecordsJSON = "[" + string.Join(",", DOCJSONList) + "]";
            return pointGroupRecordsJSON;
        }

        public string getGroupJSON(string pointID)
        {
            return this.toApproveDict[pointID].getJSON();
        }

        public string approveGroup(string pointID, string userID)
        {
            this.toApproveDict[pointID].approve(userID);
            this.populateRecordsList();
            return this.pointGroupRecordsJSON;
        }

        public string deleteGroup(string pointID)
        {
            if (toApproveDict.Keys.Contains(pointID))
            {
                this.toApproveDict[pointID].delete();
                this.populateRecordsList();
                toApproveDict.Remove(pointID);
            }
            return this.pointGroupRecordsJSON;
        }
    }   
}