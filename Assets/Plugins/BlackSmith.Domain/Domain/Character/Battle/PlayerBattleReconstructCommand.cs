using Newtonsoft.Json;

namespace BlackSmith.Domain.Character.Battle
{
    public record PlayerBattleReconstructCommand
    {
        public CharacterID Id { get; }
        public CharacterBattleModule Module { get; }

        [JsonConstructor]
        internal PlayerBattleReconstructCommand(CharacterID id, CharacterBattleModule module)
        {
            Id = id;
            Module = module;
        }
    }
}
