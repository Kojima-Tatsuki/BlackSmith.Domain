using Newtonsoft.Json;
using NUnit.Framework;

namespace BlackSmith.Domain.PassiveEffect
{
    public class BattleStatusEffectTest
    {
        [Test(Description = "BattleStatusEffectModuleのシリアライズ・デシリアライズテスト")]
        public void BattleStatusEffectModuleSerializeTestPasses()
        {
            var effect = new BattleStatusEffect(new EffectID(), new BattleStatusEffectModel(0, 0, 0, 0));

            var serialized = JsonConvert.SerializeObject(effect);
            var deserialized = JsonConvert.DeserializeObject<BattleStatusEffect>(serialized);

            Assert.That(effect, Is.EqualTo(deserialized));
        }
    }

    public class EffectIDTest
    {
        [Test(Description = "EffectIDのシリアライズ・デシリアライズテスト")]
        public void EffectIDSerializeTestPasses()
        {
            var id = new EffectID();

            var serialized = JsonConvert.SerializeObject(id);
            var deserialized = JsonConvert.DeserializeObject<EffectID>(serialized);

            Assert.That(id, Is.EqualTo(deserialized));
        }
    }

    public class BattleStatusEffectModelTest
    {
        [Test(Description = "BattleStatusEffectModelのシリアライズ・デシリアライズテスト")]
        public void BattleStatusEffectModelSerializeTestPasses()
        {
            var model = new BattleStatusEffectModel(0, 0, 0, 0);

            var serialized = JsonConvert.SerializeObject(model);
            var deserialized = JsonConvert.DeserializeObject<BattleStatusEffectModel>(serialized);

            Assert.That(model, Is.EqualTo(deserialized));
        }
    }
}