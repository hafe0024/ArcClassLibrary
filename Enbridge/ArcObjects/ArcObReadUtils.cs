using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Enbridge.ArcObjects
{
    /// <summary>
    /// 
    /// </summary>
    public static class ArcObReadUtils
    {
        /// <summary>
        /// Open a feature class from an SDE Workspace
        /// </summary>
        /// <param name="conFilePath">string path to the SDE connection file</param>
        /// <param name="fcName">Feature class name</param>
        /// <param name="fdsName">Optional name of feature dataset</param>
        /// <param name="versionName">Optional version name, defaults to the version specified by the connection file</param>
        /// <returns></returns>

        public static IFeatureClass openFeatureClassFromSDE(string conFilePath, string fcName, string fdsName = "", string versionName = "")
        {
            IFeatureClass featClass = null;
            IWorkspaceFactory factory = new SdeWorkspaceFactory();
            IPropertySet props = factory.ReadConnectionPropertiesFromFile(conFilePath);
            if (versionName != "")
            {
                props.SetProperty("VERSION", versionName);
            }
            IWorkspace workspace = factory.Open(props, 0);

            IFeatureWorkspace featWorkspace = (IFeatureWorkspace)workspace;
            if (fdsName != "")
            {
                IFeatureDataset featdataset = featWorkspace.OpenFeatureDataset(fdsName);
                IFeatureWorkspace datasetWorkspace = (IFeatureWorkspace)featdataset.Workspace;
                featClass = datasetWorkspace.OpenFeatureClass(fcName);
                //featClass = featWorkspace.OpenFeatureClass(fdsName + "\\" + fcName);
            }
            else
            {
                featClass = featWorkspace.OpenFeatureClass(fcName);
            }
            return featClass;
        }

        /// <summary>
        /// Open a feature class from a GDB
        /// </summary>
        /// <param name="conFilePath">Path to GDB workspace</param>
        /// <param name="fcName">Feature class name</param>
        /// <param name="fdsName">Optional feature dataset name</param>
        /// <returns></returns>

        public static IFeatureClass openFeatureClassFromGDB(string conFilePath, string fcName, string fdsName = "")
        {
            IFeatureClass featClass = null;
            IWorkspaceFactory factory = new FileGDBWorkspaceFactoryClass();

            IWorkspace workspace = factory.OpenFromFile(conFilePath, 0);
            IFeatureWorkspace featWorkspace = (IFeatureWorkspace)workspace;

            if (fdsName != "")
            {
                IFeatureDataset featdataset = featWorkspace.OpenFeatureDataset(fdsName);
                IFeatureWorkspace datasetWorkspace = (IFeatureWorkspace)featdataset.Workspace;
                featClass = datasetWorkspace.OpenFeatureClass(fcName);
            }
            else
            {
                featClass = featWorkspace.OpenFeatureClass(fcName);
            }
            return featClass;
        }

        /// <summary>
        /// Open a table class from an SDE Workspace
        /// </summary>
        /// <param name="conFilePath">string path to the SDE connection file</param>
        /// <param name="tabName">Table class name</param>
        /// <param name="fdsName">Optional name of feature dataset</param>
        /// <param name="versionName">Optional version name, defaults to the version specified by the connection file</param>
        /// <returns></returns>

        public static ITable openTableFromSDE(string conFilePath, string tabName, string fdsName = "", string versionName = "")
        {
            ITable table = null;
            IWorkspaceFactory factory = new SdeWorkspaceFactoryClass();
            IPropertySet props = factory.ReadConnectionPropertiesFromFile(conFilePath);

            if (versionName != "")
            {
                props.SetProperty("VERSION", versionName);
            }
            IWorkspace workspace = factory.Open(props, 0);

            IFeatureWorkspace featWorkspace = (IFeatureWorkspace)workspace;
            if (fdsName != "")
            {
                IFeatureDataset featdataset = featWorkspace.OpenFeatureDataset(fdsName);
                IFeatureWorkspace datasetWorkspace = (IFeatureWorkspace)featdataset.Workspace;
                table = datasetWorkspace.OpenTable(tabName);
            }
            else
            {
                table = featWorkspace.OpenTable(tabName);
            }
            return table;
        }

        /// <summary>
        /// Open a table class from a GDB
        /// </summary>
        /// <param name="conFilePath">Path to GDB workspace</param>
        /// <param name="tabName">Table name</param>
        /// <param name="fdsName">Optional feature dataset name</param>
        /// <returns></returns>

        public static ITable openTableFromGDB(string conFilePath, string tabName, string fdsName = "")
        {
            ITable table = null;
            IWorkspaceFactory factory = new FileGDBWorkspaceFactoryClass();

            IWorkspace workspace = factory.OpenFromFile(conFilePath, 0);
            IFeatureWorkspace featWorkspace = (IFeatureWorkspace)workspace;

            if (fdsName != "")
            {
                IFeatureDataset featdataset = featWorkspace.OpenFeatureDataset(fdsName);
                IFeatureWorkspace datasetWorkspace = (IFeatureWorkspace)featdataset.Workspace;
                table = datasetWorkspace.OpenTable(tabName);
            }
            else
            {
                table = featWorkspace.OpenTable(tabName);
            }
            return table;
        }

        //public static ITable openTableFromMemory(string tabName, string fdsName = "")
        //{
        //    ITable table = null;
        //    IWorkspaceFactory factory = new InMemoryWorkspaceFactory();

        //    IWorkspace workspace = factory.OpenFromFile(conFilePath, 0);
        //    IFeatureWorkspace featWorkspace = (IFeatureWorkspace)workspace;

        //    if (fdsName != "")
        //    {
        //        IFeatureDataset featdataset = featWorkspace.OpenFeatureDataset(fdsName);
        //        IFeatureWorkspace datasetWorkspace = (IFeatureWorkspace)featdataset.Workspace;
        //        table = datasetWorkspace.OpenTable(tabName);
        //    }
        //    else
        //    {
        //        table = featWorkspace.OpenTable(tabName);
        //    }
        //    return table;
        //}
    }
}