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
            var name = new CharacterName("TestPlayerName");
            var level = new CharacterLevel(new Experience());

            var command = new CommonCharacterReconstructCommand(id, name, level);

            yield return new TestCaseData(command).SetCategory("正常系");
        }

        [Test(Description = "BuildCommonEntityのテスト")]
        [TestCaseSource(nameof(BuildCommonEntityTestCases))]
        public void BuildCommonEntityPasses(CommonCharacterReconstructCommand command)
        {
            var entity = PlayerCommonEntityProvideUsecase.BuildCommonEntity(command);

            Assert.That(entity, Is.Not.Null); // エラーが出ずインスタンスが返ればOK
        }
    }
}
