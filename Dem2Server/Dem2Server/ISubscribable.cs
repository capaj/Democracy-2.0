using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Dem2Server
{
    interface ISubscribable
    {
        public ObservableCollection<string> subscribedUserIDs { get; set; }

        public void Subscribe(string userID)
        {
            subscribedUserIDs.Add(userID)
        }

        public bool Unsubscribe(string userID)
        {
            return subscribedUserIDs.Remove(userID)
        }
    }
}
