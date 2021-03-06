﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using Raven.Imports.Newtonsoft.Json;


namespace pspScraper
{
    public class pspVoting
    {
        public string Id { get; private set; }
        public string URL { get; private set; }
        public UInt32 meetingNumber { get; private set; }
        public UInt32 votingNumber { get; private set; }
        public DateTime when { get; private set; }
        public string subject { get; private set; }
        [JsonIgnore]
        public List<individualVote> pspVotes { get; private set; }
        public string stenoprotokolURL { get; set; }            //
        
        [JsonIgnore]
        public bool resolution { 
            get {
                var positiveVotes = pspVotes.FindAll(x => x.how == individualVotingTypes.Agrees);
                return (pspVotes.Count/2)<positiveVotes.Count;
            }
        }

        [JsonConstructor]
        public pspVoting() {}

        public pspVoting(string URL, string stenoURL):this(URL)
        {
            stenoprotokolURL = stenoURL;
        }

        public pspVoting(string URL)
        {

            var webLoader = Scraper.WebGetFactory();
            var document = webLoader.Load(URL);
            try
            {
                URL = webLoader.ResponseUri.ToString();
                var mainContent = document.DocumentNode.SelectSingleNode("//div[@id = 'main-content']");
                var h1 = mainContent.SelectNodes(".//h1");
                var lis = mainContent.SelectNodes(".//li");
            
                var headingText = HttpUtility.HtmlDecode(h1.First().InnerText);
                var scrapedNumbers = ScraperStringHelper.GetNumbersFromString(headingText);

                Console.WriteLine(headingText);

                if (scrapedNumbers.Count == 6)
                {
                    var numbersAsInts = new List<int>();  //DateTime(int year, int month, int day, int hour, int minute, int second);
                    scrapedNumbers.ToList().ForEach(x => numbersAsInts.Add((int)x.Value));

                    meetingNumber = scrapedNumbers.ElementAt(0).Value;
                    votingNumber = scrapedNumbers.ElementAt(1).Value;
                    subject = headingText.Substring(headingText.LastIndexOf(":")+3).Trim();

                    when = new DateTime(numbersAsInts.ElementAt(3), czechCalendarHelper.getMonthFromString(headingText), numbersAsInts.ElementAt(2), numbersAsInts.ElementAt(4), numbersAsInts.ElementAt(5), 0);

                    pspVotes = new List<individualVote>();
                    foreach (var LINode in lis)
                    {
                        if (isLINodeaVote(LINode))
                        {
                        
                            var parliamentMemberLinkNode = LINode.LastChild;
                            var name = HttpUtility.HtmlDecode(parliamentMemberLinkNode.InnerText);
                            var link = Scraper.pspHostAppURL + parliamentMemberLinkNode.Attributes["href"].Value;

                            var vote = new individualVote() { member = new parliamentMember { name = name, pspUrl = link } };
                            switch (LINode.FirstChild.Attributes["class"].Value)
                            {
                                case "flag yes": vote.how = individualVotingTypes.Agrees;
                                    break;
                                case "flag no": vote.how = individualVotingTypes.Disagrees;
                                    break;
                                case "flag not-logged-in": vote.how = individualVotingTypes.NotPresent;
                                    break;
                                case "flag refrained": vote.how = individualVotingTypes.Refrained;
                                    break;
                                case "flag excused": vote.how = individualVotingTypes.NotPresentExcused;
                                    break;
                            }
                            AddIndividualVote(vote);
                        }

                    }

                    //foreach (var vote in pspVotes)
                    //{
                    //    using (var session = pspScraper.Scraper.docDB.OpenSession())
                    //    {
                    //        var pspMember = session.Query<parliamentMember>().FirstOrDefault(x => x.pspUrl == vote.member.pspUrl);

                    //        session.Store(voting);
                      
                    //        session.SaveChanges();
                    //    }
                    //}
                
                    Console.WriteLine("Added {0} votes", pspVotes.Count);
                }
                else
                {
                    throw new Exception { };
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void AddIndividualVote(individualVote aVote) {
            pspVotes.Add(aVote);
            Console.WriteLine("Added a vote from {0}, who {1}", aVote.member.name, aVote.how.ToString());
        }

        public bool isLINodeaVote(HtmlAgilityPack.HtmlNode LiNode) {        //helps to determine if the li node on input is a vote or not
            if (LiNode.HasChildNodes)
            {
                if (LiNode.ChildNodes.Count == 3)
                {
                    var firstChildClassAValue = LiNode.ChildNodes.First().GetAttributeValue("class","not found");
                    switch (firstChildClassAValue) //all the possible types of voting
                    {
                        case "flag yes":
                        case "flag no":
                        case "flag not-logged-in":
                        case "flag refrained":
                        case "flag excused":
                            return true;
                        default:
                            return false;
                    }
                }
                else
                {
                    return false;
                }
                

            }
            else
            {
                return false;
            }
            
        }
    }

    public class individualVote
    {
        public individualVotingTypes how { get; set; }
        public parliamentMember member { get; set; }

        public individualVote()
        {

        }
    }

    public enum individualVotingTypes
    {
        Agrees, Disagrees, Refrained, NotPresent, NotPresentExcused 
    }
}
