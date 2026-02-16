#nullable enable

namespace BlackSmith.Domain.Networking.Connection
{
    /// <summary>
    /// 接続結果のステータスを表すenum
    /// </summary>
    public enum ConnectionStatus
    {
        /// <summary>
        /// 未定義
        /// </summary>
        Undefined,

        /// <summary>
        /// 接続成功
        /// </summary>
        Success,

        /// <summary>
        /// サーバーが満員
        /// </summary>
        ServerFull,

        /// <summary>
        /// 既にログイン済み（重複ログイン）
        /// </summary>
        LoggedInAgain,

        /// <summary>
        /// ユーザーによる切断要求
        /// </summary>
        UserRequestedDisconnect,

        /// <summary>
        /// 一般的な切断
        /// </summary>
        GenericDisconnect,

        /// <summary>
        /// 再接続中
        /// </summary>
        Reconnecting,

        /// <summary>
        /// ビルドタイプの不一致
        /// </summary>
        IncompatibleBuildType,

        /// <summary>
        /// ホストがセッションを終了
        /// </summary>
        HostEndedSession,

        /// <summary>
        /// ホスト起動失敗
        /// </summary>
        StartHostFailed,

        /// <summary>
        /// クライアント起動失敗
        /// </summary>
        StartClientFailed
    }
}
