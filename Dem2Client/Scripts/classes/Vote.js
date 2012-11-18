define(["./ServerClientEntity"], function (ServerClientEntity) {
    return function (ent) {
        var r = ServerClientEntity(ent);
        r.Agrees = ko.observable(ent.Agrees);
        r.subjectID = ko.observable(ent.subjectID);
        r.delete = function () {    //should get called from VotableItem
            var VoteDeleteReq = {
                "msgType": "entity",
                "operation": "d",
                "entity": { "Id": r.Id }
            };

            WSworker.postMessage(VoteDeleteReq);
        }
        return ko.observable(r);
    };
});
