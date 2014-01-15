using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConsoleTester
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputString = "CreatedBy,CreatedDate,POINT_X,POINT_Y,POINT_Z,Measurement,Description,EquipmentID,Accuracy,Probe\n";
            inputString += "NREC,12/11/2007,-92.59408969,46.69754508,395.4645888,57, ,,,0\n";
            inputString += "NREC,12/11/2007,-92.59382724,46.69732872,398.3757336,68, ,,,0\n";
            inputString += "NREC,12/11/2007,-92.59356827,46.69711391,399.9558168,60, ,,,0\n";
            inputString += "NREC,12/11/2007,-92.59329646,46.6969074,399.0728112,62, ,,,\n";
            inputString += "NREC,12/11/2007,-92.59305609,46.69671142,397.6201344,61, ,,,0\n";
            inputString += "NREC,12/11/2007,-92.59282005,46.69651791,397.6807896,56, ,,,0\n";
            inputString += "NREC,12/11/2007,-92.59258874,46.6963447,396.2058624,80, ,,,0\n";
            inputString += "NREC,12/11/2007,-92.59235377,46.69614653,396.313152,64, ,,,0\n";
            inputString += "NREC,12/11/2007,-92.59211365,46.69596511,395.6843496,49, ,,,0\n";
            inputString += "NREC,12/11/2007,-92.59190761,46.69578785,394.048488,49, ,,,0\n";
            inputString += "NREC,12/11/2007,-92.59165039,46.69558932,394.3581648,54, ,,,0\n";
            inputString += ",,,,,,,,,\n";
            inputString += ",,,,,,,,,\n";

            Enbridge.DepthOfCover.InputDOCTable docTable = new Enbridge.DepthOfCover.InputDOCTable(inputString);

            copyDlls();
            Console.ReadLine();
        }


        /// <summary>
        /// Copy the build dlls into the workflow resources folder
        /// Also need to copy the dll and assembly xml to the  Rest Elements\REST\bin folder
        /// </summary>
        private static void copyDlls()
        {
            //Input file paths
            string dllPath = @"S:\EnbridgeClassLibrary\Enbridge\bin\Debug\Enbridge.dll";
            string dllXmlPath = @"S:\EnbridgeClassLibrary\Enbridge\bin\Debug\Enbridge.xml";

            //List of output paths for the dll
            string[] copyDllPaths = {@"Q:\Workflow Designer\Enbridge.dll",
                                    @"C:\Program Files (x86)\Latitude Geographics\Geocortex Essentials\Default\Workflow Designer\Enbridge.dll",
                                    @"Q:\REST Elements\REST\bin\Enbridge.dll"};


            //Copy to each of the output directory paths, use try catch block as the assembly may be in use and blocked from overwrite
            foreach (string copyToPath in copyDllPaths)
            {
                try
                {
                    System.IO.File.Copy(dllPath, copyToPath, true);
                    Console.WriteLine("+++++ Copied to {0}", copyToPath);
                }
                catch (IOException ex)
                {
                    Console.WriteLine("----- Not Copied to {0}", copyToPath);
                }
            }
           
            //Copy XML file to server Rest Elements\REST\bin folder
            string xmlOutputPath = @"Q:\REST Elements\REST\bin\Enbridge.xml";

            try
            {
                System.IO.File.Copy(dllXmlPath, xmlOutputPath, true);
                Console.WriteLine(@"+++++ Copied XML to {0}", xmlOutputPath);
            }
            catch (IOException ex)
            {
                Console.WriteLine(@"----- Not Copied XML to {0}", xmlOutputPath);
            }
        }
    }
}
