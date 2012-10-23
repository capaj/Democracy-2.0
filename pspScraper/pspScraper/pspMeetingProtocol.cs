using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Threading.Tasks;

namespace pspScraper
{
    public class pspMeetingProtocol        // one meeting of Poslanecká sněmovna
    {
        public string baseUrl { get; set; }
        public uint meetingNumber { get; set; }
        public List<pspProtocolPage> protocols { get; set; }

        public pspMeetingProtocol(string URLroot)
        {
            protocols = new List<pspProtocolPage>();
            meetingNumber = ScraperStringHelper.GetNumbersFromString(URLroot).ElementAt(1).Value;
            var baseUrlLimiter = URLroot.LastIndexOf("/");
            baseUrl = URLroot.Substring(0,baseUrlLimiter);


            var stopScrapingCycle = false;
            var lastScrapedPage = 1;
            while (stopScrapingCycle)
            {
                try
                {
                    var html = Scraper.WebGetFactory().Load(baseUrl + "/s" + meetingNumber.ToString("D3") + lastScrapedPage.ToString("D3") + ".htm");
                    
                    //protocols.Add();
                    lastScrapedPage += 1;
                }
                catch (System.Net.WebException exception)
                {
                    if (exception.Message == "Unable to connect to the remote server")
                    {
                        stopScrapingCycle = true;
                    }
                    Console.WriteLine("exception");
                    throw;
                }
            }

            
        }
    }

}
