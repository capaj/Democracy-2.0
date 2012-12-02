using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Threading.Tasks;

namespace pspScraper
{
    public class pspTerm    // term junction, for example:http://www.psp.cz/eknih/2010ps/index.htm
    {
        public string URL { get; set; }    
        public Dictionary<string,string> mainLinks { get; set; }
        public uint meetingsCount { 
            get {
                return (uint)MeetingProtocols.Count;
            } 
        }
        public uint lastScrapedMeeting { get; set; }
        public uint yearFrom { get; set; }
        public uint yearTo { get; set; }
        public string meetingsListStenoprotocolLinks { get; set; }          // for example: http://www.psp.cz/eknih/2010ps/stenprot/index.htm
        public List<pspMeetingProtocol> MeetingProtocols { get; set; }

        public pspTerm(string url)
        {
            mainLinks = new Dictionary<string, string>();
            MeetingProtocols = new List<pspMeetingProtocol>();
            this.URL = url;
        }
        
        public pspTerm(HtmlNode tableRow, HtmlWeb webGet)
        {
            var years = ScraperStringHelper.GetNumbersFromString(tableRow.FirstChild.InnerText);
            this.yearFrom = years[0];
            var termLink = tableRow.SelectSingleNode(".//a[@href]").GetAttributeValue("href", "");
            this.yearTo = yearFrom + 4;
            if (years.Count == 2)
            {
                yearTo = years[1];
            }
            this.URL = "http://" + webGet.ResponseUri.Host + termLink;

        }

        public void ScrapeTermJunction()
        {
            try
            {
                var mainContent = Scraper.GetMainContentDivOnURL(this.URL.ToString());
                var links = mainContent.SelectNodes(".//a[@href]");
                foreach (var link in links)
                {
                    mainLinks.Add(link.InnerText, link.GetAttributeValue("href", ""));
                }
                if (mainLinks["Stenoprotokoly"] != null)
                {
                    meetingsListStenoprotocolLinks = Scraper.pspHostURL + mainLinks["Stenoprotokoly"];

                    try
                    {
                        var stenoMainContent = Scraper.GetMainContentDivOnURL(meetingsListStenoprotocolLinks); // this should fetch for example http://www.psp.cz/eknih/2010ps/stenprot/index.htm
                        var stenoLinks = stenoMainContent.SelectNodes(".//a[@href]/b");
                        foreach (var boldNode in stenoLinks)
                        {
                            var href = boldNode.ParentNode.GetAttributeValue("href", "");
                            MeetingProtocols.Add(new pspMeetingProtocol(Scraper.pspHostURL + href));
                        }
                    }
                    catch (Exception)
                    {
                        
                        throw;
                    }
                }
            }
            catch (Exception)
            {
                //error while craping the term junction
                throw;
            }
        }
    }
}
