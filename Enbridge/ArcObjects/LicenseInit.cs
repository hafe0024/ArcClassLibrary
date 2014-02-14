using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS;


namespace Enbridge.ArcObjects
{
    /// <summary>
    /// 
    /// </summary>
    public class LicenseInit: IDisposable
    {
        private IAoInitialize I_LicenseInitializer;
        /// <summary>
        /// 
        /// </summary>
        public bool isInitialized = false;

        /// <summary>
        /// 
        /// </summary>
        public LicenseInit()
        {

            esriLicenseProductCode esriProductCode = esriLicenseProductCode.esriLicenseProductCodeArcServer;
            if (RuntimeManager.Bind(ProductCode.Server))
            {
                isInitialized = true;
                esriProductCode = esriLicenseProductCode.esriLicenseProductCodeArcServer;
            }
            else if (RuntimeManager.Bind(ProductCode.Desktop))
            {
                esriProductCode = esriLicenseProductCode.esriLicenseProductCodeStandard;
                isInitialized = true;
            }
            else
            {
                throw new Exception("ESRI license not available");
            }

            I_LicenseInitializer = new AoInitialize();
            I_LicenseInitializer.Initialize(esriProductCode);
        }

        //public LicenseInit()
        //{

            ////RuntimeManager.Bind(ProductCode.Server);
            //RuntimeManager.Bind(ProductCode.Desktop);
            //if (!RuntimeManager.Bind(ProductCode.Server))
            //{
            //    isInitialized = false;
            //    return;
            //}



            //I_LicenseInitializer = new AoInitialize();
            //I_LicenseInitializer.Initialize(esriLicenseProductCode.esriLicenseProductCodeStandard);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string getInitializedProduct()
        {
            return I_LicenseInitializer.InitializedProduct().ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        public void shutdown()
        {
            //I_LicenseInitializer.Shutdown();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            //this.shutdown();
        }
    }
}
