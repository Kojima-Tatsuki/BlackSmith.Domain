using BlackSmith.Domain.Character;
using BlackSmith.Domain.Networking.Auth;
using System;

#nullable enable

namespace BlackSmith.Domain.Usecase.Networking.Auth
{
    public record SessionPlayerData
    {
        public AuthPlayerId AuthPlayerId { get; }
        public CharacterID CharacterId { get; }
        public CharacterName CharacterName { get; }

        public SessionPlayerData(AuthPlayerId authPlayerId, CharacterID characterId, CharacterName characterName)
        {
            AuthPlayerId = authPlayerId;
            CharacterId = characterId;
            CharacterName = characterName;
        }
    }

    /// <summary>
    /// サインイン中のプレイヤーのIDを保管するリポジトリ
    /// </summary>
    public interface ISessionPlayerDataRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <exception cref="InvalidOperationException">サインイン中のユーザが存在する場合</exception>"
        void Update(SessionPlayerData data);

        SessionPlayerData? Load();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="InvalidOperationException">サインイン中のユーザが存在しない場合</exception>
        void Logout();
    }
}