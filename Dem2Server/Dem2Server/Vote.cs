using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dem2Server;

namespace Dem2Model
{
    public class Vote:ServerClientEntity
    {
        public string subjectID { get; private set; }   //can be anything which implements IVotable -a voting, a comment or another user
        
        private string _casterUserID; //Vote caster is in special private variable(it may be someone else than Owner)
        public string CasterUserID  //OwnerID from parent contains owner of the vote,
        {
            get { return _casterUserID; }
            private set { _casterUserID = value; } 
        }
        
       
        public bool Agrees { get; private set; }
        public DateTime castedTime { get; private set; }

        public Vote(User voter, bool stance)
        {
            CasterUserID = voter.Id;
            Agrees = stance;
            castedTime = DateTime.Now;
        }


    }
}
