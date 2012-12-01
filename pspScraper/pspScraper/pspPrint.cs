using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pspScraper;

namespace pspScraper
{
    public class pspPrint //sněmovní tisk
    {
        public uint number { get; set; }
        public Uri URL { get; set; }
        public string title { get; set; }
        public printType type { get; set; }
        public List<pspVoting> relatedpspVotings { get; set; }

    }

    public enum printType // typy sněmovních tisků
    {
        document, law, internationalTreaty
    }
}
