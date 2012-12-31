var socket;
var Cached = { //here we have all the entities that go through the worker cached
    votings : {},
    listings : {},
    comments : {},
    users: {},
    votes: {}
}; 

var workerMessage = (function () {
    function workerMessage(type, message, _socket) {
        this.msgType = type;
        this.message = message;
        this.readyState = 0;
        if (_socket) {
            this.readyState = _socket.readyState;
        }
    }
    return workerMessage;
})();

var host = "ws://dem2.cz:8181";
//comunication with server
var connectToWSServer = function () {
    try {
        socket = new WebSocket(host);
        socket.onopen = function () {
            self.postMessage(new workerMessage("connectionInfo", "Connected to " + host, socket));
        };
        socket.onmessage = function (WSevent) {
            self.postMessage(new workerMessage("debug", "Server sent us: " + WSevent.data));
            var entOp = JSON.parse(WSevent.data);
            if (entOp.hasOwnProperty("operation")) {    // 
                switch (entOp.operation) {
                    case "u":
                        var entityId = entOp.entity.Id;
                        var type = entityId.substring(0, entityId.indexOf("/"));
                        updateEntityInCache(type, entityId, entOp.entity);
                        break;
                    case "d":
                        break;
                    case "c":

                        break;
                    default:

                }

            }
            self.postMessage(entOp);
        };
        socket.onclose = function () {
            self.postMessage(new workerMessage("connectionInfo", "Connection to " + host + " closed", socket));
        };
    } catch (exception) {
        self.postMessage(new workerMessage("connectionInfo", "Error when connecting to " + host, socket));
    }
};

//communication with server ends

//comunication with client

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
                case "connect":
                    connectToWSServer();
                    break;
                default:
                    console.error("Unknown worker command");
            }
            break;
        default:
            send(JSON.stringify(data));
    }
   
};


function EntityOperationHandler(operation) {
    switch (operation.operation) {
        case "r":
            var entityId = operation.entity.Id;
            var type = entityId.substring(0, entityId.indexOf("/"));

            if (Cached[type].hasOwnProperty(entityId)) {
                //worker has some version of entity already cached, so we append the version to request and continue
                operation.entity.version = Cached[type][entityId].version;
                var toSend = {
                    "operation": "u",
                    "entity": Cached[type][entityId]
                };
                self.postMessage(toSend);   //before server gets a chance to respond, we provide client with the cached copy of entity
            }

            break;
        case "u":

            break;
        case "c":
            
            break;
        case "d":

            break;
        default:
        
    }
    send(JSON.stringify(operation));
}
//comunication with client end


function updateEntityInCache(type, entityId, entity) {
    if (Cached[type].hasOwnProperty(entityId) === false) {
        Cached[type][entityId] = entity;
        return true;
    }
    if (Cached[type][entityId].version < entity.version) {
        Cached[type][entityId] = entity;
        return true;
    } else {
        return false;   //cached version is higher, this should not occur
    }
    
}

function send(msgToServer) {
    try {
        socket.send(msgToServer);
    } catch (exception) {
        self.postMessage(new workerMessage("debug", "Error when sending to server the message: " + msgToServer));
    }
}
