using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dem2Model;
using Newtonsoft.Json;

namespace Dem2Server
{
    interface IVotingLeader
    {
        public delegate void VoteCastHandler();

        public event VoteCastHandler VoteCast;

        
    }
}
