using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Chattt_Server
{
    internal class Program
    {
        private static TcpListener _listener;
        private const int PORT = 3000;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Chattt Server Started ");
            var userManager = UserManager.Instance;
            var clientManager = ClientManager.Instance;

            _listener = new TcpListener(IPAddress.Any, PORT);
            _listener.Start();
            Console.WriteLine($"Server Listening on {IPAddress.Any}:{PORT}");
            while(true)
            {
                TcpClient client = await _listener.AcceptTcpClientAsync();

                ClientSession session = new ClientSession(client);
                _ = session.StartHandling();
            }
        }
    }
}
