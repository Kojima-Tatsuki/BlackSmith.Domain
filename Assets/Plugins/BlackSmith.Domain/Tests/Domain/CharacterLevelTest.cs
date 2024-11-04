using Newtonsoft.Json;
using NUnit.Framework;
using System;

#nullable enable

namespace BlackSmith.Domain.Character
{
    public class CharacterLevelTest
    {
        [Test(Description = "レベルインスタンス化テスト")]
        [TestCase(10, null, Category = "正常系")]
        [TestCase(1, null, Category = "正常系")]
        [TestCase(0, typeof(ArgumentException), Category = "異常系")]
        [TestCase(-1, typeof(ArgumentException), Category = "異常系")]
        public void LevelInstancePasses(int level, Type? exception = null)
        {
            if (exception is null)
                Assert.That(new CharacterLevel(level), Is.EqualTo(new CharacterLevel(level)));
            else
                Assert.Throws(exception, () => new CharacterLevel(level));
        }

        [Test(Description = "スキル取得数を計算するテスト")]
        [TestCase(1, ExpectedResult = 2, Category = "正常系")]
        [TestCase(6, ExpectedResult = 3, Category = "正常系")]
        [TestCase(11, ExpectedResult = 3, Category = "正常系")]
        [TestCase(18, ExpectedResult = 4, Category = "正常系")]
        [TestCase(100, ExpectedResult = 13, Category = "正常系")]
        public int NumberOfSkillAvailablePasses(int level)
        {
            return new CharacterLevel(level).GetNumberOfSkillsAvailable();
        }

        [Test(Description = "CharacterLevelのシリアライズ・デシリアライズテスト")]
        public void CharacterLevelSerializeTestPasses()
        {
            var level = new CharacterLevel(10);

            var serialized = JsonConvert.SerializeObject(level);
            var deserialized = JsonConvert.DeserializeObject<CharacterLevel>(serialized);

            Assert.That(level, Is.EqualTo(deserialized));
        }
    }
}