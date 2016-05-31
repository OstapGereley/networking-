namespace Networking.Model
{
    public class NetworkDeviceModel
    {
        public string Ip { get; set; }
        public string Mac { get; set; }
        public string HostName { get; set; }
        public string Vendor { get; set; }
        public bool IsActive { get; set; }
    }
}
