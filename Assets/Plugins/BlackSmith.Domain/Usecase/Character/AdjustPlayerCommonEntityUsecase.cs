using BlackSmith.Domain.Character;
using BlackSmith.Domain.Character.Player;
using BlackSmith.Usecase.Interface;
using Cysharp.Threading.Tasks;
using System;

#nullable enable

namespace BlackSmith.Usecase.Character
{
    /// <summary>
    /// CharacterCommonEntityの作成、削除を行うユースケース
    /// </summary>
    public class AdjustPlayerCommonEntityUsecase
    {
        private readonly ICommonCharacterEntityRepository repository;

        public AdjustPlayerCommonEntityUsecase(ICommonCharacterEntityRepository playerRepository)
        {
            repository = playerRepository;
        }

        // 外部にpublicで公開されているのは、各パラメータの値のみであるため、Entityを返却しても問題ない
        /// <summary>
        /// キャラクターデータの作成を行う
        /// </summary>
        /// <param name="name">作成するキャラクターの名前</param>
        /// <returns>作成したキャラクターエンティティ</returns>
        public async UniTask<CommonCharacterEntity> CreateCharacter(string playerName)
        {
            var name = new CharacterName(playerName);

            var entity = PlayerFactory.Create(name);

            await repository.Register(entity);

            return entity;
        }

        /// <summary>
        /// CharacterCommonEntityの再構築を行い、Repositoryに登録する
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async UniTask<CommonCharacterEntity> ReconstructPlayer(CommonCharacterReconstructCommand command)
        {
            var entity = PlayerFactory.Reconstruct(command);

            if (!await repository.IsExist(entity.ID))
                await repository.Register(entity);

            return entity;
        }

        /// <summary>
        /// キャラクターデータの取得を行う
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async UniTask<CommonCharacterEntity?> GetCharacter(CharacterID id)
        {
            if (!await repository.IsExist(id))
                return null;

            return await repository.FindByID(id);
        }

        /// <summary>
        /// キャラクターデータの削除を行う
        /// </summary>
        /// <param name="id">削除するキャラクターのID</param>
        /// <exception cref="InvalidOperationException">指定したidのキャラクターは存在しない場合</exception>
        public async UniTask DeletePlayer(CharacterID id)
        {
            if (!await repository.IsExist(id))
                throw new InvalidOperationException($"指定したidのキャラクターは存在しません {id}");

            await repository.Delete(id);
        }
    }
}
