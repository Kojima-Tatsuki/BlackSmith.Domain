using System.Collections;
using System.Collections.Generic;
using BlackSmith.Domain.Character.Player;
using BlackSmith.Domain.CharacterObject;
using BlackSmith.Domain.CharacterObject.Interface;

namespace BlackSmith.Domain.Character.Interface
{
    public interface ICharacterEntity
    {
        PlayerID ID { get; }
        CharacterLevel Level { get; }

        PlayerName Name { get; }

        void ChangeName(PlayerName name);
    }
}
