using BlackSmith.Domain.PassiveEffect;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;

#nullable enable

namespace BlackSmith.Domain.Character.Battle
{
    internal class BlattleStatusEffectModuleTest
    {
        private static IReadOnlyCollection<BattleStatusEffect>?[] CorrectMockData()
        {
            var id = new EffectID();
            var effect = new BattleStatusEffect(id, new BattleStatusEffectModel(0, 0, 0, 0));

            return new List<BattleStatusEffect>?[] {
                new List<BattleStatusEffect>()
                {
                    {effect},
                },
                new List < BattleStatusEffect >()
                {

                },
                null
            };
        }

        [Test(Description = "ステータスモジュールのインスタンスを生成するテスト")]
        [TestCaseSource(nameof(CorrectMockData), Category = "正常系")]
        public void ModuleInstancePasses(IReadOnlyCollection<BattleStatusEffect>? dict)
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

        [Test(Description = "BattleStatusEffectModuleのシリアライズ・デシリアライズテスト")]
        public void BattleStatusEffectModuleSerializeTestPasses()
        {
            var effects = new List<BattleStatusEffect>()
            {
                { new BattleStatusEffect(new EffectID(), new BattleStatusEffectModel(0, 0, 0, 0)) },
                { new BattleStatusEffect(new EffectID(), new BattleStatusEffectModel(1, 1, 1, 1)) },
            };
            var module = new BattleStatusEffectModule(effects);

            var serialized = JsonConvert.SerializeObject(module);
            var deserialized = JsonConvert.DeserializeObject<BattleStatusEffectModule>(serialized);

            Assert.That(module, Is.EqualTo(deserialized));
        }
    }
}