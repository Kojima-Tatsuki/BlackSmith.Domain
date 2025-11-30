#nullable enable

namespace BlackSmith.Domain.Usecase.Interface.Networking.Connection
{
    using Domain.Networking.Connection;

    /// <summary>
    /// イベントを発行するインターフェース
    /// Repository層からDomain層にイベントを通知
    /// </summary>
    public interface IConnectionEventHandler
    {
        /// <summary>
        /// イベントを発行し、ステートマシンに処理させる
        /// </summary>
        void PublishEvent(ConnectionEvent connectionEvent);
    }
}
