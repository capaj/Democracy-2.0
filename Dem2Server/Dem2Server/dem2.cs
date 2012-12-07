using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fleck;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Raven.Client.Document;
using Raven.Client;
using pspScraper;
using Dem2Model;


namespace Dem2Server
{
    class dem2
    {
        static int oneIPConnectionCap = 20;
        static void Main(string[] args)
        {
            
            var allSockets = new List<IWebSocketConnection>();
            DocumentStore docDB = new DocumentStore { Url = "http://localhost:8080" };        //when on the same machine where Raven runs     
            //DocumentStore docDB = new DocumentStore { Url = "http://dem2.cz:8080" };            //when on any other

#if IS_RUNNING_ON_SERVER
            FleckLog.Level = LogLevel.Error;
            var WSserver = new WebSocketServer("http://dem2.cz:8181");
#else

            FleckLog.Level = LogLevel.Debug;
            var WSserver = new WebSocketServer("ws://localhost:8181");           
#endif
            Dem2Hub.Initialize(docDB);
            // temporary scraping hack 
            //for (int i = 834; i < 844; i++)
            //{
            //    var voting = new Voting("http://www.psp.cz/sqw/historie.sqw?t="+i);
            //}

            
            WSserver.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    Console.WriteLine("Opened connection from IP: {0}", socket.ConnectionInfo.ClientIpAddress);
                    var socketOpened = true;
                    if (allSockets.Any(x=>x.ConnectionInfo.ClientIpAddress == socket.ConnectionInfo.ClientIpAddress))
                    {
                        if (allSockets.FindAll(x => x.ConnectionInfo.ClientIpAddress == socket.ConnectionInfo.ClientIpAddress).Count >= oneIPConnectionCap)
                        {
                            socket.Send("Sorry, the server has a cap of 20 simultanoues connections per IP, you cannot connect until there will be less connections");
                            socket.Close();
                            socketOpened = false;
                            
                        }   
                    }
                    if (socketOpened)
                    {
                        allSockets.Add(socket);
                        socket.ConnectionInfo.Cookies["authentication"] = "anonymous";            
                    }
                        
                 
                    
                };
                socket.OnClose = () =>
                {
                    switch (socket.ConnectionInfo.Cookies["authentication"])
                    {
                        case "awaitingFBResponse":
                                
                            break;
                        case "authenticated":
                            
                            var user = EntityRepository.allUsers.First(x => x.connection == socket);
                            user.lastDisconnected = DateTime.Now;
                            user.UnsubscribeAll();
                            socket.ConnectionInfo.Cookies.Remove("authenticated");
                            socket.ConnectionInfo.Cookies.Remove("user");
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
