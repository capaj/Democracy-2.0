using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Threading.Tasks;

namespace pspScraper
{
    public class pspMeetingProtocol        // for example: http://www.psp.cz/eknih/2010ps/stenprot/047schuz/index.htm
    {
        public string URL { get; set; }
        public string baseUrl { get; set; }
        public uint meetingNumber { get; set; }
        public List<pspProtocolPage> protocols { get; set; }

        public pspMeetingProtocol(string URLroot)
        {
            this.URL = URLroot;
            protocols = new List<pspProtocolPage>();
            meetingNumber = ScraperStringHelper.GetNumbersFromString(URLroot).ElementAt(1).Value;
            var baseUrlLimiter = URLroot.LastIndexOf("/");
            baseUrl = URLroot.Substring(0,baseUrlLimiter);

            var scrapingCycleRuns = true;
            var lastScrapedPage = 1;
            while (scrapingCycleRuns)
            {
                var URL = baseUrl + "/s" + meetingNumber.ToString("D3") + lastScrapedPage.ToString("D3") + ".htm";
                //we should end up with something like this: http://www.psp.cz/eknih/2010ps/stenprot/047schuz/s047001.htm
                try
                {
                    //System.Threading.Thread.Sleep(500);
                    protocols.Add(new pspProtocolPage(URL));
                    lastScrapedPage += 1;
                }
                catch (HtmlWebException exception)
                {
                    if (exception.Message.Contains("seems to not yieald any response"))
                    {
                        scrapingCycleRuns = false;
                    }
                    else
                    {
                        Console.WriteLine("exception");
                        throw;   
                    }
                    
                }
            }

        }
    }

}
