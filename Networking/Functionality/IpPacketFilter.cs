﻿using System;
using NetFwTypeLib;

namespace Networking.Functionality
{
    public static class FirewallControl
    {
        private const string GuidFwPolicy2 = "{E2B3C97F-6AE1-41AC-817A-F6F92166D7DD}";
        private const string GuidRwRule = "{2C5BC43E-3369-4C33-AB0C-BE9469677AF4}";

        public static void AddOutRule(string destIp, string destPort)
        {
            var typeFWPolicy2 = Type.GetTypeFromCLSID(new Guid(GuidFwPolicy2));
            var typeFWRule = Type.GetTypeFromCLSID(new Guid(GuidRwRule));
            var fwPolicy2 = (INetFwPolicy2) Activator.CreateInstance(typeFWPolicy2);
            var newRule = (INetFwRule) Activator.CreateInstance(typeFWRule);
            newRule.Name = $"Out rule for {destPort} on {destIp}";
            newRule.Description = $"Block tcp protocol on port {destPort} and Ip {destIp}";
            newRule.Protocol = (int) NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
            newRule.RemotePorts = destPort;
            newRule.RemoteAddresses = destIp;
            newRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT;
            newRule.Enabled = true;
            newRule.Grouping = "@firewallapi.dll,-23255";
            newRule.Profiles = fwPolicy2.CurrentProfileTypes;
            newRule.Action = NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
            fwPolicy2.Rules.Add(newRule);
        }

        public static void AddInRule(string destIp, string destPort)
        {
            var typeFWPolicy2 = Type.GetTypeFromCLSID(new Guid(GuidFwPolicy2));
            var typeFWRule = Type.GetTypeFromCLSID(new Guid(GuidRwRule));
            var fwPolicy2 = (INetFwPolicy2) Activator.CreateInstance(typeFWPolicy2);
            var newRule = (INetFwRule) Activator.CreateInstance(typeFWRule);
            newRule.Name = $"In rule for {destPort} on {destIp}";
            newRule.Description = $"Block tcp protocol on port {destPort} and Ip {destIp}";
            newRule.Protocol = (int) NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
            newRule.RemotePorts = destPort;
            newRule.RemoteAddresses = destIp;
            newRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;
            newRule.Enabled = true;
            newRule.Grouping = "@firewallapi.dll,-23255";
            newRule.Profiles = fwPolicy2.CurrentProfileTypes;
            newRule.Action = NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
            fwPolicy2.Rules.Add(newRule);
        }

        public static void DeleteRule(string destIp, string destPort)
        {
            var typeFWPolicy2 = Type.GetTypeFromCLSID(new Guid(GuidFwPolicy2));
            var fwPolicy2 = (INetFwPolicy2) Activator.CreateInstance(typeFWPolicy2);
            fwPolicy2.Rules.Remove($"In rule for {destPort} on {destIp}");
            fwPolicy2.Rules.Remove($"Out rule for {destPort} on {destIp}");
        }
    }
}
