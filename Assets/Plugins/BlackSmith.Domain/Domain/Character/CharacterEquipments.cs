using BlackSmith.Domain.CharacterObject;
using System;
using static BlackSmith.Domain.CharacterObject.EquipmentLocation;

#nullable enable

namespace BlackSmith.Domain.Character
{
    /// <summary> キャラクターの装備を格納するクラス </summary>
    internal record CharacterEquipments
    {
        public HeadEquipment? Head { get; private set; }
        public ChestEquipment? Chest { get; private set; }
        public HandsEquipment? Hands { get; private set; }
        public LegEquipment? Leg { get; private set; }

        internal CharacterEquipments(HeadEquipment? head, ChestEquipment? chest, HandsEquipment? hands, LegEquipment? leg)
        {
            Head = head;
            Chest = chest;
            Hands = hands;
            Leg = leg;
        }

        internal CharacterEquipments() => new CharacterEquipments(null, null, null, null);

        /// <summary>
        /// 装備可能なアイテムを対象の場所に装備する
        /// </summary>
        /// <param name="equipment">新たに装備するアイテム</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">指定した場所に既に装備が存在した場合</exception>
        internal CharacterEquipments Equip(Equipment equipment)
        {
            if (equipment is null) throw new ArgumentNullException("Not found equipment. (encRnZ4y)");

            var location = equipment.Location;
            if (!CanEquip(equipment))
                throw new ArgumentException($"指定された装備を装着することは出来ません. Location: {location}. (CptqZbf1)");

            switch (location.Value)
            {
                case LocationType.Head:
                    var head = equipment as HeadEquipment ?? throw new ArgumentException("argument object is not HeadEquipment. (fA2B4vRK)");

                    return new CharacterEquipments(head, Chest, Hands, Leg);
                case LocationType.Chest:
                    var chest = equipment as ChestEquipment ?? throw new ArgumentException("argument object is not ChestEquipment. (Yq9mQbpL)");

                    return new CharacterEquipments(Head, chest, Hands, Leg);
                case LocationType.Hands:
                    var hands = equipment as HandsEquipment ?? throw new ArgumentException("argument object is not HandsEquipment. (S4tz27cP)");

                    return new CharacterEquipments(Head, Chest, hands, Leg);
                case LocationType.Leg:
                    var leg = equipment as LegEquipment ?? throw new ArgumentException("argument object is not LegEquipment. (dE4qBgDJ)");

                    return new CharacterEquipments(Head, Chest, Hands, leg);
                //case LocationType.Acc:
                //    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException("The specified location type is not the expected equipable frame. (p7EtM3XP)");
            }
        }

        internal CharacterEquipments UnEquip(EquipmentLocation location)
        {
            if (location is null) throw new ArgumentNullException("Not found equipment location. (j2XLlWF2)");
            switch (location.Value)
            {
                case LocationType.Head:
                    if (Head is null)
                        throw new ArgumentNullException($"指定された装備スロットに装備が存在しない為、取り外すことは出来ません. Location: {LocationType.Head}, (Q9mNfrqs)");
                    
                    return new CharacterEquipments(null, Chest, Hands, Leg);
                case LocationType.Chest:
                    if (Chest is null)
                        throw new ArgumentNullException($"指定された装備スロットに装備が存在しない為、取り外すことは出来ません. Location: {LocationType.Chest}, (Qr8jLpaK)");

                    return new CharacterEquipments(Head, null, Hands, Leg);
                case LocationType.Hands:
                    if (Hands is null)
                        throw new ArgumentNullException($"指定された装備スロットに装備が存在しない為、取り外すことは出来ません. Location: {LocationType.Hands}, (R7s4fth8)");

                    return new CharacterEquipments(Head, Chest, null, Leg);
                case LocationType.Leg:
                    if (Leg is null)
                        throw new ArgumentNullException($"指定された装備スロットに装備が存在しない為、取り外すことは出来ません. Location: {LocationType.Leg}, (Z9qcngTE)");

                    return new CharacterEquipments(Head, Chest, Hands, null);
                //case LocationType.Acc:
                //    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException("The specified location type is not the expected equipable frame. (Cb86QW2H)");
            }
        }

        internal bool CanEquip(Equipment equipment)
        {
            if (equipment is null) return false;

            // 対象の場所に装備が存在しない場合のみ装備可能
            return IsEmptyEquipment(equipment.Location);
        }

        internal bool IsEmptyEquipment(EquipmentLocation location)
        {
            var result = false;

            switch (location.Value)
            {
                case LocationType.Head:
                    if (Head is null)
                        result = true;
                    break;
                case LocationType.Chest:
                    if (Chest is null)
                        result = true;
                    break;
                case LocationType.Hands:
                    if (Head is null)
                        result = true;
                    break;
                case LocationType.Leg:
                    if (Leg is null)
                        result = true;
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