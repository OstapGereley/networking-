using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Networking.Functionality
{
    class ActiveConnections
    {
        public  List<TcpConnectionInformation> ShowActiveTcpConnections()
        {
            var properties = IPGlobalProperties.GetIPGlobalProperties();
            return properties.GetActiveTcpConnections().ToList();
        }
    }
}
