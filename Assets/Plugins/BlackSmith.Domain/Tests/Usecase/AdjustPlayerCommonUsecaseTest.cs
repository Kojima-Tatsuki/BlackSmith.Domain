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

namespace BlackSmith.Usecase.Character
{
    internal class AdjustPlayerCommonEntityUsecaseTest
    {
        private static IEnumerable CreateCharacterTestCases()
        {
            var repository = new MockCharacterCommonEntityRepository();
            var name = "TestPlayerName";

            yield return new TestCaseData(repository, name, null).SetCategory("正常系");

            // PlayerNameでコケる場合
            yield return new TestCaseData(repository, "", typeof(ArgumentException)).SetCategory("異常系");
        }

        [Test(Description = "CreateCharacterのテスト")]
        [TestCaseSource(nameof(CreateCharacterTestCases))]
        public async Task CreateCharacterPasses(ICharacterCommonEntityRepository repository, string name, Type? exception)
        {
            var usecase = new AdjustPlayerCommonEntityUsecase(repository);

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

        private static IEnumerable ReconstructCharacterTestCases()
        {
            var repository = new MockCharacterCommonEntityRepository();
            var name = "TestPlayerName";
            var id = new CharacterID();
            var level = new CharacterLevel();

            var command = new CommonCharacterReconstructCommand(id.Value, name, level.CumulativeExp.Value);

            yield return new TestCaseData(repository, command, null).SetCategory("正常系");

            // 既に同じIDのプレイヤーが存在する場合
            var entity = PlayerFactory.Reconstruct(new CommonCharacterReconstructCommand(id, new CharacterName(name), level));
            var failRep = new MockCharacterCommonEntityRepository(new Dictionary<CharacterID, CommonCharacterEntity>() { { entity.ID, entity } });
            yield return new TestCaseData(failRep, command, typeof(InvalidOperationException)).SetCategory("異常系");
        }

        [Test(Description = "ReconstructCharacterのテスト")]
        [TestCaseSource(nameof(ReconstructCharacterTestCases))]
        public async Task ReconstructCharacterPasses(ICharacterCommonEntityRepository repository, CommonCharacterReconstructCommand command, Type? exception)
        {
            var usecase = new AdjustPlayerCommonEntityUsecase(repository);

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

        private static IEnumerable DeleteCharacterTestCases()
        {
            var name = "TestPlayerName";
            var id = new CharacterID();
            var level = new CharacterLevel();

            var entity = PlayerFactory.Reconstruct(new(id.Value, name, level.CumulativeExp.Value));

            var repository = new MockCharacterCommonEntityRepository(new Dictionary<CharacterID, CommonCharacterEntity>
            {
                { entity.ID, entity }
            });

            yield return new TestCaseData(repository, id, null).SetCategory("正常系");

            // 存在しないIDのキャラクターを削除しようとした場合
            var characterId = new CharacterID();
            yield return new TestCaseData(repository, characterId, typeof(InvalidOperationException)).SetCategory("異常系");
        }

        [Test(Description = "DeletePlayerのテスト")]
        [TestCaseSource(nameof(DeleteCharacterTestCases))]
        public async Task DeleteCharacterPasses(ICharacterCommonEntityRepository repository, CharacterID id, Type? exception)
        {
            var usecase = new AdjustPlayerCommonEntityUsecase(repository);

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

        private static IEnumerable GetCharacterTestCases()
        {
            var repository = new MockCharacterCommonEntityRepository();

            var name = "TestPlayerName";
            var id = new CharacterID();
            var level = new CharacterLevel();

            var entity = PlayerFactory.Reconstruct(new(id.Value, name, level.CumulativeExp.Value));

            repository.Register(entity).Forget();

            yield return new TestCaseData(repository, id, entity, null).SetCategory("正常系");

            // 存在しないIDのキャラクターを取得しようとした場合
            var donotExistId = new CharacterID();
            yield return new TestCaseData(repository, donotExistId, null, null).SetCategory("正常系");
        }

        [Test(Description = "GetPlayerDataのテスト")]
        [TestCaseSource(nameof(GetCharacterTestCases))]
        public async Task GetCharacterPasses(ICharacterCommonEntityRepository repository, CharacterID id, CommonCharacterEntity? target, Type? exception)
        {
            var usecase = new AdjustPlayerCommonEntityUsecase(repository);

            if (exception is null)
            {
                var character = await usecase.GetCharacter(id);

                Assert.That(target, Is.EqualTo(character));
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

    internal class MockCharacterCommonEntityRepository : ICharacterCommonEntityRepository
    {
        private Dictionary<CharacterID, CommonCharacterEntity> players;

        internal MockCharacterCommonEntityRepository()
        {
            players = new Dictionary<CharacterID, CommonCharacterEntity>();
        }

        internal MockCharacterCommonEntityRepository(Dictionary<CharacterID, CommonCharacterEntity> players)
        {
            this.players = players;
        }

        public async UniTask Delete(CharacterID id)
        {
            if (!await IsExist(id))
                throw new InvalidOperationException("削除対象のキャラクターが存在しません");

            var removed = players.Remove(id);
        }

        public async UniTask<CommonCharacterEntity?> FindByID(CharacterID id)
        {
            await UniTask.CompletedTask;
            return players[id] ?? throw new InvalidOperationException();
        }

        public async UniTask<bool> IsExist(CharacterID id)
        {
            await UniTask.CompletedTask;
            var result = players.ContainsKey(id);
            return result;
        }

        public async UniTask Register(CommonCharacterEntity character)
        {
            if (await IsExist(character.ID))
                throw new InvalidOperationException("既にキャラクターが登録されています");

            players.Add(character.ID, character);
        }

        public async UniTask UpdateCharacter(CommonCharacterEntity character)
        {
            await UniTask.CompletedTask;
            players[character.ID] = character;
        }
    }
}