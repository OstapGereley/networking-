﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using Networking.Model;

namespace Networking.Functionality
{
    internal class PingScan
    {
        private static CountdownEvent _countdown;
        private static int _upCount;
        private static readonly object LockObj = new object();
        public static List<NetworkDeviceModel> ResultList = new List<NetworkDeviceModel>();
        private readonly string _baseIp;

        public PingScan()
        {
            var myIp =
                Dns
                    .GetHostAddresses(Dns.GetHostName())
                    .First(adress => adress.AddressFamily == AddressFamily.InterNetwork);

            var digits = new[] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};
            _baseIp = myIp.ToString().TrimEnd(digits);
        }

        public List<NetworkDeviceModel> StartPing()
        {
            _countdown = new CountdownEvent(1);
            var sw = new Stopwatch();
            sw.Start();
            for (var i = 1; i < 255; i++)
            {
                var ip = _baseIp + i;
                var p = new Ping();
                p.PingCompleted += p_PingCompleted;
                _countdown.AddCount();
                p.SendAsync(ip, 250, ip);
            }
            _countdown.Signal();
            _countdown.Wait();
            sw.Stop();
            var span = new TimeSpan(sw.ElapsedTicks);
            Debug.WriteLine("Took {0} milliseconds. {1} hosts active.", sw.ElapsedMilliseconds, _upCount);
            return ResultList;
        }

        private static void p_PingCompleted(object sender, PingCompletedEventArgs e)
        {
            var ip = (string) e.UserState;
            if (e.Reply != null && e.Reply.Status == IPStatus.Success)
            {
                string name;
                try
                {
                    var hostEntry = Dns.GetHostEntry(ip);
                    name = hostEntry.HostName;
                }
                catch (SocketException ex)
                {
                    name = "?";
                }
                ResultList.Add(new NetworkDeviceModel {Ip = ip, HostName = name});
                lock (LockObj)
                {
                    _upCount++;
                }
            }
            else if (e.Reply == null)
            {
                Debug.WriteLine("Pinging {0} failed. (Null Reply object?)", ip);
            }
            _countdown.Signal();
        }
    }
}