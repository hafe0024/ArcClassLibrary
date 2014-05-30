using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enbridge.Examples
{
    /*On creating a new class, Add [Serializable] decorator
     * and public access modifier*/

    /// <summary>
    /// Class to support adding records to the ExamplePoints
    /// feature class in the ControlPoint database
    /// </summary>
    [Serializable]  
    public class ExamplePoint
    {
        /*class properties, be sure to add public access modifier to all
         * properties and methods that should be exposed.
         * No access modifier means property or method will be private.
         * Best to explicitly state the private access modifier
         * for readability
         * This is fine for "helper" methods and properties that are used 
         * only within the class*/

        /// <summary>
        /// Unique identifier
        /// </summary>
        public string uniqueId;

        /// <summary>
        /// Point name defaults to "default name"
        /// Public property ie can be changed after object creation
        /// </summary>
        public string pointName = "default name";

        /// <summary>
        /// Point properties class
        /// </summary>
        public PointGeometry pointGeometry;

        /// <summary>
        /// The mile post of the point
        /// </summary>
        public double milePost;

        /// <summary>
        /// Flag to keep track if the values have been set
        /// Note private access modifier
        /// Also, declaring and initializing here will have the effect
        /// of creating default values for all created objects
        /// All new ExamplePoint objects will have valuesAssigned as false
        /// </summary>
        private bool geometryValuesAssigned = false;

        /// <summary>
        /// A static variable associated with the class itself
        /// </summary>
        public static string dummyStaticVariable = "I'm (ex)static";


        /// <summary>
        /// Constructor
        /// Always public with no return type with the name matching that of the class
        /// </summary>
        public ExamplePoint()
        {
            /*stating this.<property name> isn't necessary to access class properties
             * but I think it helps to understand that the variable is refering to #this# object*/

            //You'll see frequent use of the Guid class in my code
            //Serves the same purpose as the Guid.cal routine used in the field calculator in ArcMap
            //Guid.NewGuid() returns a Guid object, need to call ToString() in order to assign
            //to uniqueId which is a string
            this.uniqueId = Guid.NewGuid().ToString();

            //Initialize pointGeometry to a new instance of PointGeometry
            //using the constructor with no parameters
            this.pointGeometry = new PointGeometry();

            //Initialize mile post to 0 just so we don't have an
            //uninitialized variable, this could also have been set as a 
            //default value up in the property declaration
            this.milePost = 0;

        }

        /// <summary>
        /// Add a point on line 1 given the mile post
        /// </summary>
        /// <param name="milePost">Mile Post</param>
        /// <returns>success status true or false</returns>
        public bool AddPointByMilePost(double milePost)
        {
            if (milePost < 774 || milePost > 1090)
            {
                Console.WriteLine("Mile Post out of range for line 1");
                return false;
            }

            //Create a locator object for Line 1
            Enbridge.LinearReferencing.ContLineLocatorSQL loc = new Enbridge.LinearReferencing.ContLineLocatorSQL("{D4D4472B-FB1E-485B-A550-DCE76F63BC08}");

            //declare variables to hold return value and out parameters from getStnFromMP
            double meas, X, Y, Z, stationing;
            //run the locator
            stationing = loc.getStnFromMP(milePost, out meas, out X, out Y, out Z);

            //reassign the pointGeometry to a new instance of PointGeometry, created
            //using the constructor with parameters
            this.pointGeometry = new PointGeometry(X, Y, Z, stationing);

            //Set the geometryValuesAssigned flag to true
            this.geometryValuesAssigned = true;

            //set *this* object property milePost to the passed in parameter milePost
            this.milePost = milePost;

            return true;
        }

        /// <summary>
        /// Save the point to the database
        /// </summary>
        /// <returns>success status true or false</returns>
        public bool SaveToDatabase()
        {
            //initialize the success flag to false
            bool successStatus = false;

            //initialize the database connection
            // the "using" syntax does some work for us
            //Remember IDisposable?  The SqlConnection class implements it
            //At the end of the using block, a number of things happen automatically
            //like closing the database connection
            using (SqlConnection conn = new SqlConnection(AppConstants.CONN_STRING_CONTROL_POINT))
            {
                //Open the database connection
                conn.Open();

                //Create the command
                SqlCommand comm = conn.CreateCommand();

                //Build the command
                //Note the words prefixed with the @symbol
                //These are parameters and serve as variables in a way
                //After the "VALUES" keyword, the inputs look similar except for that
                //of the Shape colunm, a little extra syntax to make the geometry
                comm.CommandText = "";
                comm.CommandText += "EXEC sde.set_current_version 'SDE.Working';";
                comm.CommandText += "EXEC sde.edit_version 'SDE.Working', 1;";
                comm.CommandText += "BEGIN TRANSACTION;";
                comm.CommandText += "INSERT INTO sde.EXAMPLEPOINTS_EVW ";
                comm.CommandText += "(";
                comm.CommandText += "ID, Name, Stationing, MilePost, Latitude, Longitude, Shape";
                comm.CommandText += ") ";
                comm.CommandText += "VALUES ";
                comm.CommandText += "(";
                comm.CommandText += "@ID, @Name, @Stationing, @MilePost, @Latitude, @Longitude, geometry::STPointFromText(@geomText, 4326)";
                comm.CommandText += ");";
                comm.CommandText += "COMMIT;";
                comm.CommandText += "EXEC sde.edit_version 'SDE.Working', 2;";

                //Subsitute our values into the parameters
                comm.Parameters.AddWithValue("@ID", this.uniqueId);
                comm.Parameters.AddWithValue("@Name", this.pointName);
                comm.Parameters.AddWithValue("@Stationing", this.pointGeometry.stn_M);
                comm.Parameters.AddWithValue("@MilePost", this.milePost);
                comm.Parameters.AddWithValue("@Latitude", this.pointGeometry.Y);
                comm.Parameters.AddWithValue("@Longitude", this.pointGeometry.X);
                comm.Parameters.AddWithValue("@geomText", this.pointGeometry.GetGeometryTextString());


                //Try catch block
                //Good way to fail nicely
                //If there is an exception of the type specified by the catch
                //block, it will be handled there
                try
                {
                    //Run the command with ExcecuteNonQuery since we're not retrieving anything
                    comm.ExecuteNonQuery();
                    //Set the successStatus to true if it gets this far without and exception
                    successStatus = true;
                }
                //Catch an exception of type SqlException
                //Any other type of exception will not be caught
                //catching an exception of type "Exception" with catch every possible type
                //Not good practice though, best to know what you're doing and the probable
                //types of exceptions that would be generated
                catch (Exception exc)
                {
                    //Write the exception message out to the console 
                    Console.WriteLine(exc.Message);
                    //Set the success status to false, redundant as it was initialized to false
                    //and if it gets here in the catch block, it will have never been reset
                    successStatus = false;
                }
                //finally block always runs regardless
                finally
                {
                    //Dispose of the command, probably not necessary though but it might release some memory resources
                    comm.Dispose();
                    //Close the connection, also probably not necessary as closing of the connection is implict
                    //by use of the "using" block
                    //doesn't hurt though
                    conn.Close();
                }
            }
            //return the boolean successStatus
            return successStatus;
        }

        /// <summary>
        /// Overide the ToString method inherited from Object class
        /// Good for debugging purposes
        /// </summary>
        /// <returns>String representaiton of ExamplePoint</returns>
        public override string ToString()
        {
            // String.Format does the same replacement as Console.Writeline with placeholders ex {0}
            //Concatenate strings with +=
            //works similarly with numbers

            //just for example
            int aNumber = 5;
            aNumber += 10;
            //aNumber is now 15

            string exPointAsString = "\nExample Point Properties\n";
            exPointAsString += String.Format("Unique ID: {0}\n", this.uniqueId);
            exPointAsString += String.Format("Name: {0}\n", this.pointName);
            exPointAsString += String.Format("Mile Post: {0}\n", this.milePost);
            exPointAsString += "Point Geometry\n";
            exPointAsString += String.Format("\tLatitude: {0}\n", this.pointGeometry.Y);
            exPointAsString += String.Format("\tLongitude: {0}\n", this.pointGeometry.X);
            exPointAsString += String.Format("\tElevation: {0}\n", this.pointGeometry.Z);
            exPointAsString += String.Format("\tStationing: {0}\n", this.pointGeometry.stn_M);
            exPointAsString += String.Format("\tAs Text: {0}\n", this.pointGeometry.GetGeometryTextString());

            return exPointAsString;
        }



        /// <summary>
        /// Delete point or all points
        /// </summary>
        /// <param name="pointIdToDelete">Optional, if not provided all reports deleted</param>
        /// <returns>sucess status</returns>
        public static bool DeleteRecords(string pointIdToDelete = null)
        {
            bool successStatus = false;

            using (SqlConnection conn = new SqlConnection(AppConstants.CONN_STRING_CONTROL_POINT))
            {
                conn.Open();

                //Create the command
                SqlCommand comm = conn.CreateCommand();

                comm.CommandText = "";
                comm.CommandText += "EXEC sde.set_current_version 'SDE.Working';";
                comm.CommandText += "EXEC sde.edit_version 'SDE.Working', 1;";
                comm.CommandText += "BEGIN TRANSACTION;";
                //if no id has been provided, delete all points
                if (pointIdToDelete == null)
                {
                    comm.CommandText += "DELETE FROM sde.EXAMPLEPOINTS_EVW;";
                }
                //otherwise just delete one by adding the where clause and parameter
                else
                {
                    comm.CommandText += "DELETE FROM sde.EXAMPLEPOINTS_EVW WHERE ID=@ID;";
                    comm.Parameters.AddWithValue("@ID", pointIdToDelete);
                }
                comm.CommandText += "COMMIT;";
                comm.CommandText += "EXEC sde.edit_version 'SDE.Working', 2;";

                try
                {
                    comm.ExecuteNonQuery();
                    successStatus = true;
                }

                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                    successStatus = false;
                }
                finally
                {
                    comm.Dispose();
                    conn.Close();
                }
            }
            return successStatus;
        }

        /// <summary>
        /// Select all records and return as a list
        /// </summary>
        /// <param name="pointId">Optional, if not provided, all records selected</param>
        /// <returns>A list of ExamplePoints</returns>
        public static List<ExamplePoint> RetrieveRecords(string pointId = null)
        {
            List<ExamplePoint> examplePointList = new List<ExamplePoint>();


            using (SqlConnection conn = new SqlConnection(AppConstants.CONN_STRING_CONTROL_POINT))
            {
                conn.Open();

                //Create the command
                SqlCommand comm = conn.CreateCommand();

                comm.CommandText = "";
                comm.CommandText += "EXEC sde.set_current_version 'SDE.Working';";

                /*Note on queries
                 * You can specify a comma separated list of fields to return individually or all by 'SELECT * FROM <table/view name>'
                 * One case where you would want to not retrieve a field is with binary large objects (BLOBs)
                 * if you don't plan to do anything with the returned data, it can take a while for many records or big file data
                 * This is not the same selecting features with "Feature Attachments"
                 * The attachment data is contained in a related table 
                 * check out Cartography.SDE.Field_Crossings__ATTACH in ArcCatalog to see how this is implemented
                 * This table was automatically generated when you enabled attachments on Cartography.SDE.Field_Crossings
                 * You'll see a DATA column where the file data is contained in a blob
                 * 
                 * Sometimes you'll want to specify an alias for the fields
                 * In the case below, the column associated with Shape.STAsText() has been aliased as geomText
                 * Given that it also has the wild card * to return all fields, the Shape field data will also
                 * be retrieved
                 * However, this isn't very helpful as it will just return binary junk
                 *  STAsText() The SQL Server built in function for working with the geometry type 
                 *  turns the Shape data into something that can be read, Well Known Text (WKT)
                 *  
                 * STAsText() only returns XY and 
                 * AsTextZM() returns ZM as well
                 * Use the prior for 2d geometries and the latter for XYZM geometries as in this case
                * */

                //if no id has been provided, select all points
                if (pointId == null)
                {
                    comm.CommandText += "SELECT *, Shape.AsTextZM() as geomText FROM sde.EXAMPLEPOINTS_EVW;";
                }
                //otherwise just select one by adding the where clause and parameter
                //Remember that the method will still return a list, even if there is 1 record or 0 per the query
                else
                {
                    comm.CommandText += "SELECT *, Shape.AsTextZM() as geomText FROM sde.EXAMPLEPOINTS_EVW WHERE ID=@ID;";
                    comm.Parameters.AddWithValue("@ID", pointId);
                }

                try
                {
                    //Execute the command with ExecuteReader, returns an object of type SqlDataReader, set to variable named reader
                    SqlDataReader reader = comm.ExecuteReader();

                    /*At this point the reader is just sitting there
                     * calling reader.Read() does two things
                     * It increments the reader to the next row which on the first call will be to the first row
                     * The method also returns a boolean indicating if there are more rows
                     * In the case of 0 records returned, reader.Read() will return false on the first call
                     * an nothing within the subsequent while loop will be executed
                     * */
                    while (reader.Read())
                    {
                        /*The reader can access individual data values either
                         * by index, reader[0] for the first column
                         * or by field name, reader["anyFieldName"]
                         * Best to use the latter especially when using SELECT * as you would need to look a the 
                         * table to get the field order
                         * The lookup name is case sensitive
                         * If specifying individual fields ie SELECT field4, field2, field9 FROM ...
                         * The indices correspond to order of the specified fields
                         * 
                         * Also that values returned are always objects and need to be casted 
                         * or converted to the type of variable to which it will be assigned
                         * For strings, the ToString() method is safer or required in the case of GUIDs which
                         * are not true strings in the database
                         * 
                         * Careful with casting, for example
                         * assigning 
                         * double someDouble = (double)reader["someStringField"];
                         * will throw an InvalidCastException whereas 
                         * double someDouble = (double)reader["someDoubleField"];
                         * will work just fine ... sometimes
                         * 
                         * This gets really finicky, a fallback is to do something like the following
                         * double someDouble = Double.Parse(reader["someDoubleField"].ToString());
                         * 
                         * This is converting the returned object to a string and then converting
                         * that string to a double, this has worked when other ways haven't
                         * 
                         * Best bet to avoid casting to the extent possible
                         * 
                         * You can use the following to test for Null values in the database
                         * if (reader["SomeFieldThatMightBeNull] == DBNUll.Value)
                         * {
                         *      <Do some stuff, assign a default value or assign a numllable variable to null>
                         * }
                         * else
                         * {
                         *      string someString = reader["SomeFieldThatMightBeNull].ToString();
                         * }
                         * 
                         * The null values can complicate things, this is why you 
                         * see a bunch of nullable types ie a nullable double declared as
                         * double? someDouble;
                         * Note the question mark
                         * */


                        //Create a new Example Point
                        ExamplePoint exampPoint = new ExamplePoint();

                        /*Our new exampPoint has an automatically generated id per the constructor
                         * Overwrite that with the value retrieved from the database
                         * Remember the data type of GUIDs is not a string so we'll call the ToString() method
                         * on the returned object
                         * */
                        exampPoint.uniqueId = reader["ID"].ToString();

                        //We know the Name field is a string so casting is safe here
                        exampPoint.uniqueId = reader["Name"].ToString();

                        //Same for MilePost which is a double
                        if (reader["MilePost"] != DBNull.Value)
                        {
                            exampPoint.milePost = Double.Parse(reader["MilePost"].ToString());
                        }

                        /*Parse the geometry representation
                         * Given the "field" specified in the query
                         * Shape.AsTextZM() as geomText
                         * AsTextZM() returns text so we're safe casting rather than using ToString()
                         * */
                        string geometryString = reader["geomText"].ToString();

                        /*the geometryString is now equal to something like POINT (-97.14028 48.72294 242 138758)
                         * Clean this up with a chained replace method, to remove "POINT (" and  ")"
                         * could also be done in two separate calls
                         * */

                        geometryString = geometryString.Replace("POINT (", "").Replace(")", "");

                        //geometry string is now "-97.14028 48.72294 242 138758"

                        //Split on the space character
                        string[] geomXYZM = geometryString.Split(' ');

                        //indices 0, 1, 2, 3 correspond to X, Y, Z, and M (stationing)
                        //But they're still strings!

                        //This is a big dangerous but we'll assume that these can be converted to doubles
                        //We'll use Double.Parse rather than Double.TryParse which would be safer if we
                        //weren't sure that the strings could be converted

                        double X = Double.Parse(geomXYZM[0]);
                        double Y = Double.Parse(geomXYZM[1]);
                        double Z = Double.Parse(geomXYZM[2]);
                        double M = Double.Parse(geomXYZM[3]);

                        //update the pointGeometry property to a new PointGeometry object
                        exampPoint.pointGeometry = new PointGeometry(X, Y, Z, M);

                        //finally, add it to the list that will be returned with this method is called
                        examplePointList.Add(exampPoint);
                    }
                }

                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                    //Set the examplePointList to null as a flag that something went wrong
                    //Null is a reference to nothing rather, contrast with a List that contains
                    //0 items which would result from a query returning no records
                    examplePointList = null;
                }
                finally
                {
                    comm.Dispose();
                    conn.Close();
                }
            }
            return examplePointList;
        }


        /// <summary>
        /// Get the count of points in the table
        /// </summary>
        /// <param name="pointId">Optional, if not provided, all records counted</param>
        /// <returns>Point count</returns>
        public static int GetPointCount(string pointId = null)
        {
            //Write out the dummy static variable defined in the class definition
            //Note no use of *this* as there is no this to refer to
            Console.WriteLine(dummyStaticVariable);

            //declare an int to hold the count, initialized to -1 in case there is
            //It will stay that until the SQL command is completed
            //This serves as a flag that an error occured, ie return value of -1
            //If we declared but didn't initilize it, the return value would be an
            //uninitialized variable and would casue an error
            int count = -1;

            using (SqlConnection conn = new SqlConnection(AppConstants.CONN_STRING_CONTROL_POINT))
            {
                conn.Open();

                //Create the command
                SqlCommand comm = conn.CreateCommand();

                comm.CommandText = "";
                comm.CommandText += "EXEC sde.set_current_version 'SDE.Working';";

                //if no id has been provided, count all rows
                if (pointId == null)
                {
                    //The SQL count function is pretty useful
                    //Can count all rows with COUNT(*) or specify a field name and the count will be all rows 
                    //with non null values in that field
                    comm.CommandText += "SELECT COUNT(*) as count FROM sde.EXAMPLEPOINTS_EVW ";
                }
                //otherwise just count one by adding the where clause and parameter
                else
                {
                    comm.CommandText += "SELECT COUNT(*) as count FROM sde.EXAMPLEPOINTS_EVW WHERE ID=@ID;";
                    comm.Parameters.AddWithValue("@ID", pointId);
                }
                try
                {
                    //ExecuteScalar returns the value as an object in the first column of the first row
                    //As our count query returns a result with dimensions 1 X 1, this is exactly what we want
                    //Can get away with casting here, I think it's the ArcGIS fields that cause problems elsewhere
                    //Example. Adding a column in an attribute table within ArcGIS makes a column of type "double"
                    //but maybe with a bunch of ESRI junk in the way
                    count = (int)comm.ExecuteScalar();
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
            return count;
        }


        public static List<ESRI.ArcGIS.Client.Graphic> GetGraphicList()
        {
            
            List<ESRI.ArcGIS.Client.Graphic> graphicList = new List<ESRI.ArcGIS.Client.Graphic>();


            List<ExamplePoint> examplePointList = ExamplePoint.RetrieveRecords();

            ESRI.ArcGIS.Client.Geometry.SpatialReference spatialRef = new ESRI.ArcGIS.Client.Geometry.SpatialReference(4326);

            foreach (ExamplePoint p in examplePointList)
            {
                ESRI.ArcGIS.Client.Graphic myGraphic = new ESRI.ArcGIS.Client.Graphic();

                myGraphic.Geometry = new ESRI.ArcGIS.Client.Geometry.MapPoint(p.pointGeometry.X, p.pointGeometry.Y, spatialRef);

                myGraphic.Attributes.Add("MilePost", p.milePost);

                myGraphic.Attributes.Add("PointName", p.pointName);

                graphicList.Add(myGraphic);
                
            }


            return graphicList;


        }


    }
}
