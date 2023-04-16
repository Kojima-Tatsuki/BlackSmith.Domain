using System;
using System.Collections.Generic;
using BlackSmith.Domain.Character.Player;
using BlackSmith.Usecase.Interface;

namespace BlackSmith.Usecase.Character.Player
{
    internal class PlayerRepositoryInstructor
    {
        private readonly IPlayerRepository repository;

        internal PlayerRepositoryInstructor(IPlayerRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        internal PlayerEntity GetPlayerEntity(PlayerID id)
        {
            var entity = repository.FindByID(id)
                ?? throw new NullReferenceException($"指定されたIDのプレイヤーは存在しません, ID: {id}");

            return entity;
        }

        internal IReadOnlyCollection<PlayerEntity> GetAllPlayerEntities()
        {
            var entities = repository.GetAllPlayers();

            var results = new List<PlayerEntity>(entities.Count);

            foreach (var entity in entities)
            {
                results.Add(entity);
            }

            return results;
        }

        internal void UpdatePlayerEntity(PlayerEntity entity)
        {
            repository.UpdateCharacter(entity);
        }
    }
}
