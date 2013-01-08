using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dem2Server;
using Fleck;
using Raven.Imports.Newtonsoft.Json;
using Dem2UserCreated;

namespace Dem2Model
{
    //[JsonConverter(typeof(UserConverter))]// very important here- we don't want anyone to be able to read other user's acces token simply by reading his entity
    public class User : ServerClientEntity, IVotingLeader
    {
        public string nick { get; set; }    // by default Nick will be created out of a user's name, user can change it whenever he likes as much as he likes, but it must be unique

        public string accessToken { get; set; }
        
        public Name civicName { get; set; }
        public DateTime birth { get; private set; }
        public FacebookAccount FBAccount { get; set; }
        [JsonIgnore]
        public IWebSocketConnection connection { get; set; }
        public LinkedList<IVotingLeader> votingLeadersTable { get; set; }
        public DateTime lastDisconnected { get; set; }
        public Uri URL { get; set; }
        [JsonIgnore]
        public IEnumerable<Vote> getHisVotes
        { 
            get {
                return EntityRepository.allVotes.Where(x => x.OwnerId == Id);
            } 
        }
        [JsonIgnore]
        public List<Subscription> subscriptions { get; set; }       //these need to be iterated over when user disconnects and removed

        public void SubscribeToEntity(Subscription subs) {
            ServerClientEntity ent = subs.onEntity;
            subscriptions.Add(subs);
            ent.Subscribe(this);
        }

        public void UnsubscribeFromEntity(Subscription subs) {
            ServerClientEntity ent = subs.onEntity;
            
            ent.Unsubscribe(this);
        }

        public void UnsubscribeAll()
        {  //
            foreach (var subscription in subscriptions)
            {
                UnsubscribeFromEntity(subscription);
            }
            subscriptions.RemoveAll(x => true);
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
           
            return FBAccount.GetHashCode();

        }

        public static User GetUserById(string Id)
        {
            return EntityRepository.allUsers.FirstOrDefault<User>(x => x.Id == Id);
        }

        public static User getUserFromSocket(IWebSocketConnection socket) {
            return GetUserById(socket.ConnectionInfo.Cookies["user"]);
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
        public bool LogInUser(string FBgraphJSON)
        {
            this.FBAccount = JsonConvert.DeserializeObject<FacebookAccount>(FBgraphJSON);

            var isNew = EntityRepository.Add(this);
            if (isNew)
            {
#if IS_RUNNING_ON_SERVER
                if (FBAccount.verified)     //for production deployment we do care if user is verified FB user
#else
                if (true)    //for testing we don't care if user is verified FB user
#endif
                {
                    Console.WriteLine("Stored new user {0} with FB id: {1}", this.Id, this.FBAccount.id);
                    //this is a new user, create a new model and send it to him
                    FinishLogIn();
                }
                else
                {
                    //send error message to user informing about nonverified account
                }

            }
            else
            {
                var returningUser = EntityRepository.allUsers.First<User>(x => x.Equals(this));
                returningUser.connection = this.connection;
                returningUser.FinishLogIn();
                this.connection = null;
                //this is returning user, send him his model he had last  time
            }
            return isNew;
        }

        internal void FinishLogIn()
        {
            this.connection.ConnectionInfo.Cookies["authentication"] = "authenticated";
            this.connection.ConnectionInfo.Cookies["user"] = this.Id;
            subscriptions = new List<Subscription>();
            this.Send(this);
            Console.WriteLine("Login granted to user id {0}, sending the model", this.Id);
            
        }

        //AUTHENTICATION ENDS
    }

    public class UserConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var user = value as User;
            user.accessToken = null;
            serializer.Serialize(writer, user);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize<User>(reader);
        }
        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}
