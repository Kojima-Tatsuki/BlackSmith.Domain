﻿using BlackSmith.Domain.Character;
using BlackSmith.Domain.Character.Battle;
using BlackSmith.Domain.Character.Player;
using NUnit.Framework;
using System;
using System.Collections;

#nullable enable

namespace BlackSmith.Domain.Battle
{
    internal class DefenseValueTest
    {
        private static IEnumerable InstanceTestCases()
        {
            var ldp = new LevelDependentParameters(new CharacterLevel(Experience.RequiredCumulativeExp(1)), new Strength(2), new Agility(1));

            // BattleModuleを使用しない場合
            yield return new TestCaseData(ldp, null, null, null).SetCategory("正常系");

            var eqs = BattleEquipmentModuleTest.GetBattleEquipmentModuleMocks();

            foreach (var eq in eqs)
                yield return new TestCaseData(ldp, eq, null, null).SetCategory("正常系");

            var eff = new BattleStatusEffectModule();

            yield return new TestCaseData(ldp, null, eff, null).SetCategory("正常系");
        }

        [Test(Description = "防御力のインスタンス化テスト")]
        [TestCaseSource(nameof(InstanceTestCases))]
        public void DefenseValueInstancePasses(LevelDependentParameters levelParams, BattleEquipmentModule? equipmentModule, BattleStatusEffectModule? effectModel, Type? exception)
        {
            if (exception is null)
                Assert.That(new DefenseValue(levelParams, equipmentModule, effectModel),
                    Is.EqualTo(new DefenseValue(levelParams, equipmentModule, effectModel)));
            else
                Assert.Throws(exception, () => new DefenseValue(levelParams, equipmentModule, effectModel));
        }
    }
}