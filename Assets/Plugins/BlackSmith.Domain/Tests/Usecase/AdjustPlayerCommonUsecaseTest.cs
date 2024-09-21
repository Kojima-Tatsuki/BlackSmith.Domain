using BlackSmith.Domain.Character;
using BlackSmith.Domain.Character.Player;
using BlackSmith.Usecase.Interface;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static BlackSmith.Usecase.Character.AdjustPlayerCommonUsecase;

#nullable enable

namespace BlackSmith.Usecase.Character
{
    internal class AdjustPlayerCommonUsecaseTest
    {
        #region CreateCharacter
        private static IEnumerable CreateCharacterTestCases()
        {
            var repository = new MockPlayerCommonEntityRepository();
            var name = "TestPlayerName";

            yield return new TestCaseData(repository, name, null).SetCategory("正常系");

            // PlayerNameでコケる場合
            yield return new TestCaseData(repository, "", typeof(ArgumentException)).SetCategory("異常系");
        }

        [Test(Description = "CreateCharacterのテスト")]
        [TestCaseSource(nameof(CreateCharacterTestCases))]
        public async Task CreateCharacterPasses(IPlayerCommonEntityRepository repository, string name, Type? exception)
        {
            var usecase = new AdjustPlayerCommonUsecase(repository);

            if (exception is null)
            {
                var character = await usecase.CreateCharacter(name);

                var entity = await repository.FindByID(character.ID);

                if (entity is null)
                    throw new NullReferenceException();

                Assert.That(character.Equals(entity));
            }
            else
            {
                try
                {
                    Assert.ThrowsAsync(exception, async () => await usecase.CreateCharacter(name));
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                    Assert.AreEqual(exception, e.GetType());
                }
            }
        }
        #endregion

        #region ReconstructCharacter
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

        [Test(Description = "ReconstructCharacterのテスト")]
        [TestCaseSource(nameof(ReconstructCharacterTestCases))]
        public async Task ReconstructCharacterPasses(IPlayerCommonEntityRepository repository, PlayerCommonReconstractPrimitiveModel model, Type? exception)
        {
            var usecase = new AdjustPlayerCommonUsecase(repository);

            if (exception is null)
            {
                var character = await usecase.ReconstructPlayer(model);

                var entity = await repository.FindByID(character.ID);

                if (entity is null)
                    throw new NullReferenceException();

                Assert.That(character.Equals(entity));
            }
            else
            {
                try
                {
                    await usecase.ReconstructPlayer(model);
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                    Assert.AreEqual(exception, e.GetType());
                }
            }
        }
        #endregion

        #region DeleteCharacter
        private static IEnumerable DeleteCharacterTestCases()
        {
            var repository = new MockPlayerCommonEntityRepository();

            var name = "TestPlayerName";
            var id = new CharacterID();
            var level = new PlayerLevel();

            var model = new PlayerCommonReconstractPrimitiveModel(id.Value, name, level.CumulativeExp.Value);

            Debug.Log("Reconstructing player");
            var usecase = new AdjustPlayerCommonUsecase(repository);
            UniTask.Void(async () => await usecase.ReconstructPlayer(model));

            yield return new TestCaseData(repository, id, null).SetCategory("正常系");

            // 存在しないIDのプレイヤーを削除しようとした場合
            var characterId = new CharacterID();
            yield return new TestCaseData(repository, characterId, typeof(InvalidOperationException)).SetCategory("異常系");
        }

        [Test(Description = "DeletePlayerのテスト")]
        [TestCaseSource(nameof(DeleteCharacterTestCases))]
        public async Task DeleteCharacterPasses(IPlayerCommonEntityRepository repository, CharacterID id, Type? exception)
        {
            Debug.Log("Start Delete Test");
            var usecase = new AdjustPlayerCommonUsecase(repository);

            Debug.Log("Created Usecase");

            if (exception is null)
            {
                Assert.That(await repository.IsExist(id), Is.True);

                await usecase.DeletePlayer(id);

                Assert.That(await repository.IsExist(id), Is.False);
            }
            else
            {
                Debug.Log("Start Exception Test");
                try
                {
                    Debug.Log("in Try");
                    await usecase.DeletePlayer(id);

                    // この書き方だとThrowsAsyncで非同期処理が走ってくれない
                    // await Assert.ThrowsAsync(exception, async () => await usecase.DeletePlayer(id).AsTask());
                }
                catch (Exception e)
                {
                    Debug.Log($"{e.Message}, {e.GetType()}");
                    Assert.AreEqual(exception, e.GetType());
                }
            }
        }
        #endregion
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
            Debug.Log("Start Delete");
            if (!await IsExist(id))
                throw new InvalidOperationException("削除対象のプレイヤーが存在しません");

            var removed = players.Remove(id);
            Debug.Log($"Deleted, {removed}");
        }

        public async UniTask<PlayerCommonEntity?> FindByID(CharacterID id)
        {
            await UniTask.Delay(10, delayType: DelayType.Realtime);
            return players[id] ?? throw new InvalidOperationException();
        }

        public async UniTask<bool> IsExist(CharacterID id)
        {
            Debug.Log($"IsExist: {id}, Dictionary: {players.Count}");
            await UniTask.Delay(10, delayType: DelayType.Realtime);
            var result = players.ContainsKey(id);
            Debug.Log($"IsExist: {result}");
            return result;
        }

        public async UniTask Register(PlayerCommonEntity character)
        {
            if (await IsExist(character.ID))
                throw new InvalidOperationException("既にプレイヤーが登録されています");

            players.Add(character.ID, character);

            Debug.Log($"Registered Player: {character.ID}");
        }

        public async UniTask UpdateCharacter(PlayerCommonEntity character)
        {
            await UniTask.Delay(10, delayType: DelayType.Realtime);
            players[character.ID] = character;
        }
    }
}