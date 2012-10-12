#define IS_RUNNING_ON_SERVER
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
            
            var allSockets = new List<IWebSocketConnection>();
#if IS_RUNNING_ON_SERVER
            FleckLog.Level = LogLevel.Error;
            DocumentStore docDB = new DocumentStore { Url = "http://localhost:8080" };        //when on the same machine where Raven runs     
            var WSserver = new WebSocketServer("http://dem2.cz:8181");
#else
            FleckLog.Level = LogLevel.Debug;
            DocumentStore docDB = new DocumentStore { Url = "http://dem2.cz:8080" };            //when on any other
            var WSserver = new WebSocketServer("ws://localhost:8181");           
#endif
            Dem2Hub.Initialize(docDB);



            //JavaScriptSerializer JSONSerializer = new JavaScriptSerializer();
            WSserver.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    Console.WriteLine("Opened connection from IP: {0}", socket.ConnectionInfo.ClientIpAddress);
                    allSockets.Add(socket);
                    socket.ConnectionInfo.Cookies["authentication"] = "anonymous";
                    
                };
                socket.OnClose = () =>
                {
                    switch (socket.ConnectionInfo.Cookies["authentication"])
                    {
                        case "awaitingFBResponse":
                                
                            break;
                        case "authenticated":
                            Dem2Hub.allUsers.First(x => x.connection == socket).lastDisconnected = DateTime.Now;
                            break;
                        default:
                            break;
                    }

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
