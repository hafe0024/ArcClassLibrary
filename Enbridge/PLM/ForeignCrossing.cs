using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enbridge.PLM
{
    /// <summary>
    /// Object to hold foreign crossing properties
    /// </summary>
    [Serializable]
    public class ForeignCrossing
    {
        /// <summary>
        /// ID of the crossing Type
        /// </summary>
        public string crossingType;
        /// <summary>
        /// The method used to identify pipe location
        /// </summary>
        public string methodToIdentify;
        /// <summary>
        /// Method used to confirm pipe location
        /// </summary>
        public string methodToConfirm;
        /// <summary>
        /// If the corrosion tech was notified, null enabled
        /// </summary>
        public bool? corrosionTechNotified;
        /// <summary>
        /// If the pipe was damaged during installation, null enabled
        /// </summary>
        public bool? pipeDamageDuringInstall;
        /// <summary>
        /// Was the pipe exposed during installation, null enabled
        /// </summary>
        public bool? pipeWasExposed;
        /// <summary>
        /// Was a test station or bond installed, null enabled
        /// </summary>
        public bool? testStationOrBondInstalled;
        /// <summary>
        /// Date the changed was recorded on atlas
        /// </summary>
        public DateTime dateRecordedOnAtlas;
        /// <summary>
        /// Name of the inpsector
        /// </summary>
        public string inspectorName;
        /// <summary>
        /// Dig ticket
        /// </summary>
        public string digTicket;
        /// <summary>
        /// Other info, comments
        /// </summary>
        public string otherInfo;


        public ForeignCrossing()
        {

        }

        
        /// <summary>
        /// Foreign crossing constructor
        /// </summary>
        public ForeignCrossing(Object crossType, string identifyMethod, string confirmMethod, Object corrosionTechNotified,
            Object pipeDamageDuringInstall, Object wasPipeExposed, Object testStationOrBondInstalled, DateTime dateOnAtlas,
            string inspectorName, string digTicket, string otherInfo)
        {

            this.crossingType = PLM_Helpers.getComboBoxSelectedValue(crossType);

            this.methodToIdentify = identifyMethod;
            this.methodToConfirm = confirmMethod;

            //Set corrosion tech notified property
            this.corrosionTechNotified = PLM_Helpers.trueFalseValue(corrosionTechNotified);
            

            //Set pipe damage during install property
            this.pipeDamageDuringInstall = PLM_Helpers.trueFalseValue(pipeDamageDuringInstall);
            

            //Set pipe exposed during installation property
            this.pipeWasExposed = PLM_Helpers.trueFalseValue(wasPipeExposed);
           

            //Set test station or bond installed property
            this.testStationOrBondInstalled = PLM_Helpers.trueFalseValue(testStationOrBondInstalled);
            

            this.dateRecordedOnAtlas = dateOnAtlas;
            this.inspectorName = inspectorName;
            this.digTicket = digTicket;
            this.otherInfo = otherInfo;


        }

        /// <summary>
        /// string representation of foreign crossing
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string returnString = "";
            returnString += string.Format("Type ID: {0}\n", this.crossingType);
            returnString += string.Format("Identify method: {0}\n", this.methodToIdentify);
            returnString += string.Format("Confirm method: {0}\n", this.methodToConfirm);
            returnString += string.Format("Corrosion tech notified: {0}\n", 
                this.corrosionTechNotified == null ? "Unknown or NA" : this.corrosionTechNotified.ToString());
            returnString += string.Format("Pipe damage during installation: {0}\n", 
                this.pipeDamageDuringInstall == null ? "Unknown or NA" : this.pipeDamageDuringInstall.ToString());
            returnString += string.Format("Pipe exposed: {0}\n", 
                this.pipeWasExposed == null ? "Unknown or NA" : this.pipeWasExposed.ToString());
            returnString += string.Format("Test station / bond installed: {0}\n",
                this.testStationOrBondInstalled == null ? "Unknown or NA" : this.testStationOrBondInstalled.ToString());
            returnString += string.Format("Date recorded on atlas: {0}\n", this.dateRecordedOnAtlas);
            returnString += string.Format("Inspector name: {0}\n", this.inspectorName);
            returnString += string.Format("Dig ticket: {0}\n", this.digTicket);
            returnString += string.Format("Other info: {0}", this.otherInfo);
            return returnString;
        }
    }
}
