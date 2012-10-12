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
using System.Net;

namespace Dem2Server
{
    public static class Dem2Hub         //where everything coexists-inmemory copy of the DB
    {
        public static DocumentStore docDB;

        public static HashSet<User> allUsers { get; set; }
        public static HashSet<Voting> allVotings { get; set; }        // parliamentary votings for now
        public static HashSet<Vote> allVotes { get; set; }
        public static HashSet<Comment> allComments { get; set; }
        //public static ConcurrentBag<Vote> allVotes { get; set; }

        public static void Initialize(DocumentStore documentDB)     //someone provided us with the DB to load data from
        {
            allUsers = new HashSet<User>();
            allVotings = new HashSet<Voting>();
            allVotes = new HashSet<Vote>();
            allComments = new HashSet<Comment>();
            docDB = documentDB;
            docDB.Initialize();
            using (var session = docDB.OpenSession())
            {
                foreach (var user in session.Query<User>().ToList())
                {
                    allUsers.Add(user);
                }
                foreach (var voting in session.Query<Voting>().ToList())
                {
                    allVotings.Add(voting);
                } 
               
                // var entity = session.Load<Company>(companyId);
             
            }
        }
        
        public static void ResolveMessage (string message, IWebSocketConnection socket)
        {
            JObject receivedObj = JObject.Parse(message);
           
            switch ((string)receivedObj["msgType"])
            {
                case "login":
                    Console.WriteLine("Login request");
                    
                    User heWhoWantsToLogin = receivedObj["theUser"].ToObject<User>();
                    
                    if (heWhoWantsToLogin != null)
                    {   //login successful
                        heWhoWantsToLogin.connection = socket;

                        if (heWhoWantsToLogin.accessToken != null)
                        {
                            using (WebClient asyncWebRequest = new WebClient())
                            {

                                asyncWebRequest.DownloadDataCompleted += heWhoWantsToLogin.ProcessAccesTokenCheckResponse;
                                socket.ConnectionInfo.Cookies["authentication"] = "awaitingFBResponse";

                                Uri urlToRequest = new Uri("https://graph.facebook.com/me?access_token=" + heWhoWantsToLogin.accessToken + "&fields=id,first_name,last_name,gender,link,installed,verified,picture");
                                asyncWebRequest.DownloadDataAsync(urlToRequest);

                            }
                        }
                      
                        
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

        public static serverStatistics GetStatistics() {
            return new serverStatistics { 
                userCount = (UInt32)allUsers.Count, 
                voteCount = (UInt64)allVotes.Count,
                positiveVoteCount = (UInt64)allVotes.Where(x => x.Agrees == true).Count(),
                votingCount = (UInt32)allVotings.Count,
                commentCount = (UInt64)allComments.Count,
                onlineUserCount = (UInt32)allUsers.Where(x=> x.connection != null).Count()
            };
        }
    }
}
