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
        /// <summary>
        /// 
        /// </summary>
        public string dwgName;
        /// <summary>
        /// 
        /// </summary>
        public int version;
        /// <summary>
        /// 
        /// </summary>
        public string geom;
        /// <summary>
        /// 
        /// </summary>
        public string id;
        /// <summary>
        /// 
        /// </summary>
        public string zoom;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="version"></param>
        /// <param name="geomString"></param>
        /// <param name="zoom"></param>
        public DwgRecord(string filePath, int version, string geomString, string zoom)
        {
            string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);

            this.dwgName = fileName;
            this.version = version;
            this.geom = (string.IsNullOrEmpty(geomString) ? "null" : "\"" + geomString + "\"");
            this.id = Guid.NewGuid().ToString();
            this.zoom = (string.IsNullOrEmpty(zoom) ? "null" : zoom);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "{" + string.Format("\"id\":\"{0}\",\"name\":\"{1}\",\"version\":{2},\"geom\":{3},\"zoom\": {4}",
                this.id, this.dwgName, this.version, this.geom, this.zoom) + "}";
        }
    }
}
