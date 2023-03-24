using System;
using BlackSmith.Domain.Character.Player;

namespace BlackSmith.Domain.CharacterObject
{
    /// <summary>
    /// 防御力
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
                throw new ArgumentException($"防御力には1以上の値を入力してください, value : {value}");

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