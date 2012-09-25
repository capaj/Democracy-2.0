using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using Dem2Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Fleck;
using Raven.Client.Document;
using Raven.Client;

namespace Dem2Server
{
    public static class Dem2Hub         //where everything coexists-inmemory copy of the DB
    {
        private static DocumentStore docDB;

        public static ConcurrentBag<User> allUsers { get; set; }
        public static ConcurrentBag<Voting> allVotableItems { get; set; }
        public static ConcurrentBag<Vote> allVotes { get; set; }
        //public static ConcurrentBag<Vote> allVotes { get; set; }

        public static void Initialize(DocumentStore documentDB)     //someone provided us with the DB to load data from
        {
            using (var session = docDB.OpenSession())
            {
                foreach (var user in session.Query<User>().ToList())
                {
                    allUsers.Add(user);
                }
                foreach (var votableItem in session.Query<Voting>().ToList())
                {
                    allVotableItems.Add(votableItem);
                } 
               
                // var entity = session.Load<Company>(companyId);
             
            }
            documentDB = docDB;
        }
        
        public static void ResolveMessage (string message, IWebSocketConnection socket)
        {
            dynamic receivedObj = JObject.Parse(message);
            switch ((string)receivedObj.msgType)
            {
                case "createUser":
                    Console.WriteLine("Create user request");

                    break;
                case "login":
                    Console.WriteLine("Login request");
                    User heWhoWantsToLogin = JsonConvert.DeserializeObject<User>(message);
                    
                    var ourUser = allUsers.First(x => x.nick == heWhoWantsToLogin.nick && x.hashedPwrd == heWhoWantsToLogin.hashedPwrd);     //simple authentication
                    if (ourUser != null)
                    {   //login successful
                       
                        ourUser.connection = socket;
                        Console.WriteLine("Login granted, sending the model");
                        //ClientViewModel ConnectedUserVM = 
                        socket.Send(JsonConvert.SerializeObject(ourUser.VM));
                    }
                    else
                    {    //we don't know that user
                        Console.WriteLine("Login failed");
                    }
                    break;
                case "logout":
                    Console.WriteLine("logout request");
                    User heWhoWantsToLogout = JsonConvert.DeserializeObject<User>(message);
                    break;
                case "loadView": Console.WriteLine("load request");
                    break;
                case "saveView": Console.WriteLine("save request");

                    break;
                default: Console.WriteLine("Unrecognized msgType");
                    break;
            }
        }
    }
}
