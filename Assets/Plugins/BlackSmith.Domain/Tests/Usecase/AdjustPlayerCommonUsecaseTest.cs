using BlackSmith.Domain.Character;
using BlackSmith.Domain.Character.Player;
using BlackSmith.Usecase.Interface;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

            yield return new TestCaseData(repository, name, null).SetCategory("����n");

            // PlayerName�ŃR�P��ꍇ
            yield return new TestCaseData(repository, "", typeof(ArgumentException)).SetCategory("�ُ�n");
        }

        private static IEnumerable ReconstructCharacterTestCases()
        {
            var repository = new MockPlayerCommonEntityRepository();
            var name = "TestPlayerName";
            var id = new CharacterID();
            var level = new PlayerLevel();

            var model = new PlayerCommonReconstractPrimitiveModel(id.Value, name, level.CumulativeExp.Value);

            yield return new TestCaseData(repository, model, null).SetCategory("����n");

            // model��ToCommand�ŃR�P��ꍇ
            var failModel = new PlayerCommonReconstractPrimitiveModel(id.Value, "", level.CumulativeExp.Value);
            yield return new TestCaseData(repository, failModel, typeof(ArgumentException)).SetCategory("�ُ�n");

            // ���ɓ���ID�̃v���C���[�����݂���ꍇ
            var entity = new PlayerCommonEntity(new PlayerCommonReconstractCommand(id, new PlayerName(name), level));
            var failRep = new MockPlayerCommonEntityRepository(new Dictionary<CharacterID, PlayerCommonEntity>() { { entity.ID, entity } });
            yield return new TestCaseData(failRep, model, typeof(InvalidOperationException)).SetCategory("�ُ�n");
        }

        [Test(Description = "CreateCharacter�̃e�X�g")]
        [TestCaseSource(nameof(CreateCharacterTestCases))]
        public void CreateCharacterPasses(IPlayerCommonEntityRepository repository, string name, Type? exception)
        {
            var usecase = new AdjustPlayerCommonUsecase(repository);

            if (exception is null)
            {
                var character = usecase.CreateCharacter(name);

                Assert.That(character.Equals(repository.FindByID(character.ID) ?? throw new NullReferenceException()));
            }
            else
                Assert.Throws(exception, () => usecase.CreateCharacter(name));
        }

        [Test(Description = "ReconstructCharacter�̃e�X�g")]
        [TestCaseSource(nameof(ReconstructCharacterTestCases))]
        public void ReconstructCharacterPasses(IPlayerCommonEntityRepository repository, PlayerCommonReconstractPrimitiveModel model, Type? exception)
        {
            var usecase = new AdjustPlayerCommonUsecase(repository);

            if (exception is null)
            {
                var character = usecase.ReconstructPlayer(model);

                Assert.That(character.Equals(repository.FindByID(character.ID) ?? throw new NullReferenceException()));;
            }
            else
                Assert.Throws(exception, () => usecase.ReconstructPlayer(model));
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

        public void Delete(CharacterID id) => players.Remove(id);
        public PlayerCommonEntity FindByID(CharacterID id) => players[id];
        public IReadOnlyCollection<PlayerCommonEntity> GetAllPlayers() => players.Values;
        public bool IsExist(CharacterID id) => players.ContainsKey(id);
        public void Register(PlayerCommonEntity character)
        {
            if (IsExist(character.ID))
                throw new InvalidOperationException("���Ƀv���C���[���o�^����Ă��܂�");

            players.Add(character.ID, character);
        }
        public void UpdateCharacter(PlayerCommonEntity character) => players[character.ID] = character;
    }
}