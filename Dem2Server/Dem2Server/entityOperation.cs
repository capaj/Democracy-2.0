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

        public void sendTo(IWebSocketConnection socket) {
            socket.Send(JsonConvert.SerializeObject(this, new IsoDateTimeConverter()));
        }

        public void respondToReadRequest(IWebSocketConnection socket)
        {
            string type = entity.Id.Split('/')[0];
            ServerClientEntity ent = null;

            try
            {
                ent = Dem2Hub.entityNamesToSets[type].FirstOrDefault(x => x.Id == entity.Id);
                if (typeof(VotableItem).IsAssignableFrom(ent.GetType()))    //check if the entity we are responding with is a VotableItem or not, props to http://www.hanselman.com/blog/DoesATypeImplementAnInterface.aspx
                {
                    var vote = Dem2Hub.allVotes.FirstOrDefault(x => x.subjectID == entity.Id && x.OwnerId == socket.ConnectionInfo.Cookies["user"]);
                    if (vote != null)
                    {
                        entityOperation sendVote = new entityOperation { entity = vote, operation = 'c' };
                        sendVote.sendTo(socket);
                    }
                }
            }
            catch (Exception ex)
            {
                if (ExceptionIsCriticalCheck.IsCritical(ex)) throw;
                Console.WriteLine("Entity with ID {0} was not found", entity.Id);
            }
            finally
            {
                if (ent != null)
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
}
