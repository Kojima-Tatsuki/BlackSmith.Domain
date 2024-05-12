using BlackSmith.Domain.Character.Battle;
using BlackSmith.Domain.Character.Player;
using BlackSmith.Domain.CharacterObject;
using BlackSmith.Domain.Item.Equipment;
using BlackSmith.Domain.PassiveEffect;
using NUnit.Framework;
using System.Collections.Generic;

internal class DefenseValueTest
{
    private static (LevelDependentParameters ldp, BattleEquipmentModule em, BlattleStatusEffectModule sem, int result)[] CorrectMockData()
    {
        var ldp = new LevelDependentParameters(new PlayerLevel(Experience.RequiredCumulativeExp(1)), new Strength(2), new Agility(1));

        var weapon = new EquippableItem(new(
            name: "MockWeapon",
            type: EquipmentType.Weapon,
            attack: new EquipmentAttack(1),
            deffence: new EquipmentDefense(1),
            enchancement: new EnhancementParameter(),
            additional: new AdditionalParameter(),
            require: new RequireParameter(new PlayerLevel(), new Strength(1), new Agility(1))));

        var armor = new EquippableItem(new(
            name: "MockArmor",
            type: EquipmentType.Armor,
            attack: new EquipmentAttack(1),
            deffence: new EquipmentDefense(1),
            enchancement: new EnhancementParameter(),
            additional: new AdditionalParameter(),
            require: new RequireParameter(new PlayerLevel(), new Strength(1), new Agility(1))));
        var nullEquipmentModule = new BattleEquipmentModule(null, null);
        var weaponEqupmentModule = new BattleEquipmentModule(weapon, null);
        var armorEquipmentModule = new BattleEquipmentModule(null, armor);
        var equipmentModule = new BattleEquipmentModule(weapon, armor);

        var id = new EffectID();
        var effect = new BattleStatusEffect(id, new BattleStatusEffectModel(0, 1, 1, 0));
        var statusEffect = new Dictionary<EffectID, BattleStatusEffect>()
        {
            { id, effect }
        };

        var nullStatusEffectModule = new BlattleStatusEffectModule(null);
        var statusEffectModule = new BlattleStatusEffectModule(statusEffect);

        return new (LevelDependentParameters, BattleEquipmentModule, BlattleStatusEffectModule, int)[]{
            new ( ldp, nullEquipmentModule, nullStatusEffectModule, 6),
            new ( ldp, weaponEqupmentModule, nullStatusEffectModule, 6 + 1),
            new ( ldp, armorEquipmentModule, nullStatusEffectModule, 6 + 1),
            new ( ldp, equipmentModule, nullStatusEffectModule, 6 + 2),
            new ( ldp, nullEquipmentModule, statusEffectModule, 6 + 1),
            new ( ldp, weaponEqupmentModule, statusEffectModule, 6 + 1 + 1),
            new ( ldp, armorEquipmentModule, statusEffectModule, 6 + 1 + 1),
            new ( ldp, equipmentModule, statusEffectModule, 6 + 2 + 1),
        };
    }

    [Test(Description = "防御力のインスタンス化テスト")]
    [TestCaseSource(nameof(CorrectMockData), Category = "正常系")]
    public void DefenseValueInstancePasses(LevelDependentParameters levelParams, BattleEquipmentModule? equipmentModule, BattleStatusEffectModel? effectModel)
    {
        // var defense = new DefenseValue(data.lep, data.em, data.sem);

        // Assert.AreEqual(data.result, defense.Value);
    }
}
