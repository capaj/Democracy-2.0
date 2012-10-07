using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dem2Server;
using Fleck;
using Newtonsoft.Json;

namespace Dem2Model
{
    public class User : ServerClientEntity, IVotingLeader
    {
        public string nick { get; set; }    // by default Nick will be created out of a user's name, user can change it whenever he likes as much as he likes, but it must be unique
        public string accessToken { get; set; }  
        public Name civicName { get; set; }
        public DateTime birth { get; private set; }
        public FacebookAccount FBAccount { get; set; }
        public IWebSocketConnection connection { get; set; }
        public LinkedList<IVotingLeader> votingLeadersTable { get; set; }
        public DateTime lastOnline { get; set; }

        

        public bool CastVoteFromLeader(IVotingLeader leader, Voting onWhat, Vote vote)
        {
            bool voteRegistered = onWhat.RegisterVote(vote);
            if (voteRegistered)
            {
                VoteCast(this, onWhat, vote);
            }
            return voteRegistered;
        }



        public void SubscribeToVotingLeadersVoteCasts()
        {
            foreach (var votingLeader in votingLeadersTable)
            {
                //votingLeader.VoteCast += new VotingLeader.VoteCastHandler(CastVoteFromLeader(votingLeader, ));
            }
        }

        [JsonIgnore]
        public ClientViewModel VM { get; set; }

        public event VotingLeader.VoteCastHandler VoteCast;

        public bool CastVote(Voting onWhat, Vote vote)
        {
            bool voteRegistered = onWhat.RegisterVote(vote);
            if (voteRegistered)
	        {
                VoteCast(this, onWhat, vote);
	        }
            return voteRegistered;
        }

        internal void ProcessAccesTokenCheckResponse(object sender, System.Net.DownloadDataCompletedEventArgs e) //called when facebook responds to checking the users acces token
        {
            if (e.Error != null)
            {
                Console.WriteLine(e.Error.Message);
            }
            else
            {
                if (e.Result != null && e.Result.Length > 0)
                {
                    string downloadedData = Encoding.UTF8.GetString(e.Result);

                    this.connection.ConnectionInfo.Cookies["authentication"] = "authenticated";
                    Console.WriteLine("Login granted, sending the model");
                    this.connection.Send(JsonConvert.SerializeObject(this.VM));
                    if (Dem2Hub.allUsers.Add(this))
	                {
                        //this is a new user, create a new model and send it to him
                    }
                    else
                    {
                        //this is returning user, send him his model he had last  time
                    }
                    
                    Console.WriteLine(downloadedData);
                }
                else
                {
                    Console.WriteLine("No data was downloaded.");
                }
            }


            Console.ReadLine();
        }

        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            User second = obj as User;
            if ((System.Object)second == null)
            {
                return false;
            }

            // Return true if the fields match:
            return Id == second.Id || FBAccount.Equals(second.FBAccount);
        }

        public bool Equals(User second)
        {
            // If parameter is null return false:
            if ((object)second == null)
            {
                return false;
            }

            // Return true if the fields match:
            return Id == second.Id || FBAccount.Equals(second.FBAccount);
        }
    }
}
