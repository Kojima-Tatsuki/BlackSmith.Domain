using Newtonsoft.Json;

namespace BlackSmith.Domain.Character
{
    /// <summary>プレイヤーや敵を含むすべてのキャラクターを一意に定める識別子</summary>
    public record CharacterID : BasicID
    {
        protected override string Prefix => "Character-";

        public CharacterID() : base() { }

        [JsonConstructor]
        public CharacterID(string value) : base(value) { }
    }
}