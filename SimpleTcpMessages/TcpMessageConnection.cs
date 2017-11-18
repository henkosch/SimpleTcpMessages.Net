using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SimpleTcpMessages
{
    internal class TcpMessageConnection
    {
        internal TcpClient Client { get; private set; }        
        internal NetworkStream Stream { get; private set; }

        public bool IsConnected { get; private set; }
        public EndPoint EndPoint { get; private set; }
        public DateTime ConnectedAt { get; private set; }
        public DateTime DisconnectedAt { get; private set; }

        public event Action<byte[]> OnDataReceived;
        public event Action<Exception> OnReadError;
        public event Action OnDisconnected;

        internal TcpMessageConnection(TcpClient client)
        {
            Client = client;
            EndPoint = client.Client.RemoteEndPoint;
            Stream = client.GetStream();
            IsConnected = client.Connected;
        }

        public void StartReceiving(int bufferSize = 10240)
        {
            try
            {
                Byte[] buffer = new byte[bufferSize];
                int readBytes;
                while ((readBytes = Stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    var data = new byte[readBytes];
                    Array.Copy(buffer, data, readBytes);
                    OnDataReceived?.Invoke(data);
                }
            }
            catch (Exception e)
            {
                OnReadError?.Invoke(e);
            }
            finally
            {
                Client.Close();
                IsConnected = false;
                OnDisconnected?.Invoke();
            }
        }
    }
}
