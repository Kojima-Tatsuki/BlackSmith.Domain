using BlackSmith.Domain.Character;
using BlackSmith.Domain.Character.Battle;
using BlackSmith.Domain.Character.Player;
using NUnit.Framework;
using System;
using System.Collections;

#nullable enable

namespace BlackSmith.Domain.Battle
{
    internal class PlayerBattleEntityTest
    {
        private static IEnumerable TakeDamageTestCases()
        {
            var level = new PlayerLevel(Experience.RequiredCumulativeExp(10));
            var reciever = new PlayerBattleEntity(
                new PlayerBattleReconstructCommand(new CharacterID(), 
                    new CharacterBattleModule(new HealthPoint(level), 
                        new LevelDependentParameters(level, new Strength(10), new Agility(20)), 
                        new BattleEquipmentModule(null, null), 
                        new BattleStatusEffectModule())));
            var damage = new DamageValue(10);

            yield return new TestCaseData(reciever, damage, null).SetCategory("正常系");

            // Damageがnullの場合
            yield return new TestCaseData(reciever, null, typeof(ArgumentNullException)).SetCategory("異常系");
        }

        private static IEnumerable HealHealthTestCases()
        {
            var level = new PlayerLevel(Experience.RequiredCumulativeExp(10));
            var reciever = new PlayerBattleEntity(
                new PlayerBattleReconstructCommand(new CharacterID(), 
                    new CharacterBattleModule(new HealthPoint(level), 
                        new LevelDependentParameters(level, new Strength(10), new Agility(20)), 
                        new BattleEquipmentModule(null, null), 
                        new BattleStatusEffectModule())));

            // HealValueが0の場合
            yield return new TestCaseData(reciever, 0, null).SetCategory("正常系");

            // 最大値を超える回復量の場合
            var healValue = reciever.HealthPoint.MaxValue + 1; // 最大値を超える回復量
            yield return new TestCaseData(reciever, healValue, null).SetCategory("正常系");

            // HealValueが負の値の場合
            yield return new TestCaseData(reciever, -1, typeof(ArgumentOutOfRangeException)).SetCategory("異常系");
        }

        [Test(Description = "TakeDamageのテスト")]
        [TestCaseSource(nameof(TakeDamageTestCases))]
        public void TakeDamagePasses(IBattleCharacter entity, DamageValue damage, Type? exception)
        {
            if (exception == null)
            {
                var expected = entity.HealthPoint.TakeDamage(damage);
                Assert.That(entity.TakeDamage(damage), Is.EqualTo(expected));
            }
            else
                Assert.Throws(Is.InstanceOf(exception), () => entity.TakeDamage(damage));
        }

        [Test(Description = "HealHealthのテスト")]
        [TestCaseSource(nameof(HealHealthTestCases))]
        public void HealHealthPasses(IBattleCharacter entity, int healValue, Type? exception)
        {
            if (exception == null)
            {
                var expected = entity.HealthPoint.HealHealth(healValue);
                Assert.That(entity.HealHealth(healValue), Is.EqualTo(expected));
            }
            else
                Assert.Throws(Is.InstanceOf(exception), () => entity.HealHealth(healValue));
        }
    }
}
