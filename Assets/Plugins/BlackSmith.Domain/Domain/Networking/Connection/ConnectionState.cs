#nullable enable

using System;

namespace BlackSmith.Domain.Networking.Connection
{
    /// <summary>
    /// ネットワーク接続の状態を表すイミュータブルな値オブジェクト
    /// </summary>
    public record ConnectionState
    {
        /// <summary>
        /// 接続状態のタイプ
        /// </summary>
        public ConnectionStateType StateType { get; init; }

        /// <summary>
        /// この状態になった時刻
        /// </summary>
        public DateTime Timestamp { get; init; }

        /// <summary>
        /// ConnectionStateの新しいインスタンスを作成します
        /// </summary>
        /// <param name="stateType">接続状態のタイプ</param>
        /// <param name="timestamp">この状態になった時刻</param>
        public ConnectionState(
            ConnectionStateType stateType,
            DateTime timestamp)
        {
            StateType = stateType;
            Timestamp = timestamp;
        }
    }
}
