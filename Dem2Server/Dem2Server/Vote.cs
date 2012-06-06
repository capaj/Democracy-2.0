using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dem2Server;

namespace Dem2Model
{
    class Vote:ServerClientEntity
    {
        public string CasterUserID 
        { 
            get { return OwnerId; }
            private set { OwnerId = value; } 
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
