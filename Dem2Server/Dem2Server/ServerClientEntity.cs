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

        public void IncrementVersion() {    //this should be called whenever mutation of this entity occurs
            version += 1;
            OnChange(this, new EventArgs());
        }

        [JsonIgnore]
        public ConcurrentDictionary<string,User> subscribedUsers { get; set; } // all the users that should get a newer version of the entity when entity is updated
        [JsonIgnore]
        private bool inDeletionPhase = false;

        public bool Delete()
        {
            inDeletionPhase = true;
            foreach (var user in subscribedUsers)
            {
                var userSubs = user.Value.subscriptions.Single(x => x.onEntityId == Id);
                if (userSubs != null)
                {
                    user.Value.subscriptions.Remove(userSubs);
                }
            }
            return EntityRepository.Remove(this);
        }

        public bool Subscribe(User theUser)
        {
            if (inDeletionPhase == false)
            {
                return subscribedUsers.TryAdd(theUser.Id, theUser);
            }
            return false;
        }

        public virtual bool Unsubscribe(User theUser)
        {
            if (inDeletionPhase == false)
            {
                return subscribedUsers.TryRemove(theUser.Id, out theUser);        //returns false if the item is not found
            }
            return false;
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
		    set { _OwnerId = value;}
	    }

        #region contructors
        public ServerClientEntity(string cId, uint ver)
        {
            version = ver;
            Id = cId;
            subscribedUsers = new ConcurrentDictionary<string, User>();
            OnChange += ServerClientEntity_OnChange;
        }
        
        
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
