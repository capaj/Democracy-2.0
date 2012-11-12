define(["./VotableItem"], function (VotableItem) {
    return function (ent) {
        var r = VotableItem(ent)();
        r.scrapedVoting = ko.mapping.fromJS(ent.scrapedVoting);
        r.state = ko.observable(ent.state);
        r.voteYes = function () {
            //TODO implement
        };
        r.voteNo = function () {
            //TODO implement
        };
        r.deleteVote = function () {
            //TODO implement
        };
        r.updateFromJS = function (JSObject) {
            r.scrapedVoting.subject(JSObject.scrapedVoting.subject);

        }
        r.aVote = ko.observable(null);
        return ko.observable(r);
    };
});

