using BlackSmith.Domain.Networking.Lobby;

namespace BlackSmith.Usecase.Interface.Networking.Lobby
{
    public interface ILobbyHeartbeater
    {
        bool IsHeartbeating { get; }

        void Start(LobbyId lobbyId);

        void Cancel(LobbyId lobbyId);
    }
}