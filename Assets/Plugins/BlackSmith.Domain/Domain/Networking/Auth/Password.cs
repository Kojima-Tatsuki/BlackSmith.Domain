using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BlackSmith.Domain.Networking.Auth
{
    public record Password
    {
        public string Value { get; }

        [JsonConstructor]
        public Password(string value)
        {
            if (!Validator.IsValidPassword(value))
                throw new ArgumentException($"Passwordの条件を満たしていません. {value}");
            Value = value;
        }

        // Passwordの条件
        // 8 - 30 文字
        // 小文字が1文字以上
        // 大文字が1文字以上
        // 数字が1文字以上
        // 特殊文字が1文字以上
        // ただし、独自の条件として、特殊文字を以下の32文字に限定する
        // `~!@#$%^&*()_+-={}[]\|:;"'<>,.?/
        public class Validator
        {
            // Unity Auth ServiceのUserNameとPasswordの条件を満たしているかを確認する [最終閲覧: 2024/04/13]
            // https://services.docs.unity.com/docs/client-auth/#username-and-password-restrictions

            public enum ValidationError
            {
                InvalidLength,
                InvalidCharacter,
                MissingLowercase,
                MissingUppercase,
                MissingNumber,
                MissingSpecialCharacter
            }

            public static bool IsValidPassword(string password) => ValidatePassword(password).Count == 0;

            public static IReadOnlyList<ValidationError> ValidatePassword(string password)
            {
                var errors = new List<ValidationError>();
                if (string.IsNullOrEmpty(password) || !IsValidPasswordOfCharacterLength(password))
                    errors.Add(ValidationError.InvalidLength);
                if (!IsValidPasswordCharacterTypeOfLower(password))
                    errors.Add(ValidationError.MissingLowercase);
                if (!IsValidPasswordCharacterTypeOfUpper(password))
                    errors.Add(ValidationError.MissingUppercase);
                if (!IsValidPasswordCharacterTypeOfNumber(password))
                    errors.Add(ValidationError.MissingNumber);
                if (!IsValidPasswordCharacterTypeOfSpecial(password))
                    errors.Add(ValidationError.MissingSpecialCharacter);
                if (!IsValidPasswordCharacterType(password))
                    errors.Add(ValidationError.MissingSpecialCharacter);

                return errors;
            }

            private static bool IsValidPasswordOfCharacterLength(string password)
            {
                return password.Length >= 8 && password.Length <= 30;
            }

            private static bool IsValidPasswordCharacterTypeOfLower(string password)
            {
                return System.Text.RegularExpressions.Regex.IsMatch(password, @"[a-z]");
            }

            private static bool IsValidPasswordCharacterTypeOfUpper(string password)
            {
                return System.Text.RegularExpressions.Regex.IsMatch(password, @"[A-Z]");
            }

            private static bool IsValidPasswordCharacterTypeOfNumber(string password)
            {
                return System.Text.RegularExpressions.Regex.IsMatch(password, @"\d");
            }

            private static bool IsValidPasswordCharacterTypeOfSpecial(string password)
            {
                return System.Text.RegularExpressions.Regex.IsMatch(password, @"[`~!@#$%^&*()_+\-={}[\]\\|:;""'<>,.?/]");
            }

            public static bool IsValidPasswordCharacterType(string password)
            {
                return System.Text.RegularExpressions.Regex.IsMatch(password, @"^[a-zA-Z0-9`~!@#$%^&*()_+\-={}[\]\\|:;""'<>,.?/]+$");
            }
        }
    }
}