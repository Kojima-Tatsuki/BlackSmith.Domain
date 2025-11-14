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
        public Observable<LocalLobby> OnUpdated => onUpdated;
        private Subject<LocalLobby> onUpdated;

        public IReadOnlyList<LocalLobbyUser> LobbyUsers => lobbyUsers;
        private List<LocalLobbyUser> lobbyUsers;

        public LocalLobbyModel Model { get; private set; }

        public LocalLobby()
        {
            onUpdated = new Subject<LocalLobby>();
            lobbyUsers = new List<LocalLobbyUser>();
            Model = new LocalLobbyModel();
        }

        /// <summary>
        /// <see cref="LocalLobbyModel"/>を元に更新する
        /// </summary>
        /// <remarks><see cref="LocalLobbyUser"/>はミュータブルなオブジェクトの為、破棄しないよう注意する</remarks>
        /// <param name="model"></param>
        public void ApplyFromModel(LocalLobbyModel model)
        {
            Model = model;
            lobbyUsers = model.Users.Select(userModel =>
            {
                // 既存ユーザーの場合
                var old = lobbyUsers.Find(oldUser => oldUser.Model.UserId == userModel.UserId);
                if (old != null)
                    return old;

                // 新規ユーザーの場合
                return new LocalLobbyUser(userModel);

            }).ToList();

            onUpdated.OnNext(this);
        }

        public void ResetLobbyData()
        {
            Model = new LocalLobbyModel();

            onUpdated.OnNext(this);
        }
    }

    public record LocalLobbyModel
    {
        public string LobbyId { get; init; }
        public string LobbyCode { get; init; }
        public string LobbyName { get; init; }
        public string RelayJoinCode { get; init; }

        public int MaxUsers { get; init; }
        public int AvailableSlots { get; init; }
        public bool IsPrivate { get; init; }
        public bool IsLocked { get; init; }
        public bool HasPassword { get; init; }
        public IReadOnlyList<LocalLobbyUserModel> Users { get; init; }
        public AuthPlayerId HostId { get; init; }

        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
        public int Version { get; init; }

        public string UnityProjectId { get; init; } // Unity Project Id
        public string EnvironmentId { get; init; }

        internal LocalLobbyModel()
        {
            LobbyId = "";
            LobbyCode = "";
            LobbyName = "";
            RelayJoinCode = "";

            MaxUsers = 0;
            AvailableSlots = 0;
            IsPrivate = false;
            IsLocked = false;
            HasPassword = false;
            Users = new List<LocalLobbyUserModel>();
            HostId = AuthPlayerId.CreateId("0000000000000000000000000000");

            CreatedAt = DateTime.MinValue;
            UpdatedAt = DateTime.MinValue;
            Version = 0;

            UnityProjectId = "";
            EnvironmentId = "";
        }
    }
}
