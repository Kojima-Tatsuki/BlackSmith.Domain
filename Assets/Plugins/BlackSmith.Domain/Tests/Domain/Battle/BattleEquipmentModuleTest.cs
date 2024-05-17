using BlackSmith.Domain.Character.Battle;
using BlackSmith.Domain.Character.Player;
using BlackSmith.Domain.Item.Equipment;
using NUnit.Framework;
using System;
using System.Linq;

#nullable enable

internal class BattleEquipmentModuleTest
{
    public static BattleEquipmentModule[] GetBattleEquipmentModuleMocks()
    {
        return CorrectMockDatas().Select(data => new BattleEquipmentModule(data[0], data[1])).ToArray();
    }

    // 正常系用モックデータ
    private static EquippableItem?[][] CorrectMockDatas()
    {
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

        return new EquippableItem?[][] {
            new EquippableItem?[] { null, null },
            new EquippableItem?[] { weapon, null },
            new EquippableItem?[] { null, armor },
            new EquippableItem?[] { weapon, armor }
        };
    }

    // 異常系用モックデータ
    private static EquippableItem?[][] IncorrectMockDatas()
    {
        var currectWeapon = new EquippableItem(new(
            name: "MockWeapon",
            type: EquipmentType.Weapon,
            attack: new EquipmentAttack(1),
            deffence: new EquipmentDefense(1),
            enchancement: new EnhancementParameter(),
            additional: new AdditionalParameter(),
            require: new RequireParameter(new PlayerLevel(), new Strength(1), new Agility(1))));

        var inCorrectWeapon = new EquippableItem(new(
            name: "MockWeapon",
            type: EquipmentType.Armor,
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

        return new EquippableItem?[][] {
            new EquippableItem?[] { currectWeapon, currectWeapon },
            new EquippableItem?[] { inCorrectWeapon, null },
            new EquippableItem?[] { inCorrectWeapon, armor }
        };
    }

    [Test(Description = "装備モジュールのインスタンスを生成するテスト")]
    [TestCaseSource(nameof(CorrectMockDatas), Category = "正常系")]
    public void ModuleInstancePasses(EquippableItem? weapon, EquippableItem? armor)
    {
        try
        {
            var module = new BattleEquipmentModule(weapon, armor);
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }

    [Test(Description = "装備モジュールのインスタンスを生成するテスト")]
    [TestCaseSource(nameof(IncorrectMockDatas), Category = "異常系")]
    public void ModuleInstanceFail(EquippableItem? weapon, EquippableItem? armor)
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var module = new BattleEquipmentModule(weapon, armor);
        });
    }
}