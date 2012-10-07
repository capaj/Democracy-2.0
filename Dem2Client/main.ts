///<reference path="..\Dem2Client\Declarations\knockout.d.ts"/>

declare var require: (moduleName: Array, func: any ) => any;

require(["Scripts/facebook"], function (FB) {
    
    

    var WSworker = new Worker('/Scripts/wsworker.js');   //worker handling server comunication

    WSworker.onmessage = function (event) {
        switch (event.data.type) {
            case "debug":
                console.log("Msg from wsworker> " + event.data.message);
                break;
            default:
                try {
                    var json = JSON.parse(this.responseText);
                }
                catch () {
                    console.error("Msg from wsworker not parseable> " + event.data.message);
                }
    
                console.error("Msg from wsworker> " + event.data.message);
                //other types of data
        }
    };

    FB.deffered.then(function(FBAccesToken) {
        console.log("User's facebook acces token is " + FBAccesToken);

        WSworker.postMessage(
        { msgType: "login", theUser: 
            { accesToken: FBAccesToken } 
        }
        );
    });
    
});