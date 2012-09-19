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
        public string nick { get; set; }
        public string hashedPwrd { get; set; }
        public Name civicName { get; set; }
        public DateTime birthTime { get; private set; }
        public FacebookAccount FBAccount { get; set; }
        public IWebSocketConnection connection { get; set; }
        public HashSet<IVotingLeader> votingLeadersTable { get; set; }

        

        public bool CastVoteFromLeader(IVotingLeader leader, VotableItem onWhat, Vote vote)
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

        public bool CastVote(VotableItem onWhat, Vote vote)
        {
            bool voteRegistered = onWhat.RegisterVote(vote);
            if (voteRegistered)
	        {
                VoteCast(this, onWhat, vote);
	        }
            return voteRegistered;
        }
    }
}
