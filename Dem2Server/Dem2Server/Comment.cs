using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dem2Server
{
    class Comment:ServerClientEntity
    {
        public ServerClientEntity parent { get; set; }     // comment is ALWAYS a response to some entity-whether it is a voting or other comment
        public DateTime publishedDate { get; set; }
        public KeyValuePair<int, string> revision_textPair { get; set; }
        public bool deleted { get; set; } //if the comment is deleted, this is set to false
    }
}
