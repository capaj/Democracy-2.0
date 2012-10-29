define(function () {

    function ViewModel() {
        this.connected = ko.observable(false);      //websocket connection flag
        this.comments = ko.observableArray([]);
        this.votes = ko.observableArray([]);
        this.votings = ko.observableArray([]);
    }
    return new ViewModel;; 
});