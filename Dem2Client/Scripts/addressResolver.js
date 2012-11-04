define(["./viewModel"], function (viewModel) {
    var loadStaticToMain = function (URL) {
        $.ajax({
            url: "/static" + URL,
        }).done(function (data) {
            document.getElementById("main").innerHTML = data;
        });

    };

    var viewVotings = function (url) {
        //TODO implement functions which will requst entites on server
    };

    var viewComments = function (url) {
        //TODO implement functions which will requst entites on server
    };

    var viewUsers = function (url) {
        //TODO implement functions which will requst entites on server
    };

    var staticPages =   // static pages are in group defined here, if conflicting with resolverMap, static page overrides the previous entry
        [
            "/home",
            "/contact",
            "/about"
        ];

    var resolverMap = {
        "/votings": viewVotings,
    };

    for (var i = 0; i < staticPages.length; i++) {
        resolverMap[staticPages[i]] = loadStaticToMain;
    };

    var resolver = function(link) {
        if (resolverMap.hasOwnProperty(link)) {
            resolverMap[link](link);
            return true;
        }
        return false;
    }

    return {
        "resolver": resolver,
        "resolverMap": resolverMap
    };
});