using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Dem2Server
{
    public class Comment:VotableItem
    {
        [JsonIgnore]
        private ServerClientEntity _parent;

        [JsonIgnore]
        public ServerClientEntity parent 
        { 
            get { 
                using(var session = Dem2Hub.docDB.OpenSession())
	            {
		            return session.Load<ServerClientEntity>(parentId);
	            } 
            }
            set { _parent = value; } 
        }     // comment is ALWAYS a response to some entity-whether it is a voting, other comment, anything else
        
        public string parentId { get; private set; }
        
        public DateTime publishedDate { get; set; }
        public KeyValuePair<int, string> revision_textPair { get; set; }
        public bool deleted { get; set; } //if the comment is deleted, this is set to false
    }
}
