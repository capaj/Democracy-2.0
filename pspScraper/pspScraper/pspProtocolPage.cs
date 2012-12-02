using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pspScraper
{
    public class pspProtocolPage // for example: http://www.psp.cz/eknih/2010ps/stenprot/047schuz/s047001.htm
    {
        public uint pageNumber { get; set; }
        public string URL { get; set; }
        public Dictionary<int, string> pspPrints { get; set; }        //links like http://www.psp.cz/sqw/historie.sqw?T=742
        public Dictionary<int, string> pspVotings { get; set; }       // links like http://www.psp.cz/sqw/hlasy.sqw?G=56404
        public Dictionary<int, string> pspProfiles { get; set; }      //links like this http://www.psp.cz/sqw/detail.sqw?id=252

        public pspProtocolPage(string URL):this()
        {
            var webGet = Scraper.WebGetFactory();
            
            var html = webGet.Load(URL);
            URL = webGet.ResponseUri.ToString();
            if (html.DocumentNode.InnerText != "")
            {
                var mainContent = html.DocumentNode.SelectSingleNode("//div[@id = 'main-content']");
                var links = mainContent.SelectNodes(".//a[@href]");
                var i = 0;
                foreach (var link in links)
                {
                    var linkHref = link.GetAttributeValue("href", "");  //http://www.psp.cz/sqw/hlasy.sqw?G=56651
                    if (linkHref.Contains("detail.sqw"))            //http://www.psp.cz/sqw/detail.sqw?id=401
                    {
                        pspProfiles.Add(i, linkHref);
                    }
                    else
                    {
                        if (linkHref.Contains("hlasy.sqw"))
                        {
                            pspVotings.Add(i, linkHref);
                        }
                        else if (linkHref.Contains("historie.sqw"))         //http://www.psp.cz/sqw/historie.sqw?T=823&O=6
                        {
                            pspPrints.Add(i, linkHref);
                        }
                    }
                    i++;
                }
                if (pspPrints.Count == 0 && pspProfiles.Count == 0 && pspVotings.Count == 0)
                {
                    Console.WriteLine("This should not happen");
                }
            }
            else
            {
                throw new HtmlWebException("Requested URL "+ URL + " seems to not yieald any response");
            }
        }

        public pspProtocolPage()
        {
            pspPrints = new Dictionary<int, string>();
            pspVotings = new Dictionary<int, string>();
            pspProfiles = new Dictionary<int, string>();

        }
    }
}
