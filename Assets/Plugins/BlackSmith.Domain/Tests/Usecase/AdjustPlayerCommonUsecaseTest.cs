using BlackSmith.Domain.Character;
using BlackSmith.Domain.Character.Player;
using BlackSmith.Usecase.Interface;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.TestTools;
using static BlackSmith.Usecase.Character.AdjustPlayerCommonUsecase;

#nullable enable

namespace BlackSmith.Usecase.Character
{
    internal class AdjustPlayerCommonUsecaseTest
    {
        private static IEnumerable CreateCharacterTestCases()
        {
            var repository = new MockPlayerCommonEntityRepository();
            var name = "TestPlayerName";

            yield return new TestCaseData(repository, name, null).SetCategory("正常系");

            // PlayerNameでコケる場合
            yield return new TestCaseData(repository, "", typeof(ArgumentException)).SetCategory("異常系");
        }

        private static IEnumerable ReconstructCharacterTestCases()
        {
            var repository = new MockPlayerCommonEntityRepository();
            var name = "TestPlayerName";
            var id = new CharacterID();
            var level = new PlayerLevel();

            var model = new PlayerCommonReconstractPrimitiveModel(id.Value, name, level.CumulativeExp.Value);

            yield return new TestCaseData(repository, model, null).SetCategory("正常系");

            // modelのToCommandでコケる場合
            var failModel = new PlayerCommonReconstractPrimitiveModel(id.Value, "", level.CumulativeExp.Value);
            yield return new TestCaseData(repository, failModel, typeof(ArgumentException)).SetCategory("異常系");

            // 既に同じIDのプレイヤーが存在する場合
            var entity = new PlayerCommonEntity(new PlayerCommonReconstractCommand(id, new PlayerName(name), level));
            var failRep = new MockPlayerCommonEntityRepository(new Dictionary<CharacterID, PlayerCommonEntity>() { { entity.ID, entity } });
            yield return new TestCaseData(failRep, model, typeof(InvalidOperationException)).SetCategory("異常系");
        }

        private static IEnumerable DeleteCharacterTestCases()
        {
            var repository = new MockPlayerCommonEntityRepository();
            var name = "TestPlayerName";
            var id = new CharacterID();
            var level = new PlayerLevel();

            var model = new PlayerCommonReconstractPrimitiveModel(id.Value, name, level.CumulativeExp.Value);

            var usecase = new AdjustPlayerCommonUsecase(repository);
            yield return UniTask.ToCoroutine(async () => await usecase.ReconstructPlayer(model));

            yield return new TestCaseData(repository, id, null).SetCategory("正常系");

            // 存在しないIDのプレイヤーを削除しようとした場合
            yield return new TestCaseData(repository, new CharacterID(), typeof(InvalidOperationException)).SetCategory("異常系");
        }

        // CreateCharacterのテスト
        [UnityTest]
        [TestCaseSource(nameof(CreateCharacterTestCases))]
        public IEnumerator CreateCharacterPasses(IPlayerCommonEntityRepository repository, string name, Type? exception) => UniTask.ToCoroutine(async () =>
        {
            var usecase = new AdjustPlayerCommonUsecase(repository);

            if (exception is null)
            {
                var character = await usecase.CreateCharacter(name);

                Assert.That(character.Equals(await repository.FindByID(character.ID) ?? throw new NullReferenceException()));
            }
            else
                Assert.Throws(exception, async () => await usecase.CreateCharacter(name));
        });

        // ReconstructCharacterのテスト
        [UnityTest]
        [TestCaseSource(nameof(ReconstructCharacterTestCases))]
        public IEnumerator ReconstructCharacterPasses(IPlayerCommonEntityRepository repository, PlayerCommonReconstractPrimitiveModel model, Type? exception) => UniTask.ToCoroutine(async () =>
        {
            var usecase = new AdjustPlayerCommonUsecase(repository);

            if (exception is null)
            {
                var character = await usecase.ReconstructPlayer(model);

                Assert.That(character.Equals(await repository.FindByID(character.ID) ?? throw new NullReferenceException())); ;
            }
            else
                Assert.Throws(exception, async () => await usecase.ReconstructPlayer(model));
        });

        // DeletePlayerのテスト
        [UnityTest]
        [TestCaseSource(nameof(this.DeleteCharacterTestCases))]
        public void DeleteCharacterPasses(IPlayerCommonEntityRepository repository, CharacterID id, Type? exception) => UniTask.ToCoroutine(async () =>
        {
            var usecase = new AdjustPlayerCommonUsecase(repository);

            if (exception is null)
            {
                Assert.That(await repository.IsExist(id), Is.True);

                await usecase.DeletePlayer(id);

                Assert.That(await repository.IsExist(id), Is.False);
            }
            else
                Assert.Throws(exception, async () => await usecase.DeletePlayer(id));
        });
    }

    internal class MockPlayerCommonEntityRepository : IPlayerCommonEntityRepository
    {
        private Dictionary<CharacterID, PlayerCommonEntity> players;

        internal MockPlayerCommonEntityRepository()
        {
            players = new Dictionary<CharacterID, PlayerCommonEntity>();
        }

        internal MockPlayerCommonEntityRepository(Dictionary<CharacterID, PlayerCommonEntity> players)
        {
            this.players = players;
        }

        public async UniTask Delete(CharacterID id)
        {
            if (!(await IsExist(id)))
                throw new InvalidOperationException("削除対象のプレイヤーが存在しません");

            players.Remove(id);
        }

        public async UniTask<PlayerCommonEntity?> FindByID(CharacterID id)
        {
            await UniTask.Delay(10);
            return players[id] ?? throw new InvalidOperationException();
        }

        public async UniTask<bool> IsExist(CharacterID id)
        {
            await UniTask.Delay(10);
            return players.ContainsKey(id);
        }

        public async UniTask Register(PlayerCommonEntity character)
        {
            if (await IsExist(character.ID))
                throw new InvalidOperationException("既にプレイヤーが登録されています");

            players.Add(character.ID, character);
        }

        public async UniTask UpdateCharacter(PlayerCommonEntity character)
        {
            await UniTask.Delay(10);
            players[character.ID] = character;
        }
    }
}