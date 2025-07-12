using R3;
using System;

#nullable enable

namespace BlackSmith.Usecase.Interface.Networking.Auth
{
    /// <summary>
    /// イベントの発火は、内部実装で担保すること。
    /// </summary>
    public interface IAuthEventProvider
    {
        Observable<Unit> OnSignedIn { get; }
        Observable<Unit> OnSignedOut { get; }
        Observable<Unit> OnExpired { get; }
        Observable<Unit> OnSignInFailed { get; }
    }
}