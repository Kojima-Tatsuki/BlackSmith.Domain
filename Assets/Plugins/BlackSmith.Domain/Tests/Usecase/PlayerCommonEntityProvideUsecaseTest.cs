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

            var command = new PlayerCommonReconstructCommand(id, name, level);

            yield return new TestCaseData(command).SetCategory("正常系");
        }

        [Test(Description = "BuildCommonEntityのテスト")]
        [TestCaseSource(nameof(BuildCommonEntityTestCases))]
        public void BuildCommonEntityPasses(PlayerCommonReconstructCommand command)
        {
            var entity = PlayerCommonEntityProvideUsecase.BuildCommonEntity(command);

            Assert.That(entity, Is.Not.Null); // エラーが出ずインスタンスが返ればOK
        }
    }
}
