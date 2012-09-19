using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Dem2Model;

namespace Dem2Server
{
    public abstract class ServerClientEntity
    {
        private string _Id;
        public string Id    // Raven DB sets this property
        {
            get { return _Id; }
            set { _Id = value; }
        }

        public uint version { get; set; }    // should get incremented everytime the Entity is updated/changed, on creation it is 1

        public Collection<User> subscribedUsers { get; set; } // all the users that should get a newer version of the entity when entity is updated
        public bool Send() { return true; }

        public void Subscribe(User theUser)
        {
            subscribedUsers.Add(theUser);
        }

        public bool Unsubscribe(User theUser)
        {
            return subscribedUsers.Remove(theUser);        //returns false if the item is not found
        }

        private string _OwnerId;
	    public string OwnerId
	    {
		    get { return _OwnerId;}
		    protected set { _OwnerId = value;}
	    }

        #region contructors
        protected ServerClientEntity()
        {

        }

        public ServerClientEntity(User creator)
        {
            _OwnerId = creator.Id;
        }
        #endregion


	
    }
}
