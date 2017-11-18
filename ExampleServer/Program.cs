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

        server.OnClientConnected += (endpoint) =>
        {
            Console.WriteLine("[{0}] Connected! Clients: {1}", endpoint, server.ConnectedClientsCount);
        };

        server.OnClientReceivedData += (endpoint, data) =>
        {
            Console.WriteLine("[{0}] Received bytes: {1}", endpoint, data.Length);
        };

        server.OnClientDisconnected += (endpoint) =>
        {
            Console.WriteLine("[{0}] Disconnected! Clients: {1}", endpoint, server.ConnectedClientsCount);
        };

        server.Start();

        server.WaitForStop();
    }
}