var socket;
var host = "ws://localhost:8181";

try {
    socket = new WebSocket(host);

    socket.onopen = function () {

        self.postMessage({
            type: "debug",
            message: "Connected to " + host
        });

    }

    socket.onmessage = function (WSevent) {
        var msgfromServer = JSON.parse(WSevent.data);

        self.postMessage(WSevent.data);
    }

    socket.onclose = function () {

    }

} catch (exception) {
    self.postMessage({
        type: "debug",
        message: "Error when connecting to " + host
    });
}

self.onmessage = function (event) {
    var data = event.data;
    switch (data.messageType) {
        case 'R':
            send(JSON.stringify(data));
        default:

    };
};

function send(msgToServer) {
    try {
        socket.send(msgToServer);
    } catch (exception) {
        self.postMessage("Error when sending to server.");
    }
}