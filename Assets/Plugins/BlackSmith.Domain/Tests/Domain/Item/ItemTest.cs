using Newtonsoft.Json;
using NUnit.Framework;

namespace BlackSmith.Domain.Item
{
    internal class ItemTest
    {
        [Test(Description = "Itemのシリアライズ・デシリアライズテスト")]
        public void ItemSeializeTestPasses()
        {
            var item = new Item("TestItem");

            var serialized = JsonConvert.SerializeObject(item);
            var deserialized = JsonConvert.DeserializeObject<Item>(serialized);

            Assert.That(item, Is.EqualTo(deserialized));
        }
    }

    internal class ItemNameTest
    {
        [Test(Description = "ItemNameのシリアライズ・デシリアライズテスト")]
        public void ItemNameSeializeTestPasses()
        {
            var name = new ItemName("TestItem");

            var serialized = JsonConvert.SerializeObject(name);
            var deserialized = JsonConvert.DeserializeObject<ItemName>(serialized);

            Assert.That(name, Is.EqualTo(deserialized));
        }
    }
}
