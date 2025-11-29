#nullable enable

namespace BlackSmith.Domain.Networking.Connection
{
    /// <summary>
    /// ネットワーク接続の状態を表すenum
    /// </summary>
    public enum ConnectionStateType
    {
        /// <summary>
        /// オフライン状態（接続していない）
        /// </summary>
        Offline,

        /// <summary>
        /// クライアントとして接続中
        /// </summary>
        ClientConnecting,

        /// <summary>
        /// クライアントとして接続完了
        /// </summary>
        ClientConnected,

        /// <summary>
        /// クライアントとして再接続試行中
        /// </summary>
        ClientReconnecting,

        /// <summary>
        /// ホストとして起動中
        /// </summary>
        StartingHost,

        /// <summary>
        /// ホストとして稼働中
        /// </summary>
        Hosting,
    }
}
