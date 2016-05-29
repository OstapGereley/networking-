using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Networking.Model
{
    class NetworkConnectionsModel
    {
        public string SourceIp { get; set; }
        public string SourcePort { get; set; }
        public string DestinationIp { get; set; }
        public string DestinationPort { get; set; }
        public string State { get; set; }
        public bool FirewallRule { get; set; }
    }
}
