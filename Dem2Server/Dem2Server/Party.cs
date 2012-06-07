using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dem2Server
{
    public class Party : ServerClientEntity, IVotingLeader
    {

        public event EventHandler VoteCast;
    }
}
