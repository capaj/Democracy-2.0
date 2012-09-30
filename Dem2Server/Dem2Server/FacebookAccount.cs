using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dem2Model
{
    public class FacebookAccount        //class must reflect the JSON returned from graph.facebook.com, because we want to deserialize straight into this class
    {
        public string id { get; set; }
        public string name { get; set; }
        public string gender { get; set; }
        public string link { get; set; }
        public bool installed { get; set; }
        public bool verified { get; set; }
        public string picture { get; set; }
        
    }
}
