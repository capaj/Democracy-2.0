using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Raven.Client.Document;



namespace pspScraper
{
    class Program
    {
        public static DocumentStore docDB;

        static void Main(string[] args)
        {
            //DocumentStore docDB = new DocumentStore { Url = "http://localhost:8080" };        //when on the same machine where Raven runs
            docDB = new DocumentStore { Url = "http://dem2.cz:8080" };            //when on any other
            var parliamentMembers = new List<parliamentMember>();
            
            var webGet = new HtmlWeb();
            Encoding encoding = Encoding.Default;   // this makes sense because you mostly will want to scrape pages, which are in your own language
            webGet.OverrideEncoding = encoding;
            //1839-56473
            var allMembers = new HashSet<parliamentMember>();
            //var voting = new pspVoting(@"http://www.psp.cz/sqw/hlasy.sqw?g=55431", webGet);
            docDB.Initialize();

            using (var session = docDB.OpenSession())
            {
                var pspMember = session.Query<parliamentMember>().FirstOrDefault(x => x.pspUrl == "aaa");
               
                foreach (var user in session.Query<parliamentMember>().ToList())
                {
                    allMembers.Add(user);
                }
                //foreach (var member in session.Query<parliamentMember>().ToList())
                //{
                //    parliamentMembers.Add(member);
                //}
                //session.Store(voting);
                // var entity = session.Load<Company>(companyId);
                session.SaveChanges();
            }
            
            
            
            
            Console.ReadLine();
        }
    }
}
