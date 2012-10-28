define(function () {

    function ViewModel() {
        this.connected = ko.observable(false);
    }
    return new ViewModel; // for easier debugging we will define VM in global namespace
});