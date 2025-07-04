using Newtonsoft.Json;
using NUnit.Framework;

namespace BlackSmith.Domain.Character.Player
{
    internal class PlayerCommonReconstructCommandTest
    {
        [Test(Description = "CharacterCommonReconstructCommandのシリアライズ・デシリアライズテスト")]
        public void PlayerCommonReconstructCommandSerializeTestPasses()
        {
            var characterId = new CharacterID();
            var playerName = new CharacterName("TestPlayerName");
            var playerLevel = new CharacterLevel(new Experience(100));
            var command = new CommonCharacterReconstructCommand(characterId, playerName, playerLevel);

            var serialized = JsonConvert.SerializeObject(command);
            var deserialized = JsonConvert.DeserializeObject<CommonCharacterReconstructCommand>(serialized);

            Assert.That(command, Is.EqualTo(deserialized));
        }
    }
}
