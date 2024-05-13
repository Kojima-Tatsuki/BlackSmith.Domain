using System;

#nullable enable

namespace BlackSmith.Domain.CharacterObject
{
    // Value Objectとして扱いたい
    /// <summary> 装備クラス </summary>
    internal record Equipment
    {
        /// <summary> 装備のObjectID </summary>
        public EquipmentID ID { get; }

        public EquipmentName Name { get; private set; }

        public EquipmentLocation Location { get; }

        internal Equipment(EquipmentName name, EquipmentLocation location, EquipmentID id = null!)
        {
            ID = id ?? new EquipmentID();
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Location = location ?? throw new ArgumentNullException(nameof(location));
        }

        internal void ReName(EquipmentName name)
        {
            Name = name;
        }
    }

    public record EquipmentName
    {
        public string Value { get; }

        internal EquipmentName(string value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }
    }

    public record EquipmentID : BasicID
    {
        protected override string Prefix => "Equipment-";

        public override string ToString()
        {
            return base.ToString();
        }
    }

    public record EquipmentLocation
    {
        public LocationType Value { get; }

        internal EquipmentLocation(LocationType location)
        {
            Value = location;
        }

        public enum LocationType
        {
            None,
            Head,
            Chest,
            Hands,
            Leg,
            Acc,
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    internal record NullEquipment : Equipment
    {
        internal NullEquipment() : base(new EquipmentName("NULL"), new EquipmentLocation(EquipmentLocation.LocationType.None), new EquipmentID())
        {
        }
    }

    internal record HeadEquipment : Equipment
    {
        internal HeadEquipment(EquipmentName name, EquipmentLocation location, EquipmentID id = null!) : base(name, location, id)
        {
        }
    }

    internal record ChestEquipment : Equipment
    {
        internal ChestEquipment(EquipmentName name, EquipmentLocation location, EquipmentID id = null!) : base(name, location, id)
        {
        }
    }

    internal record HandsEquipment : Equipment
    {
        internal HandsEquipment(EquipmentName name, EquipmentLocation location, EquipmentID id = null!) : base(name, location, id)
        {
        }
    }

    internal record LegEquipment : Equipment
    {
        internal LegEquipment(EquipmentName name, EquipmentLocation location, EquipmentID id = null!) : base(name, location, id)
        {
        }
    }
}