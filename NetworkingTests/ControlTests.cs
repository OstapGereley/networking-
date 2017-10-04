using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Networking.Functionality;

namespace NetworkingTests
{
    [TestClass]
    public class ControlTests
    {
        [TestMethod]
        public void GettingConnectionsTest()
        {
            var list = ActiveConnections.ShowActiveTcpConnections();
            Assert.IsNotNull(list);
        }

        [TestMethod]
        public void AddingOutBlockTest()
        {
            try
            {
                FirewallControl.AddInRule("212.42.76.253", "80");
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void AddingInBlockTest()
        {
            try
            {
                FirewallControl.AddOutRule("212.42.76.253", "80");
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void DeletingBlockTest()
        {
            try
            {
                FirewallControl.DeleteRule("212.42.76.253", "80");
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }
    }
}
