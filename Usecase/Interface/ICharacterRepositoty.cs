using BlackSmith.Domain.Character;
using BlackSmith.Domain.Character.NonPlayer;
using System.Collections.Generic;

#nullable enable

namespace BlackSmith.Repository.Interface
{
    public interface ICharacterRepositoty<T> where T : NonPlayerEntity
    {
        void Register(T character);

        void UpdateCharacter(T character);

        T? FindByID(CharacterID id);

        bool IsExist(CharacterID id);

        void Delete(CharacterID id);
    }

    public interface ICharacterRepositoty : ICharacterRepositoty<NonPlayerEntity>
    {
    }
}