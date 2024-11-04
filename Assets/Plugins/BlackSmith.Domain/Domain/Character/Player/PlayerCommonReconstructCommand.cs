using Newtonsoft.Json;

namespace BlackSmith.Domain.Character.Player
{
    /// <summary>プレイヤーの再構築を行う際に引数に指定して使う</summary>
    public record PlayerCommonReconstructCommand
    {
        public CharacterID Id { get; }
        public PlayerName Name { get; }
        public PlayerLevel Level { get; }

        [JsonConstructor]
        internal PlayerCommonReconstructCommand(CharacterID id, PlayerName name, PlayerLevel level)
        {
            Id = id;
            Name = name;
            Level = level;
        }

        public PlayerCommonReconstructCommand(string id, string name, int exp)
        {
            Id = new CharacterID(id);
            Name = new PlayerName(name);
            Level = new PlayerLevel(new Experience(exp));
        }
    }
}