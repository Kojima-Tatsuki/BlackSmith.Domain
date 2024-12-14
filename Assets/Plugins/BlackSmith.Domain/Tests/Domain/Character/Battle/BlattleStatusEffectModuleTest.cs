using BlackSmith.Domain.PassiveEffect;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlackSmith.Domain.Character.Battle
{
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

        [Test(Description = "ステータスモジュールのインスタンスを生成するテスト")]
        [TestCaseSource(nameof(CorrectMockData), Category = "正常系")]
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

        [Test(Description = "BattleStatusEffectModuleのシリアライズ・デシリアライズテスト")]
        public void BattleStatusEffectModuleSerializeTestPasses()
        {
            var dict = new Dictionary<EffectID, BattleStatusEffect>()
            {
                { new EffectID(), new BattleStatusEffect(new EffectID(), new BattleStatusEffectModel(0, 0, 0, 0)) },
            };
            var module = new BattleStatusEffectModule(dict);

            Debug.Log(module);
            var serialized = JsonConvert.SerializeObject(module);
            Debug.Log(serialized);
            var deserialized = JsonConvert.DeserializeObject<BattleStatusEffectModule>(serialized);

            Assert.That(module, Is.EqualTo(deserialized));
        }
    }
}