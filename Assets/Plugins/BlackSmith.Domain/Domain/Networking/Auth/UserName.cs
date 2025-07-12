using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BlackSmith.Domain.Networking.Auth
{
    public sealed record UserName : IEquatable<UserName>
    {
        // 表示の際には大文字に変換する
        public string Value { get; }

        [JsonConstructor]
        public UserName(string value)
        {
            if (!Validator.IsValidUserName(value))
                throw new ArgumentException($"UserNameの条件を満たしていません. {value}");
            Value = value.ToUpper(); // 入力はすべて大文字に変換する
        }

        // 大文字と小文字は区別されない
        public bool Equals(UserName other)
        {
            if (other is null) return false;
            return string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        // UserNameの条件
        // 3 - 20 文字
        // a-z, 0-9 に加えて, [. - @ _]の4種類の文字のみ
        // 大文字、小文字は区別されない
        public class Validator
        {
            // Unity Auth ServiceのUserNameとPasswordの条件を満たしているかを確認する [最終閲覧: 2024/04/13]
            // https://services.docs.unity.com/docs/client-auth/#username-and-password-restrictions

            public enum ValidationError
            {
                InvalidLength,
                InvalidCharacterType
            }

            public static bool IsValidUserName(string name) => ValidateUserName(name).Count == 0;

            public static IReadOnlyList<ValidationError> ValidateUserName(string name)
            {
                var errors = new List<ValidationError>();
                
                // Early return for null or empty name with only InvalidLength error
                if (string.IsNullOrEmpty(name))
                {
                    errors.Add(ValidationError.InvalidLength);
                    return errors;
                }
                
                if (!IsValidUserNameOfCharacterLength(name))
                    errors.Add(ValidationError.InvalidLength);
                if (!IsValidUserNameCharacterType(name))
                    errors.Add(ValidationError.InvalidCharacterType);

                return errors;
            }

            private static bool IsValidUserNameOfCharacterLength(string name)
            {
                return name.Length >= 3 && name.Length <= 20;
            }

            private static bool IsValidUserNameCharacterType(string name)
            {
                return System.Text.RegularExpressions.Regex.IsMatch(name, @"^[a-zA-Z0-9.\-@_]+$");
            }
        }
    }
}