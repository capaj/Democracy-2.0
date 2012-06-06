using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fleck;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Raven.Client.Document;
using Raven.Client;

namespace Dem2Server
{
    class Program
    {
        static void Main(string[] args)
        {
            FleckLog.Level = LogLevel.Debug;
            var allSockets = new List<IWebSocketConnection>();
            var Dem2 = new Dem2Hub();
            var WSserver = new WebSocketServer("ws://localhost:8181");
            DocumentStore docDB = new DocumentStore { Url = "http://localhost:8080" };

            //JavaScriptSerializer JSONSerializer = new JavaScriptSerializer();
            WSserver.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    Console.WriteLine("Opened connection from IP: {0}", socket.ConnectionInfo.ClientIpAddress);
                    allSockets.Add(socket);
                    //socket.Send(jsonStr);
                };
                socket.OnClose = () =>
                {
                    Console.WriteLine("Closed connection from IP: {0}", socket.ConnectionInfo.ClientIpAddress);
                    allSockets.Remove(socket);
                };
                socket.OnMessage = message =>
                {
                    //Console.WriteLine("Message from IP: {0}, {1}", socket.ConnectionInfo.ClientIpAddress, message);
                    Dem2.ResolveMessage(message, socket);
                   
                };
            });


            var input = Console.ReadLine();
            while (input != "exit")
            {
                //foreach (var socket in allSockets.ToList())
                //{
                //    socket.Send(input);
                //}
                input = Console.ReadLine();
            }
        }
    }
}
