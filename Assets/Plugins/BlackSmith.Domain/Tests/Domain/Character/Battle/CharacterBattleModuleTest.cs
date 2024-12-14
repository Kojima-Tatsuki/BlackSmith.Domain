using BlackSmith.Domain.Character.Player;
using Newtonsoft.Json;
using NUnit.Framework;

namespace BlackSmith.Domain.Character.Battle
{
    internal class CharacterBattleModuleTest
    {
        [Test(Description = "CharacterBattleModuleのシリアライズ・デシリアライズテスト")]
        public void CharacterBattleModuleSerializeTestPasses()
        {
            var health = new HealthPoint(new HealthPointValue(100), new MaxHealthPointValue(100));
            var levelDependParams = new LevelDependentParameters();
            var equipmentModule = new BattleEquipmentModule(null, null);
            var statusModule = new BattleStatusEffectModule();
            var battleModule = new CharacterBattleModule(health, levelDependParams, equipmentModule, statusModule);

            var serialized = JsonConvert.SerializeObject(battleModule);
            var deserialized = JsonConvert.DeserializeObject<CharacterBattleModule>(serialized);

            Assert.That(battleModule, Is.EqualTo(deserialized));
        }
    }
}
