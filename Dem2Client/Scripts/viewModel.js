define(function () {

    function ViewModel() {
        this.connected = ko.observable(false);      //websocket connection flag
        this.comments = ko.observableArray([]);
        this.votes = ko.observableArray([]);
        this.votings = ko.observableArray([]);
        this.currentSection = ko.observable(window.location.pathname);
    }
    return new ViewModel;; 
});





/* voting example:
{
   "operation":"u",
   "entity":{
      "timer":null,
      "scrapedVoting":{
         "Id":null,
         "scrapedURL":"http://www.psp.cz/sqw/hlasy.sqw?g=56687",
         "meetingNumber":47,
         "votingNumber":39,
         "when":"2012-10-23T18:08:00+02:00",
         "subject":"Novela z. o sdružování v politických stranách a hnutích",
         "stenoprotokolURL":"http://www.psp.cz/eknih/2010ps/stenprot/047schuz/s047025.htm"
      },
      "State":2,
      "PositiveVotesCount":0,
      "NegativeVotesCount":0,
      "GetCurrentResolve":false,
      "Id":"votings/97",
      "version":0,
      "subscribedUsers":null,
      "OwnerId":null
   }
}
*/