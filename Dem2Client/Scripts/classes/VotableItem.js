define(["./ServerClientEntity"], function (ServerClientEntity) {
    return function (ent) {
        var r = ServerClientEntity(ent);
        r.thisClientVoteId = ko.observable();
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
        r.voteYes = function () {
            var VoteReq = {
                "className": "Vote",
                "msgType": "entity",
                "operation": "c",
                "entity": { "subjectID": r.Id, "Agrees": true }
            };

            WSworker.postMessage(VoteReq);


            //TODO implement
        };
        r.voteNo = function () {
            var VoteReq = {
                "className": "Vote",
                "msgType": "entity",
                "operation": "c",
                "entity": { "subjectID": r.Id, "Agrees": false }
            };

            WSworker.postMessage(VoteReq);
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
