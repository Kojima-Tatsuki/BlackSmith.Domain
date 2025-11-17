using System.Collections.Generic;
using BlackSmith.Domain.Networking.Lobby;

namespace BlackSmith.Usecase.Interface.Networking.Lobby
{
    public interface ILobbyStateProvider
    {
        bool IsInLobby { get; }
        LobbyInfo GetLobby();
        IReadOnlyList<LobbyPlayer> GetLobbyPlayers();
    }
}