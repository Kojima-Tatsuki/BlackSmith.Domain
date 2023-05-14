using System;
using BlackSmith.Domain.Character.Player;

namespace BlackSmith.Domain.CharacterObject
{
    /// <summary>
    /// 攻撃力
    /// </summary>
    public class AttackValue
    {
        public int Value { get; }

        internal AttackValue(PlayerLevelDependentParameters levelParams)
        {
            Value = (levelParams.STR.Value + levelParams.AGI.Value) * 2;
        }

        internal AttackValue(int value)
        {
            if (!IsVaild(value))
                throw new ArgumentException($"攻撃力には1以上の値を入力してください, value : {value}");

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
