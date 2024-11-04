using Newtonsoft.Json;
using NUnit.Framework;

namespace BlackSmith.Domain.Character.Player
{
    internal class PlayerCommonReconstructCommandTest
    {
        [Test(Description = "PlayerCommonReconstructCommandのシリアライズ・デシリアライズテスト")]
        public void PlayerCommonReconstructCommandSerializeTestPasses()
        {
            var characterId = new CharacterID();
            var playerName = new PlayerName("TestPlayerName");
            var playerLevel = new PlayerLevel(new Experience(100));
            var command = new PlayerCommonReconstructCommand(characterId, playerName, playerLevel);

            var serialized = JsonConvert.SerializeObject(command);
            var deserialized = JsonConvert.DeserializeObject(serialized);

            Assert.That(command, Is.EqualTo(deserialized));
        }
    }
}
