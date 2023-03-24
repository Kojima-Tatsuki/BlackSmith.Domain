using System;
using BlackSmith.Domain.Character;

namespace BlackSmith.Domain.Character.Player
{
    public class PlayerID : CharacterID
    {
        internal PlayerID(Guid id) : base(id)
        {
        }
    }
}