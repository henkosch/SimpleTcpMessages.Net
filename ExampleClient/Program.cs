using System;
using System.Net.Sockets;
using System.Threading;

namespace ExampleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new TcpClient("127.0.0.1", 13000);
            
            Console.WriteLine("Connected.");

            NetworkStream stream = client.GetStream();

            Byte[] buffer = new Byte[1024];

            stream.Write(buffer, 0, 10);
            Console.WriteLine("Sent 10.");
            Thread.Sleep(5000);
            stream.Write(buffer, 0, 20);
            Console.WriteLine("Sent 20.");
            Thread.Sleep(5000);
            stream.Write(buffer, 0, 30);
            Console.WriteLine("Sent 30.");
            Thread.Sleep(5000);

            client.Close();
            Console.WriteLine("Disconnected");
        }
    }
}
