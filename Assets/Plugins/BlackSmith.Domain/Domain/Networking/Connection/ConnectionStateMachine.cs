#nullable enable

using System;
using BlackSmith.Usecase.Interface.Networking.Connection;

namespace BlackSmith.Domain.Networking.Connection
{
    /// <summary>
    /// 状態遷移マシン
    /// Enter/Exitアクションの実行はIConnectionStateActionExecutorに委譲します
    /// </summary>
    public class ConnectionStateMachine
    {
        private readonly ConnectionStateTransitionTable _transitionTable;
        private ConnectionState _currentState;

        /// <summary>
        /// 現在の接続状態
        /// </summary>
        public ConnectionState CurrentState => _currentState;

        /// <summary>
        /// ConnectionStateMachineの新しいインスタンスを作成します
        /// </summary>
        /// <param name="initialState">初期状態</param>
        public ConnectionStateMachine(ConnectionStateType initialState)
        {
            _transitionTable = new ConnectionStateTransitionTable();
            _currentState = new ConnectionState(initialState, DateTime.UtcNow);
        }

        /// <summary>
        /// イベントを処理し、状態遷移を実行します
        /// </summary>
        /// <param name="connectionEvent">発生したイベント</param>
        /// <param name="actionExecutor">Enter/Exitアクションを実行するエグゼキューター</param>
        /// <returns>遷移結果</returns>
        public StateTransitionResult ProcessEvent(
            ConnectionEvent connectionEvent,
            IConnectionStateActionExecutor actionExecutor)
        {
            var nextStateType = _transitionTable.GetNextState(connectionEvent, _currentState.StateType);

            if (!nextStateType.HasValue)
            {
                return StateTransitionResult.NoTransition(_currentState, connectionEvent);
            }

            // 特殊ケース: ClientDisconnectedEventの理由によって遷移先を変更
            if (connectionEvent is ClientDisconnectedEvent disconnectEvent &&
                _currentState.StateType == ConnectionStateType.ClientConnected)
            {
                nextStateType = string.IsNullOrEmpty(disconnectEvent.Reason)
                    ? ConnectionStateType.ClientReconnecting
                    : ConnectionStateType.Offline;
            }

            var previousState = _currentState;

            // Exitアクション実行
            actionExecutor.ExecuteExit(previousState.StateType);

            // 状態遷移
            _currentState = new ConnectionState(nextStateType.Value, DateTime.UtcNow);

            // Enterアクション実行
            actionExecutor.ExecuteEnter(_currentState.StateType, connectionEvent);

            return StateTransitionResult.Success(previousState, _currentState, connectionEvent);
        }

        /// <summary>
        /// 現在の状態を強制的に設定します（テスト用）
        /// </summary>
        /// <param name="state">設定する状態</param>
        internal void SetStateForTest(ConnectionState state)
        {
            _currentState = state;
        }
    }

    /// <summary>
    /// 状態遷移の結果を表すイミュータブルなrecord
    /// </summary>
    public record StateTransitionResult
    {
        /// <summary>
        /// 遷移が成功したかどうか
        /// </summary>
        public bool Succeeded { get; init; }

        /// <summary>
        /// 遷移前の状態
        /// </summary>
        public ConnectionState? PreviousState { get; init; }

        /// <summary>
        /// 遷移後の新しい状態
        /// </summary>
        public ConnectionState? NewState { get; init; }

        /// <summary>
        /// 遷移のトリガーとなったイベント
        /// </summary>
        public ConnectionEvent TriggerEvent { get; init; }

        /// <summary>
        /// エラーメッセージ（遷移失敗時のみ）
        /// </summary>
        public string? ErrorMessage { get; init; }

        private StateTransitionResult(
            bool succeeded,
            ConnectionState? previousState,
            ConnectionState? newState,
            ConnectionEvent triggerEvent,
            string? errorMessage = null)
        {
            Succeeded = succeeded;
            PreviousState = previousState;
            NewState = newState;
            TriggerEvent = triggerEvent;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// 成功した状態遷移の結果を作成します
        /// </summary>
        public static StateTransitionResult Success(
            ConnectionState previousState,
            ConnectionState newState,
            ConnectionEvent triggerEvent)
        {
            return new StateTransitionResult(true, previousState, newState, triggerEvent);
        }

        /// <summary>
        /// 遷移が定義されていない場合の結果を作成します
        /// </summary>
        public static StateTransitionResult NoTransition(
            ConnectionState currentState,
            ConnectionEvent triggerEvent)
        {
            return new StateTransitionResult(
                false,
                currentState,
                null,
                triggerEvent,
                $"No transition defined for event {triggerEvent.GetType().Name} in state {currentState.StateType}");
        }
    }
}
