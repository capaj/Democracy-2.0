using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Threading.Tasks;

namespace pspScraper
{
    class pspTerm
    {
        public Uri URL { get; set; }
        public Dictionary<string,Uri> links { get; set; }
        public uint meetingCount { get; set; }
        public uint lastScrapedMeeting { get; set; }
        public uint yearFrom { get; set; }
        public uint yearTo { get; set; }

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
            this.URL = new Uri("http://" + webGet.ResponseUri.Host + termLink);
        }
    }
}
