using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dem2Model
{
    public class FacebookAccount
    {
        public Uri AccAdress { get; private set; }
        public string accessToken { get; set; }  
        public string FacebookUserID { get; private set; }
    }
}
