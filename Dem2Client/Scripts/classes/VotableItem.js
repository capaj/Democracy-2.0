define(["./ServerClientEntity"], function (ServerClientEntity) {
    return function (ent) {
        var r = ServerClientEntity(ent);
        r.thisClientVoteId = ko.observable(null);
        r.thisClientVote = ko.computed({
            read: function () {
                var Id = r.thisClientVoteId();
                if (VM.votes.hasOwnProperty(Id) === false) {
                    VM.votes[Id] = Vote(getWillLoadEntityTemplate("votes", Id));
                }
                return VM.votes[Id]();
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
        }
        r.createThisClientVote = function (agrees) {
            var createVoteReq = {
                "className": "Vote",
                "msgType": "entity",
                "operation": "c",
                "entity": { "subjectID": r.Id, "Agrees": true }
            };
            WSworker.postMessage(createVoteReq);
        }
        r.voteYes = function () {
            if (r.thisClientVoteId()) {
                r.updateThisClientVoteTo(true);
            } else {
                r.createThisClientVote(true);
            }
        };
        r.voteNo = function () {
            if (r.thisClientVoteId()) {
                r.updateThisClientVoteTo(false);
            } else {
                r.createThisClientVote(false);
            }
        };
        r.deleteVote = function () {
            r.thisClientVote().delete();
        };
        r.PositiveVotesCount = ko.observable(ent.PositiveVotesCount);
        r.NegativeVotesCount = ko.observable(ent.NegativeVotesCount);
        r.GetCurrentResolve = ko.computed(function () {
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

        return ko.observable(r);
    };
});
