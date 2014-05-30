using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enbridge
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public static class AppConstants
    {
        /// <summary>
        /// 
        /// </summary>
        public const string ALIGNMENT_SHEETS_ROOT_DIRECTORY = @"S:\AlignmentSheets";
        /// <summary>
        /// 
        /// </summary>
        public const string DRAWINGS_ROOT_DIRECTORY = @"V:\Drawings - Field Access";

        //testing features
        /// <summary>
        /// 
        /// </summary>
        public const string TEST_GDB_PATH = @"C:\TEMP\Scratch.gdb";

        //DOC constants
        /// <summary>
        /// 
        /// </summary>
        public const string DOC_WORKING_VERSION = "sde.WORKING";
        /// <summary>
        /// 
        /// </summary>
        public const string DOC_VIEW = "sde.DEPTHOFCOVER_EVW";
        /// <summary>
        /// 
        /// </summary>
        public const string DOC_INCREM_VIEW = "sde.DEPTHOFCOVERINCREMENTAL_EVW";
        /// <summary>
        /// 
        /// </summary>
        public const string DOC_POINT_GROUP_VIEW = "sde.DOC_POINT_GROUP_EVW";

        //Connection strings
        /// <summary>
        /// 
        /// </summary>
        public const string CONN_STRING_AUTH = @"server=SUPGIS01\GISSERVER1;uid=data_write;pwd=D.A.T.A.R.W!@#123;database=webauth";
        /// <summary>
        /// 
        /// </summary>
        public const string CONN_STRING_PODS = @"server=SUPGIS01\GISSERVER1;uid=data_read;pwd=D.A.T.A.R!@#123;database=PODS";
        /// <summary>
        /// 
        /// </summary>
        public const string CONN_STRING_PLM_REPORTS = @"server=SUPGIS01\GISSERVER1;uid=data_write;pwd=D.A.T.A.R.W!@#123;database=PLM_Reports";
        /// <summary>
        /// 
        /// </summary>
        public const string CONN_STRING_CONTROL_POINT = @"server=SUPGIS01\GISSERVER1;uid=data_write;pwd=D.A.T.A.R.W!@#123;database=ControlPoint";
        /// <summary>
        /// 
        /// </summary>
        public const string CONN_STRING_DOC = @"server=SUPGIS01\GISSERVER1;uid=data_write;pwd=D.A.T.A.R.W!@#123;database=DOC";
        /// <summary>
        /// 
        /// </summary>
        public const string CONN_STRING_SUPSQL_ARCGIS = @"server=supsql01\fldsrvcnrth03;uid=supgis_data;pwd=MWRgoan1;database=ARCGIS";

        /// <summary>
        /// 
        /// </summary>
        public const string IMAGE_MAGICK_EXE = @"C:\Program Files (x86)\ImageMagick-6.8.6-Q16\convert.exe";
        /// <summary>
        /// 
        /// </summary>
        public const string GHOST_SCRIPT_EXE = @"C:\Program Files (x86)\gs\gs9.07\bin\gswin32c.exe";

        private static readonly Dictionary<string, string> lineLoopNames
            = new Dictionary<string, string>
            {
                {"{D4D4472B-FB1E-485B-A550-DCE76F63BC08}", "1"},
                {"{DA50C134-7C85-4591-A1EB-6518C6DF3DBC}", "5"},
                {"{8B443DBB-540C-491E-8CF3-86D43244C4E0}", "4"},
                {"{DED69129-4DE5-40BC-93E6-C4559C3C2986}", "67"},
                {"{181AB22C-A09A-4D50-8480-A7DCE5E92879}", "2"},
                {"{5BF4DE0F-4B0A-4156-B6DC-06421EA95263}", "3"},
                {"{E8240B72-5E83-49A7-9953-639D4213EF72}", "13BorderToClearbrook"},
                {"{972E30EB-D661-4809-A5AF-817D76225718}", "65"},
                {"{F30E3CDD-B46C-4B48-9CDB-CAC8C16A68A5}", "6A"},
                {"{C49A1247-493D-491B-A498-79F3C982E709}", "14"},
                {"{885527FA-41BC-432D-8DE2-2ACB9C862019}", "13ClearbrookToSuperior"},
                {"{CDBA67E2-2263-4841-93AF-C88B249F53AF}", "61"},
                {"{B2859FF5-1B0B-4DFA-A7A8-C421CCD40689}", "13SuperiorToManhatten"},
                {"{84716E1E-FC6D-4942-ACE5-7722B83D5261}", "6B"},
                {"{F1444556-06E9-4811-B76C-ECD32F2120CA}", "62"},
                {"{1D42DAE9-847D-467E-9939-BA39BD04DA8E}", "55"},
                {"{15C5C7F7-8E95-4B86-8153-349BD2B1F629}", "Flanagan Piping"},
                {"{B46C72A2-E736-4809-B719-F908FE8ACF8A}", "83"},
                {"{5473F771-E3EA-4917-AC40-DEB526DAE49E}", "84"},
                {"{78430E5D-FB02-4369-A0B4-CC0500E10E06}", "86"},
                {"{72B3DA17-7650-4461-A708-B051B4AF8589}", "26"},
                {"{D77ACF5B-7BC3-46C3-9F5A-E4794F9483A2}", "82_105"},
                {"{913AF7CD-7316-42B1-BF4E-55B6317D74CD}", "82_102"},
                {"{8E7DCA83-F761-4B4B-9597-0FF9DDEE8AA9}", "85A"},
                {"{5B4343D5-56AE-4EAC-87EC-3C76CA4CDCD7}", "85_10"},
                {"{78911EAC-7CF0-4C67-82F9-F2990459F723}", "85_6"},
                {"{CFEED8BC-2C7A-4C61-AD8D-156940C73255}", "51"},
                {"{47DAE505-21A9-4613-A7C5-6DFDC534227E}", "17 Stockbridge to Freedom"},
                {"{827F522A-7342-471A-B2D1-99C5444A7EAB}", "17 Freedom to Toledo"},
                {"{0E4642E5-F251-434B-81B0-1F4A417D44C7}", "10"},
                {"{5A8DBC30-FD44-498D-9A2D-BC43FA4A0A89}", "52"},
                {"{739CBB18-A91E-4243-A4EF-85C2723704A7}", "81"},
                {"{989059D5-12F9-4A5D-A523-F301DE660CC3}", "64"},
                {"{ED5A59A4-F0E0-44CC-BD22-FADAF64AAF9A}", "55_HCAConvention"},
                {"{89C24E7C-C672-4E35-928D-AF8D5188FCFC}", "26I"},
                {"{64F4E76A-46C9-4DF1-9F3D-0A4A7CB3114E}", "55_2009_Convention"},
                {"{980BA23B-CEE6-44D7-85BC-E59633D37C66}", "78"}
            };

        //get line name from event id
        /// <summary>
        /// 
        /// </summary>
        /// <param name="llID"></param>
        /// <returns></returns>
        public static string GetLineName(string llID)
        {
            try
            {
                return lineLoopNames[llID.ToUpper()];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            try
            {
                return lineLoopNames["{" + llID.ToUpper() + "}"];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("Not found");
            return "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileExtension"></param>
        /// <returns></returns>
        public static string ReturnExtensionType(string fileExtension)
        {
            fileExtension = fileExtension.ToLower();

            switch (fileExtension)
            {
                case ".htm":
                case ".html":
                case ".log":
                    return "text/HTML";
                case ".txt":
                    return "text/plain";
                case ".docx":
                case ".doc":
                    return "application/ms-word";
                case ".tiff":
                case ".tif":
                    return "image/tiff";
                case ".asf":
                    return "video/x-ms-asf";
                case ".avi":
                    return "video/avi";
                case ".zip":
                    return "application/zip";
                case ".xls":
                case ".csv":
                case ".xlsx":
                    return "application/vnd.ms-excel";
                case ".gif":
                    return "image/gif";
                case ".jpg":
                case "jpeg":
                    return "image/jpeg";
                case ".bmp":
                    return "image/bmp";
                case ".wav":
                    return "audio/wav";
                case ".mp3":
                    return "audio/mpeg3";
                case ".mpg":
                case "mpeg":
                    return "video/mpeg";
                case ".rtf":
                    return "application/rtf";
                case ".asp":
                    return "text/asp";
                case ".pdf":
                    return "application/pdf";
                case ".fdf":
                    return "application/vnd.fdf";
                case ".ppt":
                    return "application/mspowerpoint";
                case ".dwg":
                    return "image/vnd.dwg";
                case ".msg":
                    return "application/msoutlook";
                case ".xml":
                case ".sdxl":
                    return "application/xml";
                case ".xdp":
                    return "application/vnd.adobe.xdp+xml";
                default:
                    return "application/octet-stream";
            }
        }
    }
}