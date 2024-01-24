using BlackSmith.Domain.Character.Player;

namespace BlackSmith.Domain.Character.Interface
{
    internal interface ICharacterEntity
    {
        CharacterID ID { get; }
        CharacterLevel Level { get; }

        PlayerName Name { get; }

        void ChangeName(PlayerName name);
    }
}
