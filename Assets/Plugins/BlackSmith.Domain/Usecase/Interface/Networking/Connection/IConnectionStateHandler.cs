#nullable enable

namespace BlackSmith.Domain.Usecase.Interface.Networking.Connection
{
    using Domain.Networking.Connection;

    /// <summary>
    /// 接続状態を変更するインターフェース
    /// </summary>
    public interface IConnectionStateHandler
    {
        /// <summary>
        /// 状態変更を要求（既存との互換性のため残す）
        /// 新しい設計ではIConnectionEventHandlerを使用
        /// </summary>
        void ChangeState(ConnectionStateType nextState);
    }
}
