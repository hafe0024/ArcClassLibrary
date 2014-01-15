using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enbridge.DepthOfCover
{
    
    /// <summary>
    /// Input table class validates input from string representation of input file
    /// </summary>
    [Serializable]
    public class InputDOCTable
    {
        /// <summary>
        /// validation error message, initialized to null
        /// </summary>
        public string validationError = null;

        /// <summary>
        /// List of depth of cover records
        /// </summary>
        public List<DOCRecord> docRecordList = new List<DOCRecord>();

        /// <summary>
        /// Input Depth of Cover table object constructor
        /// </summary>
        /// <param name="inputString">string representation of CSV input file</param>
        public InputDOCTable(string inputString)
        {
            string[] lines = inputString.Split('\n');

            for (int i = 1; i < lines.Length; i++)
            {
                string lineContent = lines[i];

                if (lineContent.Replace(",","").Replace(" ","") == "")
                {
                    //Empty row detected
                    break;
                }

                string[] rowCells = lineContent.Split(',');

                for (int j = 0; j < rowCells.Length; j++)
                {
                    rowCells[j] = rowCells[j].Trim();
                }

                //Get created by text
                string createdBy = rowCells[0];

                //get and validate the date
                DateTime createdDate;
                if (!DateTime.TryParse(rowCells[1], out createdDate)){
                    validationError = string.Format("Date format invalid in row {0}", i);
                    return;
                }

                //Get and validate the x, y and z values
                double X, Y;
                

                if (!Double.TryParse(rowCells[2], out X))
                {
                    validationError = string.Format("POINT_X format invalid in row {0}", i);
                    return;
                }

                if (!Double.TryParse(rowCells[3], out Y))
                {
                    validationError = string.Format("POINT_Y format invalid in row {0}", i);
                    return;
                }

                double? Z_null = null;
                double Z;

                //if (rowCells[4] == ""){
                //    Z = Z_null;
                //}




                //if (!Double.TryParse(rowCells[4], (double) out Z))
                //{
                //    validationError = string.Format("POINT_X format invalid in row {0}", i);
                //    return;
                //}







                Console.WriteLine(lineContent);





                string[] lineSplit = lines[i].Split(',');


            }

            



        }
    }
}
