using NUnit.Framework;
using System.Collections.Generic;

#nullable enable

namespace BlackSmith.Domain.Networking.Auth
{
    /// <summary>
    /// UserNameクラスのEquals/GetHashCode契約をテストする
    /// </summary>
    internal class UserNameHashCodeTest
    {
        #region Equals/GetHashCode契約テスト

        [Test]
        public void EqualsTrue_SameHashCode_Contract()
        {
            // Arrange - 大文字小文字が異なる同じ論理的な値
            var userName1 = new UserName("testuser");
            var userName2 = new UserName("TESTUSER");
            var userName3 = new UserName("TestUser");

            // Act & Assert - Equalsがtrueなら、GetHashCodeも同じ値でなければならない
            Assert.That(userName1.Equals(userName2), Is.True, "大文字小文字違いでEqualsがtrueであること");
            Assert.That(userName1.GetHashCode(), Is.EqualTo(userName2.GetHashCode()), "Equalsがtrueならハッシュコードも同じであること");

            Assert.That(userName1.Equals(userName3), Is.True, "混合ケースでEqualsがtrueであること");
            Assert.That(userName1.GetHashCode(), Is.EqualTo(userName3.GetHashCode()), "Equalsがtrueならハッシュコードも同じであること");

            Assert.That(userName2.Equals(userName3), Is.True, "大文字と混合ケースでEqualsがtrueであること");
            Assert.That(userName2.GetHashCode(), Is.EqualTo(userName3.GetHashCode()), "Equalsがtrueならハッシュコードも同じであること");
        }

        [Test]
        public void DifferentValues_DifferentHashCode_ShouldBeConsistent()
        {
            // Arrange - 異なる値
            var userName1 = new UserName("user1");
            var userName2 = new UserName("user2");

            // Act & Assert - Equalsがfalseの場合、ハッシュコードは異なることが望ましい（必須ではない）
            Assert.That(userName1.Equals(userName2), Is.False, "異なる値でEqualsがfalseであること");
            // ハッシュコードの衝突は許可されるが、異なることが望ましい
            // 必須の契約ではないのでコメントアウト
            // Assert.That(userName1.GetHashCode(), Is.Not.EqualTo(userName2.GetHashCode()));
        }

        [Test]
        public void SameInstance_SameHashCode()
        {
            // Arrange
            var userName = new UserName("testuser");

            // Act & Assert - 同じインスタンスは常に同じハッシュコードを返すこと
            Assert.That(userName.GetHashCode(), Is.EqualTo(userName.GetHashCode()));
            Assert.That(userName.Equals(userName), Is.True);
        }

        #endregion

        #region ハッシュテーブル動作テスト

        [Test]
        public void HashSet_CaseInsensitive_ShouldTreatAsEqual()
        {
            // Arrange
            var hashSet = new HashSet<UserName>();
            var userName1 = new UserName("testuser");
            var userName2 = new UserName("TESTUSER");
            var userName3 = new UserName("TestUser");

            // Act
            hashSet.Add(userName1);
            var containsUpper = hashSet.Contains(userName2);
            var containsMixed = hashSet.Contains(userName3);
            var addUpperResult = hashSet.Add(userName2);
            var addMixedResult = hashSet.Add(userName3);

            // Assert
            Assert.That(containsUpper, Is.True, "HashSetが大文字版を含むと認識すること");
            Assert.That(containsMixed, Is.True, "HashSetが混合ケース版を含むと認識すること");
            Assert.That(addUpperResult, Is.False, "HashSetが大文字版を重複として扱うこと");
            Assert.That(addMixedResult, Is.False, "HashSetが混合ケース版を重複として扱うこと");
            Assert.That(hashSet.Count, Is.EqualTo(1), "HashSetのサイズが1のままであること");
        }

        [Test]
        public void Dictionary_CaseInsensitive_ShouldTreatAsEqual()
        {
            // Arrange
            var dictionary = new Dictionary<UserName, string>();
            var userName1 = new UserName("testuser");
            var userName2 = new UserName("TESTUSER");

            // Act
            dictionary[userName1] = "value1";
            var getValue = dictionary.TryGetValue(userName2, out var retrievedValue);
            dictionary[userName2] = "value2";

            // Assert
            Assert.That(getValue, Is.True, "Dictionaryが大文字版でキーを見つけること");
            Assert.That(retrievedValue, Is.EqualTo("value1"), "正しい値が取得されること");
            Assert.That(dictionary.Count, Is.EqualTo(1), "Dictionaryのサイズが1のままであること");
            Assert.That(dictionary[userName1], Is.EqualTo("value2"), "値が上書きされること");
        }

        #endregion

        #region 値の格納状態確認テスト

        [Test]
        public void Constructor_AlwaysStoresUpperCase()
        {
            // Arrange & Act
            var userName1 = new UserName("testuser");
            var userName2 = new UserName("TESTUSER");
            var userName3 = new UserName("TestUser");

            // Assert - すべて大文字で格納されることを確認
            Assert.That(userName1.Value, Is.EqualTo("TESTUSER"), "小文字入力が大文字で格納されること");
            Assert.That(userName2.Value, Is.EqualTo("TESTUSER"), "大文字入力がそのまま格納されること");
            Assert.That(userName3.Value, Is.EqualTo("TESTUSER"), "混合ケース入力が大文字で格納されること");
        }

        [Test]
        public void GetHashCode_ConsistentWithStoredValue()
        {
            // Arrange
            var userName = new UserName("testuser");
            var expectedHash = "TESTUSER".GetHashCode();

            // Act
            var actualHash = userName.GetHashCode();

            // Assert - 格納された大文字文字列のハッシュコードと一致すること
            Assert.That(actualHash, Is.EqualTo(expectedHash), "ハッシュコードが格納された大文字文字列と一致すること");
        }

        #endregion

        #region 境界値・特殊ケーステスト

        [Test]
        public void SpecialCharacters_CaseInsensitive_ShouldWork()
        {
            // Arrange - 許可された特殊文字を含む
            var userName1 = new UserName("user.name-test@123_");
            var userName2 = new UserName("USER.NAME-TEST@123_");

            // Act & Assert
            Assert.That(userName1.Equals(userName2), Is.True, "特殊文字を含む場合もケース非依存であること");
            Assert.That(userName1.GetHashCode(), Is.EqualTo(userName2.GetHashCode()), "特殊文字を含む場合もハッシュコードが一致すること");
        }

        [Test]
        public void MinMaxLength_CaseInsensitive_ShouldWork()
        {
            // Arrange - 最小・最大長での検証
            var shortName1 = new UserName("abc");
            var shortName2 = new UserName("ABC");
            var longName1 = new UserName("abcdefghijklmnopqrst");
            var longName2 = new UserName("ABCDEFGHIJKLMNOPQRST");

            // Act & Assert
            Assert.That(shortName1.Equals(shortName2), Is.True, "最小長でもケース非依存であること");
            Assert.That(shortName1.GetHashCode(), Is.EqualTo(shortName2.GetHashCode()), "最小長でもハッシュコードが一致すること");

            Assert.That(longName1.Equals(longName2), Is.True, "最大長でもケース非依存であること");
            Assert.That(longName1.GetHashCode(), Is.EqualTo(longName2.GetHashCode()), "最大長でもハッシュコードが一致すること");
        }

        #endregion
    }
}