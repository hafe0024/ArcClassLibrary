using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTester
{
    public class UpdateDLLs
    {
        public static void update()
        {
            //Input file paths
            string dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Enbridge.dll");
            string dllXmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Enbridge.xml");

            //List of output paths for the dll
            string[] copyDllPaths = {
                                    @"C:\Program Files (x86)\Latitude Geographics\Geocortex Essentials\Default\Workflow Designer\Enbridge.dll",
                                    @"Q:\Workflow Designer\Enbridge.dll",
                                    @"Q:\REST Elements\REST\bin\Enbridge.dll"
                                    };


            //Copy to each of the output directory paths, use try catch block as the assembly may be in use and blocked from overwrite
            foreach (string copyToPath in copyDllPaths)
            {
                try
                {
                    System.IO.File.Copy(dllPath, copyToPath, true);
                    Console.WriteLine("+++++ Copied to {0}", copyToPath);
                }
                catch (System.IO.IOException ex)
                {
                    string msg = ex.Message;
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
            catch (System.IO.IOException ex)
            {
                string msg = ex.Message;
                Console.WriteLine(@"----- Not Copied XML to {0}", xmlOutputPath);
            }
        }
    }
}
