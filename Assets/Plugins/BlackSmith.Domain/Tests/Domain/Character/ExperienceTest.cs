using Newtonsoft.Json;
using NUnit.Framework;
using System;

#nullable enable

namespace BlackSmith.Domain.Character
{
    internal class ExperienceTest
    {
        #region Core Experience Tests
        [Test(Description = "総経験値量からレベルを計算するテスト")]
        [TestCase(0, ExpectedResult = 1, Category = "正常系")]
        [TestCase(10, ExpectedResult = 2, Category = "正常系")]
        [TestCase(21, ExpectedResult = 3, Category = "正常系")]
        [TestCase(33, ExpectedResult = 4, Category = "正常系")]
        [TestCase(46, ExpectedResult = 5, Category = "正常系")]
        [TestCase(135, ExpectedResult = 10, Category = "正常系")]
        [TestCase(511, ExpectedResult = 20, Category = "正常系")]
        [TestCase(1486, ExpectedResult = 30, Category = "正常系")]
        [TestCase(4014, ExpectedResult = 40, Category = "正常系")] 
        [TestCase(10571, ExpectedResult = 50, Category = "正常系")]
        [TestCase(482902, ExpectedResult = 90, Category = "正常系")]
        [TestCase(1252685, ExpectedResult = 100, Category = "正常系")]
        public int CurrentLevelPasses(int exp)
        {
            return Experience.CurrentLevel(new Experience(exp));
        }

        [Test(Description = "レベルからそのレベルを満たす最低経験値量を計算するテスト")]
        [TestCase(1, ExpectedResult = 0, Category = "正常系")]
        [TestCase(2, ExpectedResult = 10, Category = "正常系")]
        [TestCase(3, ExpectedResult = 21, Category = "正常系")]
        [TestCase(4, ExpectedResult = 33, Category = "正常系")]
        [TestCase(5, ExpectedResult = 46, Category = "正常系")]
        [TestCase(10, ExpectedResult = 135, Category = "正常系")]
        [TestCase(20, ExpectedResult = 511, Category = "正常系")]
        [TestCase(30, ExpectedResult = 1486, Category = "正常系")]
        [TestCase(40, ExpectedResult = 4014, Category = "正常系")]
        [TestCase(50, ExpectedResult = 10571, Category = "正常系")]
        [TestCase(90, ExpectedResult = 482902, Category = "正常系")]
        [TestCase(100, ExpectedResult = 1252685, Category = "正常系")]
        public int RequiredCumulativeExpPasses(int level)
        {
            return Experience.RequiredCumulativeExp(level).Value;
        }

        [Test(Description = "Experienceのシリアライズ・デシリアライズテスト")]
        [TestCase(0, Category = "正常系")]
        [TestCase(10, Category = "正常系")]
        [TestCase(135, Category = "正常系")]
        [TestCase(1000, Category = "正常系")]
        [TestCase(int.MaxValue, Category = "境界値")]
        public void ExperienceSerializedPasses(int value)
        {
            var experience = new Experience(value);

            var serialized = JsonConvert.SerializeObject(experience);
            var deserialized = JsonConvert.DeserializeObject<Experience>(serialized) ?? throw new InvalidOperationException();

            Assert.That(experience, Is.EqualTo(deserialized));
            Assert.That(experience.Value, Is.EqualTo(deserialized.Value));
        }
        #endregion

        #region Add Method Tests
        [Test(Description = "経験値加算の正常系テスト")]
        [TestCase(100, 50, ExpectedResult = 150, Category = "正常系")]
        [TestCase(0, 100, ExpectedResult = 100, Category = "正常系")]
        [TestCase(100, 0, ExpectedResult = 100, Category = "正常系")]
        [TestCase(1000, 2000, ExpectedResult = 3000, Category = "正常系")]
        [TestCase(int.MaxValue - 1000, 500, ExpectedResult = int.MaxValue - 500, Category = "境界値")]
        public int AddMethodPasses(int baseExp, int addExp)
        {
            var experience = new Experience(baseExp);
            var addedExp = new Experience(addExp);
            return experience.Add(addedExp).Value;
        }
        #endregion

