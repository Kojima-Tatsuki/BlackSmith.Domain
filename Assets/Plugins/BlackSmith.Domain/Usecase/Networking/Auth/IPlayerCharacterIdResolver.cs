using BlackSmith.Domain.Character;
using BlackSmith.Domain.Networking.Auth;
using Cysharp.Threading.Tasks;

#nullable enable

namespace BlackSmith.Domain.Usecase.Networking.Auth
{
    public interface IPlayerCharacterIdResolver
    {
        /// <summary>
        /// PlayerAuthId から CharacterId を取得する
        /// </summary>
        /// <param name="authId">プレイヤー認証ID</param>
        /// <returns>キャラクターID。対応するキャラクターが存在しない場合はnull</returns>
        UniTask<CharacterID?> GetCharacterIdByPlayerAuthId(AuthPlayerId authId);

        /// <summary>
        /// CharacterId から PlayerAuthId を取得する
        /// </summary>
        /// <param name="characterId">キャラクターID</param>
        /// <returns>プレイヤー認証ID。対応するプレイヤーが存在しない場合はnull</returns>
        UniTask<AuthPlayerId?> GetPlayerAuthIdByCharacterId(CharacterID characterId);
    }
}