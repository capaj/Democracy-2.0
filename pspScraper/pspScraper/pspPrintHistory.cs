using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using pspScraper;


namespace pspScraper
{
    public class pspPrintHistory //sněmovní tisk
    {
        public pspPrintHistory(string url)
        {
            this.historyURL = url;
            var mainContent = Scraper.GetMainContentDivOnURL(url);
            try
            {
                var h1 = mainContent.SelectNodes(".//h1").First();  // there is just one in main-content
                var relatedPrintsListLink = h1.SelectSingleNode(".//a[@href]");
                relatedPrintsListURL = Scraper.pspHostAppURL + relatedPrintsListLink.Attributes["href"].Value;
                var printsListHTMLDiv = Scraper.GetMainContentDivOnURL(relatedPrintsListURL);
                relatedPrintsURLs = printsListHTMLDiv.SelectNodes(".//a[@href]").Where(link => link.Attributes["href"].Value.Contains("tiskt.sqw")).Select(link => link.Attributes["href"].Value).ToList();
                var headingText = ScraperStringHelper.RemoveHTMLmarkup(h1.InnerText);
                var scrapedNumbers = ScraperStringHelper.GetNumbersFromString(headingText);

                number = scrapedNumbers.First().Value;

                title = ScraperStringHelper.SplitByString(headingText, relatedPrintsListLink.InnerText).ElementAt(1);

                var links = mainContent.SelectNodes(".//a");
                var meetingScheduleLinks = links.Where(link => link.Attributes["href"].Value.Contains("ischuze.sqw"));
                if (meetingScheduleLinks != null)
                {
                    //implement TryGetDate from meeting schedule
                }

                Console.WriteLine("Finished scraping pspPrintHistory");

                // date 
            }
            catch (Exception)
            {

                throw;
            }
        }

        public uint number { get; set; }
        public string historyURL { get; set; }
        public string relatedPrintsListURL { get; set; }
        public string title { get; set; }
        public printType type { get; set; }
        public List<pspVoting> relatedpspVotings { get; set; }
        public List<string> relatedPrintsURLs { get; set; }


    }

    public enum printType // typy sněmovních tisků
    {
        document, law, internationalTreaty
    }
}
