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

#nullable enable

namespace BlackSmith.Usecase.Character.Battle
{
    public class CharacterDamageUsecaseTest
    {
        private static IEnumerable TakeDamagePlayerByPlayerTestCases()
        {
            var attackerLevel = new CharacterLevel(Experience.RequiredCumulativeExp(10));
            var attacker = new BattleCharacterEntity(
                new PlayerBattleReconstructCommand(new CharacterID(),
                    new CharacterBattleModule(new HealthPoint(attackerLevel),
                        new LevelDependentParameters(attackerLevel, new Strength(10), new Agility(20)),
                        new BattleEquipmentModule(null, null),
                        new BattleStatusEffectModule())));
            var recieverLevel = new CharacterLevel(Experience.RequiredCumulativeExp(10));
            var reciever = new BattleCharacterEntity(
                new PlayerBattleReconstructCommand(new CharacterID(),
                    new CharacterBattleModule(new HealthPoint(recieverLevel),
                        new LevelDependentParameters(recieverLevel, new Strength(10), new Agility(20)),
                        new BattleEquipmentModule(null, null),
                        new BattleStatusEffectModule())));
            var repository = new MockPlayerBattleEntityRepository(new Dictionary<CharacterID, BattleCharacterEntity>
                {
                    {attacker.ID, attacker},
                    {reciever.ID, reciever}
                });

            yield return new TestCaseData(repository, attacker.ID, reciever.ID, null).SetCategory("正常系");
        }

        [Test(Description = "TakeDamagePlayerByPlayerのテスト")]
        [TestCaseSource(nameof(TakeDamagePlayerByPlayerTestCases))]
        public async Task TakeDamagePlayerByPlayerPasses(IBattleCharacterEntityRepository repository, CharacterID attackerId, CharacterID recieverId, Type? exception)
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
                try
                {
                    await usecase.TakeDamagePlayerByPlayer(attackerId, recieverId);
                }
                catch (Exception e)
                {
                    Assert.AreEqual(exception, e.GetType());
                }
            }
        }
    }

    internal class MockPlayerBattleEntityRepository : IBattleCharacterEntityRepository
    {
        private Dictionary<CharacterID, BattleCharacterEntity> characters { get; }

        public MockPlayerBattleEntityRepository()
        {
            characters = new Dictionary<CharacterID, BattleCharacterEntity>();
        }

        public MockPlayerBattleEntityRepository(Dictionary<CharacterID, BattleCharacterEntity> characters)
        {
            this.characters = characters;
        }

        public async UniTask Register(BattleCharacterEntity character)
        {
            await UniTask.CompletedTask;

            if (characters.ContainsKey(character.ID))
                throw new InvalidOperationException("既に存在するキャラクターです");
            characters.Add(character.ID, character);
        }

        public async UniTask<BattleCharacterEntity?> FindByID(CharacterID id)
        {
            await UniTask.CompletedTask;

            if (!characters.ContainsKey(id))
                return null;
            return characters[id];
        }

        public async UniTask<bool> IsExist(CharacterID id)
        {
            await UniTask.CompletedTask;

            return characters.ContainsKey(id);
        }

        public async UniTask UpdateCharacter(BattleCharacterEntity character)
        {
            await UniTask.CompletedTask;

            if (!characters.ContainsKey(character.ID))
                throw new InvalidOperationException("存在しないキャラクターです");
            characters[character.ID] = character;
        }

        public async UniTask Delete(CharacterID id)
        {
            await UniTask.CompletedTask;

            if (!characters.ContainsKey(id))
                throw new InvalidOperationException("存在しないキャラクターです");

            characters.Remove(id);
        }
    }
}