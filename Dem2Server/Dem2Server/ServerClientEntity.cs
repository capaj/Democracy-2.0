using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Dem2Model;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace Dem2Server
{
    public class ServerClientEntity
    {
        public string Id    // Raven DB sets this property, it is set upon creation and it CANNOT be changed ever, under any circumstances
        {
            get;
            set;
        }
        public delegate void OnChangeHandler();
        public event OnChangeHandler OnChange;


        public uint version { get; set; }    // should get incremented everytime the Entity is updated/changed, on creation it is 1

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

        private string _OwnerId;
	    public string OwnerId       // usually the creator
	    {
		    get { return _OwnerId;}
		    protected set { _OwnerId = value;}
	    }

        #region contructors
        [JsonConstructor]
        public ServerClientEntity() { }

        public ServerClientEntity(User creator)
        {
            _OwnerId = creator.Id;
        }
        #endregion

        public override int GetHashCode()
        {
                return Id.GetHashCode();
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
