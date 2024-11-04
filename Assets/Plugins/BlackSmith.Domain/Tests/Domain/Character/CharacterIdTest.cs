using Newtonsoft.Json;
using NUnit.Framework;

namespace BlackSmith.Domain.Character
{
    internal class CharacterIdTest
    {
        [Test(Description = "CharacterIdの同一性のテスト")]
        public void CharacterIdEqualsPasses()
        {
            var characterId1 = new CharacterID();
            var characterId2 = new CharacterID(characterId1.Value);

            Assert.That(characterId1, Is.EqualTo(characterId2));
        }

        [Test(Description = "CharacterIdのシリアライズ・デシリアライズテスト")]
        public void CharacterIdSerializePasses()
        {
            var characterId = new CharacterID();

            var serialized = JsonConvert.SerializeObject(characterId);
            var deserialized = JsonConvert.DeserializeObject<CharacterID>(serialized);

            Assert.That(characterId, Is.EqualTo(deserialized));
        }
    }
}
