#nullable enable

using System.Collections.Generic;
using System.Linq;
using BlackSmith.Domain.Networking.Auth;

namespace BlackSmith.Domain.Networking.Connection
{
    /// <summary>
    /// 接続承認のビジネスルールを実装
    /// 純粋な関数として実装され、副作用を持ちません
    /// </summary>
    public class ConnectionApprovalPolicy
    {
        /// <summary>
        /// 接続を承認するかどうかを判定します
        /// </summary>
        /// <param name="playerId">接続を要求しているプレイヤーID</param>
        /// <param name="currentConnectionCount">現在の接続数</param>
        /// <param name="maxConnectionCount">最大接続数</param>
        /// <param name="connectedPlayerIds">現在接続中のプレイヤーIDリスト</param>
        /// <returns>接続承認の結果</returns>
        public ConnectionApprovalResult Approve(
            AuthPlayerId playerId,
            int currentConnectionCount,
            int maxConnectionCount,
            IEnumerable<AuthPlayerId> connectedPlayerIds)
        {
            // サーバー満員チェック
            if (currentConnectionCount >= maxConnectionCount)
            {
                return ConnectionApprovalResult.Reject(ConnectionStatus.ServerFull);
            }

            // 重複ログインチェック
            if (connectedPlayerIds.Contains(playerId))
            {
                return ConnectionApprovalResult.Reject(ConnectionStatus.LoggedInAgain);
            }

            return ConnectionApprovalResult.Approve();
        }
    }

    /// <summary>
    /// 接続承認の結果を表すイミュータブルなrecord
    /// </summary>
    public record ConnectionApprovalResult
    {
        /// <summary>
        /// 接続が承認されたかどうか
        /// </summary>
        public bool IsApproved { get; init; }

        /// <summary>
        /// 承認結果のステータス
        /// </summary>
        public ConnectionStatus Status { get; init; }

        private ConnectionApprovalResult(bool isApproved, ConnectionStatus status)
        {
            IsApproved = isApproved;
            Status = status;
        }

        /// <summary>
        /// 接続を承認した結果を作成します
        /// </summary>
        public static ConnectionApprovalResult Approve()
        {
            return new ConnectionApprovalResult(true, ConnectionStatus.Success);
        }

        /// <summary>
        /// 接続を拒否した結果を作成します
        /// </summary>
        /// <param name="reason">拒否理由</param>
        public static ConnectionApprovalResult Reject(ConnectionStatus reason)
        {
            return new ConnectionApprovalResult(false, reason);
        }
    }
}
