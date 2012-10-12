using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dem2Server
{
    public class serverStatistics
    {
        public UInt32 userCount { get; set; }
        public UInt64 voteCount { get; set; }
        public UInt64 positiveVoteCount { get; set; }
        public UInt64 negativeVoteCount { get; set; }
        public UInt32 votingCount { get; set; }
        public UInt64 commentCount { get; set; }

        public UInt32 onlineUserCount { get; set; }
    }
}
