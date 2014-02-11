using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enbridge.Drawings
{
    /// <summary>
    /// object rerpesenting properties of a drawing, 
    /// to string method overridden to output properties in JSON format
    /// </summary>
    public class DwgRecord
    {
        public string dwgName;
        public int version;
        public string geom;
        public string id;
        public string zoom;


        public DwgRecord(string filePath, int version, string geomString, string zoom)
        {
            string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);

            this.dwgName = fileName;
            this.version = version;
            this.geom = (string.IsNullOrEmpty(geomString) ? "null" : "\"" + geomString + "\"");
            this.id = Guid.NewGuid().ToString();
            this.zoom = (string.IsNullOrEmpty(zoom) ? "null" : zoom);
        }


        public override string ToString()
        {
            return "{" + string.Format("\"id\":\"{0}\",\"name\":\"{1}\",\"version\":{2},\"geom\":{3},\"zoom\": {4}",
                this.id, this.dwgName, this.version, this.geom, this.zoom) + "}";
        }
    }
}
