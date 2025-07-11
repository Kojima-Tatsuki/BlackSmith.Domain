﻿using BlackSmith.Domain.Character;
using BlackSmith.Domain.Character.Battle;
using Newtonsoft.Json;
using NUnit.Framework;
using System;

#nullable enable

namespace BlackSmith.Domain.Battle
{
    public class HealthPointTest
    {
        [Test(Description = "HelthPointインスタンス化テスト")]
        [TestCase(10, 10, null, Category = "正常系")]
        [TestCase(0, 0, null, Category = "正常系")]
        [TestCase(10, 20, null, Category = "正常系")]
        [TestCase(20, 10, typeof(ArgumentException), Category = "異常系")] // 現在値が最大値より大きい場合
        [TestCase(null, 10, typeof(ArgumentNullException), Category = "異常系")] // 現在値がnullの場合
        [TestCase(10, null, typeof(ArgumentNullException), Category = "異常系")] // 最大値がnullの場合
        [TestCase(null, null, typeof(ArgumentNullException), Category = "異常系")] // 両方nullの場合
        public void HealthPointInstancePasses(int? value, int? max, Type? exception = null)
        {
            if (exception is null)
                Assert.That(new HealthPoint(new HealthPointValue(value ?? 0), new MaxHealthPointValue(max ?? 0)),
                    Is.EqualTo(new HealthPoint(new HealthPointValue(value ?? 0), new MaxHealthPointValue(max ?? 0))));
            else
            {
                // 引数にNullが渡された場合のテスト
                if (value == null && max == null)
                    Assert.Throws(exception, () => new HealthPoint(null!, null!));
                else if (value == null)
                    Assert.Throws(exception, () => new HealthPoint(null!, new MaxHealthPointValue(max ?? 0)));
                else if (max == null)
                    Assert.Throws(exception, () => new HealthPoint(new HealthPointValue(value ?? 0), null!));
                else
                    Assert.Throws(exception, () => new HealthPoint(new HealthPointValue(value ?? 0), new MaxHealthPointValue(max ?? 0)));
            }
        }

        [Test(Description = "HelthPointインスタンス化テスト")]
        [TestCase(10, null, Category = "正常系")]
        [TestCase(0, null, Category = "正常系")]
        [TestCase(-1, typeof(ArgumentException), Category = "異常系")]
        [TestCase(null, typeof(ArgumentNullException), Category = "異常系")]
        public void HealthPointInstancePasses(int? max, Type? exception = null)
        {
            if (exception is null)
                Assert.That(new HealthPoint(new MaxHealthPointValue(max ?? 0)),
                    Is.EqualTo(new HealthPoint(new MaxHealthPointValue(max ?? 0))));
            else
            {
                if (max == null)
                    Assert.Throws(exception, () => new HealthPoint(max: null!));
                else
                    Assert.Throws(exception, () => new HealthPoint(new MaxHealthPointValue(max ?? 0)));
            }
        }

        // CaracterLevelクラスを使用したHealthPointのインスタンス化テスト
        [Test(Description = "HealthPointインスタンス化テスト")]
        [TestCase(10, null, Category = "正常系")]
        [TestCase(0, typeof(ArgumentOutOfRangeException), Category = "異常系")]
        [TestCase(-1, typeof(ArgumentOutOfRangeException), Category = "異常系")]
        [TestCase(null, typeof(ArgumentNullException), Category = "異常系")]
        public void HealthPointInstanceFromCharacterLevelPasses(int? level, Type? exception = null)
        {
            if (exception is null)
                Assert.That(new HealthPoint(new CharacterLevel(Experience.RequiredCumulativeExp(level ?? 0))),
                                   Is.EqualTo(new HealthPoint(new CharacterLevel(Experience.RequiredCumulativeExp(level ?? 1)))));
            else
            {
                if (level == null)
                    Assert.Throws(exception, () => new HealthPoint(level: null!));
                else
                    Assert.Throws(exception, () => new HealthPoint(new CharacterLevel(Experience.RequiredCumulativeExp(level ?? 1))));
            }
        }

