using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dem2Server
{
    class DiscussionThread:ServerClientEntity
    {

        public ConcurrentBag<Comment> Comments { get; set; }
        
    }
}
