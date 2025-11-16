using Cysharp.Threading.Tasks;
using BlackSmith.Domain.Character;
using System;
using BlackSmith.Domain.Networking.Lobby;

#nullable enable

namespace BlackSmith.Usecase.Interface.Networking.Lobby
{
    public interface ILobbyMembershipService
    {
        /// <summary>
        /// ロビーを作成する
        /// </summary>
        /// <param name="lobbyName">ロビー名</param>
        /// <param name="isPrivate">プライベートロビーかどうか</param>
        /// <returns>作成されたロビー情報</returns>
        /// <exception cref="InvalidOperationException">ロビー作成に失敗した場合</exception>
        UniTask<LobbyInfo> CreateLobbyAsync(CharacterName characterName, LobbyName lobbyName, bool isPrivate);

        /// <summary>
        /// ロビーに参加する
        /// </summary>
        /// <param name="lobbyCode">参加するロビーコード</param>
        /// <returns>参加したロビー情報</returns>
        /// <exception cref="ArgumentException">ロビーが存在しない、または満員の場合</exception>
        UniTask<LobbyInfo> JoinLobbyByCodeAsync(CharacterName characterName, LobbyJoinCode lobbyCode);

        /// <summary>
        /// ロビーに再接続する
        /// </summary>
        /// <returns>再接続したロビー情報</returns>
        /// <exception cref="InvalidOperationException">ロビーに参加していない場合</exception>
        UniTask<LobbyInfo> ReconnectToLobbyAsync();
    }
}