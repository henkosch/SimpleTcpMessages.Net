using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleTcpMessages
{
    public class TcpPacketProtocol
    {
        TcpPacketProtocolReceivingMode receivingMode;
        TcpReceiveBuffer currentBuffer;      
        
        public event Action<byte[]> OnPacketReceived;

        public TcpPacketProtocolReceivingMode CurrentReceivingMode { get { return receivingMode; } }

        static readonly int packetHeaderSize = 4;

        public TcpPacketProtocol()
        {
            receivingMode = TcpPacketProtocolReceivingMode.None;
        }

        public static byte[] GetPacketLengthData(byte[] data)
        {
            return BitConverter.GetBytes(data.Length);
        }

        public static int GetPacketLengthFromData(byte[] length)
        {
            return BitConverter.ToInt32(length, 0);
        }

        public static void SendPacket(byte[] body, Action<byte[]> sendFunction)
        {
            var packetLength = GetPacketLengthData(body);
            sendFunction(packetLength);
            sendFunction(body);
        }

        public static byte[] GetPacket(byte[] body)
        {
            var packetLength = GetPacketLengthData(body);
            var packet = new byte[packetLength.Length + body.Length];
            Array.Copy(packetLength, packet, packetLength.Length);
            Array.Copy(body, 0, packet, packetLength.Length, body.Length);
            return packet;
        }

        public void ReceiveBytes(byte[] data)
        {
            int receivedBytes = 0;

            while (receivedBytes < data.Length) {
                switch (receivingMode)
                {
                    case TcpPacketProtocolReceivingMode.None:
                        currentBuffer = new TcpReceiveBuffer(packetHeaderSize);
                        receivingMode = TcpPacketProtocolReceivingMode.Header;
                        break;

                    case TcpPacketProtocolReceivingMode.Header:
                        receivedBytes += currentBuffer.ReceiveBytes(data, receivedBytes);
                        if (currentBuffer.Completed)
                        {
                            var packetLength = GetPacketLengthFromData(currentBuffer.ReceivedBytesResult);
                            currentBuffer = new TcpReceiveBuffer(packetLength);
                            receivingMode = TcpPacketProtocolReceivingMode.Body;
                        }
                        break;

                    case TcpPacketProtocolReceivingMode.Body:
                        receivedBytes += currentBuffer.ReceiveBytes(data, receivedBytes);
                        if (currentBuffer.Completed)
                        {
                            OnPacketReceived?.Invoke(currentBuffer.ReceivedBytesResult);
                            receivingMode = TcpPacketProtocolReceivingMode.None;
                        }
                        break;
                }
            }
        }
    }
}
