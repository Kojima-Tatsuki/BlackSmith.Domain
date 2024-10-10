using BlackSmith.Domain.Character;
using BlackSmith.Domain.Character.Player;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections;
using UnityEngine;

namespace BlackSmith.Usecase.Character.PlayerCommonEntityProvideUsecaseTest
{
    internal class BuildCommonEntityMethodTest
    {
        private static IEnumerable BuildCommonEntityTestCases()
        {
            var id = new CharacterID();
            var name = new PlayerName("TestPlayerName");
            var level = new PlayerLevel(new Experience());

            var command = new PlayerCommonReconstractCommand(id, name, level);

            yield return new TestCaseData(command).SetCategory("正常系");
        }

        [Test(Description = "BuildCommonEntityのテスト")]
        [TestCaseSource(nameof(BuildCommonEntityTestCases))]
        public void BuildCommonEntityPasses(PlayerCommonReconstractCommand command)
        {
            var entity = PlayerCommonEntityProvidUsecase.BuildCommonEntity(command);

            Assert.That(entity, Is.Not.Null); // エラーが出ずインスタンスが返ればOK
        }

        [Test(Description = "Commandのシリアライズ・デシリアライズのテスト")]
        public void CommandSerializationTest()
        {
            var id = new CharacterID();
            var idSerialized = JsonConvert.SerializeObject(id);
            Debug.Log(idSerialized);
            var idDeserialized = JsonConvert.DeserializeObject<CharacterID>(idSerialized);
            Debug.Log(idSerialized + ", " + idDeserialized);

            Assert.That(id, Is.EqualTo(idDeserialized));
            

            var name = new PlayerName("TestPlayerName");
            var nameSerialized = JsonConvert.SerializeObject(name);
            var nameDeserialized = JsonConvert.DeserializeObject<PlayerName>(nameSerialized);

            Assert.That(name, Is.EqualTo(nameDeserialized));


            var level = new PlayerLevel(new Experience());
            var levelSerialized = JsonConvert.SerializeObject(level);
            var levelDeserialized = JsonConvert.DeserializeObject<PlayerLevel>(levelSerialized);

            Assert.That(level, Is.EqualTo(levelDeserialized));


            var command = new PlayerCommonReconstractCommand(id, name, level);
            var commandSerialized = JsonConvert.SerializeObject(command);
            var commandDeseroalized = JsonConvert.DeserializeObject<PlayerCommonReconstractCommand>(commandSerialized);

            Assert.That(command, Is.EqualTo(commandDeseroalized));
        }
    }
}