        #region NeedToNextLevel Method Tests
        [Test(Description = "次レベルまでの必要経験値計算テスト")]
        [TestCase(1, ExpectedResult = 10, Category = "正常系")]
        [TestCase(2, ExpectedResult = 11, Category = "正常系")]
        [TestCase(3, ExpectedResult = 12, Category = "正常系")]
        [TestCase(4, ExpectedResult = 13, Category = "正常系")]
        [TestCase(5, ExpectedResult = 14, Category = "正常系")]
        [TestCase(10, ExpectedResult = 23, Category = "正常系")]
        [TestCase(20, ExpectedResult = 61, Category = "正常系")]
        [TestCase(30, ExpectedResult = 158, Category = "正常系")]
        [TestCase(40, ExpectedResult = 411, Category = "正常系")]
        [TestCase(50, ExpectedResult = 1067, Category = "正常系")]
        [TestCase(90, ExpectedResult = 48300, Category = "正常系")]
        [TestCase(100, ExpectedResult = 125278, Category = "境界値")]
        public int NeedToNextLevelPasses(int level)
        {
            return Experience.NeedToNextLevel(level).Value;
        }

        [Test(Description = "NeedToNextLevelメソッドの境界値・異常系テスト")]
        [TestCase(0, null, Category = "異常系")]
        [TestCase(-1, null, Category = "異常系")]
        [TestCase(101, null, Category = "異常系")]
        [TestCase(int.MinValue, typeof(ArgumentOutOfRangeException), Category = "異常系")]
        [TestCase(int.MaxValue, typeof(ArgumentOutOfRangeException), Category = "異常系")]
        public void NeedToNextLevelEdgeCases(int level, Type? exception = null)
        {
            if (exception is null)
                Assert.DoesNotThrow(() => Experience.NeedToNextLevel(level));
            else
                Assert.Throws(exception, () => Experience.NeedToNextLevel(level));
        }
        #endregion

        #region ReceiveExp Method Tests
        [Test(Description = "敵撃破時の獲得経験値計算テスト")]
        [TestCase(1, ExpectedResult = 2, Category = "正常系")]
        [TestCase(2, ExpectedResult = 2, Category = "正常系")]
        [TestCase(3, ExpectedResult = 2, Category = "正常系")]
        [TestCase(4, ExpectedResult = 2, Category = "正常系")]
        [TestCase(5, ExpectedResult = 2, Category = "正常系")]
        [TestCase(10, ExpectedResult = 4, Category = "正常系")]
        [TestCase(20, ExpectedResult = 12, Category = "正常系")]
        [TestCase(30, ExpectedResult = 31, Category = "正常系")]
        [TestCase(40, ExpectedResult = 82, Category = "正常系")]
        [TestCase(50, ExpectedResult = 213, Category = "正常系")]
        [TestCase(90, ExpectedResult = 9660, Category = "正常系")]
        [TestCase(100, ExpectedResult = 25055, Category = "境界値")]
        public int ReceiveExpPasses(int level)
        {
            return Experience.ReceiveExp(level).Value;
        }

        [Test(Description = "ReceiveExpメソッドの境界値・異常系テスト")]
        [TestCase(0, null, Category = "異常系")]
        [TestCase(-1, null, Category = "異常系")]
        [TestCase(101, null, Category = "異常系")]
        [TestCase(int.MinValue, typeof(ArgumentOutOfRangeException), Category = "異常系")]
        [TestCase(int.MaxValue, typeof(ArgumentOutOfRangeException), Category = "異常系")]
        public void ReceiveExpEdgeCases(int level, Type? exception)
        {
            if (exception is null)
                Assert.DoesNotThrow(() => Experience.ReceiveExp(level));
            else
                Assert.Throws(exception, () => Experience.ReceiveExp(level));
        }
        #endregion

        #region Constructor Tests
        [Test(Description = "コンストラクタの正常系テスト")]
        [TestCase(0, ExpectedResult = 0, Category = "正常系")]
        [TestCase(1, ExpectedResult = 1, Category = "正常系")]
        [TestCase(10, ExpectedResult = 10, Category = "正常系")]
        [TestCase(int.MaxValue, ExpectedResult = int.MaxValue, Category = "境界値")]
        public int ConstructorPasses(int value)
        {
            return new Experience(value).Value;
        }

        [Test(Description = "コンストラクタの異常系テスト")]
        [TestCase(-1, Category = "異常系")]
        [TestCase(-10, Category = "異常系")]
        [TestCase(int.MinValue, Category = "異常系")]
        public void ConstructorThrowsException(int value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Experience(value));
        }

        [Test(Description = "デフォルトコンストラクタのテスト")]
        public void DefaultConstructorPasses()
        {
            var exp = new Experience();
            Assert.That(exp.Value, Is.EqualTo(0));
        }

