using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackSmith.Usecase.Player;

#nullable enable

namespace BlackSmith.Usecase.Interface
{
    /// <summary>
    /// Presenter層にPlayerEntityのデータを提供する
    /// </summary>
    public interface IPlayerEntityDataProvider
    {
        /// <summary>
        /// 現在ゲーム中のプレイヤーのデータを返す
        /// </summary>
        /// <returns>プレイヤーのデータ</returns>
        PlayerEntityData? GetPlayerData();

        /// <summary>
        /// すべてのプレイヤーのデータを返す
        /// </summary>
        /// <returns>すべてのプレイヤーのデータ</returns>
        IReadOnlyCollection<PlayerEntityData> GetAllPlayerDatas();
    }
}
