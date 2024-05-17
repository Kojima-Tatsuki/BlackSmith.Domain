using BlackSmith.Domain.Character;
using BlackSmith.Domain.Character.Player;
using System;
using System.Collections.Generic;

#nullable enable

namespace BlackSmith.Usecase.Interface
{
    /// <summary>
    /// すべてのプレイヤーエンティティを保管するリポジトリ
    /// </summary>
    public interface IPlayerCommonEntityRepository
    {
        /// <summary>
        /// キャラクターの新規登録を行う
        /// </summary>
        /// <param name="character">登録を行うキャラクター</param>
        /// <exception cref="InvalidOperationException">既にキャラクターが登録されている場合</exception>
        void Register(PlayerCommonEntity character);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <exception cref="InvalidOperationException">変更を行うキャラクターが存在しない場合</exception>
        void UpdateCharacter(PlayerCommonEntity character);

        PlayerCommonEntity? FindByID(CharacterID id);

        IReadOnlyCollection<PlayerCommonEntity> GetAllPlayers();

        bool IsExist(CharacterID id);

        /// <summary>
        /// キャラクターの登録を削除する
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="InvalidOperationException">削除を行うキャラクターが存在しない場合</exception>
        void Delete(CharacterID id);
    }
}
