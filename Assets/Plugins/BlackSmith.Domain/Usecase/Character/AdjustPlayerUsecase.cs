using BlackSmith.Domain.Character;
using BlackSmith.Domain.Character.Player;
using BlackSmith.Domain.CharacterObject;
using BlackSmith.Usecase.Interface;

namespace BlackSmith.Usecase.Character
{
    /// <summary>
    /// プレイヤーの作成、削除を行うユースケース
    /// </summary>
    public class AdjustPlayerUsecase
    {
        private readonly IPlayerRepository repository;

        public AdjustPlayerUsecase(IPlayerRepository playerRepository)
        {
            repository = playerRepository;
        }

        /// <summary>
        /// プレイヤーデータの作成を行う
        /// </summary>
        /// <param name="name">作成するプレイヤーの名前</param>
        /// <returns>作成したプレイヤーエンティティのデータ</returns>
        public PlayerEntityData CreateCharacter(string playerName)
        {
            var name = new PlayerName(playerName);

            var entity = PlayerFactory.Create(name);

            repository.Register(entity);

            return new PlayerEntityData(entity);
        }

        public void ReconstrunctPlayer(PlayerEntityData data)
        {
            var command = new PlayerCreateCommand(
                data.ID, new PlayerName(data.Name),
                new HealthPoint(new(data.CurrentHealth), new(data.MaxHealth)),
                new LevelDependentParameters(new PlayerLevel(new(data.Exp)), new(data.Strength), new(data.Agility)));

            var entity = PlayerFactory.Create(command);

            repository.Register(entity);
        }

        public void ReconstrunctPlayer(
                string id, string name, int? level, int exp, int currentHealth, int maxHealth,
                int strength, int agility, int attack, int defence)
        {
            var data = new PlayerEntityData(id, name, level, exp, currentHealth, maxHealth, strength, agility, attack, defence);
            ReconstrunctPlayer(data);
        }

        /// <summary>
        /// プレイヤーデータの削除を行う
        /// </summary>
        /// <param name="id">削除するプレイヤーのID</param>
        public void DeletePlayer(CharacterID id)
        {
            repository.Delete(id);
        }
    }
}
