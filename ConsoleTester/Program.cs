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

        //static void Main(string[] args)
        //{

            
        //    Console.WriteLine("How many legs {0}", 10);
        //    Console.ReadLine();



        //}



        static void Main(string[] args)
        {




            #region hide


            //    //Enbridge.LinearReferencing.ContLineLocatorSQL locator = new Enbridge.LinearReferencing.ContLineLocatorSQL("{D4D4472B-FB1E-485B-A550-DCE76F63BC08}");

        //    //double stn, meas, MP;
        //    //string stnSeries = locator.getLocation(-93.2462, 47.1114, out stn, out meas, out MP);
            
            
            
            
            
        //    //Console.WriteLine("Series Event ID: {0}", stnSeries);
        //    //Console.WriteLine("Stationing: {0}", stn);
        //    //Console.WriteLine("Continous Measue: {0}", meas);
        //    //Console.WriteLine("Mile Post: {0}", MP);

        //    //Enbridge.LinearReferencing.VolumeOutResult result = locator.getVolumeOut(mp: 972.03);
        //    //Console.WriteLine(result.getJSON());
        //    //Console.ReadLine();

            



        //    //IDictionary<string, bool> iDict = null;

        //    //Dictionary<string, bool> realDict = new Dictionary<string,bool>(){{"car" , true}, {"bird", false}};
        //    //iDict = (IDictionary<string, bool>)realDict;
        //    //Console.WriteLine(iDict["car"]);
        //    //Console.WriteLine(iDict["bird"]);
            
        //    //Console.WriteLine(iDict.ContainsKey("hat"));
        //    //Console.WriteLine(iDict == null);
        //    //Console.ReadLine();


        //    //return;
        //    //DateTime date2 = DateTime.Parse("02/19/2000");
        //    //DateTime date1 = DateTime.Now;

        //    //Console.WriteLine(date1 < date2);

        //    //Enbridge.DepthOfCover.UpdateDocPointsSegments make = new Enbridge.DepthOfCover.UpdateDocPointsSegments();

        //    //Enbridge.ArcObjects.LicenseInit licenseInit = new Enbridge.ArcObjects.LicenseInit();
        //    //Console.WriteLine(licenseInit.isInitialized);
        //    //Console.WriteLine(licenseInit.getInitializedProduct());
        //    //licenseInit.Dispose();

        //    #region update and georef

        //    //Enbridge.Drawings.DrawingsSearch.dirSearch(Enbridge.AppConstants.DRAWINGS_ROOT_DIRECTORY);
        //    //Enbridge.Drawings.DrawingsSearch.clearRemoved();
        //    //Enbridge.Drawings.AlignmentSheets.GenerateAlignmentSheet.generateAlignmentPNG();

        //    //Console.WriteLine(ArcClassLibrary.Drawings.GeorefSheetsMethods.getDrawingsJSON());

        //    //ArcClassLibrary.Drawings.GeoRefRecord georef =
        //    //    new ArcClassLibrary.Drawings.GeoRefRecord("090-00-5.920-12150-01-MODEL", 3, 4, 1, 2, 571, 167, 1311, 313);
        //    ////ArcClassLibrary.Drawings.GeoRefRecord georef = new ArcClassLibrary.Drawings.GeoRefRecord("090-00-5.920-12150-01-MODEL");
        //    //georef.saveGeoRefInfo();

        //    #endregion

        //    //Enbridge.LinearReferencing.ContLineLocatorSQL locator = new Enbridge.LinearReferencing.ContLineLocatorSQL("{D4D4472B-FB1E-485B-A550-DCE76F63BC08}");




        //    //Enbridge.LinearReferencing.VolumeOutResult result = locator.getVolumeOut(mp: 956.4);

        //    //Console.WriteLine("Assumed: {0}", result.vAssumed);
        //    //Console.WriteLine("Out: {0}", result.vOut);

        //    //Console.WriteLine("Storage: {0}", result.vStorage);
        //    //Console.WriteLine("Total: {0}", result.vTotal);
        //    //Console.WriteLine("Percent: {0}", result.percentAccounted);

        //    //Console.WriteLine("here");




        //    #region example
        //    //Animal anAnimal = new Bird("green");
        //    //anAnimal.eat();

        //    //Bird aBird = new Bird("blue");
        //    //Console.WriteLine(aBird.color);
        //    //Console.WriteLine(aBird.num_legs);
        //    //Console.WriteLine(aBird.GetType());
            
        //    //Console.WriteLine("Bird is an Animal? {0}", aBird is Animal);

        //    //Console.WriteLine("Bird implements IDoStuff? {0}", aBird is IAnimalActions);

        //    //Console.WriteLine("Bird is an Object {0}?", aBird is Object);
        //    //aBird.eat();

        //    //Console.WriteLine("Bird's color is {0}", aBird.getColor(5));
        //    //Console.WriteLine("Another way to get Bird's color {0}", aBird.color);

        //    //Object obj = new Object();
        //    //Console.WriteLine(obj.ToString());

        //    //Console.WriteLine(aBird.ToString());

        //    //using (Cat brownCat = new Cat("brown"))
        //    //{
        //    //    brownCat.talk();
        //    //}
            
        //    //using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection())
        //    //{
        //    //    //Do Stuff with the connection here
        //    //}

        //    ////using (Enbridge.Examples.Bird oneBird = new Enbridge.Examples.Bird("brown"))
        //    ////{
        //    ////    oneBird.talk();
        //    ////}



        //    //Enbridge.Examples.SomeStaticMethods.writeSomeStuff();


        //    //Enbridge.Examples.Cat tabbyCat = new Enbridge.Examples.Cat("tabby");
        //    //Console.WriteLine("Tabby is awake before dispose? {0}", tabbyCat.is_awake);
        //    //Enbridge.Examples.SomeStaticMethods.run_dispose(tabbyCat);
        //    //Console.WriteLine("Tabby is awake after dispose? {0}", tabbyCat.is_awake);


        //    //Enbridge.Examples.SomeStaticMethods.run_dispose(tabbyCat);


        //    //Animal oneAnimal = new Bird("blue");
        //    //IAnimalActions iAnimalAct = new Bird("blue");


        //    ////List<string> list_of_strings = new List<string>();
        //    ////list_of_strings.Add("one");
        //    ////list_of_strings.Add("two");


        //    ////foreach (string a_string in list_of_strings)
        //    ////{
        //    ////    Console.WriteLine(a_string);
        //    ////}


        //    #endregion


        //    //#region test_DOC

        //    //Enbridge.DepthOfCover.InputDOCTable docTable = new Enbridge.DepthOfCover.InputDOCTable(inputString, "D4D4472B-FB1E-485B-A550-DCE76F63BC08", "descript");
        //    //Console.WriteLine(docTable.validationError);

        //    //Console.WriteLine(docTable.docRecordList[3].guid);





            
        //    ////Console.ReadLine();
        //    //#endregion

           copyDlls();
            Console.WriteLine("finish");
            Console.ReadLine();

            #endregion
        }
        #region copy dll method
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


        static double? parseDoubleString(string somestring)
        {
            double returnValue;
            bool didItWork = Double.TryParse(somestring, out returnValue);

            if (didItWork)
            {
                return returnValue;
            }
            else
            {
                return null;
            }
        }





    }    
}
