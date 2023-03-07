using System;

#nullable enable

namespace BlackSmith.Domain.Player
{
    public class PlayerName
    {
        public string Value { get; }

        public PlayerName(string value)
        {
            if (!IsValid(value))
                throw new ArgumentException("名前は1文字以上でなければなりません");

            Value = value;
        }

        public static bool IsValid(string value)
        {
            if (value is null) return false;
            if (value.Length <= 0) return false;

            return true;
        }

        public override string ToString() => Value;
    }
}