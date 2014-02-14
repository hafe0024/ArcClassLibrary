using System;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using Enbridge.Examples;


namespace ConsoleTester
{
    class Program
    {
        static void Main(string[] args)
        {




            Enbridge.DepthOfCover.UpdateDocPointsSegments.MakeSegments();

          



            //Enbridge.ArcObjects.LicenseInit licenseInit = new Enbridge.ArcObjects.LicenseInit();
            //Console.WriteLine(licenseInit.isInitialized);
            //Console.WriteLine(licenseInit.getInitializedProduct());
            //licenseInit.Dispose();

            #region update and georef

            //ArcClassLibrary.Drawings.DrawingsSearch.dirSearch(AppConstants.DRAWINGS_ROOT_DIRECTORY);
            //ArcClassLibrary.Drawings.DrawingsSearch.clearRemoved();
            //Enbridge.Drawings.AlignmentSheets.GenerateAlignmentSheet.generateAlignmentPNG();

            //Console.WriteLine("done");

            //Console.WriteLine(ArcClassLibrary.Drawings.GeorefSheetsMethods.getDrawingsJSON());

            //ArcClassLibrary.Drawings.GeoRefRecord georef =
            //    new ArcClassLibrary.Drawings.GeoRefRecord("090-00-5.920-12150-01-MODEL", 3, 4, 1, 2, 571, 167, 1311, 313);
            ////ArcClassLibrary.Drawings.GeoRefRecord georef = new ArcClassLibrary.Drawings.GeoRefRecord("090-00-5.920-12150-01-MODEL");
            //georef.saveGeoRefInfo();

            #endregion

            //Enbridge.LinearReferencing.ContLineLocatorSQL locator = new Enbridge.LinearReferencing.ContLineLocatorSQL("{D4D4472B-FB1E-485B-A550-DCE76F63BC08}");

            //Enbridge.LinearReferencing.VolumeOutResult result = locator.getVolumeOut(mp: 956.4);

            //Console.WriteLine("Assumed: {0}", result.vAssumed);
            //Console.WriteLine("Out: {0}", result.vOut);

            //Console.WriteLine("Storage: {0}", result.vStorage);
            //Console.WriteLine("Total: {0}", result.vTotal);
            //Console.WriteLine("Percent: {0}", result.percentAccounted);

            //Console.WriteLine("here");

       


            #region example
            //Animal anAnimal = new Bird("green");

            //anAnimal.eat();

            
          




            //Bird aBird = new Bird("blue");
            //Console.WriteLine(aBird.color);
            //Console.WriteLine(aBird.num_legs);
            //Console.WriteLine(aBird.GetType());
            
            //Console.WriteLine("Bird is an Animal? {0}", aBird is Animal);

            //Console.WriteLine("Bird implements IDoStuff? {0}", aBird is IAnimalActions);

            //Console.WriteLine("Bird is an Object {0}?", aBird is Object);
            //aBird.eat();

            //Console.WriteLine("Bird's color is {0}", aBird.getColor(5));
            //Console.WriteLine("Another way to get Bird's color {0}", aBird.color);

            //Object obj = new Object();
            //Console.WriteLine(obj.ToString());

            //Console.WriteLine(aBird.ToString());

            //using (Cat brownCat = new Cat("brown"))
            //{
            //    brownCat.talk();
            //}
            
            //using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection())
            //{
            //    //Do Stuff with the connection here
            //}

            ////using (Enbridge.Examples.Bird oneBird = new Enbridge.Examples.Bird("brown"))
            ////{
            ////    oneBird.talk();
            ////}



            //Enbridge.Examples.SomeStaticMethods.writeSomeStuff();


            //Enbridge.Examples.Cat tabbyCat = new Enbridge.Examples.Cat("tabby");
            //Console.WriteLine("Tabby is awake before dispose? {0}", tabbyCat.is_awake);
            //Enbridge.Examples.SomeStaticMethods.run_dispose(tabbyCat);
            //Console.WriteLine("Tabby is awake after dispose? {0}", tabbyCat.is_awake);


            //Enbridge.Examples.SomeStaticMethods.run_dispose(tabbyCat);


            //Animal oneAnimal = new Bird("blue");
            //IAnimalActions iAnimalAct = new Bird("blue");


            ////List<string> list_of_strings = new List<string>();
            ////list_of_strings.Add("one");
            ////list_of_strings.Add("two");


            ////foreach (string a_string in list_of_strings)
            ////{
            ////    Console.WriteLine(a_string);
            ////}


            #endregion


            //#region test_DOC
            //string inputString = "CreatedBy,CreatedDate,POINT_X,POINT_Y,POINT_Z,Measurement,Description,EquipmentID,Accuracy,Probe\n";
            //inputString += "NREC,11/21/2007,-92.68507821,46.7608733,394.6309608,38, ,,10,0\n";
            //inputString += "NREC,11/21/2007,-92.68481501,46.76068996,395.9879304,47, ,,5,0\n";
            //inputString += "NREC,11/21/2007,-92.68457329,46.76053073,395.1192504,24, ,,,0\n";
            //inputString += "NREC,11/21/2007,-92.68435982,46.76038175,395.566392,45, ,,,0\n";
            //inputString += "NREC,11/21/2007,-92.69478264,46.76761193,394.7827512,15,.4'  turned over dirt.,,,1\n";
            //inputString += "NREC,11/21/2007,-92.69233474,46.76590621,394.4374128,13,.3' brusher turnaround area,,,1\n";
            //inputString += "NREC,11/28/2007,-92.63395151,46.72615016,417.0630216,0,w end of span,,,0\n";
            //inputString += "NREC,11/28/2007,-92.63667742,46.72808821,423.2998392,516,unreliable,,,0\n";
            //inputString += "NREC,12/12/2007,-92.56644628,46.67902546,390.698736,49,construction area,,,0\n";
            //inputString += "NREC,12/11/2007,-92.58632704,46.69130319,393.8826768,56, ,,,0\n";
            //inputString += "NREC,11/28/2007,-92.63133786,46.72430701,416.841432,0,e end of span,,,0\n";
            //inputString += ",,,,,,,,,\n";
            //inputString += ",,,,,,,,,\n";

            //Enbridge.DepthOfCover.InputDOCTable docTable = new Enbridge.DepthOfCover.InputDOCTable(inputString, "D4D4472B-FB1E-485B-A550-DCE76F63BC08", "descript");
            //Console.WriteLine(docTable.validationError);

            //Console.WriteLine(docTable.docRecordList[3].guid);





            
            ////Console.ReadLine();
            //#endregion

            //copyDlls();
            Console.ReadLine();
        }
        #region test_DOC2
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
            string[] copyDllPaths = {
                                    @"C:\Program Files (x86)\Latitude Geographics\Workflow Designer\Enbridge.dll",
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
                catch (IOException ex)
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
            catch (IOException ex)
            {
                string msg = ex.Message;
                Console.WriteLine(@"----- Not Copied XML to {0}", xmlOutputPath);
            }
        }
        #endregion
    }    
}
