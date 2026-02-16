#nullable enable

using System.Collections.Generic;

namespace BlackSmith.Domain.Usecase.Interface.Networking.Connection
{
    using Domain.Networking.Auth;

    /// <summary>
    /// ネットワーク情報の取得（CQS: Query側）
    /// 状態を変更しない読み取り専用操作のみ
    /// </summary>
    public interface INetworkQuery
    {
        /// <summary>
        /// ホストかどうか
        /// </summary>
        bool IsHost { get; }

        /// <summary>
        /// クライアントかどうか
        /// </summary>
        bool IsClient { get; }

        /// <summary>
        /// サーバーかどうか
        /// </summary>
        bool IsServer { get; }

        /// <summary>
        /// 接続中のプレイヤーIDリストを取得
        /// </summary>
        IEnumerable<AuthPlayerId> GetConnectedPlayerIds();

        /// <summary>
        /// 最大接続数を取得
        /// </summary>
        int GetMaxConnectionCount();

        /// <summary>
        /// ローカルプレイヤーIDを取得
        /// </summary>
        AuthPlayerId? GetLocalPlayerId();
    }
}
