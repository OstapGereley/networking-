namespace Networking.Model
{
    public class NetworkConnectionsModel
    {
        public string SourceIp { get; set; }
        public string SourcePort { get; set; }
        public string DestinationIp { get; set; }
        public string DestinationPort { get; set; }
        public string State { get; set; }
        public bool FirewallRule { get; set; }
    }
}
