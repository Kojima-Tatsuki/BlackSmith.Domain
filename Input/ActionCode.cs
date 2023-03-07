using System;

#nullable enable

namespace BlackSmith.Domain.Input
{
    public class ActionCode
    {
        public ActionName Name { get; }

        public ActionCode(ActionName name)
        {
            if (name is null) throw new ArgumentNullException(nameof(name));

            this.Name = name;
        }
    }

    public class MoveActionCode
    {
        public ActionName Name { get; }
        public MoveDirection Direction { get; }

        public MoveActionCode(ActionName name, MoveDirection direction)
        {
            if (name is null) throw new ArgumentNullException(nameof(name));

            this.Name = name;
            this.Direction = direction;
        }
    }

    public class ActionName : IEquatable<ActionName>
    {
        public string Value { get; }

        public ActionName(string value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));
            if (value.Length < 3)
                throw new AggregateException("ActionNameは3文字以上です");

            Value = value;
        }

        public bool Equals(ActionName other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return string.Equals(this.Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return false;

            if (this.GetType() != obj.GetType()) return false;
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