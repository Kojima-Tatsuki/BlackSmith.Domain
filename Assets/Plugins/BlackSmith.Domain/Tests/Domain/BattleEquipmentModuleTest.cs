using BlackSmith.Domain.Character.Battle;
using BlackSmith.Domain.Item;
using BlackSmith.Domain.Item.Equipment;
using NUnit.Framework;
using System;

public class BattleEquipmentModuleTest
{
    // ����n�p���b�N�f�[�^
    private static EquippableItem?[][] CorrectMockData()
    {
        var weapon = new EquippableItem(new(
            name: "MockWeapon",
            type: EquipmentType.Weapon,
            attack: new EquipmentAttack(1),
            deffence: new EquipmentDefense(1),
            enchancement: new EnhancementParameter(),
            additional: new AdditionalParameter(),
            require: new RequireParameter()));

        var armor = new EquippableItem(new(
            name: "MockArmor",
            type: EquipmentType.Armor,
            attack: new EquipmentAttack(1),
            deffence: new EquipmentDefense(1),
            enchancement: new EnhancementParameter(),
            additional: new AdditionalParameter(),
            require: new RequireParameter()));

        return new EquippableItem?[][] { 
            new EquippableItem?[] { null, null },
            new EquippableItem?[] { weapon, null },
            new EquippableItem?[] { null, armor },
            new EquippableItem?[] { weapon, armor }
        };
    }

    // �ُ�n�p���b�N�f�[�^
    private static EquippableItem?[][] IncorrectMockData()
    {
        var currectWeapon = new EquippableItem(new(
            name: "MockWeapon",
            type: EquipmentType.Weapon,
            attack: new EquipmentAttack(1),
            deffence: new EquipmentDefense(1),
            enchancement: new EnhancementParameter(),
            additional: new AdditionalParameter(),
            require: new RequireParameter()));

        var inCorrectWeapon = new EquippableItem(new(
            name: "MockWeapon",
            type: EquipmentType.Armor,
            attack: new EquipmentAttack(1),
            deffence: new EquipmentDefense(1),
            enchancement: new EnhancementParameter(),
            additional: new AdditionalParameter(),
            require: new RequireParameter()));

        var armor = new EquippableItem(new(
            name: "MockArmor",
            type: EquipmentType.Armor,
            attack: new EquipmentAttack(1),
            deffence: new EquipmentDefense(1),
            enchancement: new EnhancementParameter(),
            additional: new AdditionalParameter(),
            require: new RequireParameter()));

        return new EquippableItem?[][] {
            new EquippableItem?[] { currectWeapon, currectWeapon },
            new EquippableItem?[] { inCorrectWeapon, null },
            new EquippableItem?[] { inCorrectWeapon, armor }
        };
    }

    [Test(Description = "�������W���[���̃C���X�^���X�𐶐�����e�X�g")]
    [TestCaseSource(nameof(CorrectMockData), Category = "����n")]
    public void ModuleInstancePasses(EquippableItem? weapon, EquippableItem? armor)
    {
        try {
            var module = new BattleEquipmentModule(weapon, armor);
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }

    [Test(Description = "�������W���[���̃C���X�^���X�𐶐�����e�X�g")]
    [TestCaseSource(nameof(IncorrectMockData), Category = "�ُ�n")]
    public void ModuleInstanceFail(EquippableItem? weapon, EquippableItem? armor)
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var module = new BattleEquipmentModule(weapon, armor);
        });
    }
}