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

        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            FacebookAccount second = obj as FacebookAccount;
            if ((System.Object)second == null)
            {
                return false;
            }

            // Return true if the fields match:
            return id == second.id;
        }

        public bool Equals(FacebookAccount second)
        {
            // If parameter is null return false:
            if ((object)second == null)
            {
                return false;
            }

            // Return true if the fields match:
            return id == second.id;
        }
    }
}
