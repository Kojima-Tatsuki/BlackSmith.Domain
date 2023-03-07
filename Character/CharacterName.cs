using System;

namespace BlackSmith.Domain.Character
{
    public class CharacterName
    {
        public string Value { get; }

        public CharacterName(string value)
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
