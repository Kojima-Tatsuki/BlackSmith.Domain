using NUnit.Framework;
using System;

#nullable enable

namespace BlackSmith.Domain.Networking.Auth
{
    internal class AuthPlayerIdTest
    {
        #region 正常系テスト

        [Test]
        public void CreateId_ValidRawPlayerId_ShouldCreateAuthPlayerId()
        {
            // Arrange
            var rawPlayerId = "abcdefghijklmnopqrstuvwxyz12"; // 28文字の英数字

            // Act
            var authPlayerId = AuthPlayerId.CreateId(rawPlayerId);

            // Assert
            Assert.That(authPlayerId.Value, Is.EqualTo("AuthPId-abcdefghijklmnopqrstuvwxyz12"));
        }

        [Test]
        public void CreateId_MixedCaseAlphanumeric_ShouldCreateAuthPlayerId()
        {
            // Arrange
            var rawPlayerId = "AbCdEfGhIjKlMnOpQrStUvWxYz12"; // 28文字の大小文字混合英数字

            // Act
            var authPlayerId = AuthPlayerId.CreateId(rawPlayerId);

            // Assert
            Assert.That(authPlayerId.Value, Is.EqualTo("AuthPId-AbCdEfGhIjKlMnOpQrStUvWxYz12"));
        }

        [Test]
        public void CreateId_AllNumbers_ShouldCreateAuthPlayerId()
        {
            // Arrange
            var rawPlayerId = "1234567890123456789012345678"; // 28文字の数字のみ

            // Act
            var authPlayerId = AuthPlayerId.CreateId(rawPlayerId);

            // Assert
            Assert.That(authPlayerId.Value, Is.EqualTo("AuthPId-1234567890123456789012345678"));
        }

        [Test]
        public void IsEqualRawPlayerId_SameRawPlayerId_ShouldReturnTrue()
        {
            // Arrange
            var rawPlayerId = "abcdefghijklmnopqrstuvwxyz12";
            var authPlayerId = AuthPlayerId.CreateId(rawPlayerId);

            // Act & Assert
            Assert.That(authPlayerId.IsEqualRawPlayerId(rawPlayerId), Is.True);
        }

        [Test]
        public void IsEqualRawPlayerId_DifferentRawPlayerId_ShouldReturnFalse()
        {
            // Arrange
            var rawPlayerId1 = "abcdefghijklmnopqrstuvwxyz12";
            var rawPlayerId2 = "differentrawplayeridstring12";
            var authPlayerId = AuthPlayerId.CreateId(rawPlayerId1);

            // Act & Assert
            Assert.That(authPlayerId.IsEqualRawPlayerId(rawPlayerId2), Is.False);
        }

        [Test]
        public void ToString_ShouldReturnValue()
        {
            // Arrange
            var rawPlayerId = "abcdefghijklmnopqrstuvwxyz12";
            var authPlayerId = AuthPlayerId.CreateId(rawPlayerId);

            // Act
            var result = authPlayerId.ToString();

            // Assert
            Assert.That(result, Is.EqualTo("AuthPId-abcdefghijklmnopqrstuvwxyz12"));
        }

        #endregion

        #region 異常系テスト

        [Test]
        public void CreateId_NullRawPlayerId_ShouldThrowArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => AuthPlayerId.CreateId(null!));
        }

        [Test]
        public void CreateId_EmptyRawPlayerId_ShouldThrowArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => AuthPlayerId.CreateId(""));
        }

        [Test]
        public void CreateId_TooShortRawPlayerId_ShouldThrowArgumentException()
        {
            // Arrange - 27文字（1文字不足）
            var rawPlayerId = "abcdefghijklmnopqrstuvwxyz1";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => AuthPlayerId.CreateId(rawPlayerId));
        }

        [Test]
        public void CreateId_TooLongRawPlayerId_ShouldThrowArgumentException()
        {
            // Arrange - 29文字（1文字超過）
            var rawPlayerId = "abcdefghijklmnopqrstuvwxyz123";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => AuthPlayerId.CreateId(rawPlayerId));
        }

        [Test]
        public void CreateId_InvalidCharacters_ShouldThrowArgumentException()
        {
            // Arrange - 特殊文字を含む28文字
            var rawPlayerId = "abcdefghijklmnopqrstuvwxy@1";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => AuthPlayerId.CreateId(rawPlayerId));
        }

        [Test]
        public void CreateId_WithSpaces_ShouldThrowArgumentException()
        {
            // Arrange - スペースを含む28文字
            var rawPlayerId = "abcdefghijklmnopqrstuvwxy 1";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => AuthPlayerId.CreateId(rawPlayerId));
        }

        [Test]
        public void CreateId_WithHyphen_ShouldThrowArgumentException()
        {
            // Arrange - ハイフンを含む28文字
            var rawPlayerId = "abcdefghijklmnopqrstuvwxy-1";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => AuthPlayerId.CreateId(rawPlayerId));
        }

        #endregion

        #region 境界値テスト

        [Test]
        public void CreateId_ExactLength28_ShouldSuccess()
        {
            // Arrange - 正確に28文字
            var rawPlayerId = "abcdefghijklmnopqrstuvwxyz12";

            // Act
            var authPlayerId = AuthPlayerId.CreateId(rawPlayerId);

            // Assert
            Assert.That(authPlayerId.Value, Is.EqualTo("AuthPId-abcdefghijklmnopqrstuvwxyz12"));
        }

        [Test]
        public void CreateId_AllLowerCase_ShouldSuccess()
        {
            // Arrange - 全て小文字
            var rawPlayerId = "abcdefghijklmnopqrstuvwxyz12";

            // Act
            var authPlayerId = AuthPlayerId.CreateId(rawPlayerId);

            // Assert
            Assert.That(authPlayerId.Value, Contains.Substring(rawPlayerId));
        }

        [Test]
        public void CreateId_AllUpperCase_ShouldSuccess()
        {
            // Arrange - 全て大文字
            var rawPlayerId = "ABCDEFGHIJKLMNOPQRSTUVWXYZ12";

            // Act
            var authPlayerId = AuthPlayerId.CreateId(rawPlayerId);

            // Assert
            Assert.That(authPlayerId.Value, Contains.Substring(rawPlayerId));
        }

        #endregion
    }
}