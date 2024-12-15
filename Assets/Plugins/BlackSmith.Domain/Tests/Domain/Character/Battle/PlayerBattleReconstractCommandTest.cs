using BlackSmith.Domain.Character.Player;
using Newtonsoft.Json;
using NUnit.Framework;

namespace BlackSmith.Domain.Character.Battle
{
    internal class PlayerBattleReconstractCommandTest
    {
        [Test(Description = "PlayerBattleReconstractCommandのシリアライズ・デシリアライズテスト")]
        public void PlayerBattleReconstractCommandSerializeTestPasses()
        {
            var id = new CharacterID();
            var health = new HealthPoint(new HealthPointValue(100), new MaxHealthPointValue(100));
            var levelDependParams = new LevelDependentParameters();
            var equipmentModule = new BattleEquipmentModule(null, null);
            var statusModule = new BattleStatusEffectModule();

            var module = new CharacterBattleModule(health, levelDependParams, equipmentModule, statusModule);
            var command = new PlayerBattleReconstructCommand(id, module);

            var serialized = JsonConvert.SerializeObject(command);
            var deserialized = JsonConvert.DeserializeObject<PlayerBattleReconstructCommand>(serialized);

            Assert.That(command, Is.EqualTo(deserialized));
        }
    }
}
