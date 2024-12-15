using Newtonsoft.Json;
using NUnit.Framework;
using System;

namespace BlackSmith.Domain.Item
{
    internal class ItemTest
    {
        // Item のテスト
        [Test(Description = "Itemのインスタンス化テスト")]
        [TestCase("ITEM_NAME", "ITEM_NAME", null, Category = "正常系")]
        [TestCase("I", "I", null, Category = "正常系")]
        [TestCase("", null, typeof(ArgumentOutOfRangeException), Category = "正常系")]
        [TestCase(null, null, typeof(ArgumentNullException), Category = "正常系")]
        public void ItemInstance(string itemName, string resultItemName, Type? exception = null)
        {
            if (exception is null)
                Assert.That(new Item(itemName), Is.EqualTo(new Item(resultItemName)));
            else
                Assert.Throws(exception, () => new Item(itemName));
        }

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
        // ItemName のテスト
        [Test(Description = "ItemNameのインスタンス化テスト")]
        [TestCase("ITEM_NAME", "ITEM_NAME", null, Category = "正常系")]
        [TestCase("I", "I", null, Category = "正常系")]
        [TestCase("", null, typeof(ArgumentOutOfRangeException), Category = "正常系")]
        [TestCase(null, null, typeof(ArgumentNullException), Category = "正常系")]
        public void ItemNameInstance(string itemName, string resultItemName, Type? exception = null)
        {
            if (exception is null)
                Assert.That(new ItemName(itemName), Is.EqualTo(new ItemName(resultItemName)));
            else
                Assert.Throws(exception, () => new ItemName(itemName));
        }

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
