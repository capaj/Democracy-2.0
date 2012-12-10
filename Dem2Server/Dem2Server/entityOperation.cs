﻿using Fleck;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dem2UserCreated;
using Dem2Model;

namespace Dem2Server
{
    
    class entityOperation
    {
        public char operation { get; set; }     //crud
        public ServerClientEntity entity { get; set; }

        public void sendTo(IWebSocketConnection socket) {
            socket.Send(JsonConvert.SerializeObject(this, new IsoDateTimeConverter()));
            if (operation == 'u' || operation == 'c')
            {
                var subs = new Subscription() { onEntityId = entity.Id };  //we presume, that the entity will be displayed at the client, so we subscribe him
                subs.subscribe(socket);
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
                         "entity": {"subjectID": "voting/215", "Agrees": true}
                       }
                     or subscription
                      {
                         "msgType": "entity",
                         "operation": "c",
                         "className": "Subscription",
                         "entity": {"onEntityId": "voting/215"}
                       }

                       // TODO implement create spam check here
                    */
                    {
                        try
                        {
                            var className = (string)receivedObj["className"];
                            Type type = Type.GetType("Dem2UserCreated." + className);
                            //object instance = Activator.CreateInstance(type, (Array)receivedObj["ctorArguments"]); old way, TODO test and remove this line
                            var instance = JsonConvert.DeserializeObject(receivedObj["entity"].ToString(), type, new IsoDateTimeConverter());
                            switch (className)
                            {
                                case "Vote":
                                    var theVote = (Vote)instance;   //the serialized entity must be initialized
                                    instance = Vote.Initialization(socket, theVote);    //the after initialization, we return the entity back
                                    break;
                                case "Subscription":
                                    var subs = (Subscription)instance;
                                    subs.subscribe(socket);
                                    break;
                                default:
                                    break;
                            }
                            entity = instance as ServerClientEntity;
                            //var op = new entityOperation() { operation = 'c', entity = instance as ServerClientEntity };
                            sendTo(socket);
                            Console.WriteLine("Object {0} created", instance.ToString());
                        }
                        catch (Exception)
                        {

                            throw;
                        }
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
                case 'u':   //read an entity
                    /*
                    Example shows json for this branch 
                    {
                      "msgType": "entity",
                      "operation": "r",
                      "entity":{
                          "Id": "user/132"
                      }
                    }*/
                    respondToUpdateRequest(socket);
                    break;
                case 'd': //delete an entity
                    /*
                    {
                        "msgType": "entity",
                        "operation": "d",
                        "className": "Subscription",
                        "entity": {"onEntityId": "voting/215"}
                    }
                      
                     */
                    {
                        var className = (string)receivedObj["className"];
                        Type type = Type.GetType("Dem2UserCreated." + className);

                        var instance = JsonConvert.DeserializeObject(receivedObj["entity"].ToString(), type, new IsoDateTimeConverter());
                        
                        if (className == "Subscription")
                        {
                            var subs = (Subscription)instance;
                            User.getUserFromSocket(socket).UnsubscribeFromEntity(subs);
                        }
                        else
                        {
                            var Id = (string)receivedObj["entity"]["Id"];
                            ServerClientEntity toDelete = EntityRepository.GetEntityFromSetsByID(Id);
                            var deletor = socket.ConnectionInfo.Cookies["user"];
                            if (deletor == toDelete.OwnerId)
                            {
                                var success = toDelete.Delete();
                                if (success)
                                {
                                    if (toDelete is Vote)
                                    {   // update the subject
                                        Vote aVote = (Vote)toDelete;
                                        var subject = EntityRepository.GetEntityFromSetsByID(aVote.subjectId);
                                        subject.IncrementVersion();
                                    }
                                    var op = new entityOperation() { operation = 'd', entity = new ServerClientEntity(toDelete.Id, toDelete.version) };
                                    Dem2Hub.sendItTo(op, socket);
                                }
                            }
                            else
                            {
                                var err = new ServerError() { message = "sorry, you can't delete the entity id " + Id };
                                Dem2Hub.sendItTo(err, socket);
                            }
                        }
                    }
                    break;
                default:
                    Console.WriteLine("Unknown type of request: {0}", operation);
                    break;
            }
        }

        public void respondToReadRequest(IWebSocketConnection socket)       //"r" operation, respond to it can be only "u" (update) or "n" (not found) 
        {
            Vote vote = null;
            ServerClientEntity foundEntity = EntityRepository.GetEntityFromSetsByID(entity.Id);

            if (foundEntity != null)
            {
                try
                {
                    if (foundEntity is VotableItem)    //check if the entity we are responding with is a VotableItem or not, props to http://www.hanselman.com/blog/DoesATypeImplementAnInterface.aspx
                    //if (typeof(VotableItem).IsAssignableFrom(entityOnServer.GetType()))    //check if the entity we are responding with is a VotableItem or not, props to http://www.hanselman.com/blog/DoesATypeImplementAnInterface.aspx
                    {
                        vote = EntityRepository.allVotes.FirstOrDefault(x => x.subjectId == entity.Id && x.OwnerId == socket.ConnectionInfo.Cookies["user"]);
                    }
                }
                catch (Exception ex)
                {
                    if (ExceptionIsCriticalCheck.IsCritical(ex)) throw;
                }

                if (entity.version < foundEntity.version)
                {
                    //first possible outcome
                    entity = foundEntity;    //so if the version on client is current by any chance we send back the same as we received
                }
                //second possible outcome
                operation = 'u';
                //client must check whether the update he received has greater version of entity, if not, he knows he has the latest version of entity
                entity = foundEntity;
            }
            else
            {
                //third possible outcome
                operation = 'n';        //not found, nonexistent, in the entity field there is still the one that was in the request, so when it arrives back to the client he will know which request ended up not found

            }
            sendTo(socket);

            if (vote != null)   //votable Item needs to be sent to client before the vote on a votable itself
            {
                entityOperation sendVoteOp = new entityOperation { entity = vote, operation = 'u' };
                sendVoteOp.sendTo(socket);
            }

        }

        private void respondToUpdateRequest(IWebSocketConnection socket)
        {
            ServerClientEntity foundEntity = EntityRepository.GetEntityFromSetsByID(entity.Id);

            if (foundEntity != null)
            {
                foundEntity = entity;
                foundEntity.IncrementVersion();
            }
        }

        
    }
}
