using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enbridge.PLM
{
    [Serializable]
    public class PermanentRepair
    {
        public DateTime dateOfDiscovery;
        public DateTime dateOfRepair;
        public Boolean? repairDueToLeak;
        public string causeOfBreakLeak;
        public double? oilOut;
        public double? oilRecovered;
        public string crudeType;
        public string batchNumber;
        public string dispositionOfOil;
        public string nameAndAddressOfReporter;
        public Boolean? reportFeePaid;

        public Boolean? stockPipeInstalled;
        public string hydrotestNumber;
        public string remarks;
        public Boolean? govAgencyContacted;
        public DateTime contactTime;
        public double? lat;
        public double? lon;
        public Boolean? gpsReading;
        public string locationDescription;
        public string fittingsAddedToMainline;

        public PermanentRepair()
        {
            this.contactTime = DateTime.Now;
            this.contactTime = this.dateOfRepair = DateTime.Now;
            this.repairDueToLeak = this.reportFeePaid = this.stockPipeInstalled = this.govAgencyContacted = this.gpsReading = null;
            this.oilOut = this.oilRecovered = this.lat = this.lon = null;
            this.causeOfBreakLeak = this.crudeType = this.batchNumber = this.dispositionOfOil = this.nameAndAddressOfReporter = null;
            this.hydrotestNumber = this.remarks = this.locationDescription = this.fittingsAddedToMainline = null;

        }

        public void setTab1Values(DateTime dateOfDiscovery, DateTime dateOfRepair, Object repairDueToLeak, string causeOfBreakLeak, string oilOut, 
            string oilRecovered, string crudeType, string batchNumber, string dispositionOfOil, string nameAndAddressOfReporter, Object reportFeePaid)
        {
            this.dateOfDiscovery = dateOfDiscovery;
            this.dateOfRepair = dateOfRepair;
            this.repairDueToLeak = PLM_Helpers.trueFalseValue(repairDueToLeak);
            this.causeOfBreakLeak = causeOfBreakLeak;
            this.oilOut = PLM_Helpers.convertStringToNullableDouble(oilOut);
            this.oilRecovered = PLM_Helpers.convertStringToNullableDouble(oilRecovered);
            this.crudeType = crudeType;
            this.batchNumber = batchNumber;
            this.dispositionOfOil = dispositionOfOil;
            this.nameAndAddressOfReporter = nameAndAddressOfReporter;
            this.reportFeePaid = PLM_Helpers.trueFalseValue(reportFeePaid);
        }


        public void setTab2Values(Object stockPipeInstalled, string hydrotestNumber, string remarks, Object govAgencyContacted, DateTime contactTime,
            string lat, string lon, Object gpsReading, string locationDescription, string fittingsAddedToMainline)
        {
            this.stockPipeInstalled = PLM_Helpers.trueFalseValue(stockPipeInstalled);
            this.hydrotestNumber = hydrotestNumber;
            this.remarks = remarks;
            this.govAgencyContacted = PLM_Helpers.trueFalseValue(govAgencyContacted);
            this.contactTime = contactTime;
            this.lat = PLM_Helpers.convertStringToNullableDouble(lat);
            this.lon = PLM_Helpers.convertStringToNullableDouble(lon);
            this.gpsReading = PLM_Helpers.trueFalseValue(gpsReading);
            this.locationDescription = locationDescription;
            this.fittingsAddedToMainline = fittingsAddedToMainline;
        }

        public void saveToDatabase(string reportID)
        {

            using (SqlConnection conn = new SqlConnection(AppConstants.CONN_STRING_PLM_REPORTS))
            {
                conn.Open();
                SqlCommand comm = conn.CreateCommand();
                comm.CommandText = "";
                comm.CommandText += "EXEC sde.set_current_version 'SDE.Working';";
                comm.CommandText += "EXEC sde.edit_version 'SDE.Working', 1;";
                comm.CommandText += "BEGIN TRANSACTION;";
                comm.CommandText += "INSERT into sde.PERMANENT_REPAIR_EVW (";

                comm.CommandText += "ID, ReportID, DateOfDiscovery, DateOfRepair, RepairDueToLeak, CauseOfBreak, OilOut, ";
                comm.CommandText += "OilRecovered, CrudeType, BatchNumber, DispositionOfOil, NameAndAddressOfReporter, ";
                comm.CommandText += "ReportFeePaid, StockPipeInstalled, Remarks, GovAgencyContacted, GovContactTime, ";
                comm.CommandText += "Latitude, Longitude, GPSReading, LocationDescription";
                comm.CommandText += ") Values (";
                comm.CommandText += "NEWID(), @ReportID, @DateOfDiscovery, @DateOfRepair, @RepairDueToLeak, @CauseOfBreak, @OilOut, ";
                comm.CommandText += "@OilRecovered, @CrudeType, @BatchNumber, @DispositionOfOil, @NameAndAddressOfReporter, ";
                comm.CommandText += "@ReportFeePaid, @StockPipeInstalled, @Remarks, @GovAgencyContacted, @GovContactTime, ";
                comm.CommandText += "@Latitude, @Longitude, @GPSReading, @LocationDescription";
                comm.CommandText += ");";
                comm.CommandText += "COMMIT;";
                comm.CommandText += "EXEC sde.edit_version 'SDE.Working', 2;";

                comm.Parameters.AddWithValue("@reportID", reportID);
                comm.Parameters.AddWithValue("@DateOfDiscovery", this.dateOfDiscovery);
                comm.Parameters.AddWithValue("@DateOfRepair", this.dateOfRepair);
                comm.Parameters.AddWithValue("@RepairDueToLeak", PLM_Helpers.nullOneOrZeroFromNullableBool(this.repairDueToLeak));
                comm.Parameters.AddWithValue("@CauseOfBreak", PLM_Helpers.nullOrStringFromString(this.causeOfBreakLeak));
                comm.Parameters.AddWithValue("@OilOut", PLM_Helpers.nullOrNumberFromNullableDouble(this.oilOut));
                comm.Parameters.AddWithValue("@OilRecovered", PLM_Helpers.nullOrNumberFromNullableDouble(this.oilRecovered));
                comm.Parameters.AddWithValue("@CrudeType", PLM_Helpers.nullOrStringFromString(this.crudeType));
                comm.Parameters.AddWithValue("@BatchNumber", PLM_Helpers.nullOrStringFromString(this.batchNumber));
                comm.Parameters.AddWithValue("@DispositionOfOil", PLM_Helpers.nullOrStringFromString(this.dispositionOfOil));
                comm.Parameters.AddWithValue("@NameAndAddressOfReporter", PLM_Helpers.nullOrStringFromString(this.dispositionOfOil));
                comm.Parameters.AddWithValue("@ReportFeePaid", PLM_Helpers.nullOneOrZeroFromNullableBool(this.reportFeePaid));
                comm.Parameters.AddWithValue("@StockPipeInstalled", PLM_Helpers.nullOneOrZeroFromNullableBool(this.stockPipeInstalled));
                comm.Parameters.AddWithValue("@Remarks", PLM_Helpers.nullOrStringFromString(this.remarks));
                comm.Parameters.AddWithValue("@GovAgencyContacted", PLM_Helpers.nullOneOrZeroFromNullableBool(this.govAgencyContacted));
                comm.Parameters.AddWithValue("@GovContactTime", this.contactTime);
                comm.Parameters.AddWithValue("@Latitude", PLM_Helpers.nullOrNumberFromNullableDouble(this.lat));
                comm.Parameters.AddWithValue("@Longitude", PLM_Helpers.nullOrNumberFromNullableDouble(this.lon));
                comm.Parameters.AddWithValue("@GPSReading", PLM_Helpers.nullOneOrZeroFromNullableBool(this.gpsReading));
                comm.Parameters.AddWithValue("@LocationDescription", PLM_Helpers.nullOrStringFromString(this.locationDescription));

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
    }
}
