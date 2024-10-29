using BlackSmith.Domain.Character;
using BlackSmith.Domain.Character.Player;
using NUnit.Framework;
using System.Collections;

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
            var name = new PlayerName("TestPlayerName");
            var level = new PlayerLevel(new Experience());

            var command = new PlayerCommonReconstractCommand(id, name, level);

            var serialized = PlayerCommonEntityProvidUsecase.Serialize(command);

            var commandDeseroalized = PlayerCommonEntityProvidUsecase.Deserialize(serialized);

            Assert.That(command, Is.EqualTo(commandDeseroalized));
        }
    }
}
