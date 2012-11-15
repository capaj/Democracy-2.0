var socket;
var Cached = {}; //here we have all the entities that go through the worker cached
Cached.votings = {};
Cached.listings = {};
Cached.comments = {};
Cached.users = {};

var host = "ws://dem2.cz:8181";
//comunication with server
var connectToWSServer = function () {
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
//communication with server ends

//comunication with client

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


self.onmessage = function (event) {
    var data = event.data;
    switch (data.msgType) {
        case 'entity':  //this 
            EntityOperationHandler(data);
            break;
        case "hostAdress":
            host = data.host;     //for testing we are connecting to local websocket server
            break;
        case "WorkerCommand":
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

function EntityOperationHandler(operation) {
    var entityId = operation.entity.Id;
    var type = entityId.substring(0, entityId.indexOf("/"));
    if (Cached[type].hasOwnProperty(entityId) === false) {
        Cached[type][entityId] = getWillLoadEntityTemplate(type);
    }else { //worker has some version of entity already cached, so we append the version to request and continue
        operation.entity.version = Cached[type][entityId].version;
    }
    
    send(JSON.stringify(operation));
}

function getWillLoadEntityTemplate(type) {
    switch (type) {
        case "votings":
            return {
                "scrapedVoting": {
                    "scrapedURL": "psp odkaz pro hlasování " + entityId,
                    "meetingNumber": "číslo hlasování u " + entityId,
                    "votingNumber": "nenačteno",
                    "when": "nenačteno",
                    "subject": entityId,
                    "stenoprotokolURL": "nenačteno"
                },
                "State": "nenačteno",
                "PositiveVotesCount": "nenačteno",
                "NegativeVotesCount": "nenačteno",
                "Id": entityId,
                "version": "nenačteno",
            };
            break;
        case "listings":
            return {};
    }
}

function send(msgToServer) {
    try {
        socket.send(msgToServer);
    } catch (exception) {
        self.postMessage(new workerMessage("debug", "Error when sending to server the message: " + msgToServer));
    }
}
