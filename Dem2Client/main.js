﻿
//IS_RUNNING_ON_SERVER = true;    //use true when deploying on the live server
IS_RUNNING_ON_SERVER = false;   // use false value for local testing

navigator.sayswho = (function () {      // thanks to kennebec on stackoverflow.com
    var N = navigator.appName, ua = navigator.userAgent, tem;
    var M = ua.match(/(opera|chrome|safari|firefox|msie)\/?\s*(\.?\d+(\.\d+)*)/i);
    if (M && (tem = ua.match(/version\/([\.\d]+)/i)) != null) M[2] = tem[1];
    M = M ? [M[1], M[2]] : [N, navigator.appVersion, '-?'];
    return M;
})();

require(["Scripts/facebook", "Scripts/viewModel", "Scripts/addressResolver" ], function (FB, viewModel, addressResolver) {
    if (navigator.sayswho[0] != "Chrome") { 
        $('#notChromeWarning').modal('show')    //warning about non chrome environment
    }

    window.onpopstate = function (event) {
        console.log("window.onpopstate event fired, location: " + document.location + ", state: " + JSON.stringify(event.state));
        var path = window.location.pathname;  // future or init, in fact with HTML5 history API we don't care
        addressResolver.resolve(path);
    };

    $(document).click(function (e) {
        
        if (e.target.pathname) {
            var link = e.target.pathname;
            //console.log("Click on link intercepted with href " + link);
            var section = addressResolver.getStringBeforeLastSlash(link);
            if (addressResolver.resolverMap.hasOwnProperty(section)) {
                var title = e.srcElement.innerText; 
                e.preventDefault();
                addressResolver.resolve(link, title);
            } 
            
        }

    });
    
    ko.applyBindings(VM);   //VM is global defined in viewModel.js

    WSworker = new Worker('Scripts/wsworker.js');   //worker handling server comunication

    FB.deffered.then(function (FBAccesToken) {
        WSworker.onmessage = function (event) {
            switch (event.data.msgType) {
                case "debug":
                    console.log("Debug message from wsworker> " + event.data.message);
                    break;
                case "connectionInfo":
                    console.log("connection status> " + event.data.message);
                    switch (event.data.readyState) {
                        case 1:
                            VM.connected(true);
                            WSworker.postMessage(
                                {
                                    msgType: "login", theUser:
                                        { accessToken: FBAccesToken }
                                }
                            );
                            break;
                        case 3:
                        case 4:
                            VM.connected(false);
                            break;
                    }
                    
                    break;
                default: //other types of data
                    try {
                        var entOp = event.data; // deserialization happens in ws worker, so no need to do it here
                        if (entOp.hasOwnProperty("operation")) {
                            var entityId = entOp.entity.Id;
                            var type = entityId.substring(0, entityId.indexOf("/"));
                            switch (entOp.operation) {                               
                                case "c":   //create
                                    VM.createEntityFromEntOp(type, entityId, entOp);
                                    break;
                                case "u":   //update
                                  
                                    if (VM[type].hasOwnProperty(entityId) === false) {
                                        VM.createEntityFromEntOp(type, entityId, entOp);
                                    } else {
                                        VM[type][entityId](VM.constructors[type](entOp.entity)());       //we will contruct the type we have received from server and store it in the proper table under his id
                                    }

                                    break;
                                case "d":   //delete
                                    delete VM[type][entityId];
                                    break;
                                default:

                            }
                           
                        }
 
                    }
                    catch (e) {
                        console.error('invalid json arrived from server');
                    }

            }
        };
        console.log("User's facebook acces token is " + FBAccesToken);

        if (IS_RUNNING_ON_SERVER == false) {
           
            WSworker.postMessage(
                {
                    msgType: "hostAdress", host: "ws://localhost:8181"
                }
            );
        }
        WSworker.postMessage(
            {
                msgType: "WorkerCommand", cmdType: "connect"
            }
        );
    });

    addressResolver.resolve(window.location.pathname);
});