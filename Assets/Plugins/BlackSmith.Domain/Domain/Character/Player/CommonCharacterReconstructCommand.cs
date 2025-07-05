using Newtonsoft.Json;

namespace BlackSmith.Domain.Character.Player
{
    /// <summary>キャラクターの再構築を行う際に引数に指定して使う</summary>
    public record CommonCharacterReconstructCommand
    {
        public CharacterID Id { get; }
        public CharacterName Name { get; }
        public CharacterLevel Level { get; }

        [JsonConstructor]
        internal CommonCharacterReconstructCommand(CharacterID id, CharacterName name, CharacterLevel level)
        {
            Id = id;
            Name = name;
            Level = level;
        }

        public CommonCharacterReconstructCommand(string id, string name, int exp)
        {
            Id = new CharacterID(id);
            Name = new CharacterName(name);
            Level = new CharacterLevel(new Experience(exp));
        }
    }
}