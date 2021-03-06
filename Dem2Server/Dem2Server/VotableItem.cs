﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dem2Model;
using Dem2UserCreated;
using Raven.Imports.Newtonsoft.Json;
using Raven.Imports.Newtonsoft.Json.Serialization;
using Raven.Imports.Newtonsoft.Json.Converters;
using ServiceStack.Text.Json;
using ServiceStack.Text;

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
                var votablePropsDict = new Dictionary<string, dynamic>() { 
                    { "Id", Id }, 
                    { "PositiveVotesCount", PositiveVotesCount }, 
                    { "NegativeVotesCount", NegativeVotesCount }
                };
                var opDict = new Dictionary<string, object>() { { "operation", 'u' }, { "entity", votablePropsDict } };

                var socket = subscriber.Value.connection;
                var serialized = opDict.ToJson();
                socket.Send(serialized);

            }
        }
    }

}
