#nullable enable

using Cysharp.Threading.Tasks;
using System.Threading;

namespace BlackSmith.Domain.Usecase.Interface.Networking.Connection
{
    /// <summary>
    /// 接続方法の抽象化（Relay等）
    /// Unity用の非同期処理にUniTaskを使用
    /// </summary>
    public interface IConnectionMethod
    {
        /// <summary>
        /// ホスト接続のセットアップ（非同期）
        /// Relay Allocationの作成、Lobby情報の更新等を実行
        /// </summary>
        UniTask SetupHostConnectionAsync();

        /// <summary>
        /// クライアント接続のセットアップ（非同期）
        /// Relay Joinコードの取得、接続開始を実行
        /// </summary>
        /// <param name="cancellationToken">キャンセルトークン</param>
        UniTask SetupClientConnectionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// クライアント再接続のセットアップ（非同期）
        /// </summary>
        /// <param name="cancellationToken">キャンセルトークン</param>
        /// <returns>
        /// success: 再接続が成功したか
        /// shouldTryAgain: 再接続を再試行すべきか
        /// </returns>
        UniTask<(bool success, bool shouldTryAgain)> SetupClientReconnectionAsync(CancellationToken cancellationToken = default);
    }
}
