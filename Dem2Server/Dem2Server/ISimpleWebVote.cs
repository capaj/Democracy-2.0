using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dem2Server
{
    interface ISimpleWebVote
    {
        ConcurrentBag<SimpleWebVote> SimpleWebVotes { get; set; }
        ISimpleWebVote UpVote();
        ISimpleWebVote DownVote();
    }

    public class SimpleWebVote
	{
		
	}
}
