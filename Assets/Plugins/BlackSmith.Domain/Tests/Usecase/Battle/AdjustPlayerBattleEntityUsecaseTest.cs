using BlackSmith.Usecase.Interface;
using Cysharp.Threading.Tasks;
using System;
using NUnit.Framework;
using System.Collections;
using BlackSmith.Domain.Character.Player;
using BlackSmith.Domain.Character;
using System.Threading.Tasks;
using BlackSmith.Domain.Character.Battle;

#nullable enable

namespace BlackSmith.Usecase.Character.Battle
{
    internal class AdjustPlayerBattleEntityUsecaseTest
    {
        [Test(Description = "CreateCharacterのテスト")]
        public async Task CreateCharacterPasses()
        {
            var repository = new MockPlayerBattleEntityRepository();
            var commonEntityCommand = new PlayerCommonReconstructCommand(new CharacterID(), new PlayerName("TestCommonEntity"), new PlayerLevel(Experience.RequiredCumulativeExp(1)));
            var commonEntity = PlayerFactory.Reconstruct(commonEntityCommand);

            var usecase = new AdjustPlayerBattleEntityUsecase(repository);

            var character = await usecase.CreateCharacter(commonEntity);

            var entity = await repository.FindByID(character.ID);

            if (entity is null)
                throw new NullReferenceException("RepositoryにBattleCharacterEntityが存在しません");

            Assert.That(character.Equals(entity));
        }

        private static IEnumerable ReconstructCharacterTestCases()
        {
            var repository = new MockPlayerBattleEntityRepository();
            var commonEntityCommand = new PlayerCommonReconstructCommand(new CharacterID(), new PlayerName("TestCommonEntity"), new PlayerLevel(Experience.RequiredCumulativeExp(1)));
            var commonEntity = PlayerFactory.Reconstruct(commonEntityCommand);
            var command = new PlayerBattleReconstructCommand(commonEntity.ID, new CharacterBattleModule(new HealthPoint(commonEntity.Level), new LevelDependentParameters(commonEntity.Level, new Strength(1), new Agility(1)), new BattleEquipmentModule(null, null), new BattleStatusEffectModule(null)));

            yield return new TestCaseData(repository, command, null).SetCategory("正常系");

            // 既に同じIDのプレイヤーが存在する場合
            yield return new TestCaseData(repository, command, typeof(InvalidOperationException)).SetCategory("異常系");
        }

        [Test(Description = "ReconstructCharacterのテスト")]
        [TestCaseSource(nameof(ReconstructCharacterTestCases))]
        public async Task ReconstructCharacterPasses(IPlayerBattleEntityRepository repository, PlayerBattleReconstructCommand command, Type? exception)
        {
            var usecase = new AdjustPlayerBattleEntityUsecase(repository);

            if (exception is null)
            {
                var character = await usecase.ReconstructPlayer(command);

                var entity = await repository.FindByID(character.ID);

                if (entity is null)
                    throw new NullReferenceException("RepositoryにBattleCharacterEntityが存在しません");

                Assert.That(character.Equals(entity));
            }
            else
            {
                try
                {
                    await usecase.ReconstructPlayer(command);
                }
                catch (Exception e)
                {
                    Assert.AreEqual(exception, e.GetType());
                }
            }
        }

        private static IEnumerable DeleteCharacterTestCases()
        {
            var repository = new MockPlayerBattleEntityRepository();
            var commonEntityCommand = new PlayerCommonReconstructCommand(new CharacterID(), new PlayerName("TestCommonEntity"), new PlayerLevel(Experience.RequiredCumulativeExp(1)));
            var commonEntity = PlayerFactory.Reconstruct(commonEntityCommand);
            var command = new PlayerBattleReconstructCommand(commonEntity.ID, new CharacterBattleModule(new HealthPoint(commonEntity.Level), new LevelDependentParameters(commonEntity.Level, new Strength(1), new Agility(1)), new BattleEquipmentModule(null, null), new BattleStatusEffectModule(null)));

            yield return new TestCaseData(repository, command, null).SetCategory("正常系");

            // 存在しないIDのプレイヤーを削除しようとした場合
            yield return new TestCaseData(repository, command, typeof(InvalidOperationException)).SetCategory("異常系");
        }

        [Test(Description = "DeleteCharacterのテスト")]
        [TestCaseSource(nameof(DeleteCharacterTestCases))]
        public async Task DeleteCharacterPasses(IPlayerBattleEntityRepository repository, PlayerBattleReconstructCommand command, Type? exception)
        {
            var usecase = new AdjustPlayerBattleEntityUsecase(repository);

            if (exception is null)
            {
                var character = await usecase.ReconstructPlayer(command);

                Assert.That(await repository.IsExist(character.ID), Is.True);

                await usecase.DeletePlayer(character.ID);

                Assert.That(await repository.IsExist(character.ID), Is.False);
            }
            else
            {
                try
                {
                    await usecase.DeletePlayer(new CharacterID());
                }
                catch (Exception e)
                {
                    Assert.AreEqual(exception, e.GetType());
                }
            }
        }

        private static IEnumerable GetCharacterTestCases()
        {
            var repository = new MockPlayerBattleEntityRepository();
            var commonEntityCommand = new PlayerCommonReconstructCommand(new CharacterID(), new PlayerName("TestCommonEntity"), new PlayerLevel(Experience.RequiredCumulativeExp(1)));
            var commonEntity = PlayerFactory.Reconstruct(commonEntityCommand);
            var command = new PlayerBattleReconstructCommand(commonEntity.ID, new CharacterBattleModule(new HealthPoint(commonEntity.Level), new LevelDependentParameters(commonEntity.Level, new Strength(1), new Agility(1)), new BattleEquipmentModule(null, null), new BattleStatusEffectModule(null)));

            yield return new TestCaseData(repository, command, null).SetCategory("正常系");

            // 存在しないIDのプレイヤーを取得しようとした場合
            yield return new TestCaseData(repository, command, typeof(InvalidOperationException)).SetCategory("異常系");
        }

        [Test(Description = "GetCharacterのテスト")]
        [TestCaseSource(nameof(GetCharacterTestCases))]
        public async Task GetCharacterPasses(IPlayerBattleEntityRepository repository, PlayerBattleReconstructCommand command, Type? exception)
        {
            var usecase = new AdjustPlayerBattleEntityUsecase(repository);

            if (exception is null)
            {
                var character = await usecase.ReconstructPlayer(command);

                var entity = await usecase.GetCharacter(character.ID);

                if (entity is null)
                    throw new NullReferenceException("RepositoryにBattleCharacterEntityが存在しません");

                Assert.That(character.Equals(entity));
            }
            else
            {
                try
                {
                    await usecase.GetCharacter(new CharacterID());
                }
                catch (Exception e)
                {
                    Assert.AreEqual(exception, e.GetType());
                }
            }
        }
    }
}
