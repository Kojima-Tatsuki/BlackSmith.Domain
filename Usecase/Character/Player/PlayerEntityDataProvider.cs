using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using BlackSmith.Repository.Interface;
using BlackSmith.Usecase.Interface;
using BlackSmith.Domain.CharacterObjects;
using BlackSmith.Domain.Player;

#nullable enable

namespace BlackSmith.Usecase.Player
{
    /// <summary>
    /// Presenter層にPlayerEntityDataを渡すクラス
    /// </summary>
    public class PlayerEntityDataProvider : IPlayerEntityDataProvider
    {
        private readonly PlayerRepositoryInstructor instructor;

        private readonly IPlayerRepository repository;

        private readonly ISessionPlayerIdRepository sessionRepository;

        public PlayerEntityDataProvider()
        {
            var provider = DIContainer.Instance.ServiceProvider;

            repository = provider.GetRequiredService<IPlayerRepository>();
            
            sessionRepository = provider.GetRequiredService<ISessionPlayerIdRepository>();

            instructor = new PlayerRepositoryInstructor(repository);
        }

        IReadOnlyCollection<PlayerEntityData> IPlayerEntityDataProvider.GetAllPlayerDatas()
        {
            var entities = instructor.GetAllPlayerEntities();

            var result = new List<PlayerEntityData>(entities.Count);

            foreach (var entity in entities)
            {
                result.Add(new PlayerEntityData(entity));
            }

            return result;
        }

        PlayerEntityData? IPlayerEntityDataProvider.GetPlayerData()
        {
            var id = sessionRepository.GetId();

            // 現在ゲーム中のプレイヤーが存在しないとき
            if (id is null)
                return null;

            if (!repository.IsExist(id))
                throw new Exception("存在しないIDがゲーム中のIDとして設定されています");

            var entity = instructor.GetPlayerEntity(id);

            return new PlayerEntityData(entity);
        }
    }

    /// <summary>
    /// Usecase層とPresenter層間のPlayerEntityのOutputData
    /// </summary>
    public class PlayerEntityData
    {
        public PlayerID ID { get; }

        public string Name { get; }
        public int Level { get; }
        public int CurrentHealth { get; }
        public int MaxHealth { get; }
        public int Attack { get; }
        public int Defence { get; }

        public (int current, int max) Health => (CurrentHealth, MaxHealth);

        internal PlayerEntityData(
            PlayerID id,
            string name,
            int level,
            int currentHealth, int maxHealth,
            int attack,
            int defence)
        {
            ID = id;
            Name = name;
            Level = level;
            CurrentHealth = currentHealth;
            MaxHealth = maxHealth;
            Attack = attack;
            Defence = defence;
        }

        internal PlayerEntityData(PlayerEntity entity)
        {
            ID = entity.ID;
            Name = entity.Name.Value;
            Level = entity.Level.Value;
            var health = entity.HealthPoint.GetValues();
            CurrentHealth = health.current;
            MaxHealth = health.max;
            Attack = entity.Attack.Value;
            Defence = entity.Defence.Value;
        }
    }
}
