//using ESRI.ArcGIS.Geodatabase;
//using ESRI.ArcGIS.Geometry;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Enbridge.GeoJSON
//{
//    [Serializable]
//    public static class ToJSONConverter
//    {
//        /// <summary>
//        /// Generate GeoJSON string from arcobject cursor
//        /// </summary>
//        /// <param name="cursor"></param>
//        /// <returns></returns>
//        public static string getJSONFromCursor(IFeatureCursor cursor)
//        {
//            IFields fields = cursor.Fields;
//            GeoJSON geoJSON = new GeoJSON();

//            IFeature featureRow = null;
//            int count = 0;
//            while ((featureRow = cursor.NextFeature()) != null)
//            {
//                count++;
//                Console.WriteLine(count);
//                JSONFeature newFeature = new JSONFeature();
//                newFeature.addGeom(featureRow.Shape);

//                for (int i = 0; i < fields.FieldCount; i++)
//                {
//                    IField field = fields.get_Field(i);
//                    if (field.Type == esriFieldType.esriFieldTypeGeometry || field.Type == esriFieldType.esriFieldTypeBlob)
//                        continue;
//                    newFeature.addProperty(field.Name, featureRow.get_Value(i));
//                }
//                geoJSON.addFeature(newFeature);
//            }
//            return geoJSON.ToString();
//        }




//    }
//}
