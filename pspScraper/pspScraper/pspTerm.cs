using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pspScraper
{
    class pspTerm
    {
        public Uri URL { get; set; }
        public Dictionary<string,Uri> links { get; set; }
        public uint meetingCount { get; set; }
        public uint lastScrapedMeeting { get; set; }
        public uint yearFrom { get; set; }
        public uint yearTo { get; set; }

        public pspTerm()
        {

        }
    }
}
