using System;
using Microsoft.Extensions.DependencyInjection;
using BlackSmith.Domain.Player;
using BlackSmith.Repository.Interface;

namespace BlackSmith.Usecase.Player
{
    /// <summary>
    /// プレイヤーのステータスに変更を与える際に用いるユースケース
    /// </summary>
    public class RewritePlayerStatusUsecase
    {
        private readonly PlayerRepositoryInstructor instructor;

        public RewritePlayerStatusUsecase()
        {
            var provider = DIContainer.Instance.ServiceProvider;

            var repository = provider.GetRequiredService<IPlayerRepository>();

            instructor = new PlayerRepositoryInstructor(repository);
        }

        /// <summary>
        /// プレイヤーの名前を書き換える
        /// </summary>
        /// <param name="id">書き換えるプレイヤーのID</param>
        /// <param name="newName">書き換え先の名前</param>
        public void RenamePlayer(PlayerID id, string newName)
        {
            var name = new PlayerName(newName);

            var entity = instructor.GetPlayerEntity(id);

            entity.ChangeName(name);

            instructor.UpdatePlayerEntity(entity);
        }

        public static bool IsValidPlayerName(string name)
        {
            return PlayerName.IsValid(name);
        }
    }
}