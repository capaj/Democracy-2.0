using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pspScraper
{
    public class pspProtocolPage
    {
        public uint pageNumber { get; set; }
        public Uri URL
        {
            get
            {
                return new Uri(Scraper.pspHost + "");
            }
        }
        public Dictionary<uint, Uri> pspPrints { get; set; }        //links like http://www.psp.cz/sqw/historie.sqw?T=742
        public Dictionary<uint, Uri> pspVotings { get; set; }       // links like http://www.psp.cz/sqw/hlasy.sqw?G=56404
        public Dictionary<uint, Uri> pspProfiles { get; set; }      //links like this http://www.psp.cz/sqw/detail.sqw?id=252

        public pspProtocolPage(HtmlDocument html)
        {
            var main = html.DocumentNode.SelectSingleNode("//div[@id = 'main-content']");
        }
    }
}
