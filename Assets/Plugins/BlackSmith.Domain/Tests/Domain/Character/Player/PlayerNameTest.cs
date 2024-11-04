using Newtonsoft.Json;
using NUnit.Framework;

namespace BlackSmith.Domain.Character.Player
{
    internal class PlayerNameTest
    {
        [Test(Description = "PlayerNameのシリアライズ・デシリアライズテスト")]
        public void PlayerNameSerializeTestPasses()
        {
            var playerName = new PlayerName("TestPlayerName");

            var serialized = JsonConvert.SerializeObject(playerName);
            var deserialized = JsonConvert.DeserializeObject<PlayerName>(serialized);

            Assert.That(playerName, Is.EqualTo(deserialized));
        }
    }
}