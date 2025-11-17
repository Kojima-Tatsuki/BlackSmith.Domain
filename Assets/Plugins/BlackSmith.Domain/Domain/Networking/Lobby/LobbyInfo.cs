using System.Collections.Generic;
using BlackSmith.Domain.Networking.Auth;

namespace BlackSmith.Domain.Networking.Lobby
{
    /// <summary>
    /// ロビーの基本情報を表すモデル
    /// </summary>
    public record LobbyInfo(
        LobbyId LobbyId,
        LobbyName LobbyName,
        LobbyJoinCode LobbyCode,
        int CurrentPlayerCount,
        int MaxPlayerCount,
        bool IsPrivate,
        AuthPlayerId HostPlayerId,
        IReadOnlyList<LobbyPlayer> Players,
        LobbyMetadata Metadata
    );

    public record LobbyId(string Value);
    public record LobbyName(string Value);
    public record LobbyJoinCode(string Value);
    public record LobbyMetadata(RelayJoinCode RelayJoinCode);
    public record RelayJoinCode(string Value);
}