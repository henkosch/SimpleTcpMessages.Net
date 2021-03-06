﻿using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SimpleTcpMessages.Tests
{
    public class TcpMessageClientTests
    {
        [Fact]
        public void ShouldFollowTestEventOrderWhenSendingData()
        {
            var events = new List<string>();

            var server = new TcpMessageServer("127.0.0.1");

            server.OnStarted += () =>
            {
                events.Add("server.OnStarted");
                var client = new TcpMessageClient(server.Host, server.Port);                

                client.OnDataReceived += (data) =>
                {
                    events.Add("client.OnDataReceived");
                    server.Stop();
                };
                
                client.Connect();
                client.StartReceiving();
                
                client.SendData(new byte[] { 1, 2, 3 });
            };

            server.OnClientConnected += (client) =>
            {
                events.Add("server.OnClientConnected");
                client.OnDataReceived += (data) =>
                {
                    events.Add("server.OnDataReceived");
                    
                    client.SendData(new byte[] { 4, 5, 6 });
                };
            };

            server.OnStopped += () =>
            {
                events.Add("server.OnStopped");
            };

            server.Start();
            
            server.WaitForStop();
            
            Assert.Equal(new List<string> {
                "server.OnStarted",
                "server.OnClientConnected",
                "server.OnDataReceived",
                "client.OnDataReceived",
                "server.OnStopped"
            }, events);
        }

        [Fact]
        public void ShouldSendAndReceivePackets()
        {
            var events = new List<string>();

            var server = new TcpMessageServer("127.0.0.1");

            var serverPacketsReceived = new List<byte[]>();
            var clientPacketsReceived = new List<byte[]>();

            server.OnStarted += () =>
            {
                var client = new TcpMessageClient(server.Host, server.Port);

                client.OnPacketReceived += (packet) =>
                {
                    clientPacketsReceived.Add(packet);
                    server.Stop();
                };

                client.Connect();
                client.StartReceiving();

                client.SendPacket(new byte[] { 1, 2, 3, 4, 5 });
            };

            server.OnClientConnected += (client) =>
            {
                client.OnPacketReceived += (packet) =>
                {
                    serverPacketsReceived.Add(packet);
                    var response = new byte[packet.Length];
                    packet.CopyTo(response, 0);
                    Array.Reverse(response);
                    client.SendPacket(response);
                };
            };

            server.Start();

            server.WaitForStop();

            Assert.Equal(new List<byte[]> {
                new byte[] { 1, 2, 3, 4, 5 }
            }, serverPacketsReceived);

            Assert.Equal(new List<byte[]> {
                new byte[] { 5, 4, 3, 2, 1 }
            }, clientPacketsReceived);
        }
    }
}
