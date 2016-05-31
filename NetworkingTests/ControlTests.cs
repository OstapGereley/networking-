using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var connections = new ActiveConnections();
            var list = connections.ShowActiveTcpConnections();
            Assert.IsNotNull(list);
        }

        [TestMethod]
        public void AddingOutBlockTest()
        {
            try
            {
                FirewallControl.AddInRule("212.42.76.253", "80");
            }
            catch (Exception ex)
            {
                Assert.Fail();
                throw;
            }
        }

        [TestMethod]
        public void AddingInBlockTest()
        {
            try
            {
                FirewallControl.AddOutRule("212.42.76.253", "80");
            }
            catch (Exception ex)
            {
                Assert.Fail();
                throw;
            }
        }

        [TestMethod]
        public void DeletingBlockTest()
        {
            try
            {
                FirewallControl.DeleteRule("212.42.76.253", "80");
            }
            catch (Exception ex)
            {
                Assert.Fail();
                throw;
            }
        }
    }
}
