var socket;
var Cached = { //here we have all the entities that go through the worker cached
    votings : {},
    listings : {},
    comments : {},
    users : {}
}; 
//var requests = {
//    votings: {},
//    listings: {},
//    comments: {},
//    users: {}
//}

var workerMessage = (function () {
    function workerMessage(type, message, _socket) {
        this.msgType = type;
        this.message = message;
        this.readyState = 0
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
}

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
    
    switch (operation.operation) {
        case "r":
            var toSend = null;
            if (Cached[type].hasOwnProperty(entityId) === false) {
                //Cached[type][entityId] = template;
                //var template = getWillLoadEntityTemplate(type, entityId);
                //toSend = {
                //    "operation": "c",
                //    "entity": template
                //};

            } else { //worker has some version of entity already cached, so we append the version to request and continue
                operation.entity.version = Cached[type][entityId].version;
                toSend = {
                    "operation": "u",
                    "entity": Cached[type][entityId]
                };
                self.postMessage(toSend);
            }
            

            send(JSON.stringify(operation));
            break;
        case "u":

            break;
        default:
        
    }
};
//comunication with client end


function updateEntityInCache(type, entityId, entity) {
    if (Cached[type].hasOwnProperty(entityId) === false) {
        Cached[type][entityId] = entity;
    }else{
        if (Cached[type][entityId].version < entity.version) {
            Cached[type][entityId] = entity;  
        }
    }
}

function send(msgToServer) {
    try {
        socket.send(msgToServer);
    } catch (exception) {
        self.postMessage(new workerMessage("debug", "Error when sending to server the message: " + msgToServer));
    }
};
