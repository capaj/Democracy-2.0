using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Dem2Server;
using Newtonsoft.Json;
using System.Threading.Tasks;

using System.Threading;
using Dem2UserCreated;

namespace Dem2Model
{
    public enum VotingStates
    {
        NotStarted, Ongoing, EndedDenied, EndedAccepted
    }
    public class Voting : VotableItem     //"votable in parliament democracy"
    {
        //public static TimeSpan votingInterval = new TimeSpan(2, 0, 0, 0);     //for final
        public static TimeSpan votingInterval = new TimeSpan(0, 0, 5, 0);     //for testing

        public Timer timer { get; set; }
        [JsonIgnore]
        public pspScraper.pspVoting scrapedVoting { get; set; }

        public string subject {
            get {
                return scrapedVoting.subject;
            }
        }
        public Uri PSPVotingLink { 
            get {
                return scrapedVoting.scrapedURL;
            }   
        }
        public string PSPStenoprotokolLink { 
            get {
            return scrapedVoting.stenoprotokolURL;
            } 
        }
        
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
        private DateTime Ends { 
            get {
                return Starts + votingInterval;
            } 
        }

        public override bool RegisterVote(Vote vote)
        {
            {

                if (this.State == VotingStates.Ongoing)
                {
                    this.CastedVotes.Add(vote);
                    return Dem2Hub.allVotes.Add(vote);
                    

                }
                else
                {
                    return false;
                }
            }
        }
      

        public Voting()        //need to set up schedulers here
        {
            TimeSpan disablePeriodic = new TimeSpan(0, 0, 0, 0, -1);
            timer = new System.Threading.Timer((cs) =>
            {
                
                timer.Dispose();
            }, null, votingInterval, disablePeriodic);
        }


    }
}
