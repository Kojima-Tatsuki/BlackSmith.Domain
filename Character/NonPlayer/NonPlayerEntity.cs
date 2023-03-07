using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using BlackSmith.Domain.Character.Interface;
using BlackSmith.Domain.Character.NonPlayer.Event;
using BlackSmith.Domain.CharacterObjects;
using BlackSmith.Domain.CharacterObjects.Interface;

#nullable enable

namespace BlackSmith.Domain.Character.NonPlayer
{
    public class NonPlayerEntity : ICharacterEntity, ITakeDamageable
    {
        public CharacterID ID { get; }

        public CharacterName Name { get; }

        public HealthPoint HealthPoint { get; private protected set; }

        ICharacterLevel ICharacterEntity.Level => Level;
        public CharacterLevel Level { get; }

        public AttackValue Attack { get; }

        public DefenceValue Defence { get; }

        private readonly NonPlayerEventPublisher publisher;

        internal NonPlayerEntity(NonPlayerCreateCommand command)
        {
            ID = command.ID;
            Name = command.Name;
            HealthPoint = command.HealthPoint;
            Level = command.Level;
            Attack = command.Attack;
            Defence = command.Defence;

            var provider = DIContainer.Instance.ServiceProvider;

            publisher = provider.GetRequiredService<NonPlayerEventPublisher>();
        }

        public HealthPoint TakeDamage(DamageValue damage)
        {
            HealthPoint = HealthPoint.TakeDamage(damage);

            publisher.SetNonPlayerHealth(new NonPlayerHealthChangedEvent(ID, HealthPoint));

            if (HealthPoint.IsDead())
            {
                // On DEAD
            }

            return HealthPoint;
        }

        public HealthPoint HealHealth(int value)
        {
            HealthPoint = HealthPoint.HealHealth(value);

            publisher.SetNonPlayerHealth(new NonPlayerHealthChangedEvent(ID, HealthPoint));

            return HealthPoint;
        }

        public NonPlayerCreateCommand GetCreateCommand()
        {
            return new NonPlayerCreateCommand(ID, Name, HealthPoint, Level, Attack, Defence);
        }
    }
}
