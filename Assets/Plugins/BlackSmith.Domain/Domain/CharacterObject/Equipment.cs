using System;

#nullable enable

namespace BlackSmith.Domain.CharacterObject
{
    /// <summary> ‘•”õ, Entity‚ÌƒNƒ‰ƒX </summary>
    internal class Equipment
    {
        /// <summary> ‘•”õ‚ÌObjectID </summary>
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

    public class EquipmentName
    {
        public string Value { get; }

        internal EquipmentName(string value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }
    }

    public class EquipmentID : BasicID
    {
        protected override string Prefix => "Equipment-";

        public override string ToString()
        {
            return base.ToString();
        }
    }

    public class EquipmentLocation : IEquatable<EquipmentLocation>
    {
        public LocationType Value { get; }

        internal EquipmentLocation(LocationType location)
        {
            Value = location;
        }

        public bool Equals(EquipmentLocation? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return Value == other.Value;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;

            if (GetType() != obj.GetType()) return false;
            return Equals((EquipmentLocation)obj);
        }

        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(EquipmentLocation x, EquipmentLocation y)
        {
            return x.Value == y.Value;
        }

        public static bool operator !=(EquipmentLocation x, EquipmentLocation y)
        {
            return x.Value != y.Value;
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

    internal class NullEquipment : Equipment
    {
        internal NullEquipment() : base(new EquipmentName("NULL"), new EquipmentLocation(EquipmentLocation.LocationType.None), new EquipmentID())
        {
        }
    }

    internal class HeadEquipment : Equipment
    {
        internal HeadEquipment(EquipmentName name, EquipmentLocation location, EquipmentID id = null!) : base(name, location, id)
        {
        }
    }

    internal class ChestEquipment : Equipment
    {
        internal ChestEquipment(EquipmentName name, EquipmentLocation location, EquipmentID id = null!) : base(name, location, id)
        {
        }
    }

    internal class HandsEquipment : Equipment
    {
        internal HandsEquipment(EquipmentName name, EquipmentLocation location, EquipmentID id = null!) : base(name, location, id)
        {
        }
    }

    internal class LegEquipment : Equipment
    {
        internal LegEquipment(EquipmentName name, EquipmentLocation location, EquipmentID id = null!) : base(name, location, id)
        {
        }
    }
}