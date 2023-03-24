using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Cysharp.Threading.Tasks;
using BlackSmith.Domain.Player;
using BlackSmith.Repository.Interface;
using BlackSmith.Usecase.Interface;

namespace BlackSmith.Usecase.Player
{
    /// <summary>
    /// プレイヤーの作成、削除を行うユースケース
    /// </summary>
    public class AdjustPlayerUsecase
    {
        private readonly PlayerFactoryInstructor playerFactory;

        private readonly IPlayerRepository repository;

        private readonly IAccountApi accountApi;

        public AdjustPlayerUsecase()
        {
            var provider = DIContainer.Instance.ServiceProvider;

            repository = provider.GetRequiredService<IPlayerRepository>();
            accountApi = provider.GetRequiredService<IAccountApi>();

            playerFactory = new PlayerFactoryInstructor(repository);
        }

        /// <summary>
        /// プレイヤーデータの作成を行う
        /// </summary>
        /// <param name="name">作成するプレイヤーの名前</param>
        /// <returns>作成したプレイヤーエンティティのデータ</returns>
        public async UniTask<PlayerEntityData> CreatePlayerAccount(string playerName)
        {
            var name = new PlayerName(playerName);

            // Gs2に接続する必要がある
            await accountApi.CreateAccountAsync(name);

            var entity = playerFactory.CreatePlayerEntity(name);

            return new PlayerEntityData(entity);
        }

        /// <summary>
        /// プレイヤーデータの削除を行う
        /// </summary>
        /// <param name="id">削除するプレイヤーのID</param>
        public void DeletePlayer(PlayerID id)
        {
            repository.Delete(id);
        }
    }
}
