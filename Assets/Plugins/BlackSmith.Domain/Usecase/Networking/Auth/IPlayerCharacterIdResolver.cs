using BlackSmith.Domain.Character;
using BlackSmith.Domain.Networking.Auth;

namespace BlackSmith.Domain.Usecase.Networking.Auth
{
    public interface IPlayerCharacterIdResolver
    {
        /// <summary>
        /// PlayerAuthId から CharacterId を取得する
        /// </summary>
        /// <param name="authId">プレイヤー認証ID</param>
        /// <returns>キャラクターID</returns>
        CharacterID GetCharacterIdByPlayerAuthId(AuthPlayerId authId);

        /// <summary>
        /// CharacterId から PlayerAuthId を取得する
        /// </summary>
        /// <param name="characterId">キャラクターID</param>
        /// <returns>プレイヤー認証ID</returns>
        AuthPlayerId GetPlayerAuthIdByCharacterId(CharacterID characterId);
    }
}