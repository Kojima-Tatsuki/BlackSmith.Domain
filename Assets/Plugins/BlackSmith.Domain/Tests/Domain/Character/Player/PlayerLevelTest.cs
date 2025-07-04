using Newtonsoft.Json;
using NUnit.Framework;
using System;

#nullable enable

namespace BlackSmith.Domain.Character.Player
{
    internal class PlayerLevelTest
    {
        internal static CharacterLevel[] GetPlayserLevelMock()
        {
            return new CharacterLevel[]
            {
                new CharacterLevel(), // 経験値を与えない場合(レベル1)
                new CharacterLevel(Experience.RequiredCumulativeExp(100)), // 最大レベル
            };
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