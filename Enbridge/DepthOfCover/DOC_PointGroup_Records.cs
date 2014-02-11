using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enbridge.DepthOfCover
{
    public class DOC_PointGroup_Records
    {
        private string pointGroup;

        /// <summary>
        /// initialize the object
        /// </summary>
        /// <param name="pointGroup">point group id</param>
        public DOC_PointGroup_Records(string pointGroup)
        {
            this.pointGroup = pointGroup;
        }


        /// <summary>
        /// Get the GeoJSON representation of the group points
        /// </summary>
        /// <returns>GeoJson string</returns>
        public string getJSON()
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
                comm.CommandText += "WHERE PointGroupID = @pointGroup ORDER BY Measure;";
                comm.Parameters.AddWithValue("@pointGroup", pointGroup);
                try
                {
                    SqlDataReader reader = comm.ExecuteReader();

                    while (reader.Read())
                    {
                        GeoJSON.JSONFeature newFeature = new GeoJSON.JSONFeature();
                        newFeature.addGeom(reader["shp"].ToString());
                        newFeature.addProperty("LineName", Enbridge.AppConstants.GetLineName(reader["RouteEventID"].ToString()));

                        //newFeature.addProperty("CreatedBy", reader["CreatedBy"]);
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
        /// Approve the point group
        /// </summary>
        /// <param name="user">The approving user identity</param>
        public void approve(string user)
        {
            using (SqlConnection conn = new SqlConnection(AppConstants.CONN_STRING_DOC))
            {
                conn.Open();
                SqlCommand comm = conn.CreateCommand();
                SqlTransaction transaction = conn.BeginTransaction();
                comm.Connection = conn;
                comm.Transaction = transaction;

                try
                {
                    comm.CommandText = "EXEC dbo.set_current_version 'working';";
                    comm.CommandText += "EXEC dbo.edit_version 'working', 1;";
                    comm.CommandText += "UPDATE DepthOfCover_mv SET LastModified=GETDATE(), ModifiedBy=@approvedBy, HistoricalState='Current', Status='Active'";
                    comm.CommandText += " WHERE PointGroupID=@pointGroup;";
                    comm.CommandText += "UPDATE DOC_Point_Group_mv SET ApprovedBy=@approvedBy, ApprovedDate=GETDATE()  WHERE PointGroup = @pointGroup;";
                    
                    comm.Parameters.AddWithValue("@approvedBy", user);
                    comm.Parameters.AddWithValue("@pointGroup", pointGroup);
                    comm.ExecuteNonQuery();
                  
                    // Attempt to commit the transaction.
                    transaction.Commit();

                    comm.CommandText = "EXEC dbo.edit_version 'working', 2;";
                    comm.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                    transaction.Rollback();
                }
                finally
                {
                    comm.Dispose();
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// Delete the point group
        /// </summary>
        public void delete()
        {
            using (SqlConnection conn = new SqlConnection(AppConstants.CONN_STRING_DOC))
            {
                conn.Open();
                SqlCommand comm = conn.CreateCommand();
                SqlTransaction transaction = conn.BeginTransaction();
                comm.Connection = conn;
                comm.Transaction = transaction;
                try
                {
                    comm.CommandText = "EXEC dbo.set_current_version 'working';";
                    comm.CommandText += "EXEC dbo.edit_version 'working', 1;";
                    comm.CommandText += "DELETE FROM DepthOfCover_mv WHERE PointGroupID = @pointGroup;";
                    comm.CommandText += "DELETE FROM DOC_Point_Group_mv WHERE PointGroup = @pointGroup;";
                    comm.Parameters.AddWithValue("@pointGroup", pointGroup);
                    comm.ExecuteNonQuery();

                    // Attempt to commit the transaction.
                    transaction.Commit();

                    comm.CommandText = "EXEC dbo.edit_version 'working', 2;";
                    comm.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                    transaction.Rollback();
                }
                finally
                {
                    comm.Dispose();
                    conn.Close();
                }
            }
        }
    }
}