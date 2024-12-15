using BlackSmith.Domain.Character;
using BlackSmith.Domain.Character.Player;
using BlackSmith.Usecase.Interface;
using Cysharp.Threading.Tasks;
using System;

#nullable enable

namespace BlackSmith.Usecase.Character
{
    /// <summary>
    /// CommonPlayerEntityの作成、削除を行うユースケース
    /// </summary>
    public class AdjustPlayerCommonEntityUsecase
    {
        private readonly IPlayerCommonEntityRepository repository;

        public AdjustPlayerCommonEntityUsecase(IPlayerCommonEntityRepository playerRepository)
        {
            repository = playerRepository;
        }

        // 外部にpublicで公開されているのは、各パラメータの値のみであるため、Entityを返却しても問題ない
        /// <summary>
        /// プレイヤーデータの作成を行う
        /// </summary>
        /// <param name="name">作成するプレイヤーの名前</param>
        /// <returns>作成したプレイヤーエンティティ</returns>
        public async UniTask<PlayerCommonEntity> CreateCharacter(string playerName)
        {
            var name = new PlayerName(playerName);

            var entity = PlayerFactory.Create(name);

            await repository.Register(entity);

            return entity;
        }

        /// <summary>
        /// CommonPlayerEntityの再構築を行い、Repositoryに登録する
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async UniTask<PlayerCommonEntity> ReconstructPlayer(PlayerCommonReconstructCommand command)
        {
            var entity = PlayerFactory.Reconstruct(command);

            if (!await repository.IsExist(entity.ID))
                await repository.Register(entity);

            return entity;
        }

        /// <summary>
        /// プレイヤーデータの取得を行う
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async UniTask<PlayerCommonEntity?> GetCharacter(CharacterID id)
        {
            if (!await repository.IsExist(id))
                return null;

            return await repository.FindByID(id);
        }

        /// <summary>
        /// プレイヤーデータの削除を行う
        /// </summary>
        /// <param name="id">削除するプレイヤーのID</param>
        /// <exception cref="InvalidOperationException">指定したidのキャラクターは存在しない場合</exception>
        public async UniTask DeletePlayer(CharacterID id)
        {
            if (!await repository.IsExist(id))
                throw new InvalidOperationException($"指定したidのキャラクターは存在しません {id}");

            await repository.Delete(id);
        }
    }
}
