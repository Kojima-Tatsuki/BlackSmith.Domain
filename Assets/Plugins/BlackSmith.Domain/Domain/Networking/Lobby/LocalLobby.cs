using BlackSmith.Domain.Networking.Auth;
using Cysharp.Threading.Tasks;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace BlackSmith.Domain.Networking.Lobby
{
    /// <summary>
    /// ミュータブル(変更可能)なクラス
    /// LobbyServiceから取得したデータをもとに更新される
    /// </summary>
    public sealed class LocalLobby
    {
        public Observable<LobbyInfo?> OnUpdated => onUpdated;
        private Subject<LobbyInfo?> onUpdated;

        public IReadOnlyList<LobbyPlayer> LobbyUsers => lobbyUsers;
        private List<LobbyPlayer> lobbyUsers;

        public LobbyInfo? Model { get; private set; }

        public LocalLobby()
        {
            onUpdated = new Subject<LobbyInfo?>();
            lobbyUsers = new List<LobbyPlayer>();
        }

        /// <summary>
        /// <see cref="LobbyInfo"/>を元に更新する
        /// </summary>
        /// <remarks><see cref="LocalLobbyUser"/>はミュータブルなオブジェクトの為、破棄しないよう注意する</remarks>
        /// <param name="model"></param>
        public void ApplyFromModel(LobbyInfo model)
        {
            Model = model;
            lobbyUsers = model.Players.Select(userModel =>
            {
                // 既存ユーザーの場合
                var old = lobbyUsers.Find(oldUser => oldUser.UserId == userModel.UserId);
                if (old != null)
                    return old;

                // 新規ユーザーの場合
                return userModel;

            }).ToList();

            onUpdated.OnNext(Model);
        }

        public void ResetLobbyData()
        {
            Model = null;

            onUpdated.OnNext(Model);
        }
    }

    public record LocalLobbyModel
    {
        public LobbyId LobbyId { get; init; }
        public string LobbyCode { get; init; }
        public LobbyName LobbyName { get; init; }
        public string RelayJoinCode { get; init; }

        public int MaxUsers { get; init; }
        public int AvailableSlots { get; init; }
        public bool IsPrivate { get; init; }
        public bool IsLocked { get; init; }
        public bool HasPassword { get; init; }
        public IReadOnlyList<LobbyPlayer> Users { get; init; }
        public AuthPlayerId HostId { get; init; }

        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
        public int Version { get; init; }

        public string UnityProjectId { get; init; } // Unity Project Id
        public string EnvironmentId { get; init; }

        public LocalLobbyModel(LobbyId lobbyId, string lobbyCode, LobbyName lobbyName, string relayJoinCode, int maxUsers, int availableSlots, bool isPrivate, bool isLocked, bool hasPassword, IReadOnlyList<LobbyPlayer> users, AuthPlayerId hostId, DateTime createdAt, DateTime updatedAt, int version, string unityProjectId, string environmentId)
        {
            LobbyId = lobbyId;
            LobbyCode = lobbyCode;
            LobbyName = lobbyName;
            RelayJoinCode = relayJoinCode;
            MaxUsers = maxUsers;
            AvailableSlots = availableSlots;
            IsPrivate = isPrivate;
            IsLocked = isLocked;
            HasPassword = hasPassword;
            Users = users;
            HostId = hostId;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            Version = version;
            UnityProjectId = unityProjectId;
            EnvironmentId = environmentId;
        }
    }
}
