using System;
using SimpleTcpMessages;

class ExampleServer
{
    private static readonly int port = 13000;

    public static void Main()
    {
        var server = new TcpMessageServer("127.0.0.1", port);

        server.OnStarted += () =>
        {
            Console.WriteLine("Server started listening on port: {0}", port);
        };

        server.OnStopped += () =>
        {
            Console.WriteLine("Server stopped.");
        };

        server.OnClientConnected += (client) =>
        {
            Console.WriteLine("[{0}] Connected! Clients: {1}", client.EndPoint, server.ConnectedClientsCount);

            client.OnPacketReceived += (data) =>
            {
                Console.WriteLine("[{0}] Received packet bytes: {1}", client.EndPoint, data.Length);

                client.SendPacket(data);
            };

            client.OnDataSent += (data) =>
            {
                Console.WriteLine("[{0}] Sent bytes: {1}", client.EndPoint, data.Length);
            };

            client.OnDisconnected += () =>
            {
                Console.WriteLine("[{0}] Disconnected! Clients: {1}", client.EndPoint, server.ConnectedClientsCount);
            };
        };

        server.Start();

        server.WaitForStop();
    }
}