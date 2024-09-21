using BlackSmith.Domain.Character;
using BlackSmith.Domain.Character.Battle;
using BlackSmith.Domain.Character.Player;
using BlackSmith.Usecase.Interface;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TestTools;

#nullable enable

namespace BlackSmith.Usecase.Character.Battle
{
    public class CharacterDamageUsecaseTest
    {
        private static IEnumerable TakeDamagePlayerByPlayerTestCases()
        {
            var attackerLevel = new PlayerLevel(Experience.RequiredCumulativeExp(10));
            var attacker = new PlayerBattleEntity(
                new PlayerBattleReconstractCommand(new CharacterID(),
                    new CharacterBattleModule(new HealthPoint(attackerLevel),
                        new LevelDependentParameters(attackerLevel, new Strength(10), new Agility(20)),
                        new BattleEquipmentModule(null, null),
                        new BattleStatusEffectModule())));
            var recieverLevel = new PlayerLevel(Experience.RequiredCumulativeExp(10));
            var reciever = new PlayerBattleEntity(
                new PlayerBattleReconstractCommand(new CharacterID(),
                    new CharacterBattleModule(new HealthPoint(recieverLevel),
                        new LevelDependentParameters(recieverLevel, new Strength(10), new Agility(20)),
                        new BattleEquipmentModule(null, null),
                        new BattleStatusEffectModule())));
            var repository = new MockPlayerBattleEntityRepository(new Dictionary<CharacterID, PlayerBattleEntity>
                {
                    {attacker.ID, attacker},
                    {reciever.ID, reciever}
                });

            yield return new TestCaseData(repository, attacker.ID, reciever.ID, null).SetCategory("正常系");
        }

        [Test(Description = "TakeDamagePlayerByPlayerのテスト")]
        [TestCaseSource(nameof(TakeDamagePlayerByPlayerTestCases))]
        public async Task TakeDamagePlayerByPlayerPasses(IPlayerBattleEntityRepository repository, CharacterID attackerId, CharacterID recieverId, Type? exception)
        {
            var usecase = new CharacterDamageUsecase(repository);

            if (exception == null)
            {
                var attacker = await repository.FindByID(attackerId);
                var reciever = await repository.FindByID(recieverId);
                var health = reciever?.HealthPoint ?? throw new NullReferenceException(nameof(reciever));
                var expected = health.TakeDamage(new DamageValue(
                    new LevelGapOfAttackerAndReceiver(
                        attacker?.Level ?? throw new NullReferenceException(nameof(attacker)),
                        reciever.Level),
                    attacker.Attack, reciever.Defense));

                await usecase.TakeDamagePlayerByPlayer(attackerId, recieverId);

                Assert.That((await repository.FindByID(recieverId))?.HealthPoint, Is.EqualTo(expected));
            }
            else
            {
                Assert.ThrowsAsync(exception, () => usecase.TakeDamagePlayerByPlayer(attackerId, recieverId).AsTask());
            }
        }
    }

    internal class MockPlayerBattleEntityRepository : IPlayerBattleEntityRepository
    {
        private Dictionary<CharacterID, PlayerBattleEntity> characters { get; }

        public MockPlayerBattleEntityRepository()
        {
            characters = new Dictionary<CharacterID, PlayerBattleEntity>();
        }

        public MockPlayerBattleEntityRepository(Dictionary<CharacterID, PlayerBattleEntity> characters)
        {
            this.characters = characters;
        }

        public async UniTask Delete(CharacterID id)
        {
            await UniTask.Delay(10);
            Debug.Log("Deleted");
        }

        public async UniTask<PlayerBattleEntity?> FindByID(CharacterID id)
        {
            await UniTask.Delay(10);
            if (!characters.ContainsKey(id))
                return null;
            return characters[id];
        }

        public async UniTask<bool> IsExist(CharacterID id)
        {
            await UniTask.Delay(10);
            throw new NotImplementedException();
        }

        public async UniTask Register(PlayerBattleEntity character)
        {
            await UniTask.Delay(10);
            throw new NotImplementedException();
        }

        public async UniTask UpdateCharacter(PlayerBattleEntity character)
        {
            await UniTask.Delay(10);
            characters[character.ID] = character;
        }
    }
}