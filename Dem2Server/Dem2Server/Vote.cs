using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dem2Server;
using Dem2Model;

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

        public Vote(string userID, string subjectID, bool stance)
        {
            
            VotableItem subject = Dem2Hub.allVotable.First(x => x.Id == subjectID);
            User user = Dem2Hub.allUsers.First(x => x.Id == userID);
            Agrees = stance;
            castedTime = DateTime.Now;
            InitVote(user, subject);

            Dem2Hub.StoreThis(this);
        }
        
        private void InitVote(User voter, VotableItem subject)
        {
            casterUserID = voter.Id;
            subjectID = subject.Id;
        }


    }
}
