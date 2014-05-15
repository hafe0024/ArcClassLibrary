using System;
using System.Collections.Generic;
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
    }
}
