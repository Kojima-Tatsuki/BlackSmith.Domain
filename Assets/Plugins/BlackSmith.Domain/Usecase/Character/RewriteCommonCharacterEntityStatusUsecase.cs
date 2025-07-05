using BlackSmith.Domain.Character;
using BlackSmith.Domain.Character.Player;
using BlackSmith.Usecase.Interface;
using Cysharp.Threading.Tasks;
using System;

namespace BlackSmith.Usecase.Character
{
    /// <summary>
    /// キャラクターのステータスに変更を与える際に用いるユースケース
    /// </summary>
    public class RewriteCommonCharacterEntityStatusUsecase
    {
        private readonly ICommonCharacterEntityRepository repository;

        public RewriteCommonCharacterEntityStatusUsecase(ICommonCharacterEntityRepository repository)
        {
            this.repository = repository;
        }

        /// <summary>
        /// キャラクターの名前を書き換える
        /// </summary>
        /// <param name="id">書き換えるキャラクターのID</param>
        /// <param name="newName">書き換え先の名前</param>
        public async UniTask Rename(CharacterID id, string newName)
        {
            var name = new CharacterName(newName);

            var entity = (await repository.FindByID(id)) ?? throw new ArgumentException($"指定したidのキャラクターは存在しません, {id}");

            entity.ChangeName(name);

            await repository.UpdateCharacter((entity as CommonCharacterEntity) ?? throw new InvalidOperationException(nameof(entity)));
        }

        public static bool IsValidPlayerName(string name)
        {
            return CharacterName.IsValid(name);
        }
    }
}