﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;


namespace pspScraper
{
    class pspVoting
    {
        public string Id { get; set; }
        public string url { get; private set; }
        public UInt32 meetingNumber { get; private set; }
        public UInt32 votingNumber { get; private set; }
        public DateTime when { get; private set; }
        public string subject { get; private set; }
        public List<individualVote> pspVotes { get; private set; }
        
        [JsonIgnore]
        public bool resolution { 
            get {
                var positiveVotes = pspVotes.FindAll(x => x.how == individualVotingTypes.Agrees);
                return (pspVotes.Count/2)<positiveVotes.Count;
            }
        }

        public pspVoting(string URL, HtmlWeb webLoader)
        {
            url = URL;
            var document = webLoader.Load(URL);
            try
            {
                var h1 = document.DocumentNode.SelectNodes("//h1");
                var lis = document.DocumentNode.SelectNodes("//li");
                //var links = document.DocumentNode.SelectNodes("//a[@href]");
                //var tableLines = document.DocumentNode.SelectNodes("//tr");
            
                var headingText = ScraperStringHelper.RemoveHTMLmarkup(h1.First().InnerText);
                var scrapedNumbers = ScraperStringHelper.GetNumbersFromString(headingText);

                Console.WriteLine(headingText);

                if (scrapedNumbers.Count == 6)
                {
                    var numbersAsInts = new List<int>();  //DateTime(int year, int month, int day, int hour, int minute, int second);
                    scrapedNumbers.ToList().ForEach(x => numbersAsInts.Add((int)x.Value));

                    meetingNumber = scrapedNumbers.ElementAt(0).Value;
                    votingNumber = scrapedNumbers.ElementAt(1).Value;
                    subject = headingText.Substring(headingText.LastIndexOf(":")+3).Trim();

                    when = new DateTime(numbersAsInts.ElementAt(3), czechCalendarHelper.getMonthIntFromString(headingText), numbersAsInts.ElementAt(2), numbersAsInts.ElementAt(4), numbersAsInts.ElementAt(5), 0);

                    pspVotes = new List<individualVote>();
                    foreach (var LINode in lis)
                    {
                        if (isLINodeaVote(LINode))
                        {
                        
                            var parliamentMemberLinkNode = LINode.LastChild;
                            var name = ScraperStringHelper.RemoveHTMLmarkup(parliamentMemberLinkNode.InnerText);
                            var link = "http://www.psp.cz/sqw/" + parliamentMemberLinkNode.Attributes["href"].Value;

                            var vote = new individualVote() { parliamentMemberName = name, parliamentMemberURL = link };
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
            Console.WriteLine("Added a vote from {0}, who {1}", aVote.parliamentMemberName, aVote.how.ToString());
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

    class individualVote
    {
        public individualVotingTypes how { get; set; }
        public string parliamentMemberName { get; set; }
        public string parliamentMemberURL { get; set; }
    }

    enum individualVotingTypes
    {
        Agrees, Disagrees, Refrained, NotPresent, NotPresentExcused 
    }
}
