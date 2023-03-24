using System;
using System.Collections.Generic;
using BlackSmith.Domain;
using BlackSmith.Domain.Player;
using BlackSmith.Repository.Interface;

namespace BlackSmith.Usecase.Player
{
    // PlayerEntityの作成と、リポジトリへの更新を行うUseacase
    internal class PlayerFactoryInstructor
    {
        private readonly IPlayerRepository playerRepositoty;

        internal PlayerFactoryInstructor(IPlayerRepository repositoty)
        {
            playerRepositoty = repositoty ?? throw new ArgumentNullException(nameof(repositoty));
        }

        /// <summary>
        /// 新たにプレイヤーのエンティティの作成を行う
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal PlayerEntity CreatePlayerEntity(PlayerName name)
        {
            var factory = new PlayerFactory();

            var player = factory.Create(name);

            playerRepositoty.Register(player);

            return player;
        }
    }

    /// <summary>
    /// ゲーム中のPlayerEntityのPlayerIDを操作する
    /// </summary>
    internal class OnGamePlayerIDRepositoryInstructor
    {
        private readonly ISessionPlayerIdRepository repository;

        internal OnGamePlayerIDRepositoryInstructor(ISessionPlayerIdRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        internal void UpdatePlayerId(PlayerID id)
        {
            repository.UpdateId(id);
        }

        internal PlayerID GetPlayerID()
        {
            var id = repository.GetId(); 

            if (id is null)
                throw new NullReferenceException("現在、ゲーム中のプレイヤーは存在しません");

            return id;
        }
    }
}