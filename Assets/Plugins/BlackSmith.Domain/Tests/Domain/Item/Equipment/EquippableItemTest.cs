using BlackSmith.Domain.Character;
using BlackSmith.Domain.Character.Player;
using Newtonsoft.Json;
using NUnit.Framework;

#nullable enable

namespace BlackSmith.Domain.Item.Equipment
{
    public class EquippableItemTest
    {
        [Test(Description = "EquippableItemのシリアライズ・デシリアライズテスト")]
        public void EquippableItemSerializeTestPasses()
        {
            var requireParams = new RequireParameter(new CharacterLevel(Experience.RequiredCumulativeExp(1)), new Strength(1), new Agility(1));
            var command = new EquipableItem.CreateCommand("TestName", EquipmentType.Weapon, new EquipmentAttack(10), new EquipmentDefense(5), new EnhancementParameter(), new AdditionalParameter(), requireParams);
            var item = new EquipableItem(command);

            
            var serialized = JsonConvert.SerializeObject(item);
            var deserialized = JsonConvert.DeserializeObject<EquipableItem>(serialized);

            Assert.That(item, Is.EqualTo(deserialized));
        }

        public class CreateCommandTest
        {
            [Test(Description = "EquippableItem.CreateCommandのシリアライズ・デシリアライズテスト")]
            public void CreateCommandSerializeTestPasses()
            {
                var requireParams = new RequireParameter(new CharacterLevel(Experience.RequiredCumulativeExp(1)), new Strength(1), new Agility(1));
                var command = new EquipableItem.CreateCommand("TestName", EquipmentType.Weapon, new EquipmentAttack(10), new EquipmentDefense(5), new EnhancementParameter(), new AdditionalParameter(), requireParams);

                var serialized = JsonConvert.SerializeObject(command);
                var deserialized = JsonConvert.DeserializeObject<EquipableItem.CreateCommand>(serialized);

                Assert.That(command, Is.EqualTo(deserialized));
            }
        }
    }

    public class EquipmentAttackTest
    {
        [Test(Description = "EquipmentAttackのシリアライズ・デシリアライズテスト")]
        public void EquipmentAttackSerializeTestPasses()
        {
            var attack = new EquipmentAttack(10);

            var serialized = JsonConvert.SerializeObject(attack);
            var deserialized = JsonConvert.DeserializeObject<EquipmentAttack>(serialized);

            Assert.That(attack, Is.EqualTo(deserialized));
        }
    }

    public class EquipmentDefenseTest
    {
        [Test(Description = "EquipmentDefenseのシリアライズ・デシリアライズテスト")]
        public void EquipmentDefenseSerializeTestPasses()
        {
            var defense = new EquipmentDefense(5);

            var serialized = JsonConvert.SerializeObject(defense);
            var deserialized = JsonConvert.DeserializeObject<EquipmentDefense>(serialized);

            Assert.That(defense, Is.EqualTo(deserialized));
        }
    }

    public class EnhancementParameterTest
    {
        [Test(Description = "EnhancementParameterのシリアライズ・デシリアライズテスト")]
        public void EnhancementParameterSerializeTestPasses()
        {
            var parameter = new EnhancementParameter();

            var serialized = JsonConvert.SerializeObject(parameter);
            var deserialized = JsonConvert.DeserializeObject<EnhancementParameter>(serialized);

            Assert.That(parameter, Is.EqualTo(deserialized));
        }
    }

    public class AdditionalParameterTest
    {
        [Test(Description = "AdditionalParameterのシリアライズ・デシリアライズテスト")]
        public void AdditionalParameterSerializeTestPasses()
        {
            var parameter = new AdditionalParameter();

            var serialized = JsonConvert.SerializeObject(parameter);
            var deserialized = JsonConvert.DeserializeObject<AdditionalParameter>(serialized);

            Assert.That(parameter, Is.EqualTo(deserialized));
        }
    }

    public class RequireParameterTest
    {
        [Test(Description = "RequireParameterのシリアライズ・デシリアライズテスト")]
        public void RequireParameterSerializeTestPasses()
        {
            var requireParams = new RequireParameter(new CharacterLevel(Experience.RequiredCumulativeExp(1)), new Strength(1), new Agility(1));

            var serialized = JsonConvert.SerializeObject(requireParams);

            var deserialized = JsonConvert.DeserializeObject<RequireParameter>(serialized);

            Assert.That(requireParams, Is.EqualTo(deserialized));
        }
    }
}