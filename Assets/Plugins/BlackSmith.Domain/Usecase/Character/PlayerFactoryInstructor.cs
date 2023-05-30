using BlackSmith.Domain.Character.Player;
using BlackSmith.Usecase.Interface;
using System;

namespace BlackSmith.Usecase.Character
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
            var player = PlayerFactory.Create(name);

            playerRepositoty.Register(player);

            return player;
        }
    }
}