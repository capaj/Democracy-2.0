define(function () {
    return function (ent) {
        var r = {}
        r.Id = ent.Id;  // id is immutable, so no need to have it as observable
        r.version = ent.version;
        r.OwnerId = ent.OwnerId;
        return r;
    };
});