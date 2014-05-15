using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enbridge.PLM
{
    [Serializable]
    public class CorrosionInpsection
    {
        public string soilType;
        public string terrainType;

        public string existingCoating;
        public string newCoating;

        public double? coatingLength;
        public string trenchCondition;
        public string inspectorName;
        public DateTime inspectionDate;
        public string externalCondition;
        public string internalCondition;
        public string coatingCondition;
        public string pipeToSoilPotential;

        public string typeOfPig;
        public string indicationNumber;
        public string wheelCount;
        public string gradeDepthOrRPR;
        public string corrosionType;
        public string maxPitDepth;
        public string anomalySize;

        public CorrosionInpsection()
        {
            newCoating = null;
        }

        public void setTab1Values(Object soilType, Object terrainType, string trenchCondition, string inspectorName, DateTime inspectionDate, Object externalCondition, Object internalCondition,
            Object coatingCondition, string pipeToSoilPotential)
        {

            this.soilType = PLM_Helpers.getComboBoxSelectedValue(soilType);
            this.terrainType = PLM_Helpers.getComboBoxSelectedValue(terrainType);

            this.trenchCondition = trenchCondition;
            this.inspectorName = inspectorName;
            this.inspectionDate = inspectionDate;

            this.externalCondition = PLM_Helpers.getComboBoxSelectedValue(externalCondition);
            this.internalCondition = PLM_Helpers.getComboBoxSelectedValue(internalCondition);
            this.coatingCondition = PLM_Helpers.getComboBoxSelectedValue(coatingCondition);

            this.pipeToSoilPotential = pipeToSoilPotential;
        }

        public void setTab2Values(Object existingCoating, Object newCoating, string coatingLength)
        {
            this.existingCoating = PLM_Helpers.getComboBoxSelectedValue(existingCoating);
            this.newCoating = PLM_Helpers.getComboBoxSelectedValue(newCoating);

            this.coatingLength = PLM_Helpers.convertStringToNullableDouble(coatingLength);

            
        }

        public void setTab3Values(Object typeOfPig, string indicationNumber, string wheelCount, string gradeDepthOrRPR, Object corrosionType,
            string maxPitDepth, string anomalySize)
        {
            this.typeOfPig = PLM_Helpers.getComboBoxSelectedValue(typeOfPig);
            this.indicationNumber = indicationNumber;
            this.wheelCount = wheelCount;
            this.gradeDepthOrRPR = gradeDepthOrRPR;
            this.corrosionType = PLM_Helpers.getComboBoxSelectedValue(corrosionType);
            this.maxPitDepth = maxPitDepth;
            this.anomalySize = anomalySize;
        }

       
    }
}
