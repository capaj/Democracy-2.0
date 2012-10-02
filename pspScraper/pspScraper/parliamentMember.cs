using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace pspScraper
{
    public class parliamentMember
    {
        public string Id { get; private set; }
        public string pspUrl { get; set; }
        public string name { get; set; }
        public string partyID { get; set; }

        public parliamentMember(string url, HtmlWeb webLoader)
        {

            
        }

        public parliamentMember()
        {
            
        }


        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            parliamentMember second = obj as parliamentMember;
            if ((System.Object)second == null)
            {
                return false;
            }

            // Return true if the fields match:
            return Id == second.Id ;
        }

        public bool Equals(parliamentMember second)
        {
            // If parameter is null return false:
            if ((object)second == null)
            {
                return false;
            }

            // Return true if the fields match:
            return Id == second.Id;
        }
    }
}
