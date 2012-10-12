using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dem2Model
{
    public class facebookPictureDetails
    {
        public string url { get; set; }
        public bool is_silhouette { get; set; }
    }

    public class facebookPicture
    {
        public facebookPictureDetails data { get; set; }
    }

    public class FacebookAccount        //class must reflect the JSON returned from graph.facebook.com, because we want to deserialize straight into this class
    {
        public string id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string gender { get; set; }
        public string link { get; set; }
        public bool installed { get; set; }
        public bool verified { get; set; }
        public facebookPicture picture { get; set; }

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

        public override int GetHashCode()
        {
            if (id != null)
            {
                return id.GetHashCode();
            }
            else
            {
                return 0;
            }
        }
    }
}
