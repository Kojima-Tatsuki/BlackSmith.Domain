using System;
using System.Collections.Generic;
using BlackSmith.Usecase.Interface;
using BlackSmith.Domain.Character.Player;
using BlackSmith.Domain.Character;

namespace BlackSmith.Usecase.Character
{
    /// <summary>
    /// Presenter層にPlayerEntityDataを渡すクラス
    /// </summary>
    public class PlayerEntityDataProvidUsecase
    {
        private readonly IPlayerRepository repository;

        public PlayerEntityDataProvidUsecase(IPlayerRepository playerRepository)
        {
            repository = playerRepository;
        }

        /// <summary>
        /// すべてのプレイヤーのデータを返す
        /// </summary>
        /// <returns>すべてのプレイヤーのデータ</returns>
        public IReadOnlyCollection<PlayerEntityData> GetAllPlayerDatas()
        {
            var entities = repository.GetAllPlayers();

            var result = new List<PlayerEntityData>(entities.Count);

            foreach (var entity in entities)
            {
                result.Add(new PlayerEntityData(entity));
            }

            return result;
        }

        public PlayerEntityData GetPlayerData(CharacterID id)
        {
            if (!repository.IsExist(id))
                throw new Exception($"指定したidのキャラクターは存在しません {id}");

            var entity = repository.FindByID(id) ?? throw new ArgumentException($"指定したidのキャラクターは存在しません {id}");

            return new PlayerEntityData(entity);
        }
    }

    /// <summary>
    /// Usecase層とPresenter層間のPlayerEntityのOutputData
    /// </summary>
    public class PlayerEntityData
    {
        public CharacterID ID { get; }

        public string Name { get; }
        public int Level { get; }
        public int Exp { get; }
        public int CurrentHealth { get; }
        public int MaxHealth { get; }
        
        public int Strength { get; }
        public int Agility { get; }

        public int Attack { get; }
        public int Defence { get; }

        public (int current, int max) Health => (CurrentHealth, MaxHealth);

        // AdjustPlayerUsecaseから呼ぶ再構築用のコンストラクタ
        // DomainObjectからではなく、Usecaseから再構築する事を強制する役割
        internal PlayerEntityData(
            string id, string name,
            int? level, int exp,
            int currentHealth, int maxHealth,
            int strength, int agility,
            int attack, int defence)
        {
            ID = new CharacterID(Guid.Parse(id));
            Name = name;
            Level = level ?? Experience.CurrentLevel(new Experience(exp));
            Exp = exp;
            CurrentHealth = currentHealth;
            MaxHealth = maxHealth;
            Strength = strength; Agility = agility;
            Attack = attack; Defence = defence;
        }

        internal PlayerEntityData(PlayerEntity entity)
        {
            ID = entity.ID;
            Name = entity.Name.Value;
            Level = entity.Level.Value;
            Exp = entity.Level.CumulativeExp.Value;
            var health = entity.HealthPoint.GetValues();
            CurrentHealth = health.current;
            MaxHealth = health.max;
            Strength = entity.BattleModule.LevelDependentParameters.STR.Value;
            Agility = entity.BattleModule.LevelDependentParameters.AGI.Value;
            Attack = entity.Attack.Value;
            Defence = entity.Defense.Value;
        }
    }
}
