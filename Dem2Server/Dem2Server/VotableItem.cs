using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dem2Model;

namespace Dem2Server
{
    abstract public class VotableItem:ServerClientEntity
    {
        protected List<Vote> CastedVotes
        {
            get
            {
                return Dem2Hub.allVotes.ToList().FindAll(x => x.subjectID == this.Id);
            }
        }  // or ConcurrentBag?

        public int PositiveVotesCount { get { return CastedVotes.Where(vote => vote.Agrees == true).Count(); } }
        public int NegativeVotesCount { get { return CastedVotes.Where(vote => vote.Agrees == false).Count(); } }

        public abstract bool RegisterVote(Vote vote);
  

        public bool GetCurrentResolve
        {
            get { return PositiveVotesCount > NegativeVotesCount; }

        }
    }
}
