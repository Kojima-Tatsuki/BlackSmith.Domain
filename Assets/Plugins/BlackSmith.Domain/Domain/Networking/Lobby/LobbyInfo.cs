using System.Collections.Generic;
using BlackSmith.Domain.Character;
using BlackSmith.Domain.Networking.Auth;

namespace BlackSmith.Domain.Networking.Lobby
{
    /// <summary>
    /// ロビーの基本情報を表すモデル
    /// </summary>
    public record LobbyInfo
    {
        // TODO: プリミティブ型をドメインオブジェクトに変更する
        // - string LobbyCode → LobbyCode (ValueObject)
        // - string JoinCode → JoinCode (ValueObject)
        // - int CurrentPlayerCount → PlayerCount (ValueObject)
        // - int MaxPlayerCount → MaxPlayerCount (ValueObject)
        public LobbyId LobbyId { get; init; } = new LobbyId(string.Empty);
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
}