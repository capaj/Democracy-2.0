using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dem2Server;
using Dem2Model;
using Fleck;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Dem2UserCreated
{
    public class Vote:ServerClientEntity
    {
        public string subjectId { get; set; }   //can be anything which implements IVotable -a voting, a comment or another user
        
        private string _casterUserId; //Vote caster is in special private variable(it may be someone else than Owner)
        public string casterUserId  //OwnerID from parent ServerClientEntity contains owner of the vote, caster is the one who initiated the creation of the vote
        {
            get { return _casterUserId; }
            private set { _casterUserId = value; } 
        }
        
       
        public bool Agrees { get; set; }
        public DateTime castedTime { get; set; }

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

        public static object Initialization(User user, Vote vote)
        {
            var succes = vote.InitVote(user); // stores the vote in a DB
            if (succes)
            {
                vote.Subscribe(user);
            }
            return vote;
        }

        public bool InitVote(User caster)
        {
            castedTime = DateTime.Now;
            OwnerId = caster.Id;
            casterUserId = OwnerId;
            VotableItem subject = (VotableItem)EntityRepository.GetEntityFromSetsByID(subjectId);
            if (subject != null)
            {
                if (EntityRepository.Add(this))
                {
                    EntityRepository.StoreToDB(this);
                    IncrementVersion();
                    subject.IncrementVersion(); // this triggers on change and notifies the subscribers, because on the subject, properties VoteCounts changed 
                    return true;
                }
                else
                {
                    
                } 
               
            }
            return false;
        }

        public void sendTo(IWebSocketConnection socket)
        {
            socket.Send(JsonConvert.SerializeObject(this, new IsoDateTimeConverter()));
        }

        public bool updateFromJO(JObject updateSource)
        {
            //var existingVote = EntityRepository.allVotes.First<Vote>(x => x.OwnerId == OwnerId && x.subjectId == subjectId && x.Id == Id);
            var newStance = (bool)updateSource["entity"]["Agrees"];
            if (Agrees != newStance)  // when user changed his vote on a subject
            {
                using (var session = Dem2Hub.docDB.OpenSession())
                {
                    Agrees = newStance;
                    castedTime = DateTime.Now;

                    IncrementVersion();
                    session.Store(this);

                    session.SaveChanges();
                }

                return true;
            }
            return false;
        }

        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            Vote second = obj as Vote;
            if ((System.Object)second == null)
            {
                return false;
            }

            // Return true if the fields match:
            return OwnerId.Equals(second.OwnerId) && subjectId == second.subjectId;
        }

        public bool Equals(Vote second)
        {
            // If parameter is null return false:
            if ((object)second == null)
            {
                return false;
            }

            // Return true if the fields match:
            return OwnerId.Equals(second.OwnerId) && subjectId == second.subjectId;
        }

        public override int GetHashCode()
        {

            return subjectId.GetHashCode()+OwnerId.GetHashCode();
        }

    }
}
