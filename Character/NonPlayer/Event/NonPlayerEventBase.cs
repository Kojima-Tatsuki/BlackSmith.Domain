using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackSmith.Domain.Character.NonPlayer.Event
{
    public abstract class NonPlayerEventBase
    {
        public CharacterID ID { get; }

        public DateTime DateTime { get; }

        internal NonPlayerEventBase(CharacterID id)
        {
            ID = id;
            DateTime = new DateTime();
        }
    }
}
