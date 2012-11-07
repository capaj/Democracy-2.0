﻿using Fleck;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dem2Server
{
    
    class entityOperation
    {
        public char operation { get; set; }     //crud
        public ServerClientEntity entity { get; set; }

        public void respondToReadRequest(IWebSocketConnection socket)
        {
            string type = entity.Id.Split('/')[0];
            var ent = Dem2Hub.entityNamesToSets[type].First(x => x.Id == entity.Id);

            operation = 'u';
            socket.Send(JsonConvert.SerializeObject(ent, new IsoDateTimeConverter()));
        }
    }
}
