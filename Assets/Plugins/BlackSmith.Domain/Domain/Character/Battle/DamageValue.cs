using System;

#nullable enable

namespace BlackSmith.Domain.Character.Battle
{
    /// <summary>
    /// ダメージ量を表すクラス
    /// </summary>
    public record DamageValue
    {
        /// <summary> レベル差関数用の補正値 </summary>
        private const double LevelGapCorrection = 1.2599210498948731;

        public int Value { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="levelGap"></param>
        /// <param name="attackerAttack">攻め手の攻撃力</param>
        /// <param name="recieverDefence">受け手の防御力</param>
        internal DamageValue(LevelGapOfAttackerAndReceiver levelGap, AttackValue attackerAttack, DefenseValue recieverDefence)
        {
            var a = attackerAttack.Value;
            var d = recieverDefence.Value;

            // 3 レベル差で威力が約2倍になる
            // lev = LGCV ^ levelGap
            var lev = Math.Pow(LevelGapCorrection, levelGap.GetGepFromAttackerToReceiver());

            // レベル差関数 * (attackerAttack^2) / (attackerAttack + recieverDefence) * 補正値
            var damage = (int)Math.Floor(lev * a * a / (a + d)); // 切り捨てて計算

            Value = UnderClamp(damage);
        }

        /// <summary>
        /// [非推奨] 固定ダメージを与える場合に使用する
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
    internal record LevelGapOfAttackerAndReceiver
    {
        private readonly CharacterLevel attacker;
        private readonly CharacterLevel receiver;

        /// <summary>
        /// インスタンス化を行う
        /// </summary>
        /// <param name="receiver">受け手のレベル</param>
        /// <param name="attacker">攻め手のレベル</param>
        internal LevelGapOfAttackerAndReceiver(CharacterLevel attacker, CharacterLevel receiver)
        {
            this.attacker = attacker ?? throw new ArgumentNullException(nameof(attacker));
            this.receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
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