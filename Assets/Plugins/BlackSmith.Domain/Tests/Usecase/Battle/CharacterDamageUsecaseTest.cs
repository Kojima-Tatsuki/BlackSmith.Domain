using BlackSmith.Domain.Character;
using BlackSmith.Domain.Character.Battle;
using BlackSmith.Domain.Character.Player;
using BlackSmith.Usecase.Interface;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

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
        public void TakeDamagePlayerByPlayerPasses(IPlayerBattleEntityRepository repository, CharacterID attackerId, CharacterID recieverId, Type? exception)
        {
            var usecase = new CharacterDamageUsecase(repository);

            if (exception == null)
            {
                var attacker = repository.FindByID(attackerId);
                var reciever = repository.FindByID(recieverId);
                var health = reciever?.HealthPoint ?? throw new NullReferenceException(nameof(reciever));
                var expected = health.TakeDamage(new DamageValue(
                    new LevelGapOfAttackerAndReceiver(
                        attacker?.Level ?? throw new NullReferenceException(nameof(attacker)), 
                        reciever.Level), 
                    attacker.Attack, reciever.Defense));

                usecase.TakeDamagePlayerByPlayer(attackerId, recieverId);

                Assert.That(repository.FindByID(recieverId)?.HealthPoint, Is.EqualTo(expected));
            }
            else
                Assert.Throws(exception, () => usecase.TakeDamagePlayerByPlayer(attackerId, recieverId));
        }
    }

    internal class MockPlayerBattleEntityRepository: IPlayerBattleEntityRepository
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

        public void Delete(CharacterID id)
        {
            throw new NotImplementedException();
        }

        public PlayerBattleEntity FindByID(CharacterID id)
        {
            return characters[id];
        }

        public bool IsExist(CharacterID id)
        {
            throw new NotImplementedException();
        }

        public void Register(PlayerBattleEntity character)
        {
            throw new NotImplementedException();
        }

        public void UpdateCharacter(PlayerBattleEntity character)
        {
            characters[character.ID] = character;
        }
    }
}