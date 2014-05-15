using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enbridge.PLM
{
    [Serializable]
    public class ReportProperties
    {
        /// <summary>
        /// 
        /// </summary>
        public string plmFacility;
        /// <summary>
        /// 
        /// </summary>
        public string reportName;
        /// <summary>
        /// the user name
        /// </summary>
        public string username;
        /// <summary>
        /// ID of report type
        /// </summary>
        public string reportType;
        /// <summary>
        /// Sleeve used
        /// </summary>
        public Boolean? sleeveUsed;
        /// <summary>
        /// Welding done
        /// </summary>
        public Boolean? weldingDone;
        /// <summary>
        /// Stopple equipment used
        /// </summary>
        public Boolean? stoppleEquipmentUsed;
        /// <summary>
        /// AFR needed
        /// </summary>
        public Boolean? afrNeeded;

        public ReportProperties(string username, Object reportType, Object sleeveUsed, Object weldingDone, Object stoppleEquipmentUsed, Object afrNeeded,
            Object plmFacility, string reportName) 
        {

            this.plmFacility = PLM_Helpers.getComboBoxSelectedValue(plmFacility);
            this.reportName = reportName;

            this.username = username;

            this.reportType = PLM_Helpers.getComboBoxSelectedValue(reportType);

            this.sleeveUsed = PLM_Helpers.trueFalseValue(sleeveUsed);
            this.weldingDone = PLM_Helpers.trueFalseValue(weldingDone);
            this.stoppleEquipmentUsed = PLM_Helpers.trueFalseValue(stoppleEquipmentUsed);
            this.afrNeeded = PLM_Helpers.trueFalseValue(afrNeeded);

            Console.WriteLine(this.ToString());
        }

        public override string ToString()
        {
            string returnString = "";
            returnString += string.Format("PLM ID: {0}\n", this.plmFacility);
            returnString += string.Format("Report Name: {0}\n", this.reportName);
            returnString += string.Format("User Name: {0}\n", this.username);
            returnString += string.Format("Sleeve used: {0}\n", (this.sleeveUsed == null ? "null" : this.sleeveUsed.ToString()));
            returnString += string.Format("Welding done: {0}\n", (this.weldingDone == null ? "null" : this.weldingDone.ToString()));
            returnString += string.Format("Stopple Equipment: {0}\n", (this.stoppleEquipmentUsed == null ? "null" : this.stoppleEquipmentUsed.ToString()));
            returnString += string.Format("AFR needed: {0}", (this.afrNeeded == null ? "null" : this.afrNeeded.ToString()));
            return returnString;
        }

        
    }
}
