using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SimpleTcpMessages.Tests
{
    public class TcpPacketProtocolTests
    {
        [Fact]
        public void ShouldReceiveNormalPacketAtOnce()
        {
            var protocol = new TcpPacketProtocol();

            List<byte[]> packetsReceived = new List<byte[]>();

            protocol.OnPacketReceived += (received) =>
            {
                packetsReceived.Add(received);
            };

            protocol.ReceiveBytes(new byte[] {
                3, 0, 0, 0,   1, 2, 3
            });

            Assert.Equal(new List<byte[]>
            {
                new byte[] { 1, 2, 3}
            }, packetsReceived);
        }

        [Fact]
        public void ShouldReceiveNormalPacketInChunks()
        {
            var protocol = new TcpPacketProtocol();

            List<byte[]> packetsReceived = new List<byte[]>();

            protocol.OnPacketReceived += (received) =>
            {
                packetsReceived.Add(received);
            };

            // Header
            protocol.ReceiveBytes(new byte[] { 3, 0 });
            protocol.ReceiveBytes(new byte[] { 0 });
            protocol.ReceiveBytes(new byte[] { 0 });

            // Body
            protocol.ReceiveBytes(new byte[] { 1 });
            protocol.ReceiveBytes(new byte[] { 2, 3, 4 });

            Assert.Equal(new List<byte[]>
            {
                new byte[] { 1, 2, 3}
            }, packetsReceived);
        }

        [Fact]
        public void ShouldReceiveMultiplePacketsAtOnce()
        {
            var protocol = new TcpPacketProtocol();
            
            List<byte[]> packetsReceived = new List<byte[]>();

            protocol.OnPacketReceived += (received) =>
            {
                packetsReceived.Add(received);
            };

            protocol.ReceiveBytes(new byte[] 
            {
                3, 0, 0, 0,   1, 2, 3,
                2, 0, 0, 0,   4, 5,
                1, 0
            });

            Assert.Equal(new List<byte[]>
            {
                new byte[] { 1, 2, 3},
                new byte[] { 4, 5 }
            }, packetsReceived);
        }

        [Fact]
        public void ShouldReceiveMultiplePacketsInChunks()
        {
            var protocol = new TcpPacketProtocol();
            
            List<byte[]> packetsReceived = new List<byte[]>();

            protocol.OnPacketReceived += (received) =>
            {
                packetsReceived.Add(received);
            };

            protocol.ReceiveBytes(new byte[]
            {
                3, 0, 0,
            });

            protocol.ReceiveBytes(new byte[]
            {
                         0,   1, 
            });

            protocol.ReceiveBytes(new byte[]
            {
                                  2, 3,
                2, 0, 
            });

            protocol.ReceiveBytes(new byte[]
            {
                      0, 0,   4, 5,
                1, 0
            });

            Assert.Equal(new List<byte[]>
            {
                new byte[] { 1, 2, 3},
                new byte[] { 4, 5 }
            }, packetsReceived);
        }
    }
}
