using BlackSmith.Domain.Character;
using BlackSmith.Domain.Character.Player;
using Newtonsoft.Json;
using System;

namespace BlackSmith.Domain.Item.Equipment
{
    public interface IEquippableItem : IItem
    {
        IEquippableItem Enchant(EnhancementParameter parameter);

        IEquippableItem Repair();
    }

    public record EquippableItem : Item, IEquippableItem
    {
        public EquipmentType EquipType { get; }

        public EquipmentAttack Attack { get; }

        public EquipmentDefense Defense { get; }

        public EnhancementParameter EnhancementParameter { get; }

        public AdditionalParameter AdditionalParameter { get; }

        public RequireParameter RequireParameter { get; }

        internal EquippableItem(CreateCommand command) : base(command.ItemName)
        {
            Attack = command.Attack;
            Defense = command.Defense;
            EquipType = command.EquipType;
            EnhancementParameter = command.EnhancementParameter;
            AdditionalParameter = command.AdditionalParameter;
            RequireParameter = command.RequireParameter;
        }

        // Serialize/Deserialize用のコンストラクタ
        [JsonConstructor]
        private EquippableItem(
            string itemName,
            EquipmentType equipType,
            EquipmentAttack attack,
            EquipmentDefense defense,
            EnhancementParameter enhancementParameter,
            AdditionalParameter additionalParameter,
            RequireParameter requireParameter) : base(itemName)
        {
            EquipType = equipType;
            Attack = attack;
            Defense = defense;
            EnhancementParameter = enhancementParameter;
            AdditionalParameter = additionalParameter;
            RequireParameter = requireParameter;
        }

        IEquippableItem IEquippableItem.Enchant(EnhancementParameter parameter) => Enchant(parameter);
        internal EquippableItem Enchant(EnhancementParameter parameter)
        {
            return new EquippableItem(
                new CreateCommand(
                    ItemName,
                    EquipType,
                    Attack,
                    Defense,
                    parameter,
                    AdditionalParameter,
                    RequireParameter
                    ));
        }

        IEquippableItem IEquippableItem.Repair() => Repair();
        internal IEquippableItem Repair()
        {
            throw new NotImplementedException();
        }

        internal record CreateCommand
        {
            public string ItemName { get; }
            public EquipmentType EquipType { get; }
            public EquipmentAttack Attack { get; }
            public EquipmentDefense Defense { get; }
            public EnhancementParameter EnhancementParameter { get; }
            public AdditionalParameter AdditionalParameter { get; }
            public RequireParameter RequireParameter { get; }

            [JsonConstructor]
            internal CreateCommand(
                string itemName,
                EquipmentType type,
                EquipmentAttack attack,
                EquipmentDefense defense,
                EnhancementParameter enhancementParameter,
                AdditionalParameter additionalParameter,
                RequireParameter requireParameter)
            {
                ItemName = itemName;
                EquipType = type;
                Attack = attack;
                Defense = defense;
                EnhancementParameter = enhancementParameter;
                AdditionalParameter = additionalParameter;
                RequireParameter = requireParameter;
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
        Shoos       = 0x40,
        */
    }

    /// <summary>装備攻撃力</summary>
    public record EquipmentAttack
    {
        public int Value { get; }

        [JsonConstructor]
        internal EquipmentAttack(int value)
        {
            Value = value;
        }
    }

    /// <summary>装備防御力</summary>
    public record EquipmentDefense
    {
        public int Value { get; }

        [JsonConstructor]
        internal EquipmentDefense(int value)
        {
            Value = value;
        }
    }

    /// <summary>装備を強化した際に付与されるパラメータ</summary>
    public record EnhancementParameter
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

        [JsonConstructor]
        internal EnhancementParameter(int sharpness, int quickness, int accuracy, int heaviness, int durability)
        {
            Sharpness = sharpness;
            Quickness = quickness;
            Accuracy = accuracy;
            Heaviness = heaviness;
            Durability = durability;
        }

        public int GetEnhancedCount => Sharpness + Quickness + Accuracy + Heaviness + Durability;

        // ここの switch 処理は簡略化可能な気がする
        /// <summary>
        /// 強化を行う. 強化は必ず成功する.
        /// </summary>
        /// <param itemName="equipType">強化するパラメータの種類</param>
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
    public record AdditionalParameter
    {
        public int Attack { get; }
        public int Defense { get; }
        public int STR { get; }
        public int AGI { get; }
    }

    /// <summary>装備を行う際に要求されるパラメータ</summary>
    public record RequireParameter
    {
        public CharacterLevel Level { get; }

        public Strength Strength { get; }
        public Agility Agility { get; }

        [JsonConstructor]
        internal RequireParameter(CharacterLevel level, Strength strength, Agility agility)
        {
            Level = level;
            Strength = strength;
            Agility = agility;
        }
    }
}