using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pspScraper
{
    class pspMeetingProtocol        // one meeting of Poslanecká sněmovna
    {
        public List<pspProtocolPage> protocols { get; set; }
    }

    class pspProtocolPage
	{
		public Uri URL { get; set; }
        public Dictionary<uint, Uri> pspPrints { get; set; }        //links like http://www.psp.cz/sqw/historie.sqw?T=742
        public Dictionary<uint, Uri> pspVotings { get; set; }       // links like http://www.psp.cz/sqw/hlasy.sqw?G=56404
        public Dictionary<uint, Uri> pspProfiles { get; set; }      //links like this http://www.psp.cz/sqw/detail.sqw?id=252
	}
}
