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

        public async UniTask CreateLobbyAsync(CharacterName characterName, LobbyName lobbyName, bool isPrivate)
        {
            if (!authStateProvider.IsSignedIn)
                throw new InvalidOperationException("User is not signed in.");

            var lobby = await membershipService.CreateLobbyAsync(characterName, lobbyName, isPrivate);

            await lobbyEventSubscriber.SubscribeAsync(lobby.LobbyId);
            lobbyHeartbeater.Start(lobby.LobbyId);
        }

        public async UniTask JoinLobbyAsync(CharacterName characterName, LobbyJoinCode joinCode)
        {
            if (!authStateProvider.IsSignedIn)
                throw new InvalidOperationException("User is not signed in.");

            var lobby = await membershipService.JoinLobbyByCodeAsync(characterName, joinCode);

            await lobbyEventSubscriber.SubscribeAsync(lobby.LobbyId);
            lobbyHeartbeater.Start(lobby.LobbyId);
        }
    }
}