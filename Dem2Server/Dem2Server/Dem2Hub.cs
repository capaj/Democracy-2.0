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
using Newtonsoft.Json.Converters;

namespace Dem2Server
{
    public static class Dem2Hub         //where everything coexists-inmemory copy of the DB
    {
        public static DocumentStore docDB;
        
        //public static IEnumerable<VotableItem> allVotable
        //{
        //    get
        //    {
        //        List<VotableItem> votings = allVotings.Select(x => (VotableItem)x).ToList();
        //        List<VotableItem> comments = allVotable.Select(x => (VotableItem)x).ToList();

        //        return votings.Concat(comments);
        //    }
        //}

        public static void Initialize(DocumentStore documentDB)     //someone provided us with the DB to load data from
        {
            docDB = documentDB;
            docDB.Initialize();
            EntityRepository.Initialize();
            pspScraper.Scraper.docDB = docDB;

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
                case "entity":
                    entityOperation op = JsonConvert.DeserializeObject<entityOperation>(message);
                    op.resolveEntityRequest(socket, receivedObj);
                    break;
                default: Console.WriteLine("Unrecognized msgType");
                    break;
            }
        }

        public static serverStatistics GetStatistics() {
            return new serverStatistics {
                userCount = (UInt32)EntityRepository.allUsers.Count(),
                voteCount = (UInt64)EntityRepository.allVotes.Count(),
                positiveVoteCount = (UInt64)EntityRepository.allVotes.Where(x => x.Agrees == true).Count(),
                votingCount = (UInt32)EntityRepository.allVotings.Count(),
                commentCount = (UInt64)EntityRepository.allComments.Count(),
                onlineUserCount = (UInt32)EntityRepository.allUsers.Where(x => x.connection != null).Count()
            };
        }

        internal static void StoreToDB(ServerClientEntity entity)
        {
            using (var session = docDB.OpenSession())
            {
                session.Store(entity);

                session.SaveChanges();
            }
        }

        public static void sendItTo(object that,IWebSocketConnection socket)
        {
            socket.Send(JsonConvert.SerializeObject(that, new IsoDateTimeConverter()));
        }
    }
}
