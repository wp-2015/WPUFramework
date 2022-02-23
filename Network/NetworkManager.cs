﻿using System;
using System.Collections.Generic;
using System.Net;

namespace Netwrok
{
    public static class NetworkManager
    {
        private static Dictionary<string, NetworkChannel> dicNetworkChannel = new Dictionary<string, NetworkChannel>();

        public static void Connect(string channelName, string ip, int port, bool isIpHostName = false, Action successCB = null, Action failCB = null, Action closeCB = null)
        {
            if(dicNetworkChannel.TryGetValue(channelName, out NetworkChannel channel))
            {
                return;
            }

            IPAddress ipAddress;
            if (isIpHostName)
            {
                IPHostEntry host = Dns.GetHostByName(ip);
                ipAddress = host.AddressList[0];
            }
            else
            {
                ipAddress = IPAddress.Parse(ip);
            }

            dicNetworkChannel[channelName] = new NetworkChannel(ipAddress, port, successCB, failCB, ()=> 
            {
                dicNetworkChannel.Remove(channelName);
                closeCB();
            });
        }

        public static void SendMessage(string channelName, Packet packet)
        {
            dicNetworkChannel[channelName].Send(packet);
        }

        public static void Update()
        {
            foreach(var channel in dicNetworkChannel)
            {
                channel.Value.Update();
            }
        }
    }
}