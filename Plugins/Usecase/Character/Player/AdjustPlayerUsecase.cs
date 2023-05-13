using Cysharp.Threading.Tasks;
using BlackSmith.Usecase.Interface;
using BlackSmith.Domain.Character.Player;

namespace BlackSmith.Usecase.Character.Player
{
    /// <summary>
    /// プレイヤーの作成、削除を行うユースケース
    /// </summary>
    public class AdjustPlayerUsecase
    {
        private readonly PlayerFactoryInstructor playerFactory;

        private readonly IPlayerRepository repository;

        private readonly IAccountApi accountApi;

        public AdjustPlayerUsecase(IPlayerRepository playerRepository, IAccountApi accountApi)
        {
            repository = playerRepository;
            this.accountApi = accountApi;

            playerFactory = new PlayerFactoryInstructor(repository);
        }

        /// <summary>
        /// プレイヤーデータの作成を行う
        /// </summary>
        /// <param name="name">作成するプレイヤーの名前</param>
        /// <returns>作成したプレイヤーエンティティのデータ</returns>
        public async UniTask<PlayerEntityData> CreatePlayerAccount(string playerName)
        {
            var name = new PlayerName(playerName);

            // Gs2に接続する必要がある
            await accountApi.CreateAccountAsync(name);

            var entity = playerFactory.CreatePlayerEntity(name);

            return new PlayerEntityData(entity);
        }

        /// <summary>
        /// プレイヤーデータの削除を行う
        /// </summary>
        /// <param name="id">削除するプレイヤーのID</param>
        public void DeletePlayer(PlayerID id)
        {
            repository.Delete(id);
        }
    }
}
