using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Dem2Server;

namespace Dem2UserCreated
{
    public class Comment:VotableItem
    {
        [JsonIgnore]
        private ServerClientEntity _parent;

        [JsonIgnore]
        public ServerClientEntity parent 
        { 
            get {
                return EntityRepository.GetEntityFromSetsByID(parentId);
            }
            set { _parent = value; } 
        }     // comment is ALWAYS a response to some entity-whether it is a voting, other comment, anything else
        
        public string parentId { get; set; }

        public List<CommentText> texts { get; set; }
        public bool deleted { get; set; } //if the comment is deleted, this is set to false
    }

    public class CommentText
    {
        public uint revision { get; set; }
        public string text { get; set; }
    }
}
