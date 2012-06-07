using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dem2Server;
using Fleck;
using Newtonsoft.Json;

namespace Dem2Model
{
    class User: ServerClientEntity
    {
        public string nick { get; set; }
        public string hashedPwrd { get; set; }
        public Name civicName { get; set; }
        public DateTime birthTime { get; private set; }
        public FacebookAccount FBAccount { get; set; }
        public IWebSocketConnection connection { get; set; }
        public SortedList<Int16,IVotingLeader> votingLeaders { get; set; }

        public bool CastVote(VotableItem onWhat,Vote vote)
        {
            if (onWhat.State == VotableItemStates.Ongoing)
            {
                onWhat.CastedVotes.Add(vote);
                return true;

            }
            else
            {
                return false;
            }
        }

        [JsonIgnore]
        public ClientViewModel VM { get; set; }
    }
}
