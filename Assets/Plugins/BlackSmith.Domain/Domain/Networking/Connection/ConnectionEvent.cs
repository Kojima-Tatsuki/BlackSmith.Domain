#nullable enable

using System;
using BlackSmith.Domain.Networking.Auth;
using BlackSmith.Domain.Character;

namespace BlackSmith.Domain.Networking.Connection
{
    /// <summary>
    /// 接続イベントの基底record
    /// </summary>
    public abstract record ConnectionEvent
    {
        /// <summary>
        /// イベント発生時刻
        /// </summary>
        public DateTime OccurredAt { get; init; }

        /// <summary>
        /// ConnectionEventの新しいインスタンスを作成します
        /// </summary>
        protected ConnectionEvent()
        {
            OccurredAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// クライアント接続イベント
    /// </summary>
    public record ClientConnectedEvent(AuthPlayerId PlayerId) : ConnectionEvent;

    /// <summary>
    /// クライアント切断イベント
    /// </summary>
    public record ClientDisconnectedEvent(AuthPlayerId PlayerId, string? Reason) : ConnectionEvent;

    /// <summary>
    /// サーバー開始イベント
    /// </summary>
    public record ServerStartedEvent : ConnectionEvent;

    /// <summary>
    /// サーバー停止イベント
    /// </summary>
    public record ServerStoppedEvent(bool IsHost) : ConnectionEvent;

    /// <summary>
    /// トランスポート失敗イベント
    /// </summary>
    public record TransportFailureEvent : ConnectionEvent;

    /// <summary>
    /// ユーザーによるシャットダウン要求イベント
    /// </summary>
    public record UserRequestedShutdownEvent : ConnectionEvent;

    /// <summary>
    /// 接続承認要求イベント
    /// </summary>
    public record ConnectionApprovalRequestEvent(
        AuthPlayerId PlayerId,
        CharacterName CharacterName) : ConnectionEvent;
}
