#nullable enable

namespace BlackSmith.Domain.Usecase.Interface.Networking.Connection
{
    using Domain.Networking.Auth;

    /// <summary>
    /// ネットワーク操作のコマンド（CQS: Command側）
    /// 状態を変更する操作のみ
    /// </summary>
    public interface INetworkCommand
    {
        /// <summary>
        /// クライアントとして開始
        /// </summary>
        void StartClient();

        /// <summary>
        /// ホストとして開始
        /// </summary>
        void StartHost();

        /// <summary>
        /// サーバーとして開始
        /// </summary>
        void StartServer();

        /// <summary>
        /// ネットワークをシャットダウン
        /// </summary>
        void Shutdown();

        /// <summary>
        /// プレイヤーを切断
        /// </summary>
        void DisconnectPlayer(AuthPlayerId playerId, string? reason = null);
    }
}
