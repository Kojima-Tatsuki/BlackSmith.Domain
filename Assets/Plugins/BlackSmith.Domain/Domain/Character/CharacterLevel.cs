using System;
using BlackSmith.Domain.Character.Interface;

namespace BlackSmith.Domain.Character
{
    // Expを利用しないため、レベルに変動を起こさせない
    public class CharacterLevel : ICharacterLevel
    {
        int ICharacterLevel.Value => Value;
        public int Value { get; }

        internal CharacterLevel(int level)
        {
            if (!IsVaild(level))
                throw new ArgumentException($"0以下の値はレベルとして扱えません, value : {level}");

            Value = level;
        }

        private bool IsVaild(int value)
        {
            if (value <= 0)
                return false;

            return true;
        }
    }
}