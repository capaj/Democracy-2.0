using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using pspScraper;


namespace pspScraper
{

    public class pspPrintHistory //sněmovní tisk http://www.psp.cz/sqw/historie.sqw?t=857
    {
        private static Dictionary<string, printType> typesMapper = new Dictionary<string, printType>() { 
            //case does not matter
            {"Návrh zákona", printType.law},
            {"Mezinárodní smlouva", printType.internationalTreaty},
            {"Výroční zpráva", printType.document}
        };

        public uint number { get; set; }
        public string URL { get; set; }
        public string relatedPrintsListURL { get; set; }
        public string title { get; set; }
        public printType type { get; set; }
        public List<pspVoting> relatedpspVotings { get; set; }
        public List<string> relatedPrintsURLs { get; set; }
        public pspMeetingAgenda inAgenda { get; set; }
        public DateTime scrapedDate { get; set; }

        public bool approved { 
            get { 
                if (relatedpspVotings.Count != 0)
	            {
		            return relatedpspVotings.Last().resolution;
	            }
                return false;
            } 
        }
        
        public pspPrintHistory(string url)
        {
            this.URL = url;
            var mainContent = Scraper.GetMainContentDivOnURL(url);
            try
            {
                var h1 = mainContent.SelectNodes(".//h1").First();  // there is just one in main-content up at the top
                var relatedPrintsListLink = h1.SelectSingleNode(".//a[@href]"); //in the title
                var docTypeDiv = mainContent.SelectSingleNode(".//div[@class='section-content simple']");
                var typeText = HttpUtility.HtmlDecode(docTypeDiv.InnerText);
                foreach (var atype in typesMapper)
                {
                    if (typeText.Contains(atype.Key, StringComparison.OrdinalIgnoreCase))   //case insensitive search
                    {
                        type = atype.Value;
                        break;
                    }
                }

                relatedPrintsListURL = Scraper.pspHostAppURL + relatedPrintsListLink.Attributes["href"].Value;
                var printsListHTMLDiv = Scraper.GetMainContentDivOnURL(relatedPrintsListURL);
                relatedPrintsURLs = printsListHTMLDiv.SelectNodes(".//a[@href]").Where(link => link.Attributes["href"].Value.Contains("tiskt.sqw")).Select(link => link.Attributes["href"].Value).ToList();
                var headingText = HttpUtility.HtmlDecode(h1.InnerText);

                var dividedTitle = ScraperStringHelper.SplitByString(headingText, relatedPrintsListLink.InnerText);
                title = dividedTitle.ElementAt(1);
                var scrapedNumbers = ScraperStringHelper.GetNumbersFromString(relatedPrintsListLink.InnerText);
                
                number = scrapedNumbers.First().Value;

                var links = mainContent.SelectNodes(".//a");
                var pspVotingsURLs = links.Where(link => link.Attributes["href"].Value.Contains("hlasy.sqw")).Select(x=>x.Attributes["href"].Value).ToList();
                relatedpspVotings = new List<pspVoting>();
                foreach (var votingLink in pspVotingsURLs)
                {
                    var voting = new pspVoting(Scraper.pspHostAppURL + votingLink);
                    relatedpspVotings.Add(voting);
                }


                var meetingScheduleLinks = links.Where(link => link.Attributes["href"].Value.Contains("ischuze.sqw")).ToList();  //this should always return one element or null
                
                if (meetingScheduleLinks.Count != 0)
                {
                    //implement TryGetDate from meeting schedule
                    var agendaLink = meetingScheduleLinks.First().Attributes["href"].Value;
                    inAgenda = new pspMeetingAgenda(Scraper.pspHostAppURL + agendaLink);
                }
                scrapedDate = DateTime.Now;

                Console.WriteLine("Finished scraping pspPrintHistory");

                // date 
            }
            catch (Exception)
            {

                throw;
            }
        }


    }

    public enum printType // typy sněmovních tisků
    {
        document, law, internationalTreaty
    }
}
