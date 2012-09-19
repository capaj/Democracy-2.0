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
            //DocumentStore docDB = new DocumentStore { Url = "http://localhost:8080" };        //when on the same machine where Raven runs
            DocumentStore docDB = new DocumentStore { Url = "http://dem2.cz:8080" };            //when on any other
            Dem2Hub.Initialize(docDB);
            var WSserver = new WebSocketServer("ws://localhost:8181");

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
                    Dem2Hub.ResolveMessage(message, socket);
                   
                };
            });


            var input = Console.ReadLine();
            while (input != "exit")
            {
                input = Console.ReadLine();
            }
        }
    }
}
