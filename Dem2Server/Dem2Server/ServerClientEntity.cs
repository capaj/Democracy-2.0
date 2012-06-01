using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dem2Model
{
    class ServerClientEntity
    {
        private string _Id;

        public string Id
        {
            get { return _Id; }
            set { _Id = value; }
        }
        
        public bool Send() { return true; }
    }
}
