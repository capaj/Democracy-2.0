using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Dem2Server;

namespace Dem2Model
{
    public enum VotingStates
    {
        NotStarted, Ongoing, EndedDenied, EndedAccepted
    }
    public class Voting : VotableItem     //"votable in parliament democracy"
    {
        
        public VotingStates State
        {
            get {
                DateTime now = DateTime.Now;
                if (Ends<now)
                {
                    if (GetCurrentResolve == true)
                    {
                        return VotingStates.EndedAccepted;
                    }
                    else
                    {
                        return VotingStates.EndedDenied;
                    }
                }
                else
                {
                    if (Starts<now)
                    {
                        return VotingStates.Ongoing;
                    }
                    else
	                {
                        return VotingStates.NotStarted;
	                }
                }
            }
        }
        
        private DateTime Starts { get; set; }
        private DateTime Ends { get; set; }

        public override bool RegisterVote(Vote vote)
        {
            {
                if (this.State == VotingStates.Ongoing)
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
      

        public Voting()        //need to set up schedulers here
        {
            
        }


    }
}
