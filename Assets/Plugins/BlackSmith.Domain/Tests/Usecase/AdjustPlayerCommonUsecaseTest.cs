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

#nullable enable

namespace BlackSmith.Usecase.Character.AdjustPlayerCommonUsecaseTest
{
    internal class CreateCharacterMethodTest
    {
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
                    Assert.AreEqual(exception, e.GetType());
                }
            }
        }
    }

    internal class ReconstructCharacterMethodTest
    {
        private static IEnumerable ReconstructCharacterTestCases()
        {
            var repository = new MockPlayerCommonEntityRepository();
            var name = "TestPlayerName";
            var id = new CharacterID();
            var level = new PlayerLevel();

            var command = new PlayerCommonReconstractCommand(id.Value, name, level.CumulativeExp.Value);

            yield return new TestCaseData(repository, command, null).SetCategory("正常系");

            // 既に同じIDのプレイヤーが存在する場合
            var entity = PlayerFactory.Reconstruct(new PlayerCommonReconstractCommand(id, new PlayerName(name), level));
            var failRep = new MockPlayerCommonEntityRepository(new Dictionary<CharacterID, PlayerCommonEntity>() { { entity.ID, entity } });
            yield return new TestCaseData(failRep, command, typeof(InvalidOperationException)).SetCategory("異常系");
        }

        [Test(Description = "ReconstructCharacterのテスト")]
        [TestCaseSource(nameof(ReconstructCharacterTestCases))]
        public async Task ReconstructCharacterPasses(IPlayerCommonEntityRepository repository, PlayerCommonReconstractCommand command, Type? exception)
        {
            var usecase = new AdjustPlayerCommonUsecase(repository);

            if (exception is null)
            {
                var character = await usecase.ReconstructPlayer(command);

                var entity = await repository.FindByID(character.ID);

                if (entity is null)
                    throw new NullReferenceException();

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
    }

    internal class DeleteCharacterMethodTest
    {
        private static IEnumerable DeleteCharacterTestCases()
        {
            var name = "TestPlayerName";
            var id = new CharacterID();
            var level = new PlayerLevel();

            var entity = PlayerFactory.Reconstruct(new(id.Value, name, level.CumulativeExp.Value));

            var repository = new MockPlayerCommonEntityRepository(new Dictionary<CharacterID, PlayerCommonEntity>
            {
                { entity.ID, entity }
            });

            yield return new TestCaseData(repository, id, null).SetCategory("正常系");

            // 存在しないIDのプレイヤーを削除しようとした場合
            var characterId = new CharacterID();
            yield return new TestCaseData(repository, characterId, typeof(InvalidOperationException)).SetCategory("異常系");
        }

        [Test(Description = "DeletePlayerのテスト")]
        [TestCaseSource(nameof(DeleteCharacterTestCases))]
        public async Task DeleteCharacterPasses(IPlayerCommonEntityRepository repository, CharacterID id, Type? exception)
        {
            var usecase = new AdjustPlayerCommonUsecase(repository);

            if (exception is null)
            {

                Assert.That(await repository.IsExist(id), Is.True);

                await usecase.DeletePlayer(id);

                Assert.That(await repository.IsExist(id), Is.False);

                await repository.Register(PlayerFactory.Reconstruct(new(id.Value, "TestPlayerName", 0))); // 消した後は追加して、元に戻す
            }
            else
            {
                try
                {
                    await usecase.DeletePlayer(id);

                    // この書き方だとThrowsAsyncで非同期処理が走ってくれない
                    // await Assert.ThrowsAsync(exception, async () => await usecase.DeletePlayer(id).AsTask());
                }
                catch (Exception e)
                {
                    Assert.AreEqual(exception, e.GetType());
                }
            }
        }
    }

    internal class GetCharacterMethodTest
    {
        private static IEnumerable GetCharacterTestCases()
        {
            var repository = new MockPlayerCommonEntityRepository();

            var name = "TestPlayerName";
            var id = new CharacterID();
            var level = new PlayerLevel();

            var entity = PlayerFactory.Reconstruct(new(id.Value, name, level.CumulativeExp.Value));

            repository.Register(entity).Forget();

            yield return new TestCaseData(repository, id, entity, null).SetCategory("正常系");

            // 存在しないIDのプレイヤーを取得しようとした場合
            var donotExistId = new CharacterID();
            yield return new TestCaseData(repository, donotExistId, null, typeof(ArgumentException)).SetCategory("異常系");
        }

        [Test(Description = "GetPlayerDataのテスト")]
        [TestCaseSource(nameof(GetCharacterTestCases))]
        public async Task GetCharacterPasses(IPlayerCommonEntityRepository repository, CharacterID id, PlayerCommonEntity? target, Type? exception)
        {
            var usecase = new AdjustPlayerCommonUsecase(repository);

            if (exception is null)
            {
                var character = await usecase.GetCharacter(id);

                if (target is null)
                    throw new InvalidOperationException("比較対象が引数で与えられていません");

                Assert.That(character.Equals(target));
            }
            else
            {
                try
                {
                    await usecase.GetCharacter(id);
                }
                catch (Exception e)
                {
                    Assert.AreEqual(exception, e.GetType());
                }
            }
        }
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
            if (!await IsExist(id))
                throw new InvalidOperationException("削除対象のプレイヤーが存在しません");

            var removed = players.Remove(id);
        }

        public async UniTask<PlayerCommonEntity?> FindByID(CharacterID id)
        {
            await UniTask.Delay(10, delayType: DelayType.Realtime);
            return players[id] ?? throw new InvalidOperationException();
        }

        public async UniTask<bool> IsExist(CharacterID id)
        {
            await UniTask.Delay(10, delayType: DelayType.Realtime);
            var result = players.ContainsKey(id);
            return result;
        }

        public async UniTask Register(PlayerCommonEntity character)
        {
            if (await IsExist(character.ID))
                throw new InvalidOperationException("既にプレイヤーが登録されています");

            players.Add(character.ID, character);
        }

        public async UniTask UpdateCharacter(PlayerCommonEntity character)
        {
            await UniTask.Delay(10, delayType: DelayType.Realtime);
            players[character.ID] = character;
        }
    }
}