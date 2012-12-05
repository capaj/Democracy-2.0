using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Dem2Server;
using Newtonsoft.Json;
using System.Threading.Tasks;
using pspScraper;
using System.Threading;
using Dem2UserCreated;

namespace Dem2Model
{
    public enum VotingStates
    {
        NotStarted, Ongoing, EndedDenied, EndedAccepted
    }
    public class Voting : VotableItem     
    {
        public delegate void OnEndHandler();
        public event OnEndHandler Ends;

        public Timer timer { get; set; }
        //this represents a so called "sněmovní tisk" in czech parliament
        public pspPrintHistory scrapedPrint { get; set; }   // example can be found here: http://www.psp.cz/sqw/historie.sqw?t=857
        [JsonIgnore]
        public string title {
            get {
                return scrapedPrint.title;
            }
        }
        [JsonIgnore]
        public Uri PSPVotingLink { 
            get {
                return new Uri(scrapedPrint.URL);
            }   
        }
        
        public VotingStates State
        {
            get {
                DateTime now = DateTime.Now;
                if (votingEndDate < now)
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
                    if (creationTime < now)
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
        
        private DateTime creationTime { get; set; }
        public DateTime votingEndDate { get; set; }

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

        public Voting(string printHistory):this(new pspPrintHistory(printHistory)){}

        private void StartVoting()        //need to set up schedulers here
        {
           
            TimeSpan disablePeriodic = new TimeSpan(0, 0, 0, 0, -1);
            timer = new System.Threading.Timer((cs) =>
            {
                if (Ends != null)
                {
                    Ends();
                }
                Console.WriteLine("Voting id {0} ended.", Id);
                timer.Dispose();
            }, null, votingInterval, disablePeriodic);

            
        }

        public Voting(pspScraper.pspPrintHistory printHistory)
        {
            creationTime = DateTime.Now;
            scrapedPrint = printHistory;

            Dem2Hub.StoreToDB(this);

        }

        public TimeSpan votingInterval
        {
            get
            {
                return votingEndDate - DateTime.Now;
            } 
        }
    }
}
