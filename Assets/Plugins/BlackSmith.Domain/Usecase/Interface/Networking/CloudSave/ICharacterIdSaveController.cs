using Cysharp.Threading.Tasks;
using BlackSmith.Domain.Character;
using BlackSmith.Domain.Networking.Auth;
using System.Collections.Generic;

#nullable enable

namespace BlackSmith.Usecase.Interface.Networking.CloudSave
{
    /// <summary>
    /// キャラクターIDの永続化を管理するインターフェース
    /// </summary>
    public interface ICharacterIdSaveController
    {
        /// <summary>
        /// キャラクターIDを保存する
        /// </summary>
        /// <param name="authPlayerId">認証されたプレイヤーID</param>
        /// <param name="characterId">保存するキャラクターID</param>
        /// <returns>保存処理のタスク</returns>
        UniTask SaveCharacterIdAsync(AuthPlayerId authPlayerId, CharacterID characterId);

        /// <summary>
        /// 保存されているキャラクターIDを読み込む
        /// </summary>
        /// <param name="authPlayerId">認証されたプレイヤーID</param>
        /// <returns>保存されているキャラクターID（存在しない場合はnull）</returns>
        UniTask<CharacterID?> LoadCharacterIdAsync(AuthPlayerId authPlayerId);

        /// <summary>
        /// 保存されているキャラクターIDが存在するかを確認する
        /// </summary>
        /// <param name="authPlayerId">認証されたプレイヤーID</param>
        /// <returns>存在する場合はtrue</returns>
        UniTask<bool> ExistsCharacterIdAsync(AuthPlayerId authPlayerId);
    }
}