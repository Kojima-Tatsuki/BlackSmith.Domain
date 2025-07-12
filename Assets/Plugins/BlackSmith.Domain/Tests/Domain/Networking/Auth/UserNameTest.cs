using NUnit.Framework;
using System;
using System.Linq;

#nullable enable

namespace BlackSmith.Domain.Networking.Auth
{
    internal class UserNameTest
    {
        #region 正常系テスト

        [Test]
        [TestCase("user123")]
        [TestCase("TestUser")]
        [TestCase("player.name")]
        [TestCase("user-name")]
        [TestCase("user@domain")]
        [TestCase("user_name")]
        public void Constructor_ValidUserName_ShouldCreateUserName(string userName)
        {
            // Act
            var result = new UserName(userName);

            // Assert
            Assert.That(result.Value, Is.EqualTo(userName.ToUpper()));
        }

        [Test]
        public void Constructor_LowerCaseInput_ShouldConvertToUpperCase()
        {
            // Arrange
            var input = "testuser";

            // Act
            var userName = new UserName(input);

            // Assert
            Assert.That(userName.Value, Is.EqualTo("TESTUSER"));
        }

        [Test]
        public void Constructor_MixedCaseInput_ShouldConvertToUpperCase()
        {
            // Arrange
            var input = "TestUser123";

            // Act
            var userName = new UserName(input);

            // Assert
            Assert.That(userName.Value, Is.EqualTo("TESTUSER123"));
        }

        [Test]
        public void Equals_SameValueDifferentCase_ShouldReturnTrue()
        {
            // Arrange
            var userName1 = new UserName("TestUser");
            var userName2 = new UserName("testuser");

            // Act & Assert
            Assert.That(userName1.Equals(userName2), Is.True);
            Assert.That(userName1 == userName2, Is.True);
        }

        [Test]
        public void Equals_DifferentValues_ShouldReturnFalse()
        {
            // Arrange
            var userName1 = new UserName("user1");
            var userName2 = new UserName("user2");

            // Act & Assert
            Assert.That(userName1.Equals(userName2), Is.False);
            Assert.That(userName1 == userName2, Is.False);
        }

        [Test]
        public void Equals_WithNull_ShouldReturnFalse()
        {
            // Arrange
            var userName = new UserName("testuser");

            // Act & Assert
            Assert.That(userName.Equals(null), Is.False);
        }

        [Test]
        public void GetHashCode_SameValueDifferentCase_ShouldReturnSameHashCode()
        {
            // Arrange
            var userName1 = new UserName("TestUser");
            var userName2 = new UserName("testuser");

            // Act & Assert
            Assert.That(userName1.GetHashCode(), Is.EqualTo(userName2.GetHashCode()));
        }

        #endregion

        #region 異常系テスト

