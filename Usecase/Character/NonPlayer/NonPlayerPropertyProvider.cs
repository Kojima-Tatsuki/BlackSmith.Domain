using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using BlackSmith.Domain.Character;
using BlackSmith.Domain.Character.NonPlayer;
using BlackSmith.Repository.Interface;

namespace BlackSmith.Usecase.Character.NonPlayer
{
    public class NonPlayerPropertyProvider
    {
        private readonly ICharacterRepositoty repositoty;

        public NonPlayerPropertyProvider()
        {
            var provider = DIContainer.Instance.ServiceProvider;

            repositoty = provider.GetRequiredService<ICharacterRepositoty>();
        }

        public NonPlayerProperties GetPropertis(CharacterID id)
        {
            var entity = repositoty.FindByID(id);

            return new NonPlayerProperties(entity);
        }
    }

    public class NonPlayerProperties
    {
        public string Name { get; }
        public int CurrentHealth { get; }
        public int MaxHealth { get; }
        public int Level { get; }
        public int Attack { get; }
        public int Defence { get; }

        internal NonPlayerProperties(NonPlayerEntity entity)
        {
            Name = entity.Name.Value;
            var health = entity.HealthPoint.GetValues();
            CurrentHealth = health.current;
            MaxHealth = health.max;
            Level = entity.Level.Value;
            Attack = entity.Attack.Value;
            Defence = entity.Defence.Value;
        }
    }
}
