using System;

namespace BlackSmith.Domain
{
    public abstract class BasicID : IEquatable<BasicID>
    {
        internal Guid Value { get; }

        internal BasicID(Guid id)
        {
            Value = id;
        }

        public bool Equals(BasicID? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return Equals(Value.GetHashCode(), other.GetHashCode());
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;

            if (GetType() != obj.GetType()) return false;
            return Equals((BasicID)obj);
        }

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value.ToString();
    }

    public interface IDetailBaseValueObject<T> where T : notnull{
        T Value { get; }
    }

    public class BaseValueObject<T> : 
        IDetailBaseValueObject<T>, 
        IEquatable<BaseValueObject<T>> where T : notnull
    {
        T IDetailBaseValueObject<T>.Value => Value;
        protected T Value { get; }

        public BaseValueObject(T value)
        {
            Value = value;
        }

        public bool Equals(BaseValueObject<T>? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return Value.Equals(other.Value);
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;

            if (GetType() != obj.GetType()) return false;
            return Equals((BaseValueObject<T>)obj);
        }

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value.ToString() ?? "";
    }
}