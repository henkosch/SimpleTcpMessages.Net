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
        public event Action<TcpMessageClient> OnClientConnected;

        private ManualResetEvent stop;

        public int ReceiveBufferSize { get; set; } = 10240;

        private HashSet<TcpMessageClient> clients;

        public int ConnectedClientsCount
        {
            get
            {
                return clients.Count;
            }
        }

        public IPEndPoint EndPoint { get { return server.LocalEndpoint as IPEndPoint; } }
        public int Port { get { return EndPoint.Port; } }
        public IPAddress Address { get { return EndPoint.Address; } }
        public string Host { get { return Address.ToString(); } }

        public TcpMessageServer(string host = "0.0.0.0", int port = 0)
        {
            IPAddress localAddr = IPAddress.Parse(host);
            server = new TcpListener(localAddr, port);
            clients = new HashSet<TcpMessageClient>();
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
                    var tcpClient = server.AcceptTcpClient();

                    var client = new TcpMessageClient(tcpClient);

                    client.OnDisconnected += () =>
                    {
                        clients.Remove(client);
                    };

                    clients.Add(client);

                    OnClientConnected?.Invoke(client);

                    client.StartReceiving();
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
    }
}
