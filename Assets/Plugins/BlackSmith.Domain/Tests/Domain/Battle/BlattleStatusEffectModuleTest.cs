using BlackSmith.Domain.Character.Battle;
using BlackSmith.Domain.PassiveEffect;
using NUnit.Framework;
using System;
using System.Collections.Generic;

internal class BlattleStatusEffectModuleTest
{
    private static IReadOnlyDictionary<EffectID, BattleStatusEffect>?[] CorrectMockData()
    {
        var id = new EffectID();
        var effect = new BattleStatusEffect(id, new BattleStatusEffectModel(0, 0, 0, 0));

        return new Dictionary<EffectID, BattleStatusEffect>?[] {
            new Dictionary<EffectID, BattleStatusEffect>()
            {
                {id, effect},
            },
            new Dictionary<EffectID, BattleStatusEffect>()
            {

            },
            null
        };
    }

    [Test(Description = "�X�e�[�^�X���W���[���̃C���X�^���X�𐶐�����e�X�g")]
    [TestCaseSource(nameof(CorrectMockData), Category = "����n")]
    public void ModuleInstancePasses(IReadOnlyDictionary<EffectID, BattleStatusEffect>? dict)
    {
        try
        {
            var module = new BattleStatusEffectModule(dict);
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }
}
