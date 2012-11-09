define(["./viewModel"], function (viewModel) {
    var loadStaticToMain = function (URL) {
        $.ajax({
            url: "/static" + URL,
        }).done(function (data) {
            document.getElementById("main").innerHTML = data;
        });

    };

    var viewSingleVoting = function (url) {
        //TODO implement 
    };
    var listVotings = function (url) {
        //TODO implement 
    };

    var viewComments = function (url) {
        //TODO implement 
    };

    var viewUsers = function (url) {
        //TODO implement 
    };

    var staticPages =   // static pages are in group defined here, if conflicting with resolverMap, static page overrides the previous entry
        [
            "/home",
            "/contact",
            "/about"
        ];

    var resolverMap = {
        "/voting": viewSingleVoting,
        "/votings": listVotings,
        "/": function () {
            resolverMap["/home"]("/home");
        }
    };

    for (var i = 0; i < staticPages.length; i++) {
        resolverMap[staticPages[i]] = loadStaticToMain;
    };

    var resolver = function(link, title) {
        var indexOfSlash = link.lastIndexOf("/");
        if (indexOfSlash > 0) {
            var sectionOnly = link.substring(0, indexOfSlash);
        } else {
            var sectionOnly = link;
        }
        
        if (resolverMap.hasOwnProperty(sectionOnly)) {
            VM.currentSection(sectionOnly);
            resolverMap[sectionOnly](link);
            if (title) {
                history.pushState(ko.toJS(VM), title, link);
            }
            
            return true;
        }
        return false;
    }

    return {
        "resolve": resolver,
        "resolverMap": resolverMap
    };
});