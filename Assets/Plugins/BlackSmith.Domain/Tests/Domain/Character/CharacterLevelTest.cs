using BlackSmith.Domain.Character.Player;
using Newtonsoft.Json;
using NUnit.Framework;
using System;

#nullable enable

namespace BlackSmith.Domain.Character
{
    internal class CharacterLevelTest
    {
        internal static CharacterLevel[] GetCharacterLevelMock()
        {
            return new CharacterLevel[]
            {
                new CharacterLevel(), // 経験値を与えない場合(レベル1)
                new CharacterLevel(Experience.RequiredCumulativeExp(100)), // 最大レベル
            };
        }

        [Test(Description = "レベルインスタンス化テスト")]
        [TestCase(10, null, Category = "正常系")]
        [TestCase(1, null, Category = "正常系")]
        [TestCase(0, typeof(ArgumentOutOfRangeException), Category = "異常系")]
        [TestCase(-1, typeof(ArgumentOutOfRangeException), Category = "異常系")]
        public void LevelInstancePasses(int level, Type? exception = null)
        {
            if (exception is null)
                Assert.That(new CharacterLevel(Experience.RequiredCumulativeExp(level)), Is.EqualTo(new CharacterLevel(Experience.RequiredCumulativeExp(level))));
            else
                Assert.Throws(exception, () => new CharacterLevel(Experience.RequiredCumulativeExp(level)));
        }

        [Test(Description = "スキル取得数を計算するテスト")]
        [TestCase(1, ExpectedResult = 2, Category = "正常系")]
        [TestCase(6, ExpectedResult = 3, Category = "正常系")]
        [TestCase(11, ExpectedResult = 3, Category = "正常系")]
        [TestCase(18, ExpectedResult = 4, Category = "正常系")]
        [TestCase(100, ExpectedResult = 13, Category = "正常系")]
        public int NumberOfSkillAvailablePasses(int level)
        {
            var exp = Experience.RequiredCumulativeExp(level);
            return new CharacterLevel(exp).GetNumberOfSkillsAvailable();
        }

        [Test(Description = "CharacterLevelのシリアライズ・デシリアライズテスト")]
        public void CharacterLevelSerializeTestPasses()
        {
            var level = new CharacterLevel(Experience.RequiredCumulativeExp(10));

            var serialized = JsonConvert.SerializeObject(level);
            var deserialized = JsonConvert.DeserializeObject<CharacterLevel>(serialized);

            Assert.That(level, Is.EqualTo(deserialized));
        }

        [Test(Description = "PlayerLevelインスタンス化テスト")]
        [TestCase(null, null, null, Category = "正常系")] // 経験値を与えない場合
        public void PlayerLevelInstancePasses(Experience? exp, Experience? resultExp, Type? exception)
        {
            if (exception is null)
                Assert.That(new CharacterLevel(exp ?? new Experience()), Is.EqualTo(new CharacterLevel(resultExp ?? new Experience())));
            else
                Assert.Throws(exception, () => new CharacterLevel(exp ?? new Experience()));
        }

        [Test(Description = "ステータス上昇ポイントの残り数テスト")]
        [TestCase(1, 1, 1, null, ExpectedResult = 1, Category = "正常系")]
        [TestCase(5, 10, 5, null, ExpectedResult = 0, Category = "正常系")]
        [TestCase(1, 3, 1, typeof(ArgumentException), ExpectedResult = 0, Category = "異常系")]
        public int GetRemainingParamPointPasses(int level, int str, int agi, Type? exception)
        {
            if (exception is null)
                return new LevelDependentParameters(new(Experience.RequiredCumulativeExp(level)), new(str), new(agi))
                    .GetRemainingParamPoint();
            else
                Assert.Throws(exception, () => new LevelDependentParameters(new(Experience.RequiredCumulativeExp(level)), new(str), new(agi))
                               .GetRemainingParamPoint());

            return 0; // ここには到達しない
        }

        [Test(Description = "PlayerLevelのシリアライズ・デシリアライズテスト")]
        public void PlayerLevelSerializeTestPasses()
        {
            var playerLevel = new CharacterLevel(Experience.RequiredCumulativeExp(10));

            var serialized = JsonConvert.SerializeObject(playerLevel);
            var deserialized = JsonConvert.DeserializeObject<CharacterLevel>(serialized);

            Assert.That(playerLevel, Is.EqualTo(deserialized));
        }
    }
}