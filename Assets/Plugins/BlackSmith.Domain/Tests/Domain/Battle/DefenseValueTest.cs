using BlackSmith.Domain.Character.Battle;
using BlackSmith.Domain.Character.Player;
using BlackSmith.Domain.CharacterObject;
using NUnit.Framework;
using System;
using System.Collections;

#nullable enable

internal class DefenseValueTest
{
    private static IEnumerable InstanceTestCases()
    {
        var ldp = new LevelDependentParameters(new PlayerLevel(Experience.RequiredCumulativeExp(1)), new Strength(2), new Agility(1));

        // BattleModule���g�p���Ȃ��ꍇ
        yield return new TestCaseData(ldp, null, null, null).SetCategory("����n");

        var eqs = BattleEquipmentModuleTest.GetBattleEquipmentModuleMocks();

        foreach (var eq in eqs)
            yield return new TestCaseData(ldp, eq, null, null).SetCategory("����n");

        var eff = new BattleStatusEffectModule();

        yield return new TestCaseData(ldp, null, eff, null).SetCategory("����n");
    }

    [Test(Description = "�h��͂̃C���X�^���X���e�X�g")]
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
