using BlackSmith.Domain.Character;
using BlackSmith.Domain.Character.Player;
using BlackSmith.Usecase.Interface;
using Cysharp.Threading.Tasks;
using System;

namespace BlackSmith.Usecase.Character
{
    /// <summary>
    /// CommonPlayerEntityの作成、削除を行うユースケース
    /// </summary>
    public class AdjustPlayerCommonUsecase
    {
        private readonly IPlayerCommonEntityRepository repository;

        public AdjustPlayerCommonUsecase(IPlayerCommonEntityRepository playerRepository)
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
        public async UniTask<PlayerCommonEntity> ReconstructPlayer(PlayerCommonReconstractCommand command)
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
        /// <exception cref="ArgumentException">指定したIdのキャラクターが存在しない場合</exception>
        public async UniTask<PlayerCommonEntity> GetCharacter(CharacterID id)
        {
            if (!await repository.IsExist(id))
                throw new ArgumentException($"指定したidのキャラクターは存在しません {id}");

            var entity = (await repository.FindByID(id)) ?? throw new ArgumentException($"指定したidのキャラクターは存在しません {id}");

            return entity;
        }

        /// <summary>
        /// プレイヤーデータの削除を行う
        /// </summary>
        /// <param name="id">削除するプレイヤーのID</param>
        public async UniTask DeletePlayer(CharacterID id)
        {
            await repository.Delete(id);
        }
    }
}
