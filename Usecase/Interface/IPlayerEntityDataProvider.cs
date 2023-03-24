using BlackSmith.Usecase.Character.Player;

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
