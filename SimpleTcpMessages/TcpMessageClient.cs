using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SimpleTcpMessages
{
    public class TcpMessageClient
    {
        private string host;
        private int port;

        internal TcpClient Client { get; private set; }        
        internal NetworkStream Stream { get; private set; }

        public int ReceiveBufferSize { get; set; } = 10240;

        public bool IsConnected { get { return Client.Connected; } }
        public EndPoint EndPoint { get; private set; }
        public DateTime ConnectedAt { get; private set; }
        public DateTime DisconnectedAt { get; private set; }

        public event Action OnConnected;
        public event Action<byte[]> OnDataReceived;
        public event Action<byte[]> OnDataSent;
        public event Action<byte[]> OnPacketReceived;
        public event Action<Exception> OnReadError;
        public event Action OnDisconnected;
        
        private ManualResetEvent disconnect;

        internal TcpMessageClient(TcpClient client)
        {
            disconnect = new ManualResetEvent(false);
            Client = client;
            EndPoint = client.Client.RemoteEndPoint;
            if (IsConnected)
            {
                Stream = client.GetStream();
                OnConnected?.Invoke();
            }
        }

        public TcpMessageClient(string host, int port): this(new TcpClient())
        {
            this.host = host;
            this.port = port;
        }

        public void Connect()
        {
            disconnect = new ManualResetEvent(false);
            Client.Connect(host, port);
            Stream = Client.GetStream();
            OnConnected?.Invoke();
        }

        public void Disconnect()
        {
            Client.Close();
        }

        public void SendPacketKeepAlive()
        {
            SendData(new byte[] { 0 });
        }

        public void SendPacket(byte[] packet)
        {
            TcpPacketProtocol.SendPacket(packet, SendData);
        }

        public void SendData(byte[] data)
        {
            Stream.Write(data, 0, data.Length);
            OnDataSent?.Invoke(data);
        }

        public void StartReceiving()
        {
            ThreadPool.QueueUserWorkItem(ReceiveThread);
        }

        public void WaitForDisconnect()
        {
            disconnect.WaitOne();
        }

        private void ReceiveThread(object obj)
        {
            try
            {
                var packetProtocol = new TcpPacketProtocol();
                packetProtocol.OnPacketReceived += (data) =>
                {
                    OnPacketReceived?.Invoke(data);
                };

                Byte[] buffer = new byte[ReceiveBufferSize];
                int readBytes;
                while ((readBytes = Stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    var data = new byte[readBytes];
                    Array.Copy(buffer, data, readBytes);
                    OnDataReceived?.Invoke(data);
                    packetProtocol.ReceiveBytes(data);
                }
            }
            catch (Exception e)
            {
                OnReadError?.Invoke(e);
            }
            finally
            {
                Client.Close();
                OnDisconnected?.Invoke();
                disconnect.Set();
            }
        }        
    }
}
