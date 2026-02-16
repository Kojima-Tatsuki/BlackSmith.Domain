#nullable enable

using BlackSmith.Domain.Networking.Connection;

namespace BlackSmith.Usecase.Interface.Networking.Connection
{
    /// <summary>
    /// 状態遷移時のEnter/Exitアクションを実行するインターフェース
    /// Repository層で実装され、状態固有の副作用処理を担当します
    /// </summary>
    public interface IConnectionStateActionExecutor
    {
        /// <summary>
        /// 状態のEnterアクションを実行します
        /// 新しい状態に入る際の初期化処理を行います
        /// </summary>
        /// <param name="stateType">入る状態のタイプ</param>
        /// <param name="triggerEvent">状態遷移のトリガーとなったイベント</param>
        void ExecuteEnter(ConnectionStateType stateType, ConnectionEvent triggerEvent);

        /// <summary>
        /// 状態のExitアクションを実行します
        /// 現在の状態を離れる際のクリーンアップ処理を行います
        /// </summary>
        /// <param name="stateType">離れる状態のタイプ</param>
        void ExecuteExit(ConnectionStateType stateType);
    }
}
