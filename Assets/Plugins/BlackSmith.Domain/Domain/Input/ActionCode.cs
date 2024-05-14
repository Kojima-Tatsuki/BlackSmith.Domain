using System;

#nullable enable

namespace BlackSmith.Domain.Input
{
    public class ActionCode
    {
        public ActionName Name { get; }

        internal ActionCode(ActionName name)
        {
            Name = name ?? throw new ArgumentNullException("Not found ActionName. (MN0FluVx)");
        }
    }

    public class MoveActionCode
    {
        public ActionName Name { get; }
        public MoveDirection Direction { get; }

        internal MoveActionCode(ActionName name, MoveDirection direction)
        {
            Name = name ?? throw new ArgumentNullException("Not found ActionName. (rzTfsDFD6)");
            Direction = direction;
        }
    }

    public class ActionName : IEquatable<ActionName>
    {
        public string Value { get; }

        internal ActionName(string value)
        {
            if (value is null) throw new ArgumentNullException("Not found actionName guid. (ASsKMc9g)");
            if (value.Length < 3)
                throw new AggregateException("ActionNameは3文字以上です. (TtoFW9gK)");

            Value = value;
        }

        public bool Equals(ActionName? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return string.Equals(Value, other.Value);
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return false;

            if (GetType() != obj.GetType()) return false;
            return Equals((ActionName)obj);
        }

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value;
    }

    public enum MoveDirection
    {
        None = 5,
        DownLeft = 1,
        Down,
        DownRight,
        Left,
        Right = 6,
        UpLeft,
        Up,
        UpRight,
    }
}