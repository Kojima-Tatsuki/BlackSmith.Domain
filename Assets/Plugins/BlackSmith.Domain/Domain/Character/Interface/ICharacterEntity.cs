using BlackSmith.Domain.Character.Player;

namespace BlackSmith.Domain.Character.Interface
{
    internal interface ICharacterEntity
    {
        CharacterID ID { get; }

        CharacterLevel Level { get; }

        CharacterName Name { get; }

        void ChangeName(CharacterName name);
    }
}
