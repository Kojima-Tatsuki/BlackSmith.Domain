using Cysharp.Threading.Tasks;
using BlackSmith.Domain.Character;
using BlackSmith.Domain.Networking.Auth;
using System;
using System.Collections.Generic;
using BlackSmith.Domain.Networking.Lobby;

#nullable enable

namespace BlackSmith.Usecase.Interface.Networking.Lobby
{
    /// <summary>
    /// ロビー機能を管理するインターフェース
    /// </summary>
    public interface ILobbyController
    {
        /// <summary>
        /// ロビーを作成する
        /// </summary>
        /// <param name="authPlayerId">認証されたプレイヤーID</param>
        /// <param name="lobbyName">ロビー名</param>
        /// <param name="isPrivate">プライベートロビーかどうか</param>
        /// <returns>作成されたロビー情報</returns>
        /// <exception cref="InvalidOperationException">ロビー作成に失敗した場合</exception>
        UniTask<LobbyInfo> CreateLobbyAsync(AuthPlayerId authPlayerId, CharacterName characterName, LobbyName lobbyName, bool isPrivate);

        /// <summary>
        /// ロビーに参加する
        /// </summary>
        /// <param name="authPlayerId">認証されたプレイヤーID</param>
        /// <param name="lobbyCode">参加するロビーコード</param>
        /// <returns>参加したロビー情報</returns>
        /// <exception cref="ArgumentException">ロビーが存在しない、または満員の場合</exception>
        UniTask<LobbyInfo> JoinLobbyByCodeAsync(AuthPlayerId authPlayerId, CharacterName characterName, LobbyName lobbyCode);

        /// <summary>
        /// ロビーに再接続する
        /// </summary>
        /// <param name="authPlayerId">認証されたプレイヤーID</param>
        /// <returns></returns>
        UniTask<LobbyInfo> ReconnectToLobbyAsync(AuthPlayerId authPlayerId);

        /// <summary>
        /// ロビーから退出する
        /// </summary>
        /// <param name="authPlayerId">認証されたプレイヤーID</param>
        /// <returns>退出処理のタスク</returns>
        /// <exception cref="InvalidOperationException">ロビーに参加していない場合</exception>
        UniTask LeaveLobbyAsync(AuthPlayerId authPlayerId);

        /// <summary>
        /// 指定されたロビー情報を取得する
        /// </summary>
        /// <param name="authPlayerId">認証されたプレイヤーID</param>
        /// <param name="lobbyId">取得するロビーID</param>
        /// <returns>ロビー情報（存在しない場合はnull）</returns>
        UniTask<LobbyInfo?> GetLobbyAsync(AuthPlayerId authPlayerId, string lobbyId);

        /// <summary>
        /// ロビー内のプレイヤー一覧を取得する
        /// </summary>
        /// <param name="authPlayerId">認証されたプレイヤーID</param>
        /// <returns>ロビー内のプレイヤー一覧</returns>
        /// <exception cref="InvalidOperationException">ロビーに参加していない場合</exception>
        UniTask<IReadOnlyList<LobbyPlayer>> GetLobbyPlayersAsync(AuthPlayerId authPlayerId);
    }
}