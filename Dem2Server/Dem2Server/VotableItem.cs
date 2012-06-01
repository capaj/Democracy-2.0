using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Dem2Model
{
    enum VotableItemStates
    {
        NotStarted, Ongoing, Ended
    }
    class VotableItem:ServerClientEntity
    {
        
        public VotableItemStates State
        {
            get {
                DateTime now = DateTime.Now;
                if (Ends<now)
                {
                    return VotableItemStates.Ended;
                }
                else
                {
                    if (Starts<now)
                    {
                        return VotableItemStates.Ongoing;
                    }else
	                {
                        return VotableItemStates.NotStarted;
	                }
                }
            }
        }
        
        public DateTime Starts { get; set; }
        public DateTime Ends { get; set; }
        public BlockingCollection<Votes> CastedVotes { get; set; }  // or ConcurrentBag?
    }
}
