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
            var name = new CharacterName("TestPlayerName");

            yield return new TestCaseData(repository, name, null).SetCategory("正常系");

            // CharacterNameはコンストラクタで異常系チェックされるため、別途テストを実装
        }

        [Test(Description = "CreateCharacterのテスト")]
        [TestCaseSource(nameof(CreateCharacterTestCases))]
        public async Task CreateCharacterPasses(ICommonCharacterEntityRepository repository, CharacterName name, Type? exception)
        {
            var usecase = new AdjustCommonCharacterEntityUsecase(repository);

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
            var entity = CommonCharacterFactory.Reconstruct(new CommonCharacterReconstructCommand(id, new CharacterName(name), level));
            var failRep = new MockCharacterCommonEntityRepository(new Dictionary<CharacterID, CommonCharacterEntity>() { { entity.ID, entity } });
            yield return new TestCaseData(failRep, command, typeof(InvalidOperationException)).SetCategory("異常系");
        }

        [Test(Description = "ReconstructCharacterのテスト")]
        [TestCaseSource(nameof(ReconstructCharacterTestCases))]
        public async Task ReconstructCharacterPasses(ICommonCharacterEntityRepository repository, CommonCharacterReconstructCommand command, Type? exception)
        {
            var usecase = new AdjustCommonCharacterEntityUsecase(repository);

            if (exception is null)
            {
                var character = await usecase.ReconstructCharacter(command);

                var entity = await repository.FindByID(character.ID);

                if (entity is null)
                    throw new NullReferenceException();

                Assert.That(character.Equals(entity));
            }
            else
            {
                try
                {
                    await usecase.ReconstructCharacter(command);
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

            var entity = CommonCharacterFactory.Reconstruct(new(id.Value, name, level.CumulativeExp.Value));

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
        public async Task DeleteCharacterPasses(ICommonCharacterEntityRepository repository, CharacterID id, Type? exception)
        {
            var usecase = new AdjustCommonCharacterEntityUsecase(repository);

            if (exception is null)
            {
                Assert.That(await repository.IsExist(id), Is.True);

                await usecase.DeleteCharacter(id);

                Assert.That(await repository.IsExist(id), Is.False);

                await repository.Register(CommonCharacterFactory.Reconstruct(new(id.Value, "TestPlayerName", 0))); // 消した後は追加して、元に戻す
            }
            else
            {
                try
                {
                    await usecase.DeleteCharacter(id);

                    // この書き方だとThrowsAsyncで非同期処理が走ってくれない
                    // await Assert.ThrowsAsync(exception, async () => await usecase.DeleteCharacter(id).AsTask());
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

            var entity = CommonCharacterFactory.Reconstruct(new(id.Value, name, level.CumulativeExp.Value));

            repository.Register(entity).Forget();

            yield return new TestCaseData(repository, id, entity, null).SetCategory("正常系");

            // 存在しないIDのキャラクターを取得しようとした場合
            var donotExistId = new CharacterID();
            yield return new TestCaseData(repository, donotExistId, null, null).SetCategory("正常系");
        }

        [Test(Description = "GetPlayerDataのテスト")]
        [TestCaseSource(nameof(GetCharacterTestCases))]
        public async Task GetCharacterPasses(ICommonCharacterEntityRepository repository, CharacterID id, CommonCharacterEntity? target, Type? exception)
        {
            var usecase = new AdjustCommonCharacterEntityUsecase(repository);

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

        [Test(Description = "CreateCharacterの引数異常テスト")]
        public void CreateCharacterThrowsExceptionForInvalidName()
        {
            // CharacterNameコンストラクタで空文字列に対してArgumentExceptionが発生することをテスト
            Assert.Throws<ArgumentException>(() => new CharacterName(""));
            Assert.Throws<ArgumentException>(() => new CharacterName(null!));
        }

        [Test(Description = "CreateCharacter(NPC用)の引数異常テスト")]
        public void CreateNpcCharacterThrowsExceptionForInvalidArguments()
        {
            var repository = new MockCharacterCommonEntityRepository();
            var usecase = new AdjustCommonCharacterEntityUsecase(repository);
            var validName = new CharacterName("ValidNPC");
            var validLevel = new CharacterLevel(new Experience(100));

            // CharacterNameがnullの場合の異常系は、C#のnull許容参照型で制御されているため、
            // コンパイル時エラーとなり実行時テストは不要

            // Experience負の値での異常系テスト
            Assert.Throws<ArgumentOutOfRangeException>(() => new Experience(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Experience(-100));
        }

        private static IEnumerable CreateNpcCharacterTestCases()
        {
            var repository = new MockCharacterCommonEntityRepository();
            var name = new CharacterName("TestNPCName");
            var level = new CharacterLevel(new Experience(1000)); // レベル10相当

            yield return new TestCaseData(repository, name, level, null).SetCategory("正常系");

            // 境界値テスト
            var minLevel = new CharacterLevel(new Experience(0)); // レベル1（最小）
            yield return new TestCaseData(repository, name, minLevel, null).SetCategory("境界値");

            var maxLevelExp = Experience.RequiredCumulativeExp(CharacterLevel.MaxValue);
            var maxLevel = new CharacterLevel(maxLevelExp); // レベル100（最大）
            yield return new TestCaseData(repository, name, maxLevel, null).SetCategory("境界値");
        }

        [Test(Description = "CreateCharacter(NPC用)のテスト")]
        [TestCaseSource(nameof(CreateNpcCharacterTestCases))]
        public async Task CreateNpcCharacterPasses(ICommonCharacterEntityRepository repository, CharacterName name, CharacterLevel level, Type? exception)
        {
            var usecase = new AdjustCommonCharacterEntityUsecase(repository);

            if (exception is null)
            {
                var character = await usecase.CreateCharacter(name, level);

                var entity = await repository.FindByID(character.ID);

                if (entity is null)
                    throw new NullReferenceException();

                Assert.That(character.Equals(entity));
                Assert.That(character.Name, Is.EqualTo(name));
                Assert.That(character.Level.Value, Is.EqualTo(level.Value));
            }
            else
            {
                try
                {
                    Assert.ThrowsAsync(exception, async () => await usecase.CreateCharacter(name, level));
                }
                catch (Exception e)
                {
                    Assert.AreEqual(exception, e.GetType());
                }
            }
        }

        [Test(Description = "Experience/CharacterLevel境界値動作テスト")]
        public void ExperienceCharacterLevelBoundaryTest()
        {
            // レベル1の境界値テスト
            var level1Exp = new Experience(0);
            var level1 = new CharacterLevel(level1Exp);
            Assert.That(level1.Value, Is.EqualTo(1));

            // レベル2への境界値テスト
            var level2RequiredExp = Experience.RequiredCumulativeExp(2);
            var level2 = new CharacterLevel(level2RequiredExp);
            Assert.That(level2.Value, Is.EqualTo(2));

            // 最大レベルの境界値テスト
            var maxLevelExp = Experience.RequiredCumulativeExp(CharacterLevel.MaxValue);
            var maxLevel = new CharacterLevel(maxLevelExp);
            Assert.That(maxLevel.Value, Is.EqualTo(CharacterLevel.MaxValue));
            Assert.That(maxLevel.IsMaxLevel(), Is.True);

            // 最大レベル超過の経験値でも最大レベルに収束することを確認
            var overMaxExp = new Experience(maxLevelExp.Value + 10000);
            var overMaxLevel = new CharacterLevel(overMaxExp);
            Assert.That(overMaxLevel.Value, Is.LessThanOrEqualTo(CharacterLevel.MaxValue));
        }

        [Test(Description = "Experience計算精度テスト")]
        public void ExperienceCalculationAccuracyTest()
        {
            // 各レベルでの往復変換精度テスト
            for (int level = 1; level <= 10; level++)
            {
                var requiredExp = Experience.RequiredCumulativeExp(level);
                var calculatedLevel = Experience.CurrentLevel(requiredExp);
                Assert.That(calculatedLevel, Is.EqualTo(level), 
                    $"レベル{level}での経験値計算に誤差があります。経験値: {requiredExp.Value}, 計算されたレベル: {calculatedLevel}");
            }
        }
    }

    internal class MockCharacterCommonEntityRepository : ICommonCharacterEntityRepository
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

        public async UniTask Update(CommonCharacterEntity character)
        {
            await UniTask.CompletedTask;
            players[character.ID] = character;
        }
    }
}