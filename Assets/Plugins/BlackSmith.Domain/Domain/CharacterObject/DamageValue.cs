using System;
using BlackSmith.Domain.Character;
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

        public int Value { get; }

        internal DamageValue(LevelGapOfAttackerAndReceiver levelGap, AttackValue attack, DefenseValue defence)
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
        internal DamageValue(int value)
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
    internal class LevelGapOfAttackerAndReceiver
    {
        private readonly CharacterLevel receiver;

        private readonly CharacterLevel attacker;

        /// <summary>
        /// インスタンス化を行う
        /// </summary>
        /// <param name="receiver">受け手のレベル</param>
        /// <param name="attacker">攻め手のレベル</param>
        internal LevelGapOfAttackerAndReceiver(CharacterLevel receiver, CharacterLevel attacker)
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