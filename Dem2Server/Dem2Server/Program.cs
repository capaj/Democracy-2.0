using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fleck;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dem2Server
{
    class Program
    {
        static void Main(string[] args)
        {
            FleckLog.Level = LogLevel.Debug;
            var allSockets = new List<IWebSocketConnection>();
            var server = new WebSocketServer("ws://localhost:8181");


            //JavaScriptSerializer JSONSerializer = new JavaScriptSerializer();
            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    Console.WriteLine("Opened connection from IP: {0}", socket.ConnectionInfo.ClientIpAddress);
                    allSockets.Add(socket);
                };
                socket.OnClose = () =>
                {
                    Console.WriteLine("Closed connection from IP: {0}", socket.ConnectionInfo.ClientIpAddress);
                    allSockets.Remove(socket);
                };
                socket.OnMessage = message =>
                {
                    Console.WriteLine(message);
                    dynamic receivedObj = JObject.Parse(message);

                    string updateToBroadcast = JsonConvert.SerializeObject(receivedObj);
                    List<IWebSocketConnection> socketsToBroadcast = allSockets.Where(x => x.ConnectionInfo.Id != socket.ConnectionInfo.Id).ToList();    //we don't want to broadcast to the originator of the message

                    socketsToBroadcast.ForEach(s => s.Send(updateToBroadcast));
                    
                    //allSockets.ToList().ForEach(s => s.Send("Echo from " + socket.ConnectionInfo.ClientIpAddress + ": " + message));  
                    
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
