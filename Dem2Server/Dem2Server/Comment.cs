using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dem2Server
{
    class Comment:ServerClientEntity
    {
        public DateTime published { get; set; }
        public string text { get; set; }
    }
}
