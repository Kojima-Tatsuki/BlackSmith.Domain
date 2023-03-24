using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using BlackSmith.Domain.Character;
using BlackSmith.Domain.Character.NonPlayer;
using BlackSmith.Repository.Interface;

namespace BlackSmith.Usecase.Character.NonPlayer
{
    /// <summary>
    /// NPCの作成、削除を行うユースケース
    /// </summary>
    public class AdjustNonPlayerUsecase
    {
        private readonly ICharacterRepositoty repository;

        public AdjustNonPlayerUsecase()
        {
            var provider = DIContainer.Instance.ServiceProvider;

            repository = provider.GetRequiredService<ICharacterRepositoty>();
        }

        public CharacterID CreateNonPlayerEntity(EnemyCreateCommand command)
        {
            var factory = new NonPlayerFactory();

            var entity = factory.Create(command.Name, command.Health, command.Level, command.Attack, command.Defence);

            repository.Register(entity);

            return entity.ID;
        }

        public class EnemyCreateCommand
        {
            public string Name { get; }
            public int Health { get; }
            public int Level { get; }
            public int Attack { get; }
            public int Defence { get; }

            public EnemyCreateCommand(string name, int health, int level, int attack, int defence)
            {
                Name = name;
                Health = health;
                Level = level;
                Attack = attack;
                Defence = defence;
            }
        }
    }
}
