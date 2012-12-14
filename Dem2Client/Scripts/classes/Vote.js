define(["./ServerClientEntity"], function (ServerClientEntity) {
    return function (ent) {
        var r = ServerClientEntity(ent);
        r.Agrees = ko.observable(ent.Agrees);
        r.subjectId = ko.observable(ent.subjectId);
        r.castedTime = ko.observable(new Date(ent.castedTime));
        r.delete = function () {    //should get called from VotableItem
            var VoteDeleteReq = {
                "msgType": "entity",
                "operation": "d",
                "entity": { "Id": r.Id }
            };

            WSworker.postMessage(VoteDeleteReq);
        };

        return ko.observable(r);
    };
});
