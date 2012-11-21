using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dem2Server;
using Dem2Model;
using Fleck;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Dem2UserCreated
{
    public class Vote:ServerClientEntity
    {
        public string subjectID { get; private set; }   //can be anything which implements IVotable -a voting, a comment or another user
        
        private string _casterUserID; //Vote caster is in special private variable(it may be someone else than Owner)
        public string casterUserID  //OwnerID from parent ServerClientEntity contains owner of the vote, caster is the one who initiated the creation of the vote
        {
            get { return _casterUserID; }
            private set { _casterUserID = value; } 
        }
        
       
        public bool Agrees { get; private set; }
        public DateTime castedTime { get; private set; }

        //public Vote(string userID, string subjectID, bool stance)
        ////public Vote()
        //{

        //    VotableItem subject = (VotableItem)ServerClientEntity.GetEntityFromSetsByID(subjectID);
        //    User user = Dem2Hub.allUsers.First(x => x.Id == casterUserID);
        //   // Agrees = stance;
        //    castedTime = DateTime.Now;
        //    InitVote(user, subject);

        //    Dem2Hub.StoreThis(this);
        //}

        public static object Initialization(IWebSocketConnection socket, Vote vote)
        {
            var succes = vote.InitVote(socket.ConnectionInfo.Cookies["user"]); // stores the vote in a DB
            if (succes)
            {
                vote.Subscribe(User.getUserFromSocket(socket));
            }
            return vote;
        }

        public bool InitVote(string casterID)
        {
            OwnerId = casterID;
            casterUserID = casterID;
            VotableItem subject = (VotableItem)ServerClientEntity.GetEntityFromSetsByID(subjectID);
            if (subject != null)
            {
                Dem2Hub.allVotes.Add(this);
                Dem2Hub.StoreThis(this);
                subject.IncrementVersion(); // this triggers on change and notifies the subscribers, because on the subject, properties VoteCounts changed 
                return true;
            }
            return false;
        }

        public void sendTo(IWebSocketConnection socket)
        {
            socket.Send(JsonConvert.SerializeObject(this, new IsoDateTimeConverter()));
        }

    }
}
