﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enbridge.PLM
{
    [Serializable]
    public class PointFeatures
    {
        private List<PointFeat> existingFeaturesList;
        private List<PointFeat> pendingFeaturesList;

        public PointFeatures()
        {
            this.existingFeaturesList = new List<PointFeat>();
            this.pendingFeaturesList = new List<PointFeat>();
        }


        public bool addFeatureByStn(string routeId, string featureType, string description, double stn)
        {
            Enbridge.LinearReferencing.ContLineLocatorSQL loc = new Enbridge.LinearReferencing.ContLineLocatorSQL(routeId);

            double mp, meas, X, Y, Z;
            mp = loc.getMPFromStn(stn, out meas, out X, out Y, out Z);
            string stnSeriesId = loc.getLocation(X, Y);

            this.pendingFeaturesList.Add(new PointFeat(routeId, stnSeriesId, stn, mp, featureType, Y, X, description, Z));
            return true;
        }

        public bool addFeatureByMP(string routeId, string featureType, string description, double mp)
        {
            Enbridge.LinearReferencing.ContLineLocatorSQL loc = new Enbridge.LinearReferencing.ContLineLocatorSQL(routeId);
            double stn, meas, X, Y, Z;
            stn = loc.getStnFromMP(mp, out meas, out X, out Y, out Z);
            string stnSeriesId = loc.getLocation(X, Y);

            this.pendingFeaturesList.Add(new PointFeat(routeId, stnSeriesId, stn, mp, featureType, Y, X, description, Z));
            return true;
        }

        public bool addFeatureByLonLat(string routeId, string featureType, string description, double lon, double lat)
        {
            Enbridge.LinearReferencing.ContLineLocatorSQL loc = new Enbridge.LinearReferencing.ContLineLocatorSQL(routeId);
            double stn, meas, X, Y, Z, mp;
            string stnSeriesId = loc.getLocation(lon, lat, out stn, out meas, out mp);
            loc.getStnFromMP(mp, out meas, out X, out Y, out Z);

            this.pendingFeaturesList.Add(new PointFeat(routeId, stnSeriesId, stn, mp, featureType, Y, X, description, Z));
            return true;
        }

        public bool saveToDatabase(string reportID)
        {

            bool successStatus = false;

            if (this.pendingFeaturesList.Count == 0)
            {
                return true;
            }

            using (SqlConnection conn = new SqlConnection(AppConstants.CONN_STRING_PLM_REPORTS))
            {
                conn.Open();
                SqlCommand comm = conn.CreateCommand();

                string commandString = "";
                commandString += "EXEC sde.set_current_version 'SDE.Working';";
                commandString += "EXEC sde.edit_version 'SDE.Working', 1;";
                commandString += "BEGIN TRANSACTION;";
                commandString += "INSERT INTO sde.POINT_FEATURE_EVW ";
                commandString += "(ID, ReportID, RouteID, StationSeriesID, DateAdded, Stationing, ";
                commandString += "MilePost, FeatureType, Latitude, Longitude, Description, Shape) ";
                commandString += "VALUES ";
                commandString += "(@ID, @ReportID, @RouteID, @StationSeriesID, GETDATE(), @Stationing, ";
                commandString += "@MilePost, @FeatureType, @Latitude, @Longitude, @Description, {0}) ";
                commandString += "COMMIT;";
                commandString += "EXEC sde.edit_version 'SDE.Working', 2;";


                foreach (PointFeat feat in this.pendingFeaturesList)
                {
                    comm.CommandText = String.Format(commandString, feat.geomString);
                    comm.Parameters.Clear();
                    comm.Parameters.AddWithValue("@ID", feat.ID);
                    comm.Parameters.AddWithValue("@ReportID", reportID);
                    comm.Parameters.AddWithValue("@RouteID", feat.routeId);
                    comm.Parameters.AddWithValue("@StationSeriesID", feat.stnSeriesId);
                    comm.Parameters.AddWithValue("@Stationing", feat.stationing);
                    comm.Parameters.AddWithValue("@MilePost", feat.milePost);
                    comm.Parameters.AddWithValue("@FeatureType", feat.featureType);
                    comm.Parameters.AddWithValue("@Latitude", feat.latitude);
                    comm.Parameters.AddWithValue("@Longitude", feat.longitude);
                    comm.Parameters.AddWithValue("@Description", feat.description);

                    try
                    {
                        comm.ExecuteNonQuery();
                        successStatus = true;

                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine(ex.Message);
                        successStatus = false;
                    }
                }
                comm.Dispose();
                conn.Close();
            }
            return successStatus;
        }


        private class PointFeat
        {
            public string ID;
            public string routeId;
            public string stnSeriesId;
            public double stationing;
            public double milePost;
            public string featureType;
            public double latitude;
            public double longitude;
            public string description;
            public string geomString;


            public PointFeat(string routeId, string stnSeriesId, double stationing, double milePost, string featureType, 
                double lat, double lon, string description, double Z = 0)
            {
                this.ID = Guid.NewGuid().ToString();
                this.routeId = routeId;
                this.stnSeriesId = stnSeriesId;
                this.stationing = stationing;
                this.milePost = milePost;
                this.featureType = featureType;
                this.latitude = lat;
                this.longitude = lon;
                this.description = description;
                this.geomString = String.Format("geometry::STPointFromText('POINT ({0} {1} {2} {3})', 4326)", lon, lat, Z, stationing);
            }

        }
    }
}
