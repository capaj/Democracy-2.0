define(function () {
    return function (ent) {
        var r = {}
        r.Id = ent.Id;  // id is immutable, so no need to have it as observable
        r.OwnerId = ent.OwnerId;    //also immutable
        r.version = ko.observable(ent.version);

        return r;
    };
});