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

        public pspMeetingAgenda(string url)
        {
            this.URL = url;
            var mainContent = Scraper.GetMainContentDivOnURL(url);
            try
            {
                var subtitle = mainContent.SelectSingleNode(".//p[@class='date']");
                var dateNumbers = ScraperStringHelper.GetNumbersFromString(subtitle.InnerText).Select(unsigned => (int)unsigned.Value);
                
                starts = new DateTime(dateNumbers.ElementAt(2), dateNumbers.ElementAt(1), dateNumbers.ElementAt(0), 0, 0, 0);
                var strongNodes = mainContent.SelectNodes(".//strong");
                var meetingDates = strongNodes.Where(node => czechCalendarHelper.getMonthFromString(node.InnerText) != 0);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
