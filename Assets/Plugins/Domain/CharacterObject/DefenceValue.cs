using System;
using BlackSmith.Domain.Character.Player;

namespace BlackSmith.Domain.CharacterObject
{
    /// <summary>
    /// –hŒä—Í
    /// </summary>
    public class DefenceValue
    {
        public int Value { get; }

        internal DefenceValue(PlayerLevelDepentdentParameters levelParams)
        {
            Value = (levelParams.STR.Value + levelParams.AGI.Value) * 2;
        }

        internal DefenceValue(int value)
        {
            if (!IsVaild(value))
                throw new ArgumentException($"–hŒä—Í‚É‚Í1ˆÈã‚Ì’l‚ğ“ü—Í‚µ‚Ä‚­‚¾‚³‚¢, value : {value}");

            Value = value;
        }

        private bool IsVaild(int value)
        {
            if (value <= 0)
                return false;

            return true;
        }
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}