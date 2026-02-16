#nullable enable

using System;
using System.Collections.Generic;

namespace BlackSmith.Domain.Networking.Connection
{
    /// <summary>
    /// 状態遷移の定義を提供
    /// イベント×現在の状態から次の状態へのマッピングを管理
    /// </summary>
    public class ConnectionStateTransitionTable
    {
        private readonly Dictionary<(Type EventType, ConnectionStateType CurrentState), ConnectionStateType> _transitions;

        /// <summary>
        /// ConnectionStateTransitionTableの新しいインスタンスを作成し、遷移ルールを初期化します
        /// </summary>
        public ConnectionStateTransitionTable()
        {
            _transitions = new Dictionary<(Type, ConnectionStateType), ConnectionStateType>
            {
                // Offline状態からの遷移
                [(typeof(ClientConnectedEvent), ConnectionStateType.Offline)] = ConnectionStateType.ClientConnecting,
                [(typeof(ServerStartedEvent), ConnectionStateType.Offline)] = ConnectionStateType.StartingHost,

                // ClientConnecting状態からの遷移
                [(typeof(ClientConnectedEvent), ConnectionStateType.ClientConnecting)] = ConnectionStateType.ClientConnected,
                [(typeof(ClientDisconnectedEvent), ConnectionStateType.ClientConnecting)] = ConnectionStateType.Offline,
                [(typeof(TransportFailureEvent), ConnectionStateType.ClientConnecting)] = ConnectionStateType.Offline,

                // ClientConnected状態からの遷移
                [(typeof(ClientDisconnectedEvent), ConnectionStateType.ClientConnected)] = ConnectionStateType.ClientReconnecting,
                [(typeof(UserRequestedShutdownEvent), ConnectionStateType.ClientConnected)] = ConnectionStateType.Offline,

                // ClientReconnecting状態からの遷移
                [(typeof(ClientConnectedEvent), ConnectionStateType.ClientReconnecting)] = ConnectionStateType.ClientConnected,
                [(typeof(ClientDisconnectedEvent), ConnectionStateType.ClientReconnecting)] = ConnectionStateType.Offline,

                // StartingHost状態からの遷移
                [(typeof(ServerStartedEvent), ConnectionStateType.StartingHost)] = ConnectionStateType.Hosting,
                [(typeof(TransportFailureEvent), ConnectionStateType.StartingHost)] = ConnectionStateType.Offline,

                // Hosting状態からの遷移
                [(typeof(ServerStoppedEvent), ConnectionStateType.Hosting)] = ConnectionStateType.Offline,
                [(typeof(UserRequestedShutdownEvent), ConnectionStateType.Hosting)] = ConnectionStateType.Offline,
            };
        }

        /// <summary>
        /// イベントと現在の状態から、次の状態を取得します
        /// </summary>
        /// <param name="connectionEvent">発生したイベント</param>
        /// <param name="currentState">現在の状態</param>
        /// <returns>次の状態。遷移が定義されていない場合はnull</returns>
        public ConnectionStateType? GetNextState(ConnectionEvent connectionEvent, ConnectionStateType currentState)
        {
            var eventType = connectionEvent.GetType();
            if (_transitions.TryGetValue((eventType, currentState), out var nextState))
            {
                return nextState;
            }
            return null;
        }

        /// <summary>
        /// 特定の遷移が有効かどうかを判定します
        /// </summary>
        /// <param name="connectionEvent">発生したイベント</param>
        /// <param name="currentState">現在の状態</param>
        /// <returns>遷移が有効な場合true、それ以外はfalse</returns>
        public bool IsValidTransition(ConnectionEvent connectionEvent, ConnectionStateType currentState)
        {
            return GetNextState(connectionEvent, currentState).HasValue;
        }
    }
}
