using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Dem2Server
{
    public class ServerClientEntity
    {
        private string _Id;
        public string Id
        {
            get { return _Id; }
            set { _Id = value; }
        }
        
        public bool Send() { return true; }

        public ObservableCollection<string> subscribedUserIDs { get; set; }

        public void Subscribe(string userID)
        {
            subscribedUserIDs.Add(userID);
        }

        public bool Unsubscribe(string userID)
        {
            return subscribedUserIDs.Remove(userID);
        }

        private string _OwnerId;
	    public string OwnerId
	    {
		    get { return _OwnerId;}
		    protected set { _OwnerId = value;}
	    }

        #region contructors
        public ServerClientEntity()
        {

        }

        public ServerClientEntity(string author)
        {
            OwnerId = author;
        }
        #endregion


	
    }
}
