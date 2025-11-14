using BlackSmith.Domain.Networking.Auth;
using R3;
using System;
using System.Collections.Generic;

#nullable enable

namespace BlackSmith.Domain.Networking.Lobby
{
    public class LocalLobbyUser
    {
        public Observable<LocalLobbyUser> OnChanged => onChanged;
        private Subject<LocalLobbyUser> onChanged;

        public LocalLobbyUserModel Model { get; private set; }

        internal LocalLobbyUser(LocalLobbyUserModel? model = null)
        {
            Model = model ?? new LocalLobbyUserModel();
            onChanged = new Subject<LocalLobbyUser>();
        }

        public void ApplyFromModel(LocalLobbyUserModel model)
        {
            Model = model;

            onChanged.OnNext(this);
        }
    }

    public record LocalLobbyUserModel
    {
        public AuthPlayerId UserId { get; init; }
        public UserName UserName { get; init; }
        public bool IsHost { get; init; }
        public string ConnectionInfo { get; init; }
        public string AllocationId { get; init; }
        public DateTime JoinedAt { get; init; }
        public DateTime UpdatedAt { get; init; }

        internal LocalLobbyUserModel()
        {
            UserId = AuthPlayerId.CreateId("0000000000000000000000000000"); // このマジックナンバーは、怖い
            UserName = new UserName("NoName");
            IsHost = false;
            ConnectionInfo = "";
            AllocationId = "";
            JoinedAt = DateTime.MinValue;
            UpdatedAt = DateTime.MinValue;
        }
    }
}
