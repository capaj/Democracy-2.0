define(["Scripts/Classes/Voting"], function (Voting) {
    VM = {
        //contructors start
        newVotingFromJS: Voting,      //makes an observable object
        //

        connected: ko.observable(false),      //websocket connection flag
        comments: ko.observableArray([]),
        votes: ko.observableArray([]),
        votings: {
            "none": Voting({
                "scrapedVoting": {
                    "scrapedURL": "nevybráno žádné hlasování",
                    "meetingNumber": "nevybráno žádné hlasování",
                    "votingNumber": "nevybráno žádné hlasování",
                    "when": "nevybráno žádné hlasování",
                    "subject": "nevybráno žádné hlasování",
                    "stenoprotokolURL": "nevybráno žádné hlasování"
                },
                "State": "nevybráno žádné hlasování",
                "PositiveVotesCount": "nevybráno žádné hlasování",
                "NegativeVotesCount": "nevybráno žádné hlasování",
                "Id": "nevybráno žádné hlasování",
                "version": "nevybráno žádné hlasování",
            })
        },
        currentSection: ko.observable(window.location.pathname),
        currentSectionIsStatic: ko.observable(false),     //default value
        //this.currentVoting = this.votings["none"];
        currentVotingId: ko.observable("none"),
        
    }; // extend a local variable to global, since we will need to acces it from everywhere
    VM.currentVoting = ko.computed(function () {
        return VM.votings[VM.currentVotingId()]();
    })
    return VM;
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