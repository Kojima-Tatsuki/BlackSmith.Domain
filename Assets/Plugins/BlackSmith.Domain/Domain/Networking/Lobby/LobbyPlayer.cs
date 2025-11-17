using BlackSmith.Domain.Character;
using BlackSmith.Domain.Networking.Auth;

namespace BlackSmith.Domain.Networking.Lobby
{
    /// <summary>
    /// ロビー参加者の情報を表すモデル
    /// </summary>
    public record LobbyPlayer(
        AuthPlayerId UserId,
        CharacterID CharacterId,
        CharacterName CharacterName,
        bool IsHost,
        PlayerAllocationId AllocationId
    );


    /// <summary>
    /// ロビーが参加しているプレイヤーを識別するためのID
    /// </summary>
    public record PlayerAllocationId(string Value);
}