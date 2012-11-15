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

        public void respondToReadRequest(IWebSocketConnection socket)       //"r" operation, respond to it can be only "u" (update) or "n" (not found) 
        {
            ServerClientEntity entityOnServer = ServerClientEntity.GetEntityFromSetsByID(entity.Id);

            if (entityOnServer != null)
            {
                try
                {
                    if (typeof(VotableItem).IsAssignableFrom(entityOnServer.GetType()))    //check if the entity we are responding with is a VotableItem or not, props to http://www.hanselman.com/blog/DoesATypeImplementAnInterface.aspx
                    {
                        var vote = Dem2Hub.allVotes.FirstOrDefault(x => x.subjectID == entity.Id && x.OwnerId == socket.ConnectionInfo.Cookies["user"]);
                        if (vote != null)
                        {
                            entityOperation sendVote = new entityOperation { entity = vote, operation = 'u' };
                            sendVote.sendTo(socket);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ExceptionIsCriticalCheck.IsCritical(ex)) throw;
                }
                
                if (entity.version < entityOnServer.version)
                {
                    //first possible outcome
                    entity = entityOnServer;    //so if the version on client is current by any chance we send back the same as we received
                }
                //second possible outcome
                operation = 'u';
                //client must check whether the update he received has greater version of entity, if not, he knows he has the latest version of entity
            }
            else
            {
                //third possible outcome
                operation = 'n';        //not found, nonexistent, in the entity field there is still the one that was in the request, so when it arrives back to the client he will know which request ended up not found
                
            }
            socket.Send(JsonConvert.SerializeObject(this, new IsoDateTimeConverter()));    

        }
        

        public void subscribeUserToChanges(IWebSocketConnection socket) {
            string type = entity.Id.Split('/')[0];
            ServerClientEntity entityOnServer = null;
            try
            {

            }
            catch (Exception)
            {
                
                throw;
            }
        }

        internal void resolveEntityRequest(IWebSocketConnection socket, Newtonsoft.Json.Linq.JObject receivedObj)
        {
            switch (operation)
            {
                case 'c':   //create new user generated entity
                    /*    
                   //Example shows json which creates new Vote for the present user
                       {
                         "msgType": "entity",
                         "operation": "c",
                         "className": "Vote",
                         "entity": {"user/125", "subjectID": "voting/215", "Agrees": true}
                       }


                       // TODO implement create spam check here
                    */
                    try
                    {
                        Type type = Type.GetType("Dem2UserCreated." + (string)receivedObj["className"]);
                        //object instance = Activator.CreateInstance(type, (Array)receivedObj["ctorArguments"]); old way, TODO test and remove this line
                        object instance = JsonConvert.DeserializeObject((string)receivedObj["entity"], type, new IsoDateTimeConverter());
                        Console.WriteLine("Object {0} created", instance.ToString());
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    break;
                case 'r':   //read an entity
                    /*
                    Example shows json for this branch 
                    {
                      "msgType": "entity",
                      "operation": "r",
                      "entity":{
                          "Id": "user/132"
                      }
                    }*/
                    respondToReadRequest(socket);
                    break;
                case 's': //subscribe to changes on entity
                    /*
                        {
                         "msgType": "entity",
                         "operation": "s",
                         "entity": {"user/125"}
                       }
                    */
                    subscribeUserToChanges(socket);
                    break;
                default:
                    break;
            }
        }
    }
}
