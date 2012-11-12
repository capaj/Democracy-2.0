define(["./ServerClientEntity"], function (ServerClientEntity) {
    return function (ent) {
        var r = ServerClientEntity(ent);
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
