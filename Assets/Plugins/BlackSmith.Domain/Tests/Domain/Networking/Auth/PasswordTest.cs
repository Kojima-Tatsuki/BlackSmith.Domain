using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace BlackSmith.Domain.Networking.Auth
{
    internal class PasswordTest
    {
        #region 正常系テスト

        [Test]
        [TestCase("Password1@")]
        [TestCase("MySecureP@ss123")]
        [TestCase("Test123!@#$%^&*()")]
        public void Constructor_ValidPassword_ShouldSuccess(string validPassword)
        {
            // Act & Assert
            Assert.DoesNotThrow(() => new Password(validPassword));
        }

        #endregion

        #region バリデーションエラー個別テスト

        [Test]
        public void ValidatePassword_TooShort_ShouldReturnInvalidLength()
        {
            // Arrange
            var shortPassword = "Aa1@"; // 4文字

            // Act
            var errors = Password.Validator.ValidatePassword(shortPassword);

            // Assert
            Assert.That(errors, Contains.Item(Password.Validator.ValidationError.InvalidLength));
        }

        [Test]
        public void ValidatePassword_TooLong_ShouldReturnInvalidLength()
        {
            // Arrange
            var longPassword = "A".PadRight(31, 'a') + "1@"; // 31文字以上

            // Act
            var errors = Password.Validator.ValidatePassword(longPassword);

            // Assert
            Assert.That(errors, Contains.Item(Password.Validator.ValidationError.InvalidLength));
        }

        [Test]
        public void ValidatePassword_NoLowercase_ShouldReturnMissingLowercase()
        {
            // Arrange
            var password = "PASSWORD123@";

            // Act
            var errors = Password.Validator.ValidatePassword(password);

            // Assert
            Assert.That(errors, Contains.Item(Password.Validator.ValidationError.MissingLowercase));
        }

        [Test]
        public void ValidatePassword_NoUppercase_ShouldReturnMissingUppercase()
        {
            // Arrange
            var password = "password123@";

            // Act
            var errors = Password.Validator.ValidatePassword(password);

            // Assert
            Assert.That(errors, Contains.Item(Password.Validator.ValidationError.MissingUppercase));
        }

        [Test]
        public void ValidatePassword_NoNumber_ShouldReturnMissingNumber()
        {
            // Arrange
            var password = "Password@";

            // Act
            var errors = Password.Validator.ValidatePassword(password);

            // Assert
            Assert.That(errors, Contains.Item(Password.Validator.ValidationError.MissingNumber));
        }

        [Test]
        public void ValidatePassword_NoSpecialCharacter_ShouldReturnMissingSpecialCharacter()
        {
            // Arrange
            var password = "Password123";

            // Act
            var errors = Password.Validator.ValidatePassword(password);

            // Assert
            Assert.That(errors, Contains.Item(Password.Validator.ValidationError.MissingSpecialCharacter));
        }

        #endregion

        #region 重要: InvalidCharacter vs MissingSpecialCharacter の区別テスト

        [Test]
        public void ValidatePassword_InvalidCharacters_ShouldReturnInvalidCharacter()
        {
            // Arrange - 日本語文字を含む（無効な文字）
            var passwordWithInvalidChars = "Password123@あいう";

            // Act
            var errors = Password.Validator.ValidatePassword(passwordWithInvalidChars);

            // Assert
            Assert.That(errors, Contains.Item(Password.Validator.ValidationError.InvalidCharacter),
                "無効な文字が含まれる場合はInvalidCharacterエラーが返されるべき");
        }

        [Test]
        public void ValidatePassword_EmojiCharacters_ShouldReturnInvalidCharacter()
        {
            // Arrange - 絵文字を含む（無効な文字）
            var passwordWithEmoji = "Password123@😀";

            // Act
            var errors = Password.Validator.ValidatePassword(passwordWithEmoji);

            // Assert
            Assert.That(errors, Contains.Item(Password.Validator.ValidationError.InvalidCharacter),
                "絵文字が含まれる場合はInvalidCharacterエラーが返されるべき");
        }

        [Test]
        public void ValidatePassword_OnlyMissingSpecialChar_ShouldNotReturnInvalidCharacter()
        {
            // Arrange - 特殊文字がないだけで、他は有効な文字のみ
            var passwordMissingSpecial = "Password123";

            // Act
            var errors = Password.Validator.ValidatePassword(passwordMissingSpecial);

            // Assert
            Assert.That(errors, Contains.Item(Password.Validator.ValidationError.MissingSpecialCharacter),
                "特殊文字不足の場合はMissingSpecialCharacterエラーが返されるべき");
            Assert.That(errors.Contains(Password.Validator.ValidationError.InvalidCharacter), Is.False,
                "有効な文字のみの場合はInvalidCharacterエラーは返されるべきではない");
        }

        #endregion

        #region 境界値テスト

        [Test]
        public void ValidatePassword_ExactMinLength_ShouldSuccess()
        {
            // Arrange - 8文字ちょうど
            var password = "Aa1@bcde";

            // Act
            var errors = Password.Validator.ValidatePassword(password);

            // Assert
            Assert.That(errors.Contains(Password.Validator.ValidationError.InvalidLength), Is.False);
        }

        [Test]
        public void ValidatePassword_ExactMaxLength_ShouldSuccess()
        {
            // Arrange - 30文字ちょうど
            var password = "Aa1@" + new string('b', 26); // 4 + 26 = 30文字

            // Act
            var errors = Password.Validator.ValidatePassword(password);

            // Assert
            Assert.That(errors.Contains(Password.Validator.ValidationError.InvalidLength), Is.False);
        }

        #endregion

        #region 複合エラーテスト

        [Test]
        public void ValidatePassword_MultipleErrors_ShouldReturnAllErrors()
        {
            // Arrange - 複数の問題を持つパスワード
            var password = "aa"; // 短い、大文字なし、数字なし、特殊文字なし

            // Act
            var errors = Password.Validator.ValidatePassword(password);

            // Assert
            var expectedErrors = new[]
            {
                Password.Validator.ValidationError.InvalidLength,
                Password.Validator.ValidationError.MissingUppercase,
                Password.Validator.ValidationError.MissingNumber,
                Password.Validator.ValidationError.MissingSpecialCharacter
            };

            foreach (var expectedError in expectedErrors)
            {
                Assert.That(errors, Contains.Item(expectedError));
            }
        }

        #endregion

        #region null/empty テスト

        [Test]
        public void ValidatePassword_NullPassword_ShouldReturnInvalidLength()
        {
            // Act
            var errors = Password.Validator.ValidatePassword(null!);

            // Assert
            Assert.That(errors, Contains.Item(Password.Validator.ValidationError.InvalidLength));
        }

        [Test]
        public void ValidatePassword_EmptyPassword_ShouldReturnInvalidLength()
        {
            // Act
            var errors = Password.Validator.ValidatePassword(string.Empty);

            // Assert
            Assert.That(errors, Contains.Item(Password.Validator.ValidationError.InvalidLength));
        }

        #endregion

        #region 許可された特殊文字テスト

        [Test]
        public void ValidatePassword_AllAllowedSpecialCharacters_ShouldSuccess()
        {
            // Arrange - 許可されたすべての特殊文字
            var allowedSpecialChars = "`~!@#$%^&*()_+-={}[]\\|:;\"'<>,.?/";
            var password = $"Aa1{allowedSpecialChars}";

            // Act
            var errors = Password.Validator.ValidatePassword(password);

            // Assert
            Assert.That(errors.Contains(Password.Validator.ValidationError.InvalidCharacter), Is.False,
                "許可された特殊文字はInvalidCharacterエラーを起こすべきではない");
            Assert.That(errors.Contains(Password.Validator.ValidationError.MissingSpecialCharacter), Is.False,
                "特殊文字が含まれているためMissingSpecialCharacterエラーは起こるべきではない");
        }

        #endregion
    }
}