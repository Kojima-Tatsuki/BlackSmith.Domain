using BlackSmith.Domain.Networking.Auth;
using R3;
using System;
using System.Collections.Generic;

#nullable enable

namespace BlackSmith.Domain.Networking.Lobby
{
    public class LocalLobbyUser
    {
        public Observable<LobbyPlayer> OnChanged => onChanged;
        private Subject<LobbyPlayer> onChanged;

        public LobbyPlayer Model { get; private set; }

        internal LocalLobbyUser(LobbyPlayer model)
        {
            Model = model;
            onChanged = new Subject<LobbyPlayer>();
        }

        public void ApplyFromModel(LobbyPlayer model)
        {
            Model = model;

            onChanged.OnNext(Model);
        }

        public LobbyPlayer ToLobbyPlayer()
        {
            if (Model == null)
                throw new InvalidOperationException("LocalLobbyUserのModelが設定されていません");

            return Model;
        }
    }
}
