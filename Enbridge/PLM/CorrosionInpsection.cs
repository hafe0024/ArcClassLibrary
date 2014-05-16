using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
        public string trenchCondition;
        public string inspectorName;
        public DateTime inspectionDate;
        public string externalCondition;
        public string internalCondition;
        public string coatingCondition;
        public string pipeToSoilPotential;

        public string existingCoating;
        public string newCoating;
        public double? coatingLength;

        public string typeOfPig;
        public string indicationNumber;
        public string wheelCount;
        public string gradeDepthOrRPR;
        public string corrosionType;
        public string maxPitDepth;
        public string anomalySize;


        public CorrosionInpsection()
        {
            this.soilType = this.terrainType = this.existingCoating = this.newCoating = null;
            this.coatingLength = null;
            this.trenchCondition = this.inspectorName;
            this.inspectionDate = DateTime.Now;
            this.externalCondition = this.internalCondition = this.coatingCondition = this.pipeToSoilPotential = null;
            this.typeOfPig = this.indicationNumber = this.wheelCount = this.gradeDepthOrRPR = this.corrosionType = null;
            this.maxPitDepth = this.anomalySize = null;
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
                comm.CommandText += "INSERT into sde.CORROSIONINSPECTION_EVW (";
                comm.CommandText += "ID, ReportID, SoilType, TerrainType, TrenchCondition, InspectorName, InspectedDate, ";
                comm.CommandText += "ExternalCondition, InternalCondition, CoatingCondition, PipeToSoilPotential, ExistingCoating, ";
                comm.CommandText += "NewCoating, CoatingLength, TypeOfPig, IndicationNumber, WheelCount, ";
                comm.CommandText += "GradeDepthOrRPR, CorrosionType, MaxPitDepth, AnomalySize";
                comm.CommandText += ") Values (";
                comm.CommandText += "NEWID(), @ReportID, @SoilType, @TerrainType, @TrenchCondition, @InspectorName, @InspectedDate, ";
                comm.CommandText += "@ExternalCondition, @InternalCondition, @CoatingCondition, @PipeToSoilPotential, @ExistingCoating, ";
                comm.CommandText += "@NewCoating, @CoatingLength, @TypeOfPig, @IndicationNumber, @WheelCount, ";
                comm.CommandText += "@GradeDepthOrRPR, @CorrosionType, @MaxPitDepth, @AnomalySize";
                comm.CommandText += ");";
                comm.CommandText += "COMMIT;";
                comm.CommandText += "EXEC sde.edit_version 'SDE.Working', 2;";

                comm.Parameters.AddWithValue("@reportID", reportID);
                comm.Parameters.AddWithValue("@SoilType", PLM_Helpers.nullOrStringFromString(this.soilType));
                comm.Parameters.AddWithValue("@TerrainType", PLM_Helpers.nullOrStringFromString(this.terrainType));
                comm.Parameters.AddWithValue("@TrenchCondition", PLM_Helpers.nullOrStringFromString(this.trenchCondition));
                comm.Parameters.AddWithValue("@InspectorName", PLM_Helpers.nullOrStringFromString(this.inspectorName));
                comm.Parameters.AddWithValue("@InspectedDate", this.inspectionDate);
                comm.Parameters.AddWithValue("@ExternalCondition", PLM_Helpers.nullOrStringFromString(this.externalCondition));
                comm.Parameters.AddWithValue("@InternalCondition", PLM_Helpers.nullOrStringFromString(this.internalCondition));
                comm.Parameters.AddWithValue("@CoatingCondition", PLM_Helpers.nullOrStringFromString(this.coatingCondition));
                comm.Parameters.AddWithValue("@PipeToSoilPotential", PLM_Helpers.nullOrStringFromString(this.pipeToSoilPotential, 255));
                comm.Parameters.AddWithValue("@ExistingCoating", PLM_Helpers.nullOrStringFromString(this.existingCoating));
                comm.Parameters.AddWithValue("@NewCoating", PLM_Helpers.nullOrStringFromString(this.newCoating));
                comm.Parameters.AddWithValue("@CoatingLength", PLM_Helpers.nullOrNumberFromNullableDouble(this.coatingLength));
                comm.Parameters.AddWithValue("@TypeOfPig", PLM_Helpers.nullOrStringFromString(this.typeOfPig));
                comm.Parameters.AddWithValue("@IndicationNumber", PLM_Helpers.nullOrStringFromString(this.indicationNumber));
                comm.Parameters.AddWithValue("@WheelCount", PLM_Helpers.nullOrStringFromString(this.wheelCount));
                comm.Parameters.AddWithValue("@GradeDepthOrRPR", PLM_Helpers.nullOrStringFromString(this.gradeDepthOrRPR));
                comm.Parameters.AddWithValue("@CorrosionType", PLM_Helpers.nullOrStringFromString(this.corrosionType));
                comm.Parameters.AddWithValue("@MaxPitDepth", PLM_Helpers.nullOrStringFromString(this.maxPitDepth));
                comm.Parameters.AddWithValue("@AnomalySize", PLM_Helpers.nullOrStringFromString(this.anomalySize));

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
