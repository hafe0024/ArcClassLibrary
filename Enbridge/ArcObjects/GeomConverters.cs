using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESRI.ArcGIS.Geometry;
//using NetTopologySuite.IO;

//namespace Enbridge.ArcObjects
//{
//    /// <summary>
//    /// This class is used to convert a GeoAPI Geometry to ESRI and vice-versa.
//    /// It can also convert a ESRI Geometry to WKB/WKT and vice-versa.
//    /// </summary>
//    public static class Converters
//    {

//        public static byte[] ConvertGeometryToWKB(IGeometry geometry)
//        {
//            IWkb wkb = geometry as IWkb;
//            ITopologicalOperator oper = geometry as ITopologicalOperator;
//            oper.Simplify();

//            IGeometryFactory3 factory = new GeometryEnvironment() as IGeometryFactory3;
//            byte[] b = factory.CreateWkbVariantFromGeometry(geometry) as byte[];
//            return b;
//        }


//        public static byte[] ConvertWKTToWKB(string wkt)
//        {
//            WKBWriter writer = new WKBWriter();
//            WKTReader reader = new WKTReader();
//            return writer.Write(reader.Read(wkt));
//        }

//        public static string ConvertWKBToWKT(byte[] wkb)
//        {
//            WKTWriter writer = new WKTWriter();
//            WKBReader reader = new WKBReader();
//            return writer.Write(reader.Read(wkb));
//        }

//        public static string ConvertGeometryToWKT(IGeometry geometry)
//        {
//            byte[] b = ConvertGeometryToWKB(geometry);
//            WKBReader reader = new WKBReader();
//            GeoAPI.Geometries.IGeometry g = reader.Read(b);
//            WKTWriter writer = new WKTWriter();
//            return writer.Write(g);
//        }

//        public static IGeometry ConvertWKTToGeometry(string wkt)
//        {
//            byte[] wkb = ConvertWKTToWKB(wkt);
//            return ConvertWKBToGeometry(wkb);
//        }

//        public static IGeometry ConvertWKBToGeometry(byte[] wkb)
//        {
//            IGeometry geom;
//            int countin = wkb.GetLength(0);
//            IGeometryFactory3 factory = new GeometryEnvironment() as IGeometryFactory3;
//            factory.CreateGeometryFromWkbVariant(wkb, out geom, out countin);
//            return geom;
//        }


//        public static IGeometry ConvertGeoAPIToESRI(GeoAPI.Geometries.IGeometry geometry)
//        {
//            WKBWriter writer = new WKBWriter();
//            byte[] bytes = writer.Write(geometry);
//            return ConvertWKBToGeometry(bytes);
//        }

//        public static GeoAPI.Geometries.IGeometry ConvertESRIToGeoAPI(IGeometry geometry)
//        {
//            byte[] wkb = ConvertGeometryToWKB(geometry);
//            WKBReader reader = new WKBReader();
//            return reader.Read(wkb);
//        }

//        public static IPoint ConvertESRI_TextToIPoint(string input)
//        {
//            //Create the spatial reference to be used by the point, using WGS-84 as points given in lat-long
//            ISpatialReferenceFactory3 spatialReferenceFactory = new SpatialReferenceEnvironmentClass();
//            ISpatialReference spatialReference = spatialReferenceFactory.CreateSpatialReference(4326);

//            //Parse the string
//            input = input.Substring(input.IndexOf("(") + 1, input.IndexOf(")"));
//            string[] coordinates = input.Split(null);
            
//            //Generate the geometry
//            IPoint newGeom = new Point();
//            IZAware zA = (IZAware)newGeom;
//            zA.ZAware = true;
//            IMAware mA = (IMAware)newGeom;
//            mA.MAware = true;

//            if (coordinates.Length > 0)
//                newGeom.X = double.Parse(coordinates[0]);
//            if (coordinates.Length > 1)
//                newGeom.Y = double.Parse(coordinates[1]);
//            if (coordinates.Length > 2)
//                newGeom.Z = double.Parse(coordinates[2]);
//            if (coordinates.Length > 3)
//                newGeom.M = double.Parse(coordinates[4]);

//            newGeom.SpatialReference = spatialReference;

//            return newGeom;
//        }
//    }
//}