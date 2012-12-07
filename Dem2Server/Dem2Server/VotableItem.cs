using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dem2Model;
using Dem2UserCreated;
using Newtonsoft.Json;

namespace Dem2Server
{
    public class VotableItem:ServerClientEntity
    {
        protected List<Vote> CastedVotes
        {
            get
            {
                return EntityRepository.allVotes.ToList().FindAll(x => x.subjectId == this.Id);
            }
        }  // or ConcurrentBag?

        public int PositiveVotesCount { get { return CastedVotes.Where(vote => vote.Agrees == true).Count(); } }
        public int NegativeVotesCount { get { return CastedVotes.Where(vote => vote.Agrees == false).Count(); } }

        public virtual bool RegisterVote(Vote vote) {
            return EntityRepository.Add(vote);
        }

        [JsonIgnore]
        public bool GetCurrentResolve
        {
            get { return PositiveVotesCount > NegativeVotesCount; }

        }
    }
}
