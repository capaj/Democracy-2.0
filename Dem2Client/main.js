require(["Scripts/facebook"], function (FB) {
    FB.deffered.then(function(value) {
        console.log("User's facebook acces token is " + value);
    });

    var WSworker = new Worker('wsworker.js');   //worker handling server comunication
    WSworker.onmessage = function (event) {
        switch (event.data.type) {
            case "debug":
                console.log("Msg from wsworker> " + event.data.message);
                break;
            default:
                console.assert("Msg from wsworker> " + event.data.message);
                //other types of data
        }
    };

    try {
        var json = JSON.parse(this.responseText);
    }
    catch (e) {
        alert('invalid json');
    }
    
});