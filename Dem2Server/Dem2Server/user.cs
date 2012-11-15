using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dem2Server;
using Fleck;
using Newtonsoft.Json;
using Dem2UserCreated;

namespace Dem2Model
{
    public class User : ServerClientEntity, IVotingLeader
    {
        public string nick { get; set; }    // by default Nick will be created out of a user's name, user can change it whenever he likes as much as he likes, but it must be unique
        [NonSerializedAttribute]    // very important here- we don't want anyone to be able to read other user's acces token simply by reading his entity
        public string accessToken { get; set; }  
        public Name civicName { get; set; }
        public DateTime birth { get; private set; }
        public FacebookAccount FBAccount { get; set; }
        [JsonIgnore]
        public IWebSocketConnection connection { get; set; }
        public LinkedList<IVotingLeader> votingLeadersTable { get; set; }
        public DateTime lastDisconnected { get; set; }
        public Uri URL { get; set; }
        public IEnumerable<Vote> getHisVotes
        { 
            get {
                return Dem2Hub.allVotes.Where(x => x.OwnerId == Id);
            } 
        }

        public void Send(dynamic package) { //shorter version than to having to type it every time
            this.connection.Send(JsonConvert.SerializeObject(package));
        }

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
        //AUTHENTICATION
        
        
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
                    LogInUser(downloadedData);                   
                }
                else
                {
                    Console.WriteLine("No data was downloaded.");
                }
            }
            
        }

        //returns false when user is returning, true when he is a new user
        public bool LogInUser(string FBgraphJSON) {
            this.FBAccount = JsonConvert.DeserializeObject<FacebookAccount>(FBgraphJSON);

            this.connection.ConnectionInfo.Cookies["authentication"] = "authenticated";
            this.connection.ConnectionInfo.Cookies["user"] = this.Id;

            Console.WriteLine("Login granted, sending the model");
            var isNew = Dem2Hub.allUsers.Add(this);
            if (isNew)
            {
#if IS_RUNNING_ON_SERVER
                if (FBAccount.verified)     //for production deployment we do care if user is verified FB user
#else
                if (true)    //for testing we don't care if user is verified FB user
#endif
                {
                    Console.WriteLine("Created a new user with FB id: {0}", this.FBAccount.id);
                    //this is a new user, create a new model and send it to him
                    this.Send(this);
                }
                else
                {
                    //send error message to user informing about nonverified account
                }
                
            }
            else
            {
                var returningUser = Dem2Hub.allUsers.First<User>(x => x.Equals(this));
                returningUser.connection = this.connection;
                this.connection = null;
                //this is returning user, send him his model he had last  time
            }

            //Console.WriteLine(downloadedData);
            return isNew;
        }

        //AUTHENTICATION ENDS

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
            return FBAccount.Equals(second.FBAccount) || Id == second.Id;
        }

        public bool Equals(User second)
        {
            // If parameter is null return false:
            if ((object)second == null)
            {
                return false;
            }

            // Return true if the fields match:
            return FBAccount.Equals(second.FBAccount) || Id == second.Id;
        }

        public override int GetHashCode()
        {
            if (Id != null)
            {
                return Id.GetHashCode();
            }
            else
            {
                return FBAccount.GetHashCode();
            }
        }
    }
}
