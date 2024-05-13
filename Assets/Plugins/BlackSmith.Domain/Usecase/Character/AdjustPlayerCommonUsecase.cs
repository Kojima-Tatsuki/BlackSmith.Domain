using BlackSmith.Domain.Character;
using BlackSmith.Domain.Character.Player;
using BlackSmith.Domain.CharacterObject;
using BlackSmith.Usecase.Interface;

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

        /// <summary>
        /// プレイヤーデータの作成を行う
        /// </summary>
        /// <param name="name">作成するプレイヤーの名前</param>
        /// <returns>作成したプレイヤーエンティティのデータ</returns>
        public PlayerCommonEntity CreateCharacter(string playerName)
        {
            var name = new PlayerName(playerName);

            var entity = PlayerFactory.Create(name);

            repository.Register(entity);

            return entity;
        }

        public PlayerCommonEntity RreconstructPlayer(PlayerCommonReconstractPrimitiveModel model)
        {
            var command = model.ToCommand();

            var entity = PlayerFactory.Reconstruct(command);

            repository.Register(entity);

            return entity;
        }

        /// <summary>
        /// プレイヤーデータの削除を行う
        /// </summary>
        /// <param name="id">削除するプレイヤーのID</param>
        public void DeletePlayer(CharacterID id)
        {
            repository.Delete(id);
        }

        public record PlayerCommonReconstractPrimitiveModel(string Id, string Name, int Exp)
        {
            internal PlayerCommonReconstractCommand ToCommand()
            {
                var id = new CharacterID(Id);
                var name = new PlayerName(Name);
                var level = new PlayerLevel(new Experience(Exp));

                return new PlayerCommonReconstractCommand(id, name, level);
            }
        }
    }
}
