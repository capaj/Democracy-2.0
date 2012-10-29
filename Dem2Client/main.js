
//IS_RUNNING_ON_SERVER = true;    //use true when deploying on the live server
IS_RUNNING_ON_SERVER = false;   // use false value for local testing

navigator.sayswho = (function () {      // thanks to kennebec on stackoverflow.com
    var N = navigator.appName, ua = navigator.userAgent, tem;
    var M = ua.match(/(opera|chrome|safari|firefox|msie)\/?\s*(\.?\d+(\.\d+)*)/i);
    if (M && (tem = ua.match(/version\/([\.\d]+)/i)) != null) M[2] = tem[1];
    M = M ? [M[1], M[2]] : [N, navigator.appVersion, '-?'];
    return M;
})();

require(["Scripts/facebook", "Scripts/viewModel", "Scripts/linkClickHandler"], function (FB, viewModel, linkClickHandler) {
    if (navigator.sayswho[0] != "Chrome") { 
        $('#notChromeWarning').modal('show')    //warning about non chrome environment
    }

    $(document).click(function (e) {
        
        if (e.target.attributes["href"]) {
            var link = e.target.attributes["href"].value;
            console.log("Click on link intercepted with href " + link);
            if (linkClickHandler.hasOwnProperty(link)) {

                e.preventDefault();
                linkClickHandler[link](link);
            }
        }

    });
    VM = viewModel; // extend a local variable to global, since we will need to acces it from everywhere
    ko.applyBindings(VM); 

    WSworker = new Worker('Scripts/wsworker.js');   //worker handling server comunication

    FB.deffered.then(function (FBAccesToken) {
        WSworker.onmessage = function (event) {
            switch (event.data.type) {
                case "debug":
                    console.log("Msg from wsworker> " + event.data.message);
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
                        var json = JSON.parse(event.data);
                    }
                    catch (e) {
                        alert('invalid json');
                    }
                    console.assert("Msg from wsworker> " + event.data);

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
                msgType: "command", cmdType: "connect"
            }
        );
    });
});