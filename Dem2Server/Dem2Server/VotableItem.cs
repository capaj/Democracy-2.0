using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Dem2Server;

namespace Dem2Model
{
    public enum VotableItemStates
    {
        NotStarted, Ongoing, EndedDenied, EndedAccepted
    }
    public class VotableItem:ServerClientEntity     //"votable in parliament democracy"
    {
        
        public VotableItemStates State
        {
            get {
                DateTime now = DateTime.Now;
                if (Ends<now)
                {
                    if (GetCurrentResolve == true)
                    {
                        return VotableItemStates.EndedAccepted;
                    }
                    else
                    {
                        return VotableItemStates.EndedDenied;
                    }
                }
                else
                {
                    if (Starts<now)
                    {
                        return VotableItemStates.Ongoing;
                    }
                    else
	                {
                        return VotableItemStates.NotStarted;
	                }
                }
            }
        }
        
        private DateTime Starts { get; set; }
        private DateTime Ends { get; set; }
        private ConcurrentBag<Vote> CastedVotes { get; set; }  // or ConcurrentBag?

        public int PositiveVotesCount { get { return CastedVotes.Where(vote => vote.Agrees == true).Count(); } }
        public int NegativeVotesCount { get { return CastedVotes.Where(vote => vote.Agrees == false).Count(); } }

        public bool RegisterVote(Vote vote) 
        {
            {
                if (this.State == VotableItemStates.Ongoing)
                {
                    this.CastedVotes.Add(vote);
                    return true;

                }
                else
                {
                    return false;
                }
            }
        }

        public VotableItem()        //need to set up schedulers here
        {
            
        }

        public bool GetCurrentResolve
        {
            get { return PositiveVotesCount > NegativeVotesCount ; }

        }
        
    }
}
