using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Raven.Client.Document;



namespace pspScraper
{
    public class Scraper
    {
        public static DocumentStore docDB;
        
        public static string pspHostURL = "http://www.psp.cz";
        public static string pspHostAppURL = "http://www.psp.cz/sqw/";
        public static string documentRoot = "http://www.psp.cz/eknih/index.htm";
        public static Encoding encoding = Encoding.Default;   // this makes sense because you mostly will want to scrape pages, which are in your own language

        static Timer timer { get; set; }

        static void Main(string[] args)
        {
            //var aMeetingProtocol = new pspMeetingProtocol("http://www.psp.cz/eknih/2010ps/stenprot/047schuz/index.htm");
            //var aTerm = new pspTerm("http://www.psp.cz/eknih/2010ps/index.htm");
            var aPrint = new pspPrintHistory("http://www.psp.cz/sqw/historie.sqw?t=857");
            
            //GetAllTerms();
            DocumentStore docDB = new DocumentStore { Url = "http://localhost:8080" };        //when on the same machine where Raven runs
            //docDB = new DocumentStore { Url = "http://dem2.cz:8080" };            //when on any other
            var parliamentMembers = new List<parliamentMember>();
            
           
            //1839-56473


            var allPrintHistories = new HashSet<pspPrintHistory>();
            var voting = new pspVoting(@"http://www.psp.cz/sqw/hlasy.sqw?g=55431");
            docDB.Initialize();
            uint lastScrapedHistoryNum = 0;
            using (var session = docDB.OpenSession())
            {


                foreach (var history in session.Query<pspPrintHistory>().ToList())
                {
                    allPrintHistories.Add(history);
                    if (history.number > lastScrapedHistoryNum)
                    {
                        lastScrapedHistoryNum = history.number;
                    }
                }
                //foreach (var member in session.Query<parliamentMember>().ToList())
                //{
                //    parliamentMembers.Add(member);
                //}
                //session.Store(voting);
                // var entity = session.Load<Company>(companyId);
                //session.SaveChanges();
            }
            TimeSpan threeMinutes = new TimeSpan(0, 0, 3, 0, 0);
            timer = new System.Threading.Timer((cs) =>
            {
                var newScrapeTry = new pspPrintHistory(@"http://www.psp.cz/sqw/historie.sqw?t="+(lastScrapedHistoryNum+1));
                Console.WriteLine("Tried scarping print history number {0}.", lastScrapedHistoryNum);
                timer.Dispose();
            }, null, threeMinutes, threeMinutes);
            

            Console.ReadLine();
        }

        public static List<pspTerm> GetAllTerms() { 
            var listOfTerms = new List<pspTerm>();
            var webGet = new HtmlWeb();
            webGet.OverrideEncoding = encoding;

            var html = webGet.Load(documentRoot);
            try
            {
                var mainContent = html.DocumentNode.SelectSingleNode("//div[@id = 'main-content']");        // getting the div with content
                var tableRows = mainContent.SelectNodes(".//tr");
                //var links = mainContent.SelectNodes(".//a[@href]");
                //foreach (var link in links)
                foreach (var row in tableRows)
                {
                    var term = new pspTerm(row, webGet);
                    listOfTerms.Add(term);
                    
                }
                Console.WriteLine("sd");
            }
            catch (Exception)
            {
                Console.WriteLine("Error while loading " + documentRoot);
                throw;
            }

            return listOfTerms;
        }

        public static HtmlWeb WebGetFactory(){
            var webGet = new HtmlWeb();
            webGet.OverrideEncoding = encoding;
            return webGet;
        }

        public static HtmlNode GetMainContentDivOnURL(string URL) {
            var html = WebGetFactory().Load(URL);
            try
            {
                return html.DocumentNode.SelectSingleNode("//div[@id = 'main-content']");
            }
            catch (Exception)
            {
                
                throw;
            }
        }
    }
}
