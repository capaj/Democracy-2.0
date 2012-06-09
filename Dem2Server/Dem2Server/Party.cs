using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dem2Model;

namespace Dem2Server
{
    public class Party : ServerClientEntity, IVotingLeader
    {
        public string name { get; set; }
        public SortedList<Int16, IVotingLeader> votingLeadersTable { get; set; }    // in
        
        public event VotingLeader.VoteCastHandler VoteCast;
       
        public bool CastVote(Dem2Model.VotableItem onWhat, Dem2Model.Vote vote)
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
