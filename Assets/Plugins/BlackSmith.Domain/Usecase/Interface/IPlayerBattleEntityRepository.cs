using BlackSmith.Domain.Character;
using BlackSmith.Domain.Character.Battle;
using System;

#nullable enable

namespace BlackSmith.Usecase.Interface
{
    public interface IPlayerBattleEntityRepository
    {
        /// <summary>
        /// キャラクターの新規登録を行う
        /// </summary>
        /// <param name="character">登録を行うキャラクター</param>
        /// <exception cref="InvalidOperationException">既にキャラクターが登録されている場合</exception>
        void Register(PlayerBattleEntity character);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <exception cref="InvalidOperationException">変更を行うキャラクターが存在しない場合</exception>
        void UpdateCharacter(PlayerBattleEntity character);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>キャラクターが存在しない場合は、nullが返却される</returns>
        PlayerBattleEntity? FindByID(CharacterID id);

        bool IsExist(CharacterID id);

        /// <summary>
        /// キャラクターの登録を削除する
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="InvalidOperationException">削除を行うキャラクターが存在しない場合</exception>
        void Delete(CharacterID id);
    }
}
