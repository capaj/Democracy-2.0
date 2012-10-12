var socket;
var workerMessage = (function () {
    function workerMessage(type, message, readyState) {
        this.type = type;
        this.message = message;
        this.readyState = readyState;
    }
    return workerMessage;
})();

if (IS_RUNNING_ON_SERVER) {
    var host = "ws://dem2.cz:8181";
} else {
    var host = "ws://localhost:8181";     //for testing we are connecting to local websocket server
}


try {
    socket = new WebSocket(host);
    socket.onopen = function () {
        self.postMessage(new workerMessage("connectionInfo", "Connected to " + host, socket.readyState));
    };
    socket.onmessage = function (WSevent) {
        var msgfromServer = JSON.parse(WSevent.data);
        self.postMessage(WSevent.data);
    };
    socket.onclose = function () {
        self.postMessage(new workerMessage("connectionInfo", "Connection to " + host + " closed", socket.readyState));
    };
} catch (exception) {
    self.postMessage(new workerMessage("connectionInfo", "Error when connecting to " + host, socket.readyState));
}
self.onmessage = function (event) {
    var data = event.data;
    switch (data.msgType) {
        case 'R': {
            send(JSON.stringify(data));

        }
        //case "login"    
        default:
            send(JSON.stringify(data));
    }
    
};

function send(msgToServer) {
    try {
        socket.send(msgToServer);
    } catch (exception) {
        self.postMessage(new workerMessage("debug", "Error when sending to server the message: " + msgToServer));
    }
}
