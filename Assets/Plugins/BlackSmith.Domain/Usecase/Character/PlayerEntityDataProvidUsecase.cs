using BlackSmith.Domain.Character;
using BlackSmith.Domain.Character.Player;
using BlackSmith.Usecase.Interface;
using System;
using System.Collections.Generic;

namespace BlackSmith.Usecase.Character
{
    /// <summary>
    /// Presenter層にPlayerEntityDataを渡すクラス
    /// </summary>
    public class PlayerEntityDataProvidUsecase
    {
        private readonly IPlayerCommonEntityRepository repository;

        public PlayerEntityDataProvidUsecase(IPlayerCommonEntityRepository playerRepository)
        {
            repository = playerRepository;
        }

        /// <summary>
        /// すべてのプレイヤーのデータを返す
        /// </summary>
        /// <returns>すべてのプレイヤーのデータ</returns>
        public IReadOnlyCollection<PlayerCommonEntity> GetAllPlayerCommonEntities()
        {
            return repository.GetAllPlayers();
        }

        public PlayerCommonEntity GetPlayerData(CharacterID id)
        {
            if (!repository.IsExist(id))
                throw new Exception($"指定したidのキャラクターは存在しません {id}");

            var entity = repository.FindByID(id) ?? throw new ArgumentException($"指定したidのキャラクターは存在しません {id}");

            return entity;
        }
    }
}
