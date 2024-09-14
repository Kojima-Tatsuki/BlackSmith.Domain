using BlackSmith.Domain.Character;
using BlackSmith.Domain.Character.Player;
using BlackSmith.Usecase.Interface;
using Cysharp.Threading.Tasks;
using System;

namespace BlackSmith.Usecase.Character
{
    /// <summary>
    /// Presenter層にPlayerEntityDataを渡すクラス
    /// </summary>
    public class PlayerCommonEntityProvidUsecase
    {
        private readonly IPlayerCommonEntityRepository repository;

        public PlayerCommonEntityProvidUsecase(IPlayerCommonEntityRepository playerRepository)
        {
            repository = playerRepository;
        }

        public async UniTask<PlayerCommonEntity> GetPlayerData(CharacterID id)
        {
            if (!(await repository.IsExist(id)))
                throw new Exception($"指定したidのキャラクターは存在しません {id}");

            var entity = (await repository.FindByID(id)) ?? throw new ArgumentException($"指定したidのキャラクターは存在しません {id}");

            return entity;
        }
    }
}
