using BlackSmith.Domain.Networking.Auth;

#nullable enable

namespace BlackSmith.Usecase.Interface.Networking.Auth
{
    public interface IAuthenticationStateProvider
    {
        bool IsSignedIn { get; }

        /// <summary>
        /// 現在ログイン中のプレイヤーIDを取得
        /// </summary>
        /// <returns>プレイヤーID（未ログインの場合はnull）</returns>
        AuthPlayerId? GetCurrentPlayerId();
    }
}