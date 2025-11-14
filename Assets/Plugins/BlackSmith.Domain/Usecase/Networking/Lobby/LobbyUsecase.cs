using System;
using BlackSmith.Domain.Character;
using BlackSmith.Domain.Networking.Lobby;
using BlackSmith.Usecase.Interface.Networking.Auth;
using BlackSmith.Usecase.Interface.Networking.Lobby;
using Cysharp.Threading.Tasks;

namespace BlackSmith.Usecase.Networking.Lobby
{
    public class LobbyUsecase
    {
        private readonly IAuthenticationStateProvider authStateProvider;
        private readonly ILobbyController lobbyController;
        private readonly ILobbyEventSubscriber lobbyEventSubscriber;
        private readonly ILobbyHeartbeater lobbyHeartbeater;

        public LobbyUsecase(IAuthenticationStateProvider authStateProvider, ILobbyController lobbyController, ILobbyEventSubscriber lobbyEventSubscriber, ILobbyHeartbeater lobbyHeartbeater)
        {
            this.authStateProvider = authStateProvider;
            this.lobbyController = lobbyController;
            this.lobbyEventSubscriber = lobbyEventSubscriber;
            this.lobbyHeartbeater = lobbyHeartbeater;
        }

        public async UniTask CreateLobbyAsync(CharacterName characterName, LobbyName lobbyName, bool isPrivate)
        {
            if (!authStateProvider.IsSignedIn)
                throw new InvalidOperationException("User is not signed in.");

            var authPlayerId = authStateProvider.GetCurrentPlayerId() ?? throw new InvalidOperationException("Current player ID is null.");

            var lobby = await lobbyController.CreateLobbyAsync(authPlayerId, characterName, lobbyName, isPrivate);

            await lobbyEventSubscriber.SubscribeAsync(lobby.LobbyId);
            lobbyHeartbeater.Start(lobby.LobbyId);
        }
    }
}