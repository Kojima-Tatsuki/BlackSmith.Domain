using BlackSmith.Domain.Character;
using BlackSmith.Domain.Character.Battle;
using BlackSmith.Usecase.Interface;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BlackSmith.Usecase.Character.Battle
{
    public class CharacterDamageUsecaseTest
    {
        private static IEnumerable TakeDamagePlayerByPlayerTestCases()
        {
            var repository = new MockPlayerBattleEntityRepository();

            yield return null;
        }

        [Test(Description = "TakeDamagePlayerByPlayerのテスト")]
        [TestCaseSource(nameof(TakeDamagePlayerByPlayerTestCases))]
        public void TakeDamagePlayerByPlayerPasses(IPlayerBattleEntityRepository repository, CharacterID attackerId, CharacterID recieverId, Type? exception)
        {

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
            throw new NotImplementedException();
        }
    }
}