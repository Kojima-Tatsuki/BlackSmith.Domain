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

        public LocalLobbyUserModel? Model { get; private set; }

        internal LocalLobbyUser(LocalLobbyUserModel? model = null)
        {
            Model = model;
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

        public LocalLobbyUserModel(AuthPlayerId userId, UserName userName, bool isHost, string connectionInfo, string allocationId, DateTime joinedAt, DateTime updatedAt)
        {
            UserId = userId;
            UserName = userName;
            IsHost = isHost;
            ConnectionInfo = connectionInfo;
            AllocationId = allocationId;
            JoinedAt = joinedAt;
            UpdatedAt = updatedAt;
        }
    }
}
