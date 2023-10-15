using System;
using BlackSmith.Domain.CharacterObject;

namespace BlackSmith.Domain.Character
{
    /// <summary> キャラクターの装備を格納するクラス </summary>
    internal class CharacterEquipments
    {
        public CharacterID ID { get; }

        public HeadEquipment? Head { get; private set; }
        public ChestEquipment? Chest { get; private set; }
        public HandsEquipment? Hands { get; private set; }
        public LegEquipment? Leg { get; private set; }

        internal CharacterEquipments(CharacterID id)
        {
            ID = id ?? throw new ArgumentNullException("Not found CharacterId. (GYKvim5T)");

            Head = null!;
            Chest = null!;
            Hands = null!;
            Leg = null!;
        }

        internal void Equip(Equipment equipment)
        {
            if (equipment is null) throw new ArgumentNullException("Not found equipment. (encRnZ4y)");
            var location = equipment.Location;
            if (!IsEmptyEquipment(location))
                throw new ArgumentException($"指定されたスロットには既にアイテムが含まれています. location: {location}. (CptqZbf1)");

            switch (location.Value)
            {
                case EquipmentLocation.LocationType.None:
                    break;

                case EquipmentLocation.LocationType.Head:
                    Head = equipment as HeadEquipment;
                    break;

                case EquipmentLocation.LocationType.Chest:
                    Chest = equipment as ChestEquipment;
                    break;

                case EquipmentLocation.LocationType.Hands:
                    Hands = equipment as HandsEquipment;
                    break;

                case EquipmentLocation.LocationType.Leg:
                    Leg = equipment as LegEquipment;
                    break;

                case EquipmentLocation.LocationType.Acc:
                    break;

                default:
                    break;
            }
        }

        internal Equipment UnEquip(EquipmentLocation location)
        {
            if (location is null) throw new ArgumentNullException("Not found equipment location. (j2XLlWF2)");

            return new NullEquipment();
        }

        internal bool IsEmptyEquipment(EquipmentLocation location)
        {
            var result = false;

            switch (location.Value)
            {
                case EquipmentLocation.LocationType.None:
                    result = true;
                    break;

                case EquipmentLocation.LocationType.Head:
                    if (Head is null)
                        result = true;
                    break;

                case EquipmentLocation.LocationType.Chest:
                    if (Chest is null)
                        result = true;
                    break;

                case EquipmentLocation.LocationType.Hands:
                    if (Head is null)
                        result = true;
                    break;

                case EquipmentLocation.LocationType.Leg:
                    if (Leg is null)
                        result = true;
                    break;

                case EquipmentLocation.LocationType.Acc:
                    break;

                default:
                    break;
            }

            return result;
        }
    }

    /*public struct buff
    {
        public List<Effect.BuffEffectData.buffData> Effects;
    }*/
}