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

    var staticPages =   // static pages are in group defined here
        [
            "/contact", 
            "/about"
        ];

    var resolver = {
        "/votings": viewVotings,
    };

    for (var i = 0; i < staticPages.length; i++) {
        resolver[staticPages[i]] = loadStaticToMain;
    };

    return resolver;
});