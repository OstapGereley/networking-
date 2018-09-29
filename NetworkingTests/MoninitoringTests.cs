using Networking.Functionality;
using NUnit.Framework;

namespace NetworkingTests
{
    [TestFixture]
    public class MoninitoringTests
    {
        [Test]
        public void PingProductivityTest()
        {
            var ping = new PingScan();
            var list = ping.StartPing();
            Assert.IsNotNull(list.Count);
        }

        [Test]
        public void ArpLoadingProductivityTest()
        {
          var res = ArpTable.LoadMacVendors();
            Assert.AreEqual(res,0);
        }

        [Test]
        public void ArpRecordsProductivity()
        {
            var arp = new ArpTable();
            var res = arp.GetRecords();
            Assert.IsNotNull(res);
        }

        
    }
}
