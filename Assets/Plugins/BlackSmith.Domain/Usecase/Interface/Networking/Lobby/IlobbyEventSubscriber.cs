using BlackSmith.Domain.Networking.Lobby;
using Cysharp.Threading.Tasks;

namespace BlackSmith.Usecase.Interface.Networking.Lobby
{
    public interface ILobbyEventSubscriber
    {
        bool IsSubscribed { get; }

        UniTask SubscribeAsync(LobbyId lobbyId);
        UniTask UnSubscribeAsync();
    }
}