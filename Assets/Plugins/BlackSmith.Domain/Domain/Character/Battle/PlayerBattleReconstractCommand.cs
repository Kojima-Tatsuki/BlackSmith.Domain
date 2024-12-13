using Newtonsoft.Json;

namespace BlackSmith.Domain.Character.Battle
{
    public record PlayerBattleReconstractCommand
    {
        public CharacterID Id { get; }
        public CharacterBattleModule Module { get; }

        [JsonConstructor]
        internal PlayerBattleReconstractCommand(CharacterID id, CharacterBattleModule module)
        {
            Id = id;
            Module = module;
        }
    }
}
