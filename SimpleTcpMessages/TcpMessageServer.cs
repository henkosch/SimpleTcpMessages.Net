using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SimpleTcpMessages
{
    public class TcpMessageServer
    {
        private TcpListener server;

        public event Action OnStarted;
        public event Action OnStopped;
        public event Action<EndPoint> OnClientConnected;
        public event Action<EndPoint> OnClientDisconnected;
        public event Action<EndPoint, Exception> OnClientReadError;
        public event Action<EndPoint, byte[]> OnClientReceivedData;
        public event Action<EndPoint, byte[]> OnClientSentData;

        private ManualResetEvent stop;

        public int ReadBufferLength { get; set; } = 10240;

        private Dictionary<EndPoint, TcpClient> clients;

        public int ConnectedClientsCount
        {
            get
            {
                return clients.Count;
            }
        }

        public TcpMessageServer(string host, int port)
        {
            IPAddress localAddr = IPAddress.Parse(host);
            server = new TcpListener(localAddr, port);
            clients = new Dictionary<EndPoint, TcpClient>();
        }

        public void WaitForStop()
        {
            stop.WaitOne();
        }

        public void Start()
        {
            stop = new ManualResetEvent(false);
            server.Start();
            ThreadPool.QueueUserWorkItem(ServerThread);
            OnStarted?.Invoke();
        }

        public void Stop()
        {
            server.Stop();
        }

        private void ServerThread(object obj)
        {
            try
            {
                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(ClientConnected, client);
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                stop.Set();
                OnStopped?.Invoke();
            }
        }

        private void ClientConnected(object obj)
        {
            var connection = new TcpMessageConnection(obj as TcpClient);

            clients.Add(connection.EndPoint, connection.Client);

            OnClientConnected?.Invoke(connection.EndPoint);

            connection.OnDataReceived += (data) =>
            {
                OnClientReceivedData?.Invoke(connection.EndPoint, data);
            };

            connection.OnDisconnected += () =>
            {
                clients.Remove(connection.EndPoint);
                OnClientDisconnected?.Invoke(connection.EndPoint);
            };

            connection.StartReceiving(ReadBufferLength);
        }
    }
}
