using Fleck;
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
            var ent = Dem2Hub.entityNamesToSets[type].FirstOrDefault(x => x.Id == entity.Id);
            if (ent!= null)
            {
                entity = ent;
                operation = 'u';
                socket.Send(JsonConvert.SerializeObject(this, new IsoDateTimeConverter()));
            }
            else
            {
                operation = 'n';        //not found, nonexistent
                socket.Send(JsonConvert.SerializeObject(this, new IsoDateTimeConverter()));
            }
            
        }
    }
}
