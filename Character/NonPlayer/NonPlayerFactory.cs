using BlackSmith.Domain.CharacterObjects;

namespace BlackSmith.Domain.Character.NonPlayer
{
    public class NonPlayerFactory
    {
        public NonPlayerEntity Create(NonPlayerCreateCommand command)
        {
            return new NonPlayerEntity(command);
        }

        public NonPlayerEntity Create(string name, int maxHealth, int level, int attack, int defence)
        {
            var command = new NonPlayerCreateCommand(
                Guid.NewGuid(),
                name,
                maxHealth, maxHealth,
                level,
                attack, defence);

            return Create(command);
        }
    }

    public class NonPlayerCreateCommand
    {
        public CharacterID ID { get; }

        public CharacterName Name { get; }

        public HealthPoint HealthPoint { get; }

        public CharacterLevel Level { get; }

        public AttackValue Attack { get; }

        public DefenceValue Defence { get; }

        public NonPlayerCreateCommand(Guid id, string name, int currentHealth, int maxHealth, int level, int attack, int deffence)
        {
            ID = new CharacterID(id);
            Name = new CharacterName(name);
            HealthPoint = new HealthPoint(
                new HealthPointValue(currentHealth),
                new MaxHealthPointValue(maxHealth));
            Level = new CharacterLevel(level);
            Attack = new AttackValue(attack);
            Defence = new DefenceValue(deffence);
        }

        internal NonPlayerCreateCommand(CharacterID id, CharacterName name, HealthPoint health, CharacterLevel level, AttackValue attack, DefenceValue defence)
        {
            ID = id;
            Name = name;
            HealthPoint = health;
            Level = level;
            Attack = attack;
            Defence = defence;
        }
    }
}
