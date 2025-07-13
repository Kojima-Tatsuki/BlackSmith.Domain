using BlackSmith.Domain.Character;
using BlackSmith.Domain.Character.Player;
using Cysharp.Threading.Tasks;
using System;

#nullable enable

namespace BlackSmith.Usecase.Interface
{
    /// <summary>
    /// すべてのキャラクターエンティティを保管するリポジトリ
    /// </summary>
    public interface ICommonCharacterEntityRepository
    {
        /// <summary>
        /// キャラクターの新規登録を行う
        /// </summary>
        /// <param name="character">登録を行うキャラクター</param>
        /// <exception cref="InvalidOperationException">既にキャラクターが登録されている場合</exception>
        UniTask Register(CommonCharacterEntity character);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <exception cref="InvalidOperationException">変更を行うキャラクターが存在しない場合</exception>
        UniTask Update(CommonCharacterEntity character);

        UniTask<CommonCharacterEntity?> FindByID(CharacterID id);

        // データベースからすべてのキャラクターを取得することになるため、使用不可とする
        // IReadOnlyCollection<CommonCharacterEntity> GetAllCharacters();

        UniTask<bool> IsExist(CharacterID id);

        /// <summary>
        /// キャラクターの登録を削除する
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="InvalidOperationException">削除を行うキャラクターが存在しない場合</exception>
        UniTask Delete(CharacterID id);
    }
}
