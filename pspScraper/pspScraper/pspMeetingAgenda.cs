using Raven.Imports.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pspScraper
{
    public class pspMeetingAgenda   // for example: http://www.psp.cz/sqw/ischuze.sqw?pozvanka=1&s=47
    {
        public string URL { get; set; }
        public DateTime starts { get; set; }
        public DateTime ends { get; set; }
        public SortedDictionary<int, DateTime> meetingDates { get; set; }
        [JsonConstructor]
        public pspMeetingAgenda(){}

        public pspMeetingAgenda(string url)
        {
            this.URL = url;
            var mainContent = Scraper.GetMainContentDivOnURL(url);
            IEnumerable<HtmlAgilityPack.HtmlNode> meetingDateNodes = null;
            try
            {
                var subtitle = mainContent.SelectSingleNode(".//p[@class='subtitle']");
                var dateNumbers = ScraperStringHelper.GetNumbersFromString(subtitle.InnerText).Select(unsigned => (int)unsigned.Value);

                starts = new DateTime(dateNumbers.ElementAt(2), dateNumbers.ElementAt(1), dateNumbers.ElementAt(0), dateNumbers.ElementAt(3), 0, 0);
               
            }
            catch (Exception)
            {

                throw;
            }
            var strongNodes = mainContent.SelectNodes(".//strong");
            if (strongNodes != null)
            {
                meetingDateNodes = strongNodes.Where(node => czechCalendarHelper.getMonthFromString(node.InnerText) != 0);
                var i = 0;
                meetingDates = new SortedDictionary<int, DateTime>();
                foreach (var node in meetingDateNodes)
                {
                    var day = (int)ScraperStringHelper.GetNumbersFromString(node.InnerText).First().Value;     //there should be just one
                    var month = czechCalendarHelper.getMonthFromString(node.InnerText);
                    var date = new DateTime(starts.Year,month,day);
                    meetingDates.Add(i, date);
                    i++;
                    if (date > ends)
                    {
                        ends = date;
                    }
                }
            }
            else
            {
                ends = starts;
            }

            Console.WriteLine("New pspMeetingAgenda scraped from{0}", URL);
        }
    }
}
