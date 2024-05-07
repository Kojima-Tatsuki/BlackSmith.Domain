using System;

namespace BlackSmith.Domain.Character
{
    /// <summary>プレイヤーや敵を含むすべてのキャラクターを一意に定める識別子</summary>
    public class CharacterID : BasicID
    {
        protected override string Prefix => "Character-";

        public CharacterID() : base() { }

        public CharacterID(string id) : base(id) { }
    }
}