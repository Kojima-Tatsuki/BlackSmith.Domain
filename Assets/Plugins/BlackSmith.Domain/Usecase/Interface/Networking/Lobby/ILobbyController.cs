using Cysharp.Threading.Tasks;
using BlackSmith.Domain.Character;
using BlackSmith.Domain.Networking.Auth;
using System;
using System.Collections.Generic;

#nullable enable

namespace BlackSmith.Usecase.Interface.Networking.Lobby
{
    /// <summary>
    /// ロビーの基本情報を表すモデル
    /// </summary>
    public record LobbyInfo
    {
        // TODO: プリミティブ型をドメインオブジェクトに変更する
        // - string LobbyId → LobbyId (ValueObject)
        // - string LobbyCode → LobbyCode (ValueObject)
        // - string JoinCode → JoinCode (ValueObject)
        // - string LobbyName → LobbyName (ValueObject)
        // - int CurrentPlayerCount → PlayerCount (ValueObject)
        // - int MaxPlayerCount → MaxPlayerCount (ValueObject)
        public string LobbyId { get; init; } = string.Empty;
        public string LobbyCode { get; init; } = string.Empty;
        public string JoinCode { get; init; } = string.Empty;
        public string LobbyName { get; init; } = string.Empty;
        public int CurrentPlayerCount { get; init; }
        public int MaxPlayerCount { get; init; }
        public bool IsPrivate { get; init; }
        public AuthPlayerId HostPlayerId { get; init; } = null!;
        public IReadOnlyList<LobbyPlayer> Players { get; init; } = new List<LobbyPlayer>();
    }

    /// <summary>
    /// ロビー参加者の情報を表すモデル
    /// </summary>
    public record LobbyPlayer
    {
        public AuthPlayerId PlayerId { get; init; } = null!;
        public CharacterID CharacterId { get; init; } = null!;
        public CharacterName CharacterName { get; init; } = null!;
        public bool IsHost { get; init; }
    }


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
        UniTask<LobbyInfo> CreateLobbyAsync(AuthPlayerId authPlayerId, string lobbyName, bool isPrivate);

        /// <summary>
        /// ロビーに参加する
        /// </summary>
        /// <param name="authPlayerId">認証されたプレイヤーID</param>
        /// <param name="lobbyCode">参加するロビーコード</param>
        /// <returns>参加したロビー情報</returns>
        /// <exception cref="ArgumentException">ロビーが存在しない、または満員の場合</exception>
        UniTask<LobbyInfo> JoinLobbyAsync(AuthPlayerId authPlayerId, string lobbyCode);

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