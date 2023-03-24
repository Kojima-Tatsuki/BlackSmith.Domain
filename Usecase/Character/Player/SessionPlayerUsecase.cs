using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using BlackSmith.Domain.Player;
using BlackSmith.Repository.Interface;

#nullable enable

namespace BlackSmith.Usecase.Player
{
    /// <summary>
    /// 操作するプレイヤーのデータを設定する
    /// </summary>
    public class SessionPlayerUsecase
    {
        private readonly IPlayerRepository repositoty;

        private readonly ISessionPlayerIdRepository onGameRepository;

        public SessionPlayerUsecase()
        {
            var provider = DIContainer.Instance.ServiceProvider;

            repositoty = provider.GetRequiredService<IPlayerRepository>();
            onGameRepository = provider.GetRequiredService<ISessionPlayerIdRepository>();
        }

        public void Login(PlayerID id)
        {
            if(!IsValidID(id))
                throw new ArgumentException($"不正なIDが入力されました, ID : {id}");

            onGameRepository.UpdateId(id);
        }

        public void Logout(PlayerID id)
        {
            if (!IsValidID(id))
                throw new ArgumentException($"不正なIDが入力されました, ID : {id}");

            var currentId = onGameRepository.GetId();

            if (currentId is null || !id.Equals(currentId))
                throw new ArgumentException($"ゲーム中のプレイヤーのIDと入力されたIDが一致しません, [current, input] : [{currentId}, {id}]");

            onGameRepository.Logout(id);
        }

        public PlayerID? GetLoginingPlayerID()
        {
            return onGameRepository.GetId();
        }

        private bool IsValidID(PlayerID id)
        {
            if (id is null)
                return false;

            if (!repositoty.IsExist(id))
                return false;

            return true;
        }
    }
}
