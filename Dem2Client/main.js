﻿
IS_RUNNING_ON_SERVER = true;    //use true when deploying on the live server
IS_RUNNING_ON_SERVER = false;   // use false value for local testing

require(["Scripts/facebook"], function (FB) {
 
    var WSworker = new Worker('Scripts/wsworker.js');   //worker handling server comunication
    WSworker.onmessage = function (event) {
        switch (event.data.type) {
            case "debug":
                console.log("Msg from wsworker> " + event.data.message);
                break;
            case "connectionInfo":
                console.log("connection status> " + event.data.message);
                break;
            default:
                try {
                    var json = JSON.parse(event.data);
                }
                catch (e) {
                    alert('invalid json');
                }
                console.assert("Msg from wsworker> " + event.data);
                //other types of data
        }
    };

    FB.deffered.then(function (FBAccesToken) {
        console.log("User's facebook acces token is " + FBAccesToken);

        WSworker.postMessage(
        {
            msgType: "login", theUser:
              { accessToken: FBAccesToken }
        }
        );
    });
});