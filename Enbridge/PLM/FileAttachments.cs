using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geocortex.Forms.Client;

namespace Enbridge.PLM
{
    [Serializable]
    public class FileAttachments
    {
        public Dictionary<string, FileItem> uploadedFiles;
        public Dictionary<string, FileItem> pendingFiles;

        public FileAttachments()
        {
            this.uploadedFiles = new Dictionary<string, FileItem>();
            this.pendingFiles = new Dictionary<string, FileItem>();
        }


        public void addFiles(IList<FileItem> uploadList)
        {
            Console.WriteLine("adding files");
            foreach (FileItem fileItem in uploadList)
            {
                Console.WriteLine("adding files");
                this.pendingFiles.Add(Guid.NewGuid().ToString(), fileItem);
                Console.WriteLine("done adding files");
            }
        }

        /// <summary>
        /// Return a list of Geocortex DataItems representing the items in the FileItem dictionary
        /// </summary>
        /// <param name="existingOrPending">existing or pending files</param>
        /// <returns></returns>
        public List<DataItem> getFileDataItemList(string existingOrPending)
        {
            List<DataItem> dataItemList = new List<DataItem>();

            Dictionary<string, FileItem> selectedDictionary;
            switch (existingOrPending)
            {
                case "existing":
                    foreach (KeyValuePair<string, FileItem> entry in this.uploadedFiles)
                    {
                        dataItemList.Add(new DataItem(entry.Value.FileName, entry.Key));
                    }
                    break;
                case "pending":
                    foreach (KeyValuePair<string, FileItem> entry in this.pendingFiles)
                    {
                        dataItemList.Add(new DataItem(entry.Value.FileName, entry.Key));
                    }
                    break;
                default:
                    throw new Exception("existing or pending not set");
            }

            Console.WriteLine("Item Count: {0}", dataItemList.Count);

            return dataItemList;
        }

    }


   
}
