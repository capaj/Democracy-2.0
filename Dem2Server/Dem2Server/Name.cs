using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Dem2Server;

namespace Dem2Model
{
    class Name:ServerClientEntity
    {
        private string _firstName;

        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        private string _lastName;

        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }

        [JsonIgnore]
        public string FullName
        {
            get { return _firstName + " " + _lastName; }
            private set {
                int whitespace = value.IndexOf(" ");
                string[] names = value.Split(value[whitespace]);
                _firstName = names[0];
                _lastName = names[1];   
            }
        }
        
        
        
    }
}
