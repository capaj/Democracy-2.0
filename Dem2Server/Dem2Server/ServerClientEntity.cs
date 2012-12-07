using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Dem2Model;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Raven.Abstractions.Commands;

namespace Dem2Server
{
    public class ServerClientEntity
    {
        public string Id    // Raven DB sets this property, it is set upon creation and it CANNOT be changed ever, under any circumstances
        {
            get;
            set;
        }

        public delegate void OnChangeHandler(ServerClientEntity o, EventArgs e);
        public event OnChangeHandler OnChange;

        public uint version { get; private set; }    // should get incremented everytime the Entity is updated/changed, on creation it is 1

        public void IncrementVersion() {
            version += 1;
            OnChange(this, new EventArgs());
        }

        [JsonIgnore]
        public ConcurrentDictionary<string,User> subscribedUsers { get; set; } // all the users that should get a newer version of the entity when entity is updated
        public bool Send() { return true; }

        public void Subscribe(User theUser)
        {
            subscribedUsers.TryAdd(theUser.Id, theUser);
        }

        public bool Unsubscribe(User theUser)
        {
            return subscribedUsers.TryRemove(theUser.Id, out theUser);        //returns false if the item is not found
        }

        void ServerClientEntity_OnChange(ServerClientEntity o, EventArgs e)
        {
            foreach (var subscriber in subscribedUsers)
            {
                var op = new entityOperation() { operation = 'u', entity = this };
                Dem2Hub.sendItTo(op, subscriber.Value.connection);
            }
        }

        private string _OwnerId;
	    public string OwnerId       // usually the creator
	    {
		    get { return _OwnerId;}
		    protected set { _OwnerId = value;}
	    }

        #region contructors
        [JsonConstructor]
        public ServerClientEntity() {
            subscribedUsers = new ConcurrentDictionary<string, User>();
            OnChange += ServerClientEntity_OnChange;
        }

        public ServerClientEntity(User creator)
        {
            subscribedUsers = new ConcurrentDictionary<string, User>();
            OnChange += ServerClientEntity_OnChange;
            _OwnerId = creator.Id;
        }
        #endregion

        public static ServerClientEntity GetEntityFromSetsByID(string Id){
            string type = Id.Split('/')[0];
            ServerClientEntity entityOnServer = null;
            try
            {
                entityOnServer = Dem2Hub.entityNamesToSets[type].FirstOrDefault(x => x.Id == Id);
            }
            catch (Exception ex)
            {
                if (ExceptionIsCriticalCheck.IsCritical(ex)) throw;
                Console.WriteLine("Entity with ID {0} was not found", Id);
                return null;
            }
            return entityOnServer;
        }

        public static bool DeleteEntityById(string Id, Type pok)
        {
            string typeStr = Id.Split('/')[0];
            try
            {
                var entityOnServer = Dem2Hub.entityNamesToSets[typeStr].FirstOrDefault(x => x.Id == Id);
                //Dem2Hub.entityNamesToDynamicSets[typeStr].RemoveWhere(x => x.); TODO fix
                using (var session = Dem2Hub.docDB.OpenSession())
                {
                    session.Advanced.Defer(new DeleteCommandData { Key = entityOnServer.Id });
                    session.SaveChanges();

                }
                //.Advanced.DocumentStore.DatabaseCommands.Delete("posts/1234", null);
            }
            catch (Exception ex)
            {
                if (ExceptionIsCriticalCheck.IsCritical(ex)) throw;
                Console.WriteLine("Entity with ID {0} was not found", Id);
                return false;
            }
            
            return true;
        }

        public override int GetHashCode()
        {
            if (Id != null)
            {
                return Id.GetHashCode();
            }
            else
            {
                return RuntimeHelpers.GetHashCode(this);
            }
            
        }

        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            ServerClientEntity second = obj as ServerClientEntity;
            if ((System.Object)second == null)
            {
                return false;
            }

            // Return true if the fields match:
            return Id == second.Id;
        }

        public bool Equals(ServerClientEntity second)
        {
            // If parameter is null return false:
            if ((object)second == null)
            {
                return false;
            }

            // Return true if the fields match:
            return Id == second.Id;
        }

    }
}
