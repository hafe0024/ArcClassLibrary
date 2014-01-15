using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enbridge.DepthOfCover
{
    /// <summary>
    ///  Class to provide validation 
    /// </summary>
    /// 
    [Serializable]
    public class InputDOCTable
    {
        public string validationError = null;
        public int recordCount;
        public List<DOCRecord> docRecordList = new List<DOCRecord>();

        /// <summary>
        /// Input Depth of Cover table object constructor
        /// </summary>
        /// <param name="inputString">string representation of CSV input file</param>
        public InputDOCTable(string inputString)
        {

            docRecordList.Add(new DOCRecord("Me", DateTime.Now, 111, 111, 111));
            docRecordList.Add(new DOCRecord("cat", DateTime.Now, 222, 222, 222));
            docRecordList.Add(new DOCRecord("bird", DateTime.Now, 333, 333, 333));

            string[] lines = inputString.Split('\n');
            Console.WriteLine(lines.Length);

            recordCount = lines.Length - 1;

            for (int i = 1; i < lines.Length; i++)
            {
                string[] lineSplit = lines[i].Split(',');



            }

            validationError = "An Error";



        }
    }
}
