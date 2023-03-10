using System;
using BlackSmith.Domain.Character;

namespace BlackSmith.Domain.Player
{
    public class PlayerID : CharacterID
    {
        internal PlayerID(Guid id) : base(id)
        {
        }
    }
}