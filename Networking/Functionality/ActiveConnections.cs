using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Networking.Model;

namespace Networking.Functionality
{
    public class ActiveConnections
    {
        public  List<NetworkConnectionsModel> ShowActiveTcpConnections()
        {
            var properties = IPGlobalProperties.GetIPGlobalProperties();
            var connList = properties.GetActiveTcpConnections().ToList();
            var mappedList = connList.Select(el => new NetworkConnectionsModel()
            {
                SourceIp = el.LocalEndPoint.Address.ToString(),
                SourcePort = el.LocalEndPoint.Port.ToString(),
                DestinationIp = el.RemoteEndPoint.Address.ToString(),
                DestinationPort = el.RemoteEndPoint.Port.ToString(),
                State = el.State.ToString()
            }).ToList();

            return mappedList.Where(a => !(a.SourceIp).Contains("::") && !(a.SourceIp).Contains("127.0.0")).ToList();
        }
    }
}
