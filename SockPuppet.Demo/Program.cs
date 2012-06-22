using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fleck;

namespace SockPuppet.Demo
{
    class Program
    {
        private static List<IWebSocketConnection> allSockets = new List<IWebSocketConnection>();

        private static Dictionary<Guid, IClientWindow> clientWindows = new Dictionary<Guid, IClientWindow>();
        private static Dictionary<Guid, dynamic> clientDynamicWindows = new Dictionary<Guid, dynamic>();

        private static WebSocketServer server;

        static void Main(string[] args)
        {
            string url = args.Length > 0 ? args[0] : "ws://localhost:8181";

            server = new WebSocketServer(url);

            server.Start(socket =>
            {
                socket.OnOpen = () => Open(socket);
                socket.OnClose = () => Close(socket);
                socket.OnMessage = message => Receive(socket, message);
            });

            Console.WriteLine("Type a message to send to the client (type 'exit' to close):");

            var input = Console.ReadLine();
            while (input != "exit")
            {
                foreach (IWebSocketConnection socket in allSockets)
                {
                    //standard socket raw send method
                    socket.Send(input);

                    //find our dynamic sockpuppet for this socket, and call a dynamic method
                    clientDynamicWindows[socket.ConnectionInfo.Id].document.write("called from a dynamic object method: " + input);

                    //find our strongly typed interface sockpuppet for this socket, and call a typed method (that matches a method for our chosen context on the client)\
                    clientWindows[socket.ConnectionInfo.Id].alert("called from an interface method: " + input);
                }

                input = Console.ReadLine();
            }
        }

        private static void Open(IWebSocketConnection socket)
        {
            //add our socket to a persistent collection so we can reference later
            allSockets.Add(socket);

            //create a strongly typed sockpuppet
            IClientWindow puppet = SockPuppet.Puppet.New<IClientWindow>(r => socket.Send(r));
            //add it to a dictionary so we can reference by socket id later
            clientWindows.Add(socket.ConnectionInfo.Id, puppet);

            //create a dynamic sockpuppet
            dynamic dynamicPuppet = SockPuppet.Puppet.New(r => socket.Send(r));
            //add it to a dictionary so we can reference by socket id later
            clientDynamicWindows.Add(socket.ConnectionInfo.Id, dynamicPuppet);

            Console.WriteLine("> Socket opened to " + socket.ConnectionInfo.ClientIpAddress);
        }

        private static void Close(IWebSocketConnection socket)
        {
            allSockets.Remove(socket);
            clientWindows.Remove(socket.ConnectionInfo.Id);
            clientDynamicWindows.Remove(socket.ConnectionInfo.Id);

            Console.WriteLine("> Socket to " + socket.ConnectionInfo.ClientIpAddress + " closed");
        }

        private static void Receive(IWebSocketConnection socket, string message)
        {
            Console.WriteLine("> Message received from " + socket.ConnectionInfo.ClientIpAddress);
            Console.WriteLine(message);
        }
    }
}
