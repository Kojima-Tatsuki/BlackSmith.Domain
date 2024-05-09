using System;

#nullable enable

namespace BlackSmith.Domain.Currency
{
    /// <summary> お金 </summary>
    public class Currency
    {
        public CurrencyType Type { get; }

        public int Value => value.Value;
        private readonly CurrencyValue value;

        /// <summary>お金を生成する</summary>
        /// <remarks>通貨とその量を元に生成する</remarks>
        /// <param name="type">通貨</param>
        /// <param name="value">量</param>
        internal Currency(CurrencyType type, int value)
        {
            Type = type;
            this.value = new CurrencyValue(value);
        }

        internal Currency Add(Currency other)
        {
            if (!Type.Equals(other.Type))
                throw new ArgumentException($"通貨単位が一致していません [this: {Type}], [other: {other.Type}]. (56xTP2kw)");

            return new Currency(Type, value.Add(new CurrencyValue(other.Value)).Value);
        }

        internal Currency Subtract(Currency other)
        {
            if (!Type.Equals(other.Type))
                throw new ArgumentException($"通貨単位が一致していません [this: {Type}], [other: {other.Type}]. (XJwx0tXB)");

            var otherValue = new CurrencyValue(other.Value);

            return new Currency(Type, value.Subtract(otherValue).Value);
        }

        /// <summary>外貨両替を行う</summary>
        /// <remarks>端数は切り捨てられる</remarks>
        /// <param name="type">両替先の型</param>
        /// <param name="money">変換するお金</param>
        /// <returns></returns>
        internal Currency Exchange(CurrencyType type)
        {
            var value = (int)Math.Floor((float)type / (float)Type * Value);

            return new(type, value);
        }

        /// <summary>
        /// 通貨型が一致するかを返す
        /// </summary>
        /// <param name="other">対象の通貨</param>
        /// <returns>一致していれば真を返す</returns>
        public bool EqualsType(Currency other) => Type.Equals(other.Type);

        public bool Equals(Currency? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return Value.Equals(other.Value) && EqualsType(other);
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;

            if (GetType() != obj.GetType()) return false;
            return Equals((Currency)obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string? ToString()
        {
            return $"Type : {Type}, Value : {Value}";
        }

        public static bool operator ==(Currency x, Currency y) => x.Equals(y);

        public static bool operator !=(Currency x, Currency y) => !x.Equals(y);

        private class CurrencyValue
        {
            public int Value { get; }

            internal CurrencyValue(int value)
            {
                if (!IsValidValue(value))
                    throw new AggregateException($"通貨の値として不正な値が入力されました: value = {value}. (QOd6vrjA)");

                Value = value;
            }

            internal CurrencyValue Add(CurrencyValue other)
            {
                return new CurrencyValue(Value + other.Value);
            }

            internal CurrencyValue Subtract(CurrencyValue other)
            {
                var v = Value - other.Value;
                if (!IsValidValue(v))
                    v = 0;

                return new CurrencyValue(v);
            }

            /// <summary>
            /// 正当値かを調べる
            /// </summary>
            public static bool IsValidValue(int value)
            {
                if (value < 0) return false;
                return true;
            }

            public override string ToString()
            {
                return Value.ToString();
            }
        }
    }

    // これは、かなり問題を孕んだ実装方式。問題である。
    // 通貨の価値バランスを保持するドメインサービスを作成し、それに変換を依頼する構造にすべき
    /// <summary>
    /// 内部数が倍率を表す
    /// </summary>
    public enum CurrencyType
    {
        Sakura = 1, // 桜式通貨
        Aren = 2, // アレン式通貨
    }
}