define(function () {
    return function (ent) {
        var r = {}
        r.Id = ent.Id;
        r.version = ent.version;
        r.OwnerId = ent.OwnerId;
        return r;
    };
});