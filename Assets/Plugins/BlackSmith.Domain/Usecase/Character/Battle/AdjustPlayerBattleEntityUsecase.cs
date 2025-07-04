using BlackSmith.Domain.Character;
using BlackSmith.Domain.Character.Battle;
using BlackSmith.Domain.Character.Player;
using BlackSmith.Usecase.Interface;
using Cysharp.Threading.Tasks;
using System;

#nullable enable

namespace BlackSmith.Usecase.Character.Battle
{
    public class AdjustPlayerBattleEntityUsecase
    {
        private readonly IBattleCharacterEntityRepository repository;

        public AdjustPlayerBattleEntityUsecase(IBattleCharacterEntityRepository playerRepository)
        {
            repository = playerRepository;
        }


        public async UniTask<BattleCharacterEntity> CreateCharacter(CommonCharacterEntity entity)
        {
            if (await repository.IsExist(entity.ID))
                throw new ArgumentException($"指定したidのキャラクターは既に存在しています {entity.ID}");

            var health = new HealthPoint(entity.Level);
            var levelDependParams = new LevelDependentParameters(entity.Level, new Strength(1), new Agility(1));
            var equimentModule = new BattleEquipmentModule(null, null);
            var statusModule = new BattleStatusEffectModule(null);

            var battleModule = new CharacterBattleModule(health, levelDependParams, equimentModule, statusModule);
            var command = new PlayerBattleReconstructCommand(entity.ID, battleModule);

            var battleEntity = new BattleCharacterEntity(command);

            await repository.Register(battleEntity);

            return battleEntity;
        }

        public async UniTask<BattleCharacterEntity> ReconstructPlayer(PlayerBattleReconstructCommand command)
        {
            var entity = new BattleCharacterEntity(command);

            if (!await repository.IsExist(entity.ID))
                await repository.Register(entity);

            return entity;
        }

        public async UniTask<BattleCharacterEntity?> GetCharacter(CharacterID id)
        {
            if (!await repository.IsExist(id))
                return null;

            return await repository.FindByID(id);
        }

        public async UniTask DeletePlayer(CharacterID id)
        {
            if (!await repository.IsExist(id))
                throw new InvalidOperationException($"指定したidのキャラクターは存在しません {id}");

            await repository.Delete(id);
        }
    }
}
