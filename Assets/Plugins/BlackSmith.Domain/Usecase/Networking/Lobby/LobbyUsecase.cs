using System;
using BlackSmith.Domain.Networking.Lobby;
using BlackSmith.Usecase.Interface.Networking.Auth;
using BlackSmith.Usecase.Interface.Networking.Lobby;
using Cysharp.Threading.Tasks;

namespace BlackSmith.Usecase.Networking.Lobby
{
    public class LobbyUsecase
    {
        private readonly IAuthenticationStateProvider authStateProvider;
        private readonly ILobbyMembershipService membershipService;
        private readonly ILobbyEventSubscriber lobbyEventSubscriber;
        private readonly ILobbyHeartbeater lobbyHeartbeater;

        public LobbyUsecase(IAuthenticationStateProvider authStateProvider, ILobbyMembershipService membershipService, ILobbyEventSubscriber lobbyEventSubscriber, ILobbyHeartbeater lobbyHeartbeater)
        {
            this.authStateProvider = authStateProvider;
            this.membershipService = membershipService;
            this.lobbyEventSubscriber = lobbyEventSubscriber;
            this.lobbyHeartbeater = lobbyHeartbeater;
        }

        public async UniTask CreateLobbyAsync(LobbyName lobbyName, bool isPrivate)
        {
            if (!authStateProvider.IsSignedIn)
                throw new InvalidOperationException("User is not signed in.");

            var lobby = await membershipService.CreateLobbyAsync(lobbyName, isPrivate);

            await lobbyEventSubscriber.SubscribeAsync(lobby.LobbyId);
            lobbyHeartbeater.Start(lobby.LobbyId);
        }

        public async UniTask JoinLobbyAsync(LobbyCode joinCode)
        {
            if (!authStateProvider.IsSignedIn)
                throw new InvalidOperationException("User is not signed in.");

            var lobby = await membershipService.JoinLobbyByCodeAsync(joinCode);

            await lobbyEventSubscriber.SubscribeAsync(lobby.LobbyId);
            lobbyHeartbeater.Start(lobby.LobbyId);
        }
    }
}