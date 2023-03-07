using System;

namespace BlackSmith.Domain.Character
{
    /// <summary>プレイヤーや敵を含むすべてのキャラクターを一意に定める識別子</summary>
    public class CharacterID : BasicID
    {
        internal CharacterID(Guid id) : base(id)
        {
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}