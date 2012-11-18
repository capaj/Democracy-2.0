define(["Scripts/Classes/Voting", "Scripts/Classes/Vote"], function (Voting, Vote) {
    VM = {

        constructors: {   //contructors start
            "votings": Voting   //makes an observable object
        },// contructors end
        connected: ko.observable(false),      //websocket connection flag
        comments: {},
        votes: { "none": Vote({})},
        votings: {
            "none": Voting({
                "scrapedVoting": {
                    "scrapedURL": "#",
                    "meetingNumber": "nevybráno žádné hlasování",
                    "votingNumber": "nevybráno žádné hlasování",
                    "when": "nevybráno žádné hlasování",
                    "subject": "nevybráno žádné hlasování",
                    "stenoprotokolURL": "#"
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
        var Id = VM.currentVotingId();
        if (VM.votings.hasOwnProperty(Id) === false) {
            VM.votings[Id] = Voting(getWillLoadEntityTemplate("votings", Id));
        }
        return VM.votings[Id]();
    });

    function getWillLoadEntityTemplate(type, entityId) {
        switch (type) {
            case "votings":
                return {
                    "scrapedVoting": {
                        "scrapedURL": "psp odkaz pro hlasování " + entityId,
                        "meetingNumber": "číslo hlasování u " + entityId,
                        "votingNumber": "nenačteno",
                        "when": "nenačteno",
                        "subject": entityId,
                        "stenoprotokolURL": "nenačteno"
                    },
                    "State": "nenačteno",
                    "PositiveVotesCount": "nenačteno",
                    "NegativeVotesCount": "nenačteno",
                    "Id": entityId,
                    "version": "nenačteno",
                };
                break;
            case "listings":
                return {};
        }
    };
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