        [Test]
        public void Constructor_NullUserName_ShouldThrowArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new UserName(null!));
        }

        [Test]
        public void Constructor_EmptyUserName_ShouldThrowArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new UserName(""));
        }

        [Test]
        public void Constructor_TooShortUserName_ShouldThrowArgumentException()
        {
            // Arrange - 2文字（最小値3文字未満）
            var shortUserName = "ab";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new UserName(shortUserName));
        }

        [Test]
        public void Constructor_TooLongUserName_ShouldThrowArgumentException()
        {
            // Arrange - 21文字（最大値20文字超過）
            var longUserName = "verylongusernamethat1";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new UserName(longUserName));
        }

        [Test]
        public void Constructor_InvalidCharacters_ShouldThrowArgumentException()
        {
            // Arrange - 無効な文字を含む
            var invalidUserName = "user#name";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new UserName(invalidUserName));
        }

        [Test]
        public void Constructor_WithSpaces_ShouldThrowArgumentException()
        {
            // Arrange - スペースを含む
            var userNameWithSpaces = "user name";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new UserName(userNameWithSpaces));
        }

        [Test]
        public void Constructor_WithExclamation_ShouldThrowArgumentException()
        {
            // Arrange - 感嘆符を含む
            var userNameWithExclamation = "user!";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new UserName(userNameWithExclamation));
        }

        #endregion

        #region 境界値テスト

        [Test]
        public void Constructor_ExactMinLength_ShouldSuccess()
        {
            // Arrange - 正確に3文字
            var userName = "abc";

            // Act
            var result = new UserName(userName);

            // Assert
            Assert.That(result.Value, Is.EqualTo("ABC"));
        }

        [Test]
        public void Constructor_ExactMaxLength_ShouldSuccess()
        {
            // Arrange - 正確に20文字
            var userName = "abcdefghijklmnopqrst";

            // Act
            var result = new UserName(userName);

            // Assert
            Assert.That(result.Value, Is.EqualTo("ABCDEFGHIJKLMNOPQRST"));
        }

        [Test]
        public void Constructor_AllAllowedSpecialCharacters_ShouldSuccess()
        {
            // Arrange - 許可された特殊文字すべて
            var userName = "user.name-test@123_";

            // Act
            var result = new UserName(userName);

            // Assert
            Assert.That(result.Value, Is.EqualTo("USER.NAME-TEST@123_"));
        }

        #endregion

        #region バリデーションメソッドテスト

        [Test]
        public void ValidateUserName_ValidUserName_ShouldReturnEmptyList()
        {
            // Arrange
            var validUserName = "validuser123";

            // Act
            var errors = UserName.Validator.ValidateUserName(validUserName);

            // Assert
            Assert.That(errors.Count, Is.EqualTo(0));
        }

        [Test]
        public void ValidateUserName_NullUserName_ShouldReturnInvalidLength()
        {
            // Act
            var errors = UserName.Validator.ValidateUserName(null!);

            // Assert
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.That(errors.Contains(UserName.Validator.ValidationError.InvalidLength), Is.True);
        }

        [Test]
        public void ValidateUserName_EmptyUserName_ShouldReturnInvalidLength()
        {
            // Act
            var errors = UserName.Validator.ValidateUserName("");

            // Assert
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.That(errors.Contains(UserName.Validator.ValidationError.InvalidLength), Is.True);
        }

        [Test]
        public void ValidateUserName_TooShort_ShouldReturnInvalidLength()
        {
            // Arrange
            var shortUserName = "ab";

            // Act
            var errors = UserName.Validator.ValidateUserName(shortUserName);

            // Assert
            Assert.That(errors.Contains(UserName.Validator.ValidationError.InvalidLength), Is.True);
        }

        [Test]
        public void ValidateUserName_TooLong_ShouldReturnInvalidLength()
        {
            // Arrange
            var longUserName = "verylongusernamethat1";

            // Act
            var errors = UserName.Validator.ValidateUserName(longUserName);

            // Assert
            Assert.That(errors.Contains(UserName.Validator.ValidationError.InvalidLength), Is.True);
        }

        [Test]
        public void ValidateUserName_InvalidCharacterType_ShouldReturnInvalidCharacterType()
        {
            // Arrange - 有効な長さだが無効な文字を含む
            var userNameWithInvalidChars = "user#name";

            // Act
            var errors = UserName.Validator.ValidateUserName(userNameWithInvalidChars);

            // Assert
            Assert.That(errors.Contains(UserName.Validator.ValidationError.InvalidCharacterType), Is.True);
        }

        [Test]
        public void ValidateUserName_MultipleErrors_ShouldReturnMultipleErrors()
        {
            // Arrange - 短すぎて無効な文字も含む
            var invalidUserName = "a#";

            // Act
            var errors = UserName.Validator.ValidateUserName(invalidUserName);

            // Assert
            Assert.That(errors.Count, Is.EqualTo(2));
            Assert.That(errors.Contains(UserName.Validator.ValidationError.InvalidLength), Is.True);
            Assert.That(errors.Contains(UserName.Validator.ValidationError.InvalidCharacterType), Is.True);
        }

        [Test]
        public void IsValidUserName_ValidUserName_ShouldReturnTrue()
        {
            // Arrange
            var validUserName = "validuser123";

            // Act
            var result = UserName.Validator.IsValidUserName(validUserName);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsValidUserName_InvalidUserName_ShouldReturnFalse()
        {
            // Arrange
            var invalidUserName = "invalid#user";

            // Act
            var result = UserName.Validator.IsValidUserName(invalidUserName);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion
    }
}