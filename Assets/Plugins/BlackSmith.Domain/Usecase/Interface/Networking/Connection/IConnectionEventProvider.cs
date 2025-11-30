#nullable enable

using R3;
using System;

namespace BlackSmith.Domain.Usecase.Interface.Networking.Connection
{
    using Domain.Networking.Connection;

    /// <summary>
    /// イベントを購読するインターフェース
    /// </summary>
    public interface IConnectionEventProvider
    {
        /// <summary>
        /// すべてのConnectionEventを購読
        /// </summary>
        Observable<ConnectionEvent> OnEventOccurred { get; }

        /// <summary>
        /// 特定の型のイベントのみ購読
        /// </summary>
        Observable<T> OnEvent<T>() where T : ConnectionEvent;
    }
}
