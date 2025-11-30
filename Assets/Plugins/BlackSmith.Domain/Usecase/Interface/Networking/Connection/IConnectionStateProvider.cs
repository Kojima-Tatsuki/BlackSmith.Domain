#nullable enable

using R3;

namespace BlackSmith.Domain.Usecase.Interface.Networking.Connection
{
    using Domain.Networking.Connection;

    /// <summary>
    /// 接続状態を取得・購読するインターフェース
    /// </summary>
    public interface IConnectionStateProvider
    {
        /// <summary>
        /// 現在の接続状態を取得
        /// </summary>
        ConnectionState GetCurrentState();

        /// <summary>
        /// 状態変更を購読
        /// </summary>
        Observable<ConnectionState> OnStateChanged { get; }
    }
}
