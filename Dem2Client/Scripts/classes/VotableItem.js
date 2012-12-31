define(["./ServerClientEntity"], function (ServerClientEntity) {
    return function (ent) {
        var r = ServerClientEntity(ent);
        r.elementId = r.Id.split("/").join("");     //there were troubles with "/" in element ids, to we cut it out and use the string without it
        r.creationTime = ko.observable(new Date(ent.creationTime));
        r.thisClientVoteId = ko.observable(null);
        r.thisClientVote = ko.computed({
            read: function () {
                var Id = r.thisClientVoteId();
                if (Id) {
                    if (VM.votes.hasOwnProperty(Id) === false) {
                        VM.votes[Id] = Vote(getWillLoadEntityTemplate("votes", Id));
                    }
                    return VM.votes[Id]();
                } else {
                    return null;
                }
            },
            deferEvaluation: true       // needed or this will cause fail for the whole page init
        });
        r.updateThisClientVoteTo = function (agrees) {
            var updateVoteReq = {
                "msgType": "entity",
                "operation": "u",
                "entity": { "Id": r.thisClientVote().Id, "Agrees": agrees }
            };
            WSworker.postMessage(updateVoteReq);
        };
        r.createThisClientVote = function (agrees) {
            var createVoteReq = {
                "className": "Vote",
                "msgType": "entity",
                "operation": "c",
                "entity": { "subjectID": r.Id, "Agrees": agrees }
            };
            WSworker.postMessage(createVoteReq);
        };

        r.thisClientVoteAgrees = ko.computed({ // used for disabling the voting buttons
            read: function () {
                if (r.thisClientVote()) {
                    if (r.thisClientVote().Agrees()) {
                        return true;
                    }
                }
                return false;
            },
            deferEvaluation: true       // needed or this will cause fail for the whole page init
        });

        r.thisClientVoteDisagrees = ko.computed({ // used for disabling the voting buttons
            read: function () {
                if (r.thisClientVote()) {
                    if (r.thisClientVote().Agrees() === false) {
                        return true;
                    }
                }
                return false;
            },
            deferEvaluation: true       // needed or this will cause fail for the whole page init
        });

        r.voteYes = function () {
            if (r.thisClientVoteId()) {
                if (r.thisClientVote().Agrees()) {
                    return;
                }
                r.updateThisClientVoteTo(true);
            } else {
                r.createThisClientVote(true);
            }
            
        };
        r.voteNo = function () {
            if (r.thisClientVoteId()) {
                if (r.thisClientVote().Agrees()) {
                    r.updateThisClientVoteTo(false);
                } else {
                    return;
                }
            } else {
                r.createThisClientVote(false);
            }
            
        };

        r.deleteVote = function () {
            r.thisClientVote().delete();
            
        };
        r.PositiveVotesCount = ko.observable(ent.PositiveVotesCount);
        r.NegativeVotesCount = ko.observable(ent.NegativeVotesCount);
        r.getResolve = ko.computed(function () {
            if (isNaN(r.PositiveVotesCount()) || isNaN(r.NegativeVotesCount())) {
                return false;
            } else {

            
                if (r.PositiveVotesCount() > r.NegativeVotesCount()) {
                    return true;
                } else {
                    return false;
                }
            }
        });

        r.getResolveCount = ko.computed({
            read: function () {
                return r.PositiveVotesCount() -r.NegativeVotesCount();
            },
            deferEvaluation: true
        });

        r.showVotesListing = function () {

        };

        r.percentAgrees = ko.computed({
            read: function () {
                var votesSum = r.NegativeVotesCount() + r.PositiveVotesCount();
                if (votesSum > 0) {
                    var onePercent = votesSum / 100;
                    return r.PositiveVotesCount() / onePercent;
                }
                return 50;
            },
            deferEvaluation: true
        });

        r.percentDisagrees = ko.computed({
            read: function () {
                var votesSum = r.NegativeVotesCount() + r.PositiveVotesCount();
                if (votesSum > 0) {
                    var onePercent = votesSum / 100;
                    return r.NegativeVotesCount() / onePercent;
                }
                return 50;
            },
            deferEvaluation: true
        });

        r.responseText = ko.observable("");

        r.createComment = function () {
            var createCommentReq = {
                "className": "Comment",
                "msgType": "entity",
                "operation": "c",
                "entity": { "parentId": r.Id, "texts": {1: r.responseText() } }
            };
            WSworker.postMessage(createCommentReq);
            r.responseText("");
            console.log("New comment on entity Id " + r.Id + ".");
        };

        r.listingQuery = {
            pageSize: ko.observable(20),
            ofTypeInStr: "comments",
            toSkip: ko.observable(0),
            descending: ko.observable(true), 
            sortByProp : ko.observable("getResolveCount"),
            propertiesEqualValues: {"parentId": r.Id}
        };

        r.createCommentsListing = function () {     // we are not gonna be sending 'u' requests from client, because it would be more problematic to update a listing than to only focus on creating new one
            var listingReq = {
                "className": "Listing",
                "msgType": "entity",
                "operation": "c",
                "entity": {"JSONQuery": ko.mapping.toJS(r.listingQuery)}
            };
            WSworker.postMessage(listingReq);
        };

        return ko.observable(r);
    };
});
