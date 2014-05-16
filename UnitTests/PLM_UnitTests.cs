using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class PLM_UnitTests
    {
        [TestMethod]
        public void TestLoadPLMReportRoot()
        {
            Enbridge.PLM.PlmReport report = new Enbridge.PLM.PlmReport();

            report.setReportProperties("glenn", Guid.NewGuid(), "yes", "no", null, null, Guid.NewGuid(), "new report");
            report.permanentRepair = new Enbridge.PLM.PermanentRepair();
            report.permanentRepair.setTab1Values(DateTime.Now, DateTime.Now, "yes", "broke", "20", "4", "sweet", "2", "thick", "a house", "no");
            report.permanentRepair.setTab2Values("yes", "2342", "remark", "no", DateTime.Now, "45", "-92", "no", "here", "fittings");
            report.corrosionInspection = new Enbridge.PLM.CorrosionInpsection();
            report.corrosionInspection.setTab1Values(Guid.NewGuid(), Guid.NewGuid(), "good", "joe", DateTime.Now, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "some");
            report.corrosionInspection.setTab2Values(Guid.NewGuid(), Guid.NewGuid(), "20");
            report.corrosionInspection.setTab3Values(Guid.NewGuid(), "some", "adsf", "adsfa", Guid.NewGuid(), "10", "small");
            
            bool success = report.saveReport();
            

            Assert.IsTrue(success, "something went wrong");
        }
    }
}
