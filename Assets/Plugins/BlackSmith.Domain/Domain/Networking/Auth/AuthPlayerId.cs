using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;

namespace BlackSmith.Domain.Networking.Auth
{
    /// <summary>
    /// 全36文字の英数字
    /// </summary>
    public record AuthPlayerId
    {
        private const string Prefix = "AuthPId-";

        // PlayerIdはPrefixを含まない
        internal string PlayerId => Value.Replace(Prefix, "");

        /// <summary>
        /// Prefix + 28文字のランダムな英大小文字+数字
        /// </summary>
        public string Value { get; }

        [JsonConstructor]
        internal AuthPlayerId(string value)
        {
            if (!AuthPlayerIdValidator.IsValidate(value))
                throw new ArgumentException($"Invalid PlayerId. {value}");

            Value = value;
        }

        /// <summary>
        /// プレイヤーIDを生成します。
        /// </summary>
        /// <param name="rawPlayerId">プレイヤーID</param>
        /// <returns>生成されたAuthPlayerIdインスタンス</returns>
        public static AuthPlayerId CreateId(string rawPlayerId)
        {
            return new AuthPlayerId(Prefix + rawPlayerId);
        }

        public bool IsEqualRawPlayerId(string rawPlayerId)
        {
            return Value == CreateId(rawPlayerId).Value;
        }

        private class AuthPlayerIdValidator
        {
            /// <summary>
            /// 1文字以上28文字以下の英大小文字, 数字
            /// </summary>
            /// <param name="playerId"></param>
            /// <returns></returns>
            public static bool IsValidate(string playerId)
            {
                if (!IsValidatePlayerIdPrefix(playerId))
                    return false;

                var rawPlayerId = playerId.Replace(Prefix, "");
                return !string.IsNullOrEmpty(rawPlayerId)
                    && IsValidatePlayerIdLength(rawPlayerId)
                    && IsValidatePlayerIdCharacterType(rawPlayerId);
            }

            private static bool IsValidatePlayerIdLength(string playerId)
            {
                return playerId.Length == 28;
            }

            public static bool IsValidatePlayerIdCharacterType(string playerId)
            {
                return Regex.IsMatch(playerId, @"^[a-zA-Z0-9]+$");
            }

            private static bool IsValidatePlayerIdPrefix(string playerId)
            {
                return playerId.StartsWith(Prefix);
            }
        }

        public override string ToString() => Value;
    }
}