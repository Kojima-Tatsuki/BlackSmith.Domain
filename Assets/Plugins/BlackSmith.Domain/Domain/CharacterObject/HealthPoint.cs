﻿using BlackSmith.Domain.Character;
using System;

namespace BlackSmith.Domain.CharacterObject
{
    /// <summary>
    /// =Value= This class is used to represent HP.
    /// </summary>
    public record HealthPoint
    {
        internal HealthPointValue Value { get; }

        internal MaxHealthPointValue MaximumValue { get; }

        /// <summary>
        /// 現在値と最大値を指定してインスタンス化する
        /// </summary>
        /// <param name="value">現在値</param>
        /// <param name="max">最大値</param>
        internal HealthPoint(HealthPointValue value, MaxHealthPointValue max)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));
            if (max is null) throw new ArgumentNullException(nameof(max));

            if (!IsValidValue(value, max))
                throw new ArgumentException($"不正な値が引数として渡されました, [value, max] : [{value}, {max}]");

            Value = value;
            MaximumValue = max;
        }

        /// <summary>
        /// 最大値を指定してインスタンス化する
        /// </summary>
        /// <param name="max">最大値</param>
        internal HealthPoint(MaxHealthPointValue max)
        {
            var maxValue = max ?? throw new ArgumentNullException(nameof(max));
            var value = new HealthPointValue(maxValue.Value); // 現在値の初期値を最大値と定める

            if (!IsValidValue(value, max))
                throw new ArgumentException($"不正な値が引数として渡されました, [value, max] : [{value}, {max}]");

            Value = value;
            MaximumValue = max;
        }

        /// <summary>
        /// 最大値をレベルから算出してインスタンス化を行う
        /// </summary>
        /// <param name="level">算出に用いるレベル</param>
        internal HealthPoint(CharacterLevel level)
        {
            if (level is null) throw new ArgumentNullException(nameof(level));

            // 攻撃力の6倍の体力を作りたい
            var maxValue = level.Value * 10;

            Value = new HealthPointValue(maxValue);
            MaximumValue = new MaxHealthPointValue(maxValue);
        }

        /// <summary>
        /// ダメージを与えた後の体力を返す
        /// </summary>
        /// <remarks>体力は最小値を下回らない</remarks>
        /// <param name="damage">与えるダメージ</param>
        /// <returns>計算後の体力</returns>
        internal HealthPoint TakeDamage(DamageValue damage)
        {
            if (damage is null) throw new ArgumentNullException(nameof(damage));

            var hp = Value.Value - damage.Value;

            if (hp < 0)
                hp = 0; // HPが0を下回らないようにする

            return new HealthPoint(new HealthPointValue(hp), MaximumValue);
        }

        internal HealthPoint HealHealth(int heal)
        {
            if (heal < 0)
                throw new ArgumentOutOfRangeException(nameof(heal));

            var hp = Value.Value + heal;

            if (hp > MaximumValue.Value)
                return new HealthPoint(MaximumValue);

            return new HealthPoint(new HealthPointValue(hp), MaximumValue);
        }

        private bool IsValidValue(HealthPointValue value, MaxHealthPointValue max)
        {
            if (max.Value < 0)
                return false; // 最小値が最大値を上回っています
            if (value.Value < 0 || max < value)
                return false; // 値が最低値もしくは最大値の範囲外です
            return true;
        }

        /// <summary>
        /// 体力が残っているかを返す
        /// </summary>
        /// <returns>以下であれば真を返す</returns>
        internal bool IsDead()
        {
            if (Value.Value <= 0)
                return true;

            return false;
        }

        public override string ToString()
        {
            return $"{Value} / {MaximumValue}";
        }

        /// <summary>
        /// 体力の現在値、最大値を返す
        /// </summary>
        /// <returns>itemの数字の若い方から、現在値、最大値</returns>
        public (int current, int max) GetValues()
        {
            return (Value.Value, MaximumValue.Value);
        }
    }

    internal record HealthPointValue
    {
        internal int Value { get; }

        public HealthPointValue(int value)
        {
            if (!IsInvalid(value))
                throw new ArgumentException(nameof(value));

            Value = value;
        }

        private bool IsInvalid(int value)
        {
            if (value < 0)
                return false;
            return true;
        }

        public static bool operator <(HealthPointValue x, HealthPointValue y)
            => x.Value < y.Value;

        public static bool operator <=(HealthPointValue x, HealthPointValue y)
            => x.Value <= y.Value;

        public static bool operator >(HealthPointValue x, HealthPointValue y)
            => x.Value > y.Value;

        public static bool operator >=(HealthPointValue x, HealthPointValue y)
            => x.Value >= y.Value;

        public override string ToString() => Value.ToString();
    }

    internal record MaxHealthPointValue : HealthPointValue
    {
        public MaxHealthPointValue(int value) : base(value) { }

        /// <summary>
        /// 指定レベル時点での最大体力を返す
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        internal static MaxHealthPointValue GetMaxHealthFromPlayerLevel(CharacterLevel level)
        {
            var value = level.Value * 3 * 20;

            return new MaxHealthPointValue(value);
        }
    }
}