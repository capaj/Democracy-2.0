var socket;
var host = "ws://dem2.cz:8181";
var workerMessage = (function () {
    function workerMessage(type, message, _socket) {
        this.type = type;
        this.message = message;
        this.readyState = 0
        if (_socket) {
            this.readyState = _socket.readyState;
        }
    }
    return workerMessage;
})();


var connectToWSServer = function() {
    try {
        socket = new WebSocket(host);
        socket.onopen = function () {
            self.postMessage(new workerMessage("connectionInfo", "Connected to " + host, socket));
        };
        socket.onmessage = function (WSevent) {
            var msgfromServer = JSON.parse(WSevent.data);
            self.postMessage(WSevent.data);
        };
        socket.onclose = function () {
            self.postMessage(new workerMessage("connectionInfo", "Connection to " + host + " closed", socket));
        };
    } catch (exception) {
        self.postMessage(new workerMessage("connectionInfo", "Error when connecting to " + host, socket));
    }
}

self.onmessage = function (event) {
    var data = event.data;
    switch (data.msgType) {
        case 'R':
            send(JSON.stringify(data));
            break;
        case "hostAdress":
            host = data.host;     //for testing we are connecting to local websocket server
            break;
        case "command":
            switch (data.cmdType) {
                case "connect": {
                    connectToWSServer();

                }
            }
            break;
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
