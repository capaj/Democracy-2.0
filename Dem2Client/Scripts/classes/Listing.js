define(["./ServerClientEntity"], function (ServerClientEntity) {
    return function (ent) {
        //{
        //    "operation":"u",
        //    "entity":
        //    {   "JSONQuery":
        //        {
        //            "descending":true,
        //            "sortByProp":"getResolveCount",
        //            "propertiesEqualValues": {"parentId":"votings/162"},
        //            "ofTypeInStr":"comments",
        //            "pageSize":20,"toSkip":0
        //        },
        //        "list":["comments/1"],
        //        "Id":"listings/65",
        //        "version":1,
        //        "OwnerId":"users/1"
        //    }
        //}
        var r = ServerClientEntity(ent);
        r.list = ko.observableArray(ent.list);
        r.JSONQuery = ko.mapping.fromJS(ent.JSONQuery);
        return ko.observable(r);
    };
});
