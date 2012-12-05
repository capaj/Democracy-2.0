define(["./VotableItem"], function (VotableItem) {
    return function (ent) {
        var r = VotableItem(ent)();
        r.scrapedPrint = ko.mapping.fromJS(ent.scrapedVoting);
        r.state = ko.observable(ent.state);

        r.updateFromJS = function (JSObject) {
            r.scrapedPrint.subject(JSObject.scrapedVoting.subject);

        }
        r.aVote = ko.observable(null);
        return ko.observable(r);
    };
});

