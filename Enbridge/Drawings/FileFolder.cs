using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enbridge.Drawings
{

    /// <summary>
    /// This is some really good code, recursive generation of a JSON string 
    /// representing the file folder structure of the drawings
    /// </summary>
    public class FileFolder
    {
        /// <summary>
        /// 
        /// </summary>
        public List<DwgRecord> files;
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, FileFolder> folders;
        /// <summary>
        /// 
        /// </summary>
        public string id;
        /// <summary>
        /// 
        /// </summary>
        public string name;
        /// <summary>
        /// 
        /// </summary>
        public int version = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public FileFolder(string name)
        {
            files = new List<DwgRecord>();
            folders = new Dictionary<string, FileFolder>();
            this.id = Guid.NewGuid().ToString();
            this.name = name;
        }

        /// <summary>
        /// Add a file to the parent object given the string path, version, geometry representation, and zoom level
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="vers"></param>
        /// <param name="geomString"></param>
        /// <param name="zoom"></param>
        public void addFile(string filePath, int vers, string geomString, string zoom)
        {
            //Reference to keep track of the currently acting FileFolder Object
            FileFolder thisFileFolder = this;

            string directoryName = System.IO.Path.GetDirectoryName(filePath);

            string[] pathPieces = directoryName.Split('\\');

            //Loop through the 'folders' in the path
            for (int i = 2; i < pathPieces.Length; i++)
            {
                //create if a new FileFolder object doesn't exist at this level
                if (!thisFileFolder.folders.ContainsKey(pathPieces[i]))
                {
                    thisFileFolder.folders.Add(pathPieces[i], new FileFolder(pathPieces[i]));
                }
                //set the 'this' reference to the current level
                thisFileFolder = thisFileFolder.folders[pathPieces[i]];
            }
            //Add the file to the file folder object found
            thisFileFolder.files.Add(new DwgRecord(filePath, vers, geomString, zoom));
        }


        /// <summary>
        /// Recursively add all FileFolder and drawing records to the expected output JSON format
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            List<string> childStrings = new List<string>();
            Console.WriteLine(this.folders.Count);

            string outputString = "{";
            outputString += string.Format("\"name\":\"{0}\",", this.name);
            outputString += string.Format("\"id\":\"{0}\",", this.id);
            outputString += string.Format("\"version\":{0},", this.version);
            if (this.folders.Count > 0)
            {
                foreach (string key in this.folders.Keys)
                {
                    childStrings.Add(this.folders[key].ToString());
                }
            }

            for (int i = 0; i < this.files.Count; i++)
            {
                childStrings.Add(this.files[i].ToString());
            }

            outputString += "\"children\":[" + string.Join(",", childStrings) + "]}";

            return outputString;
        }
    }

}
