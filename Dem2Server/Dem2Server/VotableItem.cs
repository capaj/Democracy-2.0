using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dem2Model;
using Dem2UserCreated;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Dem2Server
{
    public class VotableItem:ServerClientEntity
    {
        [JsonIgnore]
        protected List<Vote> getCastedVotes
        {
            get
            {
                return EntityRepository.allVotes.ToList().FindAll(x => x.subjectId == this.Id);
            }
        }  // or ConcurrentBag?

        public int PositiveVotesCount { get { return getCastedVotes.Where(vote => vote.Agrees == true).Count(); } }
        public int NegativeVotesCount { get { return getCastedVotes.Where(vote => vote.Agrees == false).Count(); } }

        public virtual bool RegisterVote(Vote vote) {
            return EntityRepository.Add(vote);
        }

        [JsonIgnore]    //implemented on clientside in JS
        public bool getResolve
        {
            get { return PositiveVotesCount > NegativeVotesCount; }

        }

        [JsonIgnore]    //implemented on clientside in JS
        public int getResolveCount
        {
            get { return PositiveVotesCount - NegativeVotesCount; }

        }

        internal void IncrementVotableVersion()
        {
            foreach (var subscriber in subscribedUsers)
            {
                var op = new entityOperation() { operation = 'u', entity = this as VotableItem };

                Dem2Hub.sendItTo(op, subscriber.Value.connection);
            }
        }
    }

    /// <summary>
    /// json.net serializes ALL properties of a class by default
    /// this class will tell json.net to only serialize properties if
    /// they MATCH the list of valid columns passed through the querystring to criteria object
    /// </summary>
    //public class CriteriaContractResolver<T> : DefaultContractResolver
    //{
    //    List<string> _properties;

    //    public CriteriaContractResolver(List<string> properties)
    //    {
    //        _properties = properties;
    //    }

    //    protected override IList<JsonProperty> CreateProperties(JsonObjectContract contract)
    //    {
    //        IList<JsonProperty> filtered = new List<JsonProperty>();

    //        foreach (JsonProperty p in base.CreateProperties(T, contract))    //fix this problem
    //        {
    //            if (_properties.Contains(p.PropertyName))
    //                filtered.Add(p);

    //            return filtered;
    //        }
    //    }
    //}
}
