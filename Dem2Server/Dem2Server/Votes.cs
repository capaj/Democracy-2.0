using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dem2Model
{
    class Votes:ServerClientEntity
    {
        public User Caster { get; set; }
        public bool Agrees { get; set; }
    }
}
