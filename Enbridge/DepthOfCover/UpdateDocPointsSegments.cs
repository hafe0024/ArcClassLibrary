using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Enbridge.DepthOfCover
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class UpdateDocPointsSegments
    {
        public List<DocPoint> docPointsList;
        double searchDistance = 0.0000314006369436025;

        public UpdateDocPointsSegments(string routeidentifier = null)
        {

            docPointsList = new List<DocPoint>();

            List<string> routeEventIds = new List<string>();

            if (routeidentifier == null)
            {
                Console.WriteLine("route id is null");
                using (SqlConnection conn = new SqlConnection(AppConstants.CONN_STRING_DOC))
                {
                    conn.Open();
                    SqlCommand comm = conn.CreateCommand();
                    comm.CommandText = "EXEC sde.set_current_version 'sde.WORKING';";
                    comm.CommandText += "Select Distinct RouteEventID from sde.DEPTHOFCOVER_EVW WHERE Status='Active';";

                    try
                    {
                        SqlDataReader reader = comm.ExecuteReader();
                        while (reader.Read())
                        {
                            routeEventIds.Add(reader[0].ToString());
                        }
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            else
            {
                Console.WriteLine("route id not null");
                routeEventIds.Add(routeidentifier);
            }

            foreach (string id in routeEventIds)
            {
                docPointsList.Clear();

                string routeId = id.ToUpper();
                Console.WriteLine(routeId);

                using (SqlConnection conn = new SqlConnection(AppConstants.CONN_STRING_DOC))
                {
                    conn.Open();
                    SqlCommand comm = conn.CreateCommand();
                    comm.CommandText = "EXEC sde.set_current_version 'sde.WORKING';";
                    comm.CommandText += "Select EventID, CreatedBy, CreatedDate, POINT_X, POINT_Y, POINT_Z, Measurement, ";
                    comm.CommandText += "Description, EquipmentID, Accuracy, Probe, SeriesEventID, Station, Measure, ";
                    comm.CommandText += "RouteEventID, PointGroupID, ModifiedBy ";
                    comm.CommandText += "from sde.DEPTHOFCOVER_EVW ";
                    comm.CommandText += "WHERE Status='Active' AND RouteEventID = @routeId ";
                    comm.CommandText += "Order by Measure ASC;";
                    comm.Parameters.AddWithValue("routeId", routeId);

                    try
                    {
                        SqlDataReader reader = comm.ExecuteReader();
                        while (reader.Read())
                        {
                            docPointsList.Add(new DocPoint(reader));
                        }
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }

                UpdatePoints();

                this.docPointsList.RemoveAll(isArchive);
                this.docPointsList.RemoveAll(toRemove);

                switch (this.docPointsList.Count)
                {
                    case 0:
                        return;
                    case 1:
                        this.docPointsList[0].segmentStart = this.docPointsList[0].measure - 300;
                        this.docPointsList[0].segmentStop = this.docPointsList[0].measure + 300;
                        break;
                    default:
                        this.docPointsList[0].segmentStart = this.docPointsList[0].measure - 300;
                        this.docPointsList[0].segmentStop = (this.docPointsList[0].measure + this.docPointsList[1].measure) / 2;
                        this.docPointsList[0].segmentStop = (
                            this.docPointsList[0].segmentStop - this.docPointsList[0].measure > 300 ?
                            this.docPointsList[0].measure + 300 :
                            this.docPointsList[0].segmentStop);

                        for (int i = 1; i < this.docPointsList.Count - 1; i++)
                        {
                            double startMeasure = (this.docPointsList[i - 1].measure + this.docPointsList[i].measure) / 2;
                            startMeasure = (this.docPointsList[i].measure - startMeasure > 300 ? this.docPointsList[i].measure - 300 : startMeasure);
                            this.docPointsList[i].segmentStart = startMeasure;

                            double endMeasure = (this.docPointsList[i].measure + this.docPointsList[i + 1].measure) / 2;
                            endMeasure = (endMeasure - this.docPointsList[i].measure > 300 ? this.docPointsList[i].measure + 300 : endMeasure);
                            this.docPointsList[i].segmentStop = endMeasure;
                        }

                        this.docPointsList[this.docPointsList.Count - 1].segmentStart = (this.docPointsList[this.docPointsList.Count - 1].measure + this.docPointsList[this.docPointsList.Count - 2].measure) / 2;
                        this.docPointsList[this.docPointsList.Count - 1].segmentStart =
                            (this.docPointsList[this.docPointsList.Count - 1].measure - this.docPointsList[this.docPointsList.Count - 1].segmentStart > 300 ?
                             this.docPointsList[this.docPointsList.Count - 1].measure - 300 :
                             this.docPointsList[this.docPointsList.Count - 1].segmentStart);
                        this.docPointsList[this.docPointsList.Count - 1].segmentStop = this.docPointsList[this.docPointsList.Count - 1].measure + 300;

                        break;
                }

                Enbridge.LinearReferencing.ContLineLocatorSQL locator = new Enbridge.LinearReferencing.ContLineLocatorSQL(routeId);

                double minMeasure = locator.pointList[0].meas;
                double maxMeasure = locator.pointList[locator.pointList.Count - 1].meas;

                List<DocSegment> segmentList = new List<DocSegment>();

                for (int i = 0; i < this.docPointsList.Count; i++)
                {

                    //Console.WriteLine("{0} {1} {2}", segList[i][0], segList[i][1], segList[i][2]);
                    this.docPointsList[i].segmentStart = (this.docPointsList[i].segmentStart < minMeasure ? minMeasure : this.docPointsList[i].segmentStart);
                    this.docPointsList[i].segmentStop = (this.docPointsList[i].segmentStop > maxMeasure ? maxMeasure : this.docPointsList[i].segmentStop);
                    double startStn, endStn;
                    //Console.WriteLine("__{0}, {1}", this.docPointsList[i].segmentStart, this.docPointsList[i].segmentStop);
                    string geomString = locator.makeSegmentLineString(this.docPointsList[i].segmentStart, this.docPointsList[i].segmentStop, out startStn, out endStn);
                    DocSegment seg = new DocSegment(geomString, startStn, endStn, 
                        this.docPointsList[i].seriesEventId, this.docPointsList[i].measurement, 
                        this.docPointsList[i].eventID, routeId);
                    segmentList.Add(seg);
                }

                using (SqlConnection conn = new SqlConnection(AppConstants.CONN_STRING_DOC))
                {
                    conn.Open();
                    SqlCommand comm = conn.CreateCommand();
                    string commandInit = "EXEC sde.set_current_version 'sde.WORKING';";
                    commandInit += "EXEC sde.edit_version 'sde.working', 1;";
                    commandInit += "begin transaction;";
                    comm.CommandText = commandInit;
                    comm.CommandText += "DELETE from sde.depthofcoversegments_evw where routeeventid = @routeid or routeeventid is null;";
                    string commandEnd = "COMMIT;";
                    commandEnd += "EXEC sde.edit_version 'sde.WORKING', 2;";
                    comm.CommandText += commandEnd;
                    comm.Parameters.AddWithValue("@routeid", routeId);

                    try
                    {
                        comm.ExecuteNonQuery();
                        comm.Parameters.Clear();

                        comm.CommandText = commandInit;

                        int counter = 0;

                        for (int i = 0; i < segmentList.Count; i++)
                        {
                            string commandString = "INSERT INTO sde.depthofcoversegments_evw ";
                            commandString += "(";
                            commandString += "shape, startstationing, endstationing, eventid, origineventid, ";
                            commandString += "CreatedBy, ModifiedBy, ";
                            //commandString += ", createddate, lastmodified, ";
                            commandString += "serieseventid, measurement, pointeventid, routeeventid";
                            commandString += ") ";
                            commandString += "VALUES ";
                            commandString += segmentList[i].ToString();
                            commandString += ";";
                            comm.CommandText += commandString;

                            counter++;
                            if (counter > 50 || i == segmentList.Count - 1)
                            {
                                comm.CommandText += commandEnd;
                                comm.ExecuteNonQuery();
                                Console.WriteLine("segments in");
                                comm.CommandText = commandInit;
                                counter = 0;
                            }
                        }

                        Console.WriteLine("success");
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        comm.Dispose();
                        conn.Close();
                        Console.WriteLine("exited");
                    }
                }
            }
        }

        private void UpdatePoints()
        {

            List<DocPoint> pointCheckList = new List<DocPoint>(this.docPointsList);
            Dictionary<string, List<string>> neighborsDict = new  Dictionary<string, List<string>>();
            Dictionary<string, DocPoint> pointDictionary = new Dictionary<string,DocPoint>();

            while (true)
            {
                string thisPointId = pointCheckList[0].eventID;
                pointDictionary[thisPointId] = pointCheckList[0];
                neighborsDict[thisPointId] = new List<string>();
                
                for (int i = 1; i < pointCheckList.Count; i++)
                {
                    if (pointCheckList[0].getDistance(pointCheckList[i]) < searchDistance)
                    {
                        string searchPointId = pointCheckList[i].eventID;
                        neighborsDict[thisPointId].Add(searchPointId);

                        if (!neighborsDict.ContainsKey(searchPointId))
                        {
                            neighborsDict[searchPointId] = new List<string>();
                        }
                        neighborsDict[searchPointId].Add(thisPointId);
                    }
                }
                pointCheckList.Remove(pointCheckList[0]);
                if (pointCheckList.Count == 0)
                {
                    break;
                }
            }

            foreach (KeyValuePair<string, List<string>> item in neighborsDict)
            {
                string key = item.Key;
                List<string> neighborsList = item.Value;

                foreach (string checkKey in neighborsList)
                {
                    if (pointDictionary[key].createdDate < pointDictionary[checkKey].createdDate || 
                        (pointDictionary[key].probe == false && pointDictionary[checkKey].probe == true))
                    {
                        pointDictionary[key].flagArchive = true;
                        break;
                    }

                    if (pointDictionary[key].Equals(pointDictionary[checkKey]))
                    {
                        pointDictionary[key].toRemove = true;
                    }
                }
            }

            var archiveResult = from c in this.docPointsList where c.flagArchive select "'" + c.eventID + "'";
            var currentResult = from c in this.docPointsList where !c.flagArchive select "'" + c.eventID + "'";
            var removeResult = from c in this.docPointsList where c.toRemove select "'" + c.eventID + "'";
            List<string> archiveList = archiveResult.ToList();
            List<string> currentList = currentResult.ToList();
            List<string> removeList = removeResult.ToList();

            Console.WriteLine("Archive Points Count: {0}", archiveList.Count);
            Console.WriteLine("Current Points Count: {0}", currentList.Count);
            Console.WriteLine("Remove Points Count: {0}", removeList.Count);

            string initCommand = "EXEC sde.set_current_version 'sde.WORKING';";
            initCommand += "EXEC sde.edit_version 'sde.working', 1;";
            initCommand += "BEGIN TRANSACTION;";

            string endCommand = "COMMIT;";
            endCommand += "EXEC sde.edit_version 'sde.WORKING', 2;";

            using (SqlConnection conn = new SqlConnection(AppConstants.CONN_STRING_DOC))
            {
                conn.Open();
                SqlCommand comm = conn.CreateCommand();

                try
                {
                    List<string> archiveGroups = idGroupsFromList(archiveList);
                    foreach (string group in archiveGroups)
                    {
                        comm.CommandText = initCommand;
                        comm.CommandText += "UPDATE sde.DEPTHOFCOVER_EVW SET HistoricalState='Historical' WHERE EventID in (" + group + ");";
                        comm.CommandText += endCommand;
                        comm.ExecuteNonQuery();
                        Console.WriteLine("groupUdated");
                    }

                    List<string> currentGroups = idGroupsFromList(currentList);
                    foreach (string group in currentGroups)
                    {
                        comm.CommandText = initCommand;
                        comm.CommandText += "UPDATE sde.DEPTHOFCOVER_EVW SET HistoricalState='Current' WHERE EventID in (" + group + ");";
                        comm.CommandText += endCommand;
                        comm.ExecuteNonQuery();
                        Console.WriteLine("groupUdated");
                    }

                    List<string> deleteGroups = idGroupsFromList(removeList);
                    foreach (string group in deleteGroups)
                    {
                        comm.CommandText = initCommand;
                        comm.CommandText += "UPDATE sde.DEPTHOFCOVER_EVW SET HistoricalState='Duplicate', Status='Inactive' WHERE EventID in (" + group + ");";
                        comm.CommandText += endCommand;
                        comm.ExecuteNonQuery();
                        Console.WriteLine("groupUdated");
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    comm.Dispose();
                    conn.Close();
                    Console.WriteLine("exited");
                }
            }
        }

        private bool isArchive(DocPoint point)
        {
            return point.flagArchive;
        }

        private bool toRemove(DocPoint point)
        {
            return point.toRemove;
        }

        private static List<string> idGroupsFromList(List<string> inputList)
        {
            int counter = 0;
            List<string> outputList = new List<string>();
            List<string> tempList = new List<string>();

            for (int i = 0; i < inputList.Count; i++)
            {
                tempList.Add(inputList[i]);
                counter++;
                if (counter > 200 || i == inputList.Count - 1)
                {
                    counter = 0;
                    if (tempList.Count > 0)
                    {
                        outputList.Add(string.Join(",",tempList));
                    }

                    tempList.Clear();
                }
            }
            return outputList;
        }
    }
}

