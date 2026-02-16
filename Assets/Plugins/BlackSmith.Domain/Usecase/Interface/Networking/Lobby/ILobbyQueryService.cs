using BlackSmith.Domain.Networking.Auth;
using BlackSmith.Domain.Networking.Lobby;
using Cysharp.Threading.Tasks;

#nullable enable

namespace BlackSmith.Usecase.Interface.Networking.Lobby
{
    public interface ILobbyQueryService
    {
        /// <summary>
        /// 指定されたロビー情報を取得する
        /// </summary>
        /// <param name="authPlayerId">認証されたプレイヤーID</param>
        /// <param name="lobbyId">取得するロビーID</param>
        /// <returns>ロビー情報（存在しない場合はnull）</returns>
        UniTask<LobbyInfo?> GetLobbyAsync(AuthPlayerId authPlayerId, string lobbyId);
    }
}