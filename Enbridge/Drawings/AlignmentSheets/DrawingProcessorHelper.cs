using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enbridge.Drawings.AlignmentSheets
{
    public class DrawingProcessorHelper: IDisposable
    {
        public ESRI.ArcGIS.Geoprocessor.Geoprocessor gp;
        public ESRI.ArcGIS.Geometry.IPointCollection4 sourcePoints;
        public ESRI.ArcGIS.Geometry.IPointCollection4 targetPoints;
        public ESRI.ArcGIS.DataManagementTools.DefineProjection defineProj;
        public ESRI.ArcGIS.Geodatabase.IWorkspaceFactory workspaceFact;
        public ESRI.ArcGIS.DataSourcesRaster.IRasterWorkspace rastWorkspace = null;
        public ESRI.ArcGIS.Geodatabase.IRasterDataset rasDataset = null;
        public ESRI.ArcGIS.Geodatabase.IRaster rast = null;
        public ESRI.ArcGIS.DataSourcesRaster.IRasterGeometryProc rasterPropc;
        private Enbridge.ArcObjects.LicenseInit licenseInit;


        public DrawingProcessorHelper()
        {
            this.licenseInit = new Enbridge.ArcObjects.LicenseInit();

            this.gp = new ESRI.ArcGIS.Geoprocessor.Geoprocessor();
            string outputCoordinateSystem = "PROJCS['WGS_1984_Web_Mercator_Auxiliary_Sphere',";
            outputCoordinateSystem += "GEOGCS['GCS_WGS_1984',DATUM['D_WGS_1984',SPHEROID['WGS_1984',6378137.0,298.257223563]],";
            outputCoordinateSystem += "PRIMEM['Greenwich',0.0],UNIT['Degree',0.0174532925199433]],PROJECTION['Mercator_Auxiliary_Sphere'],";
            outputCoordinateSystem += "PARAMETER['False_Easting',0.0],PARAMETER['False_Northing',0.0],PARAMETER['Central_Meridian',0.0],";
            outputCoordinateSystem += "PARAMETER['Standard_Parallel_1',0.0],PARAMETER['Auxiliary_Sphere_Type',0.0],UNIT['Meter',1.0]]";
            this.gp.OverwriteOutput = true;
            this.gp.SetEnvironmentValue("outputCoordinateSystem", outputCoordinateSystem);

            this.sourcePoints = new ESRI.ArcGIS.Geometry.PathClass();
            this.sourcePoints.AddPoint(new ESRI.ArcGIS.Geometry.Point());
            this.sourcePoints.AddPoint(new ESRI.ArcGIS.Geometry.Point());
            this.targetPoints = new ESRI.ArcGIS.Geometry.PathClass();
            this.targetPoints.AddPoint(new ESRI.ArcGIS.Geometry.Point());
            this.targetPoints.AddPoint(new ESRI.ArcGIS.Geometry.Point());

            this.defineProj = new ESRI.ArcGIS.DataManagementTools.DefineProjection();
            this.defineProj.coor_system = outputCoordinateSystem;

            this.workspaceFact = new ESRI.ArcGIS.DataSourcesRaster.RasterWorkspaceFactoryClass();

            rasterPropc = new ESRI.ArcGIS.DataSourcesRaster.RasterGeometryProc();

 
        }

        public void Dispose()
        {
            licenseInit.shutdown();
        }
    }
}
