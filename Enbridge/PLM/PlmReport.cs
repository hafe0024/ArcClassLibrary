using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enbridge.PLM
{
    [Serializable]
    public class PlmReport
    {
        #region class properties

        /// <summary>
        /// Report ID
        /// </summary>
        public string reportId;

        /// <summary>
        /// Foreign crossing object
        /// </summary>
        public ForeignCrossing foreignCrossing;

        /// <summary>
        /// 
        /// </summary>
        public ReportProperties reportProperties;

        public RowInfo rowInfo;
        
        public PermanentRepair permanentRepair;

        public CorrosionInpsection corrosionInspection;

        public FileAttachments fileAttachments;

        private bool? isForeignCrossing;

        #endregion class properties

        #region constructors

        /// <summary>
        /// Object to keep track of PLM report properties
        /// </summary>
        public PlmReport()
        {
            isForeignCrossing = null;
            this.reportId = Guid.NewGuid().ToString();
            this.foreignCrossing = null;
            this.reportProperties = null;
            this.rowInfo = null;
            this.corrosionInspection = null;
            this.permanentRepair = null;
            this.fileAttachments = new FileAttachments();

        }

        /// <summary>
        /// Constructor for exiting report based on id
        /// </summary>
        /// <param name="reportId"></param>
        public PlmReport(string reportId)
        {
            
        }

        #endregion constructors


        public void setIsForeignCrossing(bool isForeign)
        {
            this.isForeignCrossing = isForeign;
        }



        #region report properties


        /// <summary>
        /// Set the report properties
        /// </summary>
        /// <param name="username"></param>
        /// <param name="reportType"></param>
        /// <param name="sleeveUsed"></param>
        /// <param name="weldingDone"></param>
        /// <param name="stoppleEquipmentUsed"></param>
        /// <param name="afrNeeded"></param>
        /// <param name="plmFacilty"></param>
        /// <param name="reportName"></param>
        public void setReportProperties(string username, Object reportType, Object sleeveUsed, Object weldingDone, Object stoppleEquipmentUsed, Object afrNeeded,
            Object plmFacilty, string reportName)
        {
            this.reportProperties = new ReportProperties(username, reportType, sleeveUsed, weldingDone, stoppleEquipmentUsed, afrNeeded, plmFacilty, reportName);
        }

        #endregion report properties

        #region foreign crossing

        /// <summary>
        /// Set the foreign crossing information
        /// </summary>
        /// <param name="crossType"></param>
        /// <param name="identifyMethod"></param>
        /// <param name="confirmMethod"></param>
        /// <param name="corrosionTechNotified"></param>
        /// <param name="pipeDamageDuringInstall"></param>
        /// <param name="wasPipeExposed"></param>
        /// <param name="testStationOrBondInstalled"></param>
        /// <param name="dateOnAtlas"></param>
        /// <param name="inspectorName"></param>
        /// <param name="digTicket"></param>
        /// <param name="otherInfo"></param>
        public void setForeignCrossing(Object crossType, string identifyMethod, string confirmMethod, Object corrosionTechNotified,
            Object pipeDamageDuringInstall, Object wasPipeExposed, Object testStationOrBondInstalled, DateTime dateOnAtlas,
            string inspectorName, string digTicket, string otherInfo)
        {
            this.foreignCrossing = new ForeignCrossing(crossType, identifyMethod, confirmMethod, corrosionTechNotified,
                pipeDamageDuringInstall, wasPipeExposed, testStationOrBondInstalled, dateOnAtlas,
                inspectorName, digTicket, otherInfo);
        }

        #endregion foreign crossing

        #region ROW info

        public void setRowInfoLocationTab(string tract, string district, string lineSection, Object state, Object country, string legalDescription,
            string section, string township, string range)
        {
            if (this.rowInfo == null)
            {
                this.rowInfo = new RowInfo();
            }
            this.rowInfo.setLocationTabValues(tract, district, lineSection, state, country, legalDescription,
                section, township, range);
        }

        public void setRowInfoTenantTab(string tenantName, string tenantMainPhone, string tenantSecondaryPhone, Object tenantCountry,
            string tenantRowComments, Object tenantContactMade)
        {
            if (this.rowInfo == null)
            {
                this.rowInfo = new RowInfo();
            }
            this.rowInfo.setTenantTabValues(tenantName, tenantMainPhone, tenantSecondaryPhone, tenantCountry,
                tenantRowComments, tenantContactMade);
        }

        public void setRowInfoAccessTab(Object accessHowGained, Object accessRoutePreapproved, string accessLength, string accessWidth,
            Object accessDamagedWhileAcessing, Object accessRestorationCompleted, Object accessRestorationApprovedByOwner)
        {
            if (this.rowInfo == null)
            {
                this.rowInfo = new RowInfo();
            }
            this.rowInfo.setAccessTabValues(accessHowGained, accessRoutePreapproved, accessLength, accessWidth,
                accessDamagedWhileAcessing, accessRestorationCompleted, accessRestorationApprovedByOwner);
        }

        public void setRowInfoWorkAreaTab(string workAreaLength, string workAreaWidth, Object workAreaRestorationCompleted,
            Object workAreaRestorationApproved, Object workAreaPesticidesInLast72Hours)
        {
            if (this.rowInfo == null)
            {
                this.rowInfo = new RowInfo();
            }
            this.rowInfo.setWorkAreaTabValues(workAreaLength, workAreaWidth, workAreaRestorationCompleted,
                workAreaRestorationApproved, workAreaPesticidesInLast72Hours);
        }





        #endregion ROW info

        #region permanent repair

        /// <summary>
        /// Set the permanent repair values from tab 1
        /// </summary>
        /// <param name="dateOfDiscovery"></param>
        /// <param name="dateOfRepair"></param>
        /// <param name="repairDueToLeak"></param>
        /// <param name="causeOfBreakLeak"></param>
        /// <param name="oilOut"></param>
        /// <param name="oilRecovered"></param>
        /// <param name="crudeType"></param>
        /// <param name="batchNumber"></param>
        /// <param name="dispositionOfOil"></param>
        /// <param name="nameAndAddressOfReporter"></param>
        /// <param name="reportFeePaid"></param>
        public void setPermanentRepairTab1(DateTime dateOfDiscovery, DateTime dateOfRepair, Object repairDueToLeak, string causeOfBreakLeak, string oilOut,
            string oilRecovered, string crudeType, string batchNumber, string dispositionOfOil, string nameAndAddressOfReporter, Object reportFeePaid)
        {
            if (this.permanentRepair == null)
            {
                this.permanentRepair = new PermanentRepair();
            }
            this.permanentRepair.setTab1Values(dateOfDiscovery, dateOfRepair, repairDueToLeak, causeOfBreakLeak, oilOut,
            oilRecovered, crudeType, batchNumber, dispositionOfOil, nameAndAddressOfReporter, reportFeePaid);
        }

        /// <summary>
        /// Set the permanent repair values from tab 2
        /// </summary>
        /// <param name="stockPipeInstalled"></param>
        /// <param name="hydrotestNumber"></param>
        /// <param name="remarks"></param>
        /// <param name="govAgencyContacted"></param>
        /// <param name="contactTime"></param>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <param name="gpsReading"></param>
        /// <param name="locationDescription"></param>
        public void setPermanentRepairTab2(Object stockPipeInstalled, string hydrotestNumber, string remarks, Object govAgencyContacted, DateTime contactTime,
            string lat, string lon, Object gpsReading, string locationDescription, string fittingsAddedToMainline)
        {
            if (this.permanentRepair == null)
            {
                this.permanentRepair = new PermanentRepair();
            }
            this.permanentRepair.setTab2Values(stockPipeInstalled, hydrotestNumber, remarks, govAgencyContacted, contactTime,
                lat, lon, gpsReading, locationDescription, fittingsAddedToMainline);
        }

        #endregion permanent repair

        #region corrosion inspection

        /// <summary>
        /// Set the properties associated with corrosion inspection tab 1
        /// </summary>
        /// <param name="soilType"></param>
        /// <param name="terrainType"></param>
        /// <param name="trenchCondition"></param>
        /// <param name="inspectorName"></param>
        /// <param name="inspectionDate"></param>
        /// <param name="externalCondition"></param>
        /// <param name="internalCondition"></param>
        /// <param name="coatingCondition"></param>
        /// <param name="pipeToSoilPotential"></param>
        public void setCorrosionInspectionTab1(Object soilType, Object terrainType, 
            string trenchCondition, string inspectorName, DateTime inspectionDate, Object externalCondition, Object internalCondition,
            Object coatingCondition, string pipeToSoilPotential)
        {
            if (this.corrosionInspection == null)
            {
                this.corrosionInspection = new CorrosionInpsection();
            }

            this.corrosionInspection.setTab1Values(soilType, terrainType, 
                trenchCondition, inspectorName, inspectionDate, externalCondition, internalCondition,
                coatingCondition, pipeToSoilPotential);

            Console.WriteLine("Corrosion tab 1 set");
        }


        /// <summary>
        /// Set the properties associated with corrosion inspection tab 2
        /// </summary>
        /// <param name="existingCoating"></param>
        /// <param name="newCoating"></param>
        /// <param name="coatingLength"></param>
        public void setCorrosionInspectionTab2(Object existingCoating, Object newCoating, string coatingLength)
        {
            if (this.corrosionInspection == null)
            {
                this.corrosionInspection = new CorrosionInpsection();
            }

            this.corrosionInspection.setTab2Values(existingCoating, newCoating, coatingLength);

            Console.WriteLine("Corrosion tab 2 set");
        }

        /// <summary>
        /// Set the properties associated with corrosion inspection tab 3
        /// </summary>
        /// <param name="typeOfPig"></param>
        /// <param name="indicationNumber"></param>
        /// <param name="wheelCount"></param>
        /// <param name="gradeDepthOrRPR"></param>
        /// <param name="corrosionType"></param>
        /// <param name="maxPitDepth"></param>
        /// <param name="anomalySize"></param>
        public void setCorrosionInspectionTab3(Object typeOfPig, string indicationNumber, string wheelCount, string gradeDepthOrRPR, Object corrosionType,
            string maxPitDepth, string anomalySize)
        {
            if (this.corrosionInspection == null)
            {
                this.corrosionInspection = new CorrosionInpsection();
            }

            this.corrosionInspection.setTab3Values(typeOfPig, indicationNumber, wheelCount, gradeDepthOrRPR, corrosionType,
                maxPitDepth, anomalySize);

            Console.WriteLine("Corrosion tab 3 set");
        }

        #endregion corrosion inspection


        /// <summary>
        /// Submit the form to the database
        /// </summary>
        public bool saveReport()
        {
            using (SqlConnection conn = new SqlConnection(AppConstants.CONN_STRING_PLM_REPORTS))
            {
                conn.Open();
                SqlCommand comm = conn.CreateCommand();
                comm.CommandText = "";
                comm.CommandText += "EXEC sde.set_current_version 'SDE.Working';";
                comm.CommandText += "EXEC sde.edit_version 'SDE.Working', 1;";
                comm.CommandText += "BEGIN TRANSACTION;";
                comm.CommandText += "INSERT into sde.PLM_REPORT_EVW ";
                comm.CommandText += "(ID, ReportName, UserName, LoadedDate, ReportTypeID, PLM_Facility_ID, SleeveUsed, WeldingDone, StoppleEquipment, AFRNeeded) ";
                comm.CommandText += "Values (@reportID, @reportName, @userName, GETDATE(), @reportTypeID, @plm_ID, @sleeveUsed, @weldingDone, @stoppleEquip, @afr);";
                comm.CommandText += "COMMIT;";
                comm.CommandText += "EXEC sde.edit_version 'SDE.Working', 2;";

                comm.Parameters.AddWithValue("@reportID", this.reportId);
                comm.Parameters.AddWithValue("@reportName", PLM_Helpers.nullOrStringFromString(this.reportProperties.username));
                comm.Parameters.AddWithValue("@userName", PLM_Helpers.nullOrStringFromString(this.reportProperties.reportName));
                comm.Parameters.AddWithValue("@reportTypeID", this.reportProperties.reportType);
                comm.Parameters.AddWithValue("@plm_ID", this.reportProperties.plmFacility);
                comm.Parameters.AddWithValue("@sleeveUsed", PLM_Helpers.nullOneOrZeroFromNullableBool(this.reportProperties.sleeveUsed));
                comm.Parameters.AddWithValue("@weldingDone", PLM_Helpers.nullOneOrZeroFromNullableBool(this.reportProperties.weldingDone));
                comm.Parameters.AddWithValue("@stoppleEquip", PLM_Helpers.nullOneOrZeroFromNullableBool(this.reportProperties.stoppleEquipmentUsed));
                comm.Parameters.AddWithValue("@afr", PLM_Helpers.nullOneOrZeroFromNullableBool(this.reportProperties.afrNeeded));

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

            this.permanentRepair.saveToDatabase(this.reportId);
            this.corrosionInspection.saveToDatabase(this.reportId);
            return true;
        }
    }
}
