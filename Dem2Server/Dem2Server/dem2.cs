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

            //var voting1 = new pspVoting(@"http://www.psp.cz/sqw/hlasy.sqw?g=56686", "http://www.psp.cz/eknih/2010ps/stenprot/047schuz/s047022.htm");
            //var voting2 = new pspVoting(@"http://www.psp.cz/sqw/hlasy.sqw?g=56687", "http://www.psp.cz/eknih/2010ps/stenprot/047schuz/s047025.htm");

            //Voting sdsd = new Voting(voting2);
            
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
                            socket.ConnectionInfo.Cookies.Remove("authenticated");
                            socket.ConnectionInfo.Cookies.Remove("user");
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
