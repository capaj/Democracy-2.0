using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dem2Model;
using Newtonsoft.Json;

namespace Dem2Server
{

    public interface IVotingLeader
    {
        event VotingLeader.VoteCastHandler VoteCast;

        bool CastVote(Voting onWhat, Vote vote);
        
    }

    public abstract class VotingLeader
    {
        public delegate bool VoteCastHandler(IVotingLeader instigator, Voting onWhat, Vote vote);
    }
}