        [Test(Description = "コンストラクタの境界値テスト")]
        public void ConstructorBoundaryTest()
        {
            // 最小有効値
            var minExp = new Experience(0);
            Assert.That(minExp.Value, Is.EqualTo(0));
            
            // 最大有効値
            var maxExp = new Experience(int.MaxValue);
            Assert.That(maxExp.Value, Is.EqualTo(int.MaxValue));
            
            // 0より1小さい値で例外が発生することを確認
            Assert.Throws<ArgumentOutOfRangeException>(() => new Experience(-1));
        }
        #endregion

        #region Exception Tests
        [Test(Description = "RequiredCumulativeExpメソッドの例外テスト")]
        [TestCase(0, Category = "例外")]
        [TestCase(-1, Category = "例外")]
        [TestCase(101, Category = "例外")]
        [TestCase(int.MinValue, Category = "例外")]
        public void RequiredCumulativeExpThrowsException(int level)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Experience.RequiredCumulativeExp(level));
        }
        #endregion

        #region Extended CurrentLevel Tests
        [Test(Description = "CurrentLevelメソッドの拡張テスト")]
        [TestCase(1, ExpectedResult = 1, Category = "境界値")]
        [TestCase(9, ExpectedResult = 1, Category = "境界値")]
        [TestCase(20, ExpectedResult = 2, Category = "境界値")]
        [TestCase(32, ExpectedResult = 3, Category = "境界値")]
        [TestCase(45, ExpectedResult = 4, Category = "境界値")]
        [TestCase(134, ExpectedResult = 9, Category = "境界値")]
        [TestCase(510, ExpectedResult = 19, Category = "境界値")]
        [TestCase(1485, ExpectedResult = 29, Category = "境界値")]
        [TestCase(4013, ExpectedResult = 39, Category = "境界値")]
        [TestCase(10570, ExpectedResult = 49, Category = "境界値")]
        [TestCase(482901, ExpectedResult = 89, Category = "境界値")]
        [TestCase(1252684, ExpectedResult = 99, Category = "境界値")]
        public int CurrentLevelExtendedPasses(int exp)
        {
            return Experience.CurrentLevel(new Experience(exp));
        }

        [Test(Description = "CurrentLevelメソッドの負の値テスト")]
        [TestCase(-1, Category = "異常系")]
        [TestCase(-100, Category = "異常系")]
        [TestCase(int.MinValue, Category = "異常系")]
        public void CurrentLevelWithNegativeValues(int exp)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Experience.CurrentLevel(new Experience(exp)));
        }
        #endregion

        #region Integration Tests
        [Test(Description = "レベル・経験値の相互変換テスト")]
        [TestCase(1, Category = "統合テスト")]
        [TestCase(2, Category = "統合テスト")]
        [TestCase(5, Category = "統合テスト")]
        [TestCase(10, Category = "統合テスト")]
        [TestCase(20, Category = "統合テスト")]
        [TestCase(50, Category = "統合テスト")]
        [TestCase(100, Category = "統合テスト")]
        public void LevelExperienceRoundTripTest(int level)
        {
            var requiredExp = Experience.RequiredCumulativeExp(level);
            var calculatedLevel = Experience.CurrentLevel(requiredExp);
            Assert.That(calculatedLevel, Is.EqualTo(level));
        }

        [Test(Description = "経験値加算と累計経験値の整合性テスト")]
        public void AddMethodIntegrationTest()
        {
            var baseExp = new Experience(100);
            var addExp = new Experience(50);
            var result = baseExp.Add(addExp);
            
            Assert.That(result.Value, Is.EqualTo(150));
            Assert.That(result, Is.Not.EqualTo(baseExp));
            Assert.That(baseExp.Value, Is.EqualTo(100));
        }
        #endregion

        #region Performance and Edge Cases
        [Test(Description = "大きな値での計算精度テスト")]
        public void LargeValueCalculationTest()
        {
            var largeExp = new Experience(1000000);
            var level = Experience.CurrentLevel(largeExp);
            
            Assert.That(level, Is.GreaterThan(0));
            Assert.That(level, Is.LessThanOrEqualTo(100));
        }

        [Test(Description = "record型の等価性テスト")]
        public void RecordEqualityTest()
        {
            var exp1 = new Experience(100);
            var exp2 = new Experience(100);
            var exp3 = new Experience(200);
            
            Assert.That(exp1, Is.EqualTo(exp2));
            Assert.That(exp1, Is.Not.EqualTo(exp3));
            Assert.That(exp1.GetHashCode(), Is.EqualTo(exp2.GetHashCode()));
        }
        #endregion
    }
}
