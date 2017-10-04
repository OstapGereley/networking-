using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Networking.Model;

namespace Networking.Functionality
{
    public class ArpTable : IDisposable
    {
        private const int MaxlenPhysaddr = 8;
        //storing mac
        private struct MibIpnetrow
        {
            public int DwIndex;
            public int DwPhysAddrLen;
            public byte Mac0;
            public byte Mac1;
            public byte Mac2;
            public byte Mac3;
            public byte Mac4;
            public byte Mac5;
            public byte Mac6;
            public byte Mac7;
            public int DwAddr;
            public int DwType;
        }

        private string HostIp { get; }

        public static string Ipstr;
        public static string Macname;

        public static Dictionary<string, string> MacVendorsDictionary; 

        //handling buffer errors 
        private const int ErrorInsufficientBuffer = 122;
        private static IntPtr _buffer;

        //geting IpNetTable function
        [DllImport("IpHlpApi.dll")]
        [return: MarshalAs(UnmanagedType.U4)]
        private static extern int GetIpNetTable(
            IntPtr pIpNetTable,
            [MarshalAs(UnmanagedType.U4)] ref int pdwSize,
            bool bOrder);

        public ArpTable()
        {
            var myIp =
                Dns
                    .GetHostAddresses(Dns.GetHostName())
                    .First(adress => adress.AddressFamily == AddressFamily.InterNetwork);

            var digits = new[] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};
            HostIp = myIp.ToString().TrimEnd(digits);


            var bytesNeeded = 0;
            var result = GetIpNetTable(IntPtr.Zero, ref bytesNeeded, false);
            if (result != ErrorInsufficientBuffer)
            {
                throw new Win32Exception(result);
            }
            _buffer = IntPtr.Zero;
            try
            {
                _buffer = Marshal.AllocCoTaskMem(bytesNeeded);
                result = GetIpNetTable(_buffer, ref bytesNeeded, false);
                if (result != 0)
                {
                    throw new Win32Exception(result);
                }
            }
            finally
            {
            }


            
        }

        public static int LoadMacVendors()
        {
            MacVendorsDictionary = new Dictionary<string, string>();
            if (File.Exists(@"macDB.txt"))
            {
                using (var reader = new StreamReader(@"macDB.txt"))
                {
                    while (!reader.EndOfStream) //21580
                    {
                        var text = reader.ReadLine();
                        var splitStrings = text.Split('\t');
                        if (splitStrings.Length == 2)
                        {
                            MacVendorsDictionary.Add(splitStrings[1].Trim(), splitStrings[0].Trim());
                        }

                    }
                }
                return 0;
            }
            return 1;
            
        }

        public static void FillVendors(ObservableCollection<NetworkDeviceModel> list)
        {
            foreach (var item in list)
            {
                string outvalue;
                if(!String.IsNullOrEmpty(item.Mac))
                item.Vendor = MacVendorsDictionary.TryGetValue(item.Mac.Substring(0, 8), out outvalue) ? outvalue : "N\\D";
            }
            
        }  

        public List<NetworkDeviceModel> GetRecords()
        {
            var arpTable = new List<NetworkDeviceModel>();
            var entries = Marshal.ReadInt32(_buffer);
            var currentBuffer = new IntPtr(_buffer.ToInt64() +
                                           Marshal.SizeOf(typeof (int)));
            var table = new MibIpnetrow[entries];
            for (var index = 0; index < entries; index++)
            {
                table[index] = (MibIpnetrow) Marshal.PtrToStructure(new
                    IntPtr(currentBuffer.ToInt64() + (index*
                                                      Marshal.SizeOf(typeof (MibIpnetrow)))), typeof (MibIpnetrow));
            }
            for (var index = 0; index < entries; index++)
            {
                var ip = new IPAddress((table[index].DwAddr & 0xFFFFFFFF));
                Ipstr = ip.ToString();
                Macname = "";
                byte b;
                b = table[index].Mac0;
                if (b < 0x10)
                {
                    Macname = Macname + "0";
                }
                Macname = Macname + b.ToString("X");
                b = table[index].Mac1;
                if (b < 0x10)
                {
                    Macname = Macname + "-0";
                }
                else
                {
                    Macname = Macname + "-";
                }

                Macname = Macname + b.ToString("X");
                b = table[index].Mac2;
                if (b < 0x10)
                {
                    Macname = Macname + "-0";
                }
                else
                {
                    Macname = Macname + "-";
                }

                Macname = Macname + b.ToString("X");
                b = table[index].Mac3;
                if (b < 0x10)
                {
                    Macname = Macname + "-0";
                }
                else
                {
                    Macname = Macname + "-";
                }

                Macname = Macname + b.ToString("X");
                b = table[index].Mac4;
                if (b < 0x10)
                {
                    Macname = Macname + "-0";
                }
                else
                {
                    Macname = Macname + "-";
                }

                Macname = Macname + b.ToString("X");
                b = table[index].Mac5;
                if (b < 0x10)
                {
                    Macname = Macname + "-0";
                }
                else
                {
                    Macname = Macname + "-";
                }

                Macname = Macname + b.ToString("X");

                var arpResonse = new NetworkDeviceModel { Ip = Ipstr, Mac = Macname};
                arpTable.Add(arpResonse);
            }
            return arpTable.Where(
                    tbl =>
                        tbl.Mac != "00-00-00-00-00-00" && tbl.Mac != "FF-FF-FF-FF-FF-FF" &&
                        tbl.Ip.Contains(HostIp)).ToList();
        }  

        public void Dispose()
        {
            Marshal.FreeCoTaskMem(_buffer);
        }
    }
}
