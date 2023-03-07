using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackSmith.Domain.CharacterObjects;

namespace BlackSmith.Domain.Character.NonPlayer.Event
{
    public class NonPlayerHealthChangedEvent : NonPlayerEventBase
    {
        public HealthPoint HealthPoint { get; }

        internal NonPlayerHealthChangedEvent(CharacterID id, HealthPoint health) : base(id)
        {
            HealthPoint = health;
        }
    }
}
