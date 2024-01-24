using BlackSmith.Domain.Character.Player;
using System;

namespace BlackSmith.Domain.Item.Equipment
{
    public interface IEquipableItem : IItem
    {
        IEquipableItem Enchant(EnhancementParameter parameter);

        IEquipableItem Repair();
    }

    public class EquippableItem : Item, IEquipableItem
    {
        public EquipmentType EquipType { get; }

        public EquipmentAttack Attack { get; }

        public EquipmentDefense Defense { get; }

        public EnhancementParameter EnhancementParameter { get; }

        public AdditionalParameter AdditionalParameter { get; }

        public RequireParameter RequireParameter { get; }

        internal EquippableItem(CreateCommand command) : base(command.Name)
        {
            Attack = command.Attack;
            Defense = command.Defence;
            EquipType = command.Type;
            EnhancementParameter = command.Enhancement;
            AdditionalParameter = command.Additional;
            RequireParameter = command.Require;
        }

        IEquipableItem IEquipableItem.Enchant(EnhancementParameter parameter) => Enchant(parameter);
        internal EquippableItem Enchant(EnhancementParameter parameter)
        {
            return new EquippableItem(
                new CreateCommand(
                    Name,
                    EquipType,
                    Attack,
                    Defense,
                    parameter,
                    AdditionalParameter,
                    RequireParameter
                    ));
        }

        IEquipableItem IEquipableItem.Repair() => Repair();
        internal IEquipableItem Repair()
        {
            throw new NotImplementedException();
        }

        internal record CreateCommand
        {
            public string Name { get; }
            public EquipmentType Type { get; }
            public EquipmentAttack Attack { get; }
            public EquipmentDefense Defence { get; }
            public EnhancementParameter Enhancement { get; }
            public AdditionalParameter Additional { get; }
            public RequireParameter Require { get; }

            internal CreateCommand(
                string name,
                EquipmentType type,
                EquipmentAttack attack,
                EquipmentDefense deffence,
                EnhancementParameter enchancement,
                AdditionalParameter additional,
                RequireParameter require)
            {
                Name = name;
                Type = type;
                Attack = attack;
                Defence = deffence;
                Additional = additional;
                Enhancement = enchancement;
                Require = require;
            }
        }
    }

    /// <summary>装備種類</summary>
    public enum EquipmentType
    {
        Weapon,     // 武器
        Armor,      // 防具
        Accessary,  // アクセサリ

        /*
        None,       = 0x00,
        MainWeapon  = 0x01,
        SubWeapon   = 0x02,
        Head        = 0x04,
        Body        = 0x08,
        Hand        = 0x10,
        Leg         = 0x20,
        Shoose      = 0x40,
        */
    }

    /// <summary>装備攻撃力</summary>
    public class EquipmentAttack
    {
        public int Value { get; }
        internal EquipmentAttack(int value)
        {
            Value = value;
        }
    }

    /// <summary>装備防御力</summary>
    public class EquipmentDefense
    {
        public int Value { get; }
        internal EquipmentDefense(int value)
        {
            Value = value;
        }
    }

    /// <summary>装備を強化した際に付与されるパラメータ</summary>
    public class EnhancementParameter
    {
        public int Sharpness { get; } // 鋭さ
        public int Quickness { get; } // 速さ
        public int Accuracy { get; } // 正確さ
        public int Heaviness { get; } // 重さ
        public int Durability { get; } // 丈夫さ

        internal EnhancementParameter()
        {
            Sharpness = 0;
            Quickness = 0;
            Accuracy = 0;
            Heaviness = 0;
            Durability = 0;
        }

        internal EnhancementParameter(int sharpness, int quickness, int accuracy, int heaviness, int durability)
        {
            Sharpness = sharpness;
            Quickness = quickness;
            Accuracy = accuracy;
            Heaviness = heaviness;
            Durability = durability;
        }

        public int GetEnchancedCount => Sharpness + Quickness + Accuracy + Heaviness + Durability;

        // ここのswitch 処理は簡略化可能な気がする
        /// <summary>
        /// 強化を行う. 強化は必ず成功する.
        /// </summary>
        /// <param name="type">強化するパラメータの種類</param>
        /// <returns>強化結果</returns>
        internal EnhancementParameter AddEnhance(EnhanceType type)
        {
            var sharpness = Sharpness;
            var quickness = Quickness;
            var accuracy = Accuracy;
            var heaviness = Heaviness;
            var durability = Durability;

            switch (type)
            {
                case EnhanceType.Sharpness:
                    sharpness++;
                    break;
                case EnhanceType.Quickness:
                    quickness++;
                    break;
                case EnhanceType.Accuracy:
                    accuracy++;
                    break;
                case EnhanceType.Heaviness:
                    heaviness++;
                    break;
                case EnhanceType.Durability:
                    durability++;
                    break;
                default:
                    break;
            }

            return new EnhancementParameter(sharpness, quickness, accuracy, heaviness, durability);
        }

        public enum EnhanceType
        {
            Sharpness,
            Quickness,
            Accuracy,
            Heaviness,
            Durability,
        }
    }

    /// <summary>装備が作成される際にランダムで付与される追加パラメータ</summary>
    public class AdditionalParameter
    {
        public int Attack { get; }
        public int Defense { get; }
        public int STR { get; }
        public int AGI { get; }
    }

    /// <summary>装備を行う際に要求されるパラメータ</summary>
    public class RequireParameter
    {
        public PlayerLevel Level { get; }

        internal Strength Strength { get; }
        internal Agility Agility { get; }

        internal RequireParameter(PlayerLevel level, Strength strength, Agility agility)
        {
            Level = level;
            Strength = strength;
            Agility = agility;
        }
    }
}