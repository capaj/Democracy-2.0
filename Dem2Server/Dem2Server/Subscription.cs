using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dem2Model;
using Dem2Server;
using Newtonsoft.Json;

namespace Dem2UserCreated
{
    public class Subscription
    {
        //since these will never be stored in a database, they don't need string Ids, like serverclient entity has
        public string onEntityId { get; set; }

        [JsonIgnore]
        public ServerClientEntity onEntity {
            get {
                return EntityRepository.GetEntityFromSetsByID(onEntityId);
            }
        }
        //object instance = Activator.CreateInstance(type, (Array)receivedObj["ctorArguments"]);
        public void subscribe(User user)
        {
            user.SubscribeToEntity(this);
        }

        public void unsubscribe(User user)
        {
            user.UnsubscribeFromEntity(this);
        }

        public void subscribe(Fleck.IWebSocketConnection socket)
        {
            User.getUserFromSocket(socket).SubscribeToEntity(this);
        }
    }
}
