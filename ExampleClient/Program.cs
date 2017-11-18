using SimpleTcpMessages;
using System;
using System.Net.Sockets;
using System.Threading;

namespace ExampleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new TcpMessageClient("127.0.0.1", 13000);

            client.OnConnected += () =>
            {
                Console.WriteLine("Connected.");
            };

            client.OnDataSent += (data) =>
            {
                Console.WriteLine("Sent {0} bytes.", data.Length);
            };

            client.OnDataReceived += (data) =>
            {
                Console.WriteLine("Received {0} bytes.", data.Length);
            };

            client.OnDisconnected += () =>
            {
                Console.WriteLine("Disconnected");
            };

            client.Connect();
            client.StartReceiving();

            client.SendData(new byte[] { 1 });
            Thread.Sleep(2000);

            client.SendData(new byte[] { 1, 2 });
            Thread.Sleep(2000);

            client.SendData(new byte[] { 1, 2, 3 });
            Thread.Sleep(2000);

            client.SendData(new byte[] { 1, 2, 3, 4 });
            Thread.Sleep(2000);

            client.SendData(new byte[] { 1, 2, 3, 4, 5 });
            Thread.Sleep(2000);

            client.Disconnect();
        }
    }
}
