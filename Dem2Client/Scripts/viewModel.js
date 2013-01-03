define(["Scripts/Classes/Voting", "Scripts/Classes/Vote", "Scripts/Classes/Listing"], function (Voting, Vote, Listing) {
    VM = {
        //they make an observable object from plain JS object
        constructors: {   //contructors start
            "votings": Voting,   
            "votes": Vote,
            "listings": Listing
        },// contructors end
        connected: ko.observable(false),      //websocket connection flag
        comments: {},
        votes: { "none": Vote({})},
        listings: {},
        votings: {
            "none": Voting({
                "scrapedPrint": {
                    "number": 0,
                    "URL": "nenačteno",
                    "relatedPrintsListURL": "nenačteno",
                    "title": "nenačteno",
                    "type": "nenačteno",
                    "relatedpspVotings": [],
                    "relatedPrintsURLs": [],
                    "inAgenda": {
                        "URL": "nenačteno",
                        "starts": "nenačteno",
                        "ends": "nenačteno",
                        "meetingDates": {}
                    }
                },
                "State": "nenačteno",
                "PositiveVotesCount": 0,
                "NegativeVotesCount": 0,
                "Id": "nenačteno",
                "version": 0,
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
                    "scrapedPrint": {
                        "number": 0,
                        "URL": "nenačteno",
                        "relatedPrintsListURL": "nenačteno",
                        "title": "nenačteno",
                        "type": "nenačteno",
                        "relatedpspVotings": [],
                        "relatedPrintsURLs": [],
                        "inAgenda": {
                            "URL": "nenačteno",
                            "starts": "nenačteno",
                            "ends": "nenačteno",
                            "meetingDates": {}
                        }
                    },
                    "State": "nenačteno",
                    "PositiveVotesCount": 0,
                    "NegativeVotesCount": 0,
                    "Id": entityId,
                    "version": 0,
                };
            case "listings":
                return {};
        }
    }

    VM.createEntityFromEntOp = function (type, entityId, entOp) {
        VM[type][entityId] = VM.constructors[type](entOp.entity);
        if (type == "votes") {
            var subjectId = entOp.entity.subjectId;
            var subjectType = subjectId.substring(0, subjectId.indexOf("/"));
            if (VM[subjectType].hasOwnProperty(subjectId) === true) {
                VM[subjectType][subjectId]().thisClientVoteId(entityId);
            } else {    //the votable item is not here on the client yet, so it is probably fucked
                console.error("Vote on unknown subject wants to be created");
            }

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