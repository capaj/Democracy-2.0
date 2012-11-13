define(["./viewModel"], function (viewModel) {
    var loadStaticToMain = function (URL) {
        VM.currentSectionIsStatic(true);
        $.ajax({
            url: "/static" + URL,
        }).done(function (data) {
            document.getElementById("static").innerHTML = data;
        });

    };

    var viewVotings = function (url) {
        console.log("viewSingleVoting called with url " + url);
        var entityId = url.substring(1,url.length)
        WSworker.postMessage({ "msgType": "entity", "operation": "r", "entity": { "Id": entityId } });
        if (VM.votings.hasOwnProperty(entityId) === false) {
            VM.votings[entityId] = VM.newVotingFromJS({
                "scrapedVoting": {
                    "scrapedURL": "psp odkaz pro hlasování " + entityId,
                    "meetingNumber": "číslo hlasování u " + entityId,
                    "votingNumber": "nenačteno",
                    "when": "nenačteno",
                    "subject": entityId,
                    "stenoprotokolURL": "nenačteno"
                },
                "State": "nenačteno",
                "PositiveVotesCount": "nenačteno",
                "NegativeVotesCount": "nenačteno",
                "Id": entityId,
                "version": "nenačteno",
            });
        }
        
        VM.currentVotingId(entityId);
    };
    var listVotings = function (url) {
        //TODO implement 
        console.log("viewSingleVoting called with url " + url)
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
        "/votings": viewVotings,
        //"/votings": listVotings,
        "/": function () {
            resolverMap["/home"]("/home");
        }
    };

    for (var i = 0; i < staticPages.length; i++) {
        resolverMap[staticPages[i]] = loadStaticToMain;
    };

    var getStringBeforeLastSlash = function (link) {
        var indexOfSlash = link.lastIndexOf("/");
        if (indexOfSlash > 0) {
            var sectionOnly = link.substring(0, indexOfSlash);
        } else {
            var sectionOnly = link;
        }
        return sectionOnly;
    }

    var resolver = function(link, title) {
        var sectionOnly = getStringBeforeLastSlash(link);
        
        if (resolverMap.hasOwnProperty(sectionOnly)) {
            VM.currentSectionIsStatic(false);
            VM.currentSection(sectionOnly);
            resolverMap[sectionOnly](link);
            if (title) {
                //history.pushState(ko.toJS(VM), title, link);
                history.pushState({}, title, link);
            }
            
            return true;
        }
        return false;
    }

    return {
        "getStringBeforeLastSlash":getStringBeforeLastSlash,
        "resolve": resolver,
        "resolverMap": resolverMap
    };
});