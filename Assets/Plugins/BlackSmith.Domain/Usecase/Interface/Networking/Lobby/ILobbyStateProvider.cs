using System.Collections.Generic;
using BlackSmith.Domain.Networking.Lobby;

namespace BlackSmith.Usecase.Interface.Networking.Lobby
{
    public interface ILobbyStateProvider
    {
        bool IsInLobby { get; }
        LocalLobbyModel LobbyModel { get; }
        IReadOnlyList<LobbyPlayer> LobbyUsers { get; }
    }
}