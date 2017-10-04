using Microsoft.VisualStudio.TestTools.UnitTesting;
using Networking.Functionality;

namespace NetworkingTests
{
    [TestClass]
    public class MoninitoringTests
    {
        [TestMethod]
        public void PingProductivityTest()
        {
            var ping = new PingScan();
            var list = ping.StartPing();
            Assert.IsNotNull(list.Count);
        }

        [TestMethod]
        public void ArpLoadingProductivityTest()
        {
          var res = ArpTable.LoadMacVendors();
            Assert.AreEqual(res,0);
        }

        [TestMethod]
        public void ArpRecordsProductivity()
        {
            var arp = new ArpTable();
            var res = arp.GetRecords();
            Assert.IsNotNull(res);
        }

        
    }
}
