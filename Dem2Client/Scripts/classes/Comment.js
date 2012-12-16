define(["./VotableItem"], function (VotableItem) {
    return function (ent) {
        var r = VotableItem(ent)();
        r.parentId = ko.observable(ent.parentId);
        r.texts = ko.observableArray(ent.texts);
        r.deleted = ko.observable(ent.deleted);
        return ko.observable(r);
    };
});
