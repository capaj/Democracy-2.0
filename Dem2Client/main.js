require([
    "Scripts/facebook"
], function (FB) {
    FB.deffered.then(function (value) {
        console.log("User's facebook acces token is " + value);
    });
    var WSworker = new Worker('/Scripts/wsworker.js');
    WSworker.onmessage = function (event) {
        switch(event.data.type) {
            case "debug": {
                console.log("Msg from wsworker> " + event.data.message);
                break;

            }
            default: {
                console.error("Msg from wsworker> " + event.data.message);

            }
        }
    };
    try  {
        var json = JSON.parse(this.responseText);
    } catch (e) {
        alert('WS worker not started!');
    }
});
