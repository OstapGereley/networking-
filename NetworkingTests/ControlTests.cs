using System;
using Networking.Functionality;
using NUnit.Framework;

namespace NetworkingTests
{
    [TestFixture]
    public class ControlTests
    {
        [Test]
        public void GettingConnectionsTest()
        {
            var list = ActiveConnections.ShowActiveTcpConnections();
            Assert.IsNotNull(list);
        }

        [Test]
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

        [Test]
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

        [Test]
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
