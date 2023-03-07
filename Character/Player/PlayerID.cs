using System;
using BlackSmith.Domain.Character;

namespace BlackSmith.Domain.Player
{
    public class PlayerID : CharacterID
    {
        public PlayerID(Guid id) : base(id)
        {
        }
    }
}