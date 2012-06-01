using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dem2Model
{
    class User: ServerClientEntity
    {
        public Name CivicName { get; set; }
        public DateTime BirthTime { get; private set; }
        public FacebookAccount FBAccount { get; set; }
    }
}