        // HealthPointの比較テスト
        [Test(Description = "HealthPointの比較テスト")]
        [TestCase(10, 10, false, true, false, true, Category = "正常系")]
        [TestCase(10, 20, true, true, false, false, Category = "正常系")]
        [TestCase(20, 10, false, false, true, true, Category = "正常系")]
        public void HealthPointEqualsPasses(int value1, int value2, bool less, bool lesseq, bool more, bool moreeq)
        {
            var health1 = new HealthPointValue(value1);
            var health2 = new HealthPointValue(value2);

            Assert.AreEqual(less, health1 < health2);
            Assert.AreEqual(lesseq, health1 <= health2);
            Assert.AreEqual(more, health1 > health2);
            Assert.AreEqual(moreeq, health1 >= health2);
        }

        [Test(Description = "HealthPointのシリアライズ・デシリアライズテスト")]
        public void HealthPointSerializeTestPasses()
        {
            var health = new HealthPoint(new HealthPointValue(10), new MaxHealthPointValue(10));

            var serialized = JsonConvert.SerializeObject(health);
            var deserialized = JsonConvert.DeserializeObject<HealthPoint>(serialized);

            Assert.That(health, Is.EqualTo(deserialized));
        }
    }

    internal class HealthPointValueTest
    {
        [Test(Description = "HelthPointValueインスタンス化テスト")]
        [TestCase(10, 10, null, Category = "正常系")]
        [TestCase(0, 0, null, Category = "正常系")]
        [TestCase(-1, null, typeof(ArgumentException), Category = "異常系")]
        public void HealthPointValueInstancePasses(int value, int result, Type? exception = null)
        {
            if (exception is null)
                Assert.That(new HealthPointValue(value), Is.EqualTo(new HealthPointValue(result)));
            else
                Assert.Throws(exception, () => new HealthPointValue(value));
        }

        [Test(Description = "MaxHealthPointValueインスタンス化テスト")]
        [TestCase(10, 10, null, Category = "正常系")]
        [TestCase(0, 0, null, Category = "正常系")]
        [TestCase(-1, null, typeof(ArgumentException), Category = "異常系")]
        public void MaxHealthPointValueInstancePasses(int value, int result, Type? exception = null)
        {
            if (exception is null)
                Assert.That(new MaxHealthPointValue(value), Is.EqualTo(new MaxHealthPointValue(result)));
            else
                Assert.Throws(exception, () => new MaxHealthPointValue(value));
        }

        [Test(Description = "HealthPointValueのシリアライズ・デシリアライズテスト")]
        public void HealthPointValueSerializeTestPasses()
        {
            var health = new HealthPointValue(10);

            var serialized = JsonConvert.SerializeObject(health);
            var deserialized = JsonConvert.DeserializeObject<HealthPointValue>(serialized);

            Assert.That(health, Is.EqualTo(deserialized));
        }
    }

    internal class MaxHealthPointValueTest
    {
        [Test(Description = "MaxHealthPointValueインスタンス化テスト")]
        [TestCase(10, 10, null, Category = "正常系")]
        [TestCase(0, 0, null, Category = "正常系")]
        [TestCase(-1, null, typeof(ArgumentException), Category = "異常系")]
        public void MaxHealthPointValueInstancePasses(int value, int result, Type? exception = null)
        {
            if (exception is null)
                Assert.That(new MaxHealthPointValue(value), Is.EqualTo(new MaxHealthPointValue(result)));
            else
                Assert.Throws(exception, () => new MaxHealthPointValue(value));
        }

        [Test(Description = "MaxHealthPointValueのシリアライズ・デシリアライズテスト")]
        public void MaxHealthPointValueSerializeTestPasses()
        {
            var health = new MaxHealthPointValue(10);

            var serialized = JsonConvert.SerializeObject(health);
            var deserialized = JsonConvert.DeserializeObject<MaxHealthPointValue>(serialized);

            Assert.That(health, Is.EqualTo(deserialized));
        }
    }
}