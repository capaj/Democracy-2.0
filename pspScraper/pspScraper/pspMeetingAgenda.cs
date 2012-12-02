using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pspScraper
{
    public class pspMeetingAgenda   // for example: http://www.psp.cz/sqw/ischuze.sqw?s=47
    {
        public string URL { get; set; }
        public DateTime starts { get; set; }
        public DateTime ends { get; set; }


    }
}
