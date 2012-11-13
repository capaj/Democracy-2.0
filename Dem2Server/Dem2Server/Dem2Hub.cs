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
using Dem2UserCreated;

namespace Dem2Server
{
    public static class Dem2Hub         //where everything coexists-inmemory copy of the DB
    {
        public static DocumentStore docDB;

        public static HashSet<User> allUsers { get; set; }
        public static HashSet<Voting> allVotings { get; set; }        // parliamentary votings for now
        public static HashSet<Vote> allVotes { get; set; }
        public static HashSet<Comment> allComments { get; set; }
        public static HashSet<Listing> allListings { get; set; }
        //public static ConcurrentBag<Vote> allVotes { get; set; }

        public static Dictionary<string, IEnumerable<ServerClientEntity>> entityNamesToSets;
        
        public static IEnumerable<VotableItem> allVotable { 
            get {
                List<VotableItem> votings = allVotings.Select(x => (VotableItem)x).ToList();
                List<VotableItem> comments = allVotable.Select(x => (VotableItem)x).ToList();

                return votings.Concat(comments);
            } 
        }

        public static void Initialize(DocumentStore documentDB)     //someone provided us with the DB to load data from
        {
            allUsers = new HashSet<User>();
            allVotings = new HashSet<Voting>();
            allVotes = new HashSet<Vote>();
            allComments = new HashSet<Comment>();
            allListings = new HashSet<Listing>();
            
            docDB = documentDB;
            docDB.Initialize();
            pspScraper.Scraper.docDB = docDB;
            
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
            entityNamesToSets = new Dictionary<string, IEnumerable<ServerClientEntity>> {
                {"users", allUsers},
                {"votings", allVotings},
                {"votes", allVotes},
                {"comments", allComments},
                {"listings", allListings},
            };
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
#if IS_RUNNING_ON_SERVER
                            using (WebClient asyncWebRequest = new WebClient())
                            {

                                asyncWebRequest.DownloadDataCompleted += heWhoWantsToLogin.ProcessAccesTokenCheckResponse;
                                socket.ConnectionInfo.Cookies["authentication"] = "awaitingFBResponse";

                                Uri urlToRequest = new Uri("https://graph.facebook.com/me?access_token=" + heWhoWantsToLogin.accessToken + "&fields=id,first_name,last_name,gender,link,installed,verified,picture");
                                asyncWebRequest.DownloadDataAsync(urlToRequest);

                            }
#else
                            if (heWhoWantsToLogin.accessToken == "TESTING_TOKEN_1")
                            {
                                heWhoWantsToLogin.LogInUser("{\"id\":\"1533221860\",\"first_name\":\"Ji\\u0159\\u00ed\",\"last_name\":\"\\u0160p\\u00e1c\",\"gender\":\"male\",\"link\":\"http:\\/\\/www.facebook.com\\/Capaj\",\"installed\":true,\"verified\":true,\"picture\":{\"data\":{\"url\":\"https:\\/\\/fbcdn-profile-a.akamaihd.net\\/hprofile-ak-prn1\\/27439_1533221860_985_q.jpg\",\"is_silhouette\":false}}}");
                            }
                        }

#endif
                        
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
                case "entity":
                    entityOperation op = JsonConvert.DeserializeObject<entityOperation>(message);
                    
                    switch (op.operation)
	                {
                        case 'c':   //create new user generated entity
                            /*    
                           //Example shows json which creates new Vote for the present user
                               {
                                 "msgType": "entity",
                                 "operation": "c",
                                 "className": "Vote",
                                 "entity": {"user/125", "subjectID": "voting/215", "Agrees": true}
                               }


                               // TODO implement create spam check here
                            */
                               try
                               {
                                   Type type = Type.GetType("Dem2UserCreated." + (string)receivedObj["className"]);
                                   //object instance = Activator.CreateInstance(type, (Array)receivedObj["ctorArguments"]); old way, TODO test and remove this line
                                   object instance = JsonConvert.DeserializeObject((string)receivedObj["entity"], type);
                                   Console.WriteLine("Object {0} created",instance.ToString());
                               }
                               catch (Exception)
                               {
                                
                                   throw;
                               }
                               break;
                           case 'r':
                               /*
                               Example shows json for this branch 
                               {
                                 "msgType": "entity",
                                 "operation": "r",
                                 "entity":{
                                     "Id": "user/132"
                                 }
                               }*/
                            op.respondToReadRequest(socket);
                            break;
		                default:
                            break;
	                }

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
