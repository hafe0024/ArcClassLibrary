using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enbridge.PLM
{
    public static class PLM_Helpers
    {

        /// <summary>
        /// returns a nullable bool based on the passed in object
        /// no selection will return null
        /// yes or no defines the boolean as expected
        /// </summary>
        /// <param name="trueFalseObject"></param>
        /// <returns></returns>
        public static bool? trueFalseValue(Object trueFalseObject)
        {
            bool? returnValue;
            if (trueFalseObject == null)
            {
                returnValue = null;
            }
            else
            {
                switch (trueFalseObject.ToString())
                {
                    case "no":
                        returnValue = false;
                        break;
                    case "yes":
                        returnValue = true;
                        break;
                    default:
                        returnValue = null;
                        break;
                }
            }
            return returnValue;
        }

        /// <summary>
        /// get the value of the passed in object defining the selection
        /// if no selection has been made in the workflow, the object will be null
        /// return the selected value or 'null' if none has been selected
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string getComboBoxSelectedValue(Object obj)
        {
            string returnValue;
            if (obj == null)
            {
                returnValue = null;
            }
            else
            {
                returnValue = obj.ToString();
            }
            return returnValue;

        }

        public static double? convertStringToNullableDouble(string input)
        {
            double parseDoubleOutput;
            if (double.TryParse(input, out parseDoubleOutput))
            {
                return parseDoubleOutput;
            }
            else
            {
                return null;
            }
        }

    }
}
