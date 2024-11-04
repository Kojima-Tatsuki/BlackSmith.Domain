using Newtonsoft.Json;
using NUnit.Framework;
using System;

#nullable enable

namespace BlackSmith.Domain.Character.Player
{
    internal class LevelDependentParametersTest
    {
        public static LevelDependentParameters GetLevelDependentParametersMock() => new LevelDependentParameters();

        [Test(Description = "LevelDependentParametersインスタンス化テスト")]
        [TestCase(0, 1, 1, null, Category = "正常系")]
        public void LevelDependentParametersInstancePasses(int level, int str, int agi, Type? exception)
        {
            if (exception is null)
                Assert.That(new LevelDependentParameters(new(new(level)), new(str), new(agi)),
                    Is.EqualTo(new LevelDependentParameters(new(new(level)), new(str), new(agi))));
            else
                Assert.Throws(exception, () => new LevelDependentParameters(new(new(level)), new(str), new(agi)));
        }

        [Test(Description = "LevelDependentParametersのシリアライズ・デシリアライズテスト")]
        public void LevelDependentParametersSerializePasses()
        {
            var level = new LevelDependentParameters(new(new(1)), new(1), new(1));

            var serialized = JsonConvert.SerializeObject(level);
            var deserialized = JsonConvert.DeserializeObject<LevelDependentParameters>(serialized);

            Assert.That(level, Is.EqualTo(deserialized));
        }

        [Test(Description = "ステータス上昇ポイントの残り数テスト")]
        [TestCase(1, 1, 1, ExpectedResult = 1, Category = "正常系")]
        [TestCase(5, 10, 5, ExpectedResult = 0, Category = "正常系")]

        public int GetRemainingParamPointPasses(int level, int str, int agi)
        {
            return new LevelDependentParameters(new(Experience.RequiredCumulativeExp(level)), new(str), new(agi))
                .GetRemainingParamPoint();
        }

        [Test()]
        [TestCase(1, 3, 1, Category = "異常系")]
        public void GetRemainingParamPointFail(int level, int str, int agi)
        {
            Assert.Throws(Is.TypeOf<ArgumentException>().And.Message.EqualTo($"指定したSTR, AGIは割当可能量を超過しています STR: {str}, AGI: {agi}"),
            () => new LevelDependentParameters(new(Experience.RequiredCumulativeExp(level)), new(str), new(agi))
                    .GetRemainingParamPoint());
        }

        [Test(Description = "Strengthインスタンス化テスト")]
        [TestCase(1, null, Category = "正常系")]
        [TestCase(0, typeof(ArgumentException), Category = "異常系")]
        public void StrengthInstancePasses(int value, Type? excaption)
        {
            if (excaption is null)
                Assert.That(new Strength(value), Is.EqualTo(new Strength(value)));
            else
                Assert.Throws(excaption, () => new Strength(value));
        }

        [Test(Description = "Strengthのシリアライズ・デシリアライズテスト")]
        public void StrengthSerializePasses()
        {
            var strength = new Strength(1);

            var serialized = JsonConvert.SerializeObject(strength);
            var deserialized = JsonConvert.DeserializeObject<Strength>(serialized);

            Assert.That(strength, Is.EqualTo(deserialized));
        }

        [Test(Description = "Agilityインスタンス化テスト")]
        [TestCase(1, null, Category = "正常系")]
        [TestCase(0, typeof(ArgumentException), Category = "異常系")]
        public void AgilityInstancePasses(int value, Type? excaption)
        {
            if (excaption is null)
                Assert.That(new Agility(value), Is.EqualTo(new Agility(value)));
            else
                Assert.Throws(excaption, () => new Agility(value));
        }

        [Test(Description = "Agilityのシリアライズ・デシリアライズテスト")]
        public void AgilitySerializePasses()
        {
            var agility = new Agility(1);

            var serialized = JsonConvert.SerializeObject(agility);
            var deserialized = JsonConvert.DeserializeObject<Agility>(serialized);

            Assert.That(agility, Is.EqualTo(deserialized));
        }
    }
}