﻿using System;
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
    public class VotableItem:ServerClientEntity
    {
        
        public VotableItemStates State
        {
            get {
                DateTime now = DateTime.Now;
                if (Ends<now)
                {
                    if (CurrentResolve == true)
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
        
        public DateTime Starts { get; set; }
        public DateTime Ends { get; set; }
        public ConcurrentBag<Vote> CastedVotes { get; set; }  // or ConcurrentBag?

        public int PositiveVotesCount { get { return CastedVotes.Where(vote => vote.Agrees == true).Count(); } }
        public int NegativeVotesCount { get { return CastedVotes.Where(vote => vote.Agrees == false).Count(); } }

        public VotableItem()        //need to set up schedulers here
        {
            
        }

        public bool CurrentResolve
        {
            get { return PositiveVotesCount > NegativeVotesCount ; }

        }
        
    }
}
