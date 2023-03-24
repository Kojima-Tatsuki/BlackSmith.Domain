using System;
using BlackSmith.Domain.Character.Interface;

#nullable enable

namespace BlackSmith.Domain.CharacterObject
{
    /// <summary>
    /// ダメージ量を表すクラス
    /// </summary>
    public class DamageValue
    {
        /// <summary> レベル差関数用の補正値 </summary>
        private const double LevelGapCorrectionValue = 1.2599210498948731;

        internal int Value { get; }

        public DamageValue(LevelGapOfAttackerAndReceiver levelGap, AttackValue attack, DefenceValue defence)
        {
            var a = attack.Value;
            var d = defence.Value;

            // 3 レベル差で威力が約2倍になる
            // lev = LGCV ^ levelGap
            var lev = Math.Pow(LevelGapCorrectionValue, levelGap.GetGepFromAttackerToReceiver());

            // レベル差関数 * (attack^2) / (attack + deffence) * 補正値
            var damage = (int)Math.Floor(lev * a * a / (a + d)); // 切り捨てて計算

            Value = UnderClamp(damage);
        }

        /// <summary>
        /// [非推奨]
        /// </summary>
        /// <param name="value"></param>
        public DamageValue(int value)
        {
            Value = UnderClamp(value);
        }

        /// <summary>
        /// 1以上の値になるように値を返す
        /// </summary>
        /// <param name="value">範囲を固定したい値</param>
        /// <returns>固定された値</returns>
        private int UnderClamp(int value)
        {
            if (value <= 0) return 1;
            return value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    /// <summary>
    /// 攻めと守りのレベル差を表す
    /// </summary>
    public class LevelGapOfAttackerAndReceiver
    {
        private readonly ICharacterLevel receiver;

        private readonly ICharacterLevel attacker;

        /// <summary>
        /// インスタンス化を行う
        /// </summary>
        /// <param name="receiver">受け手のレベル</param>
        /// <param name="attacker">攻め手のレベル</param>
        public LevelGapOfAttackerAndReceiver(ICharacterLevel receiver, ICharacterLevel attacker)
        {
            this.receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
            this.attacker = attacker ?? throw new ArgumentNullException(nameof(attacker));
        }

        /// <summary>
        /// 攻め手から見た受け手とのレベル差を返す
        /// </summary>
        /// <returns>レベル差</returns>
        internal int GetGepFromAttackerToReceiver()
        {
            return receiver.Value - attacker.Value;
        }
    }
}