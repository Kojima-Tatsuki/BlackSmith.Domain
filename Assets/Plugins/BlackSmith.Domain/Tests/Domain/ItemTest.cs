using BlackSmith.Domain.Item;
using NUnit.Framework;
using System;

#nullable enable

public class ItemTest
{
    // Item �̃e�X�g
    [Test(Description = "ItemName�̃C���X�^���X���e�X�g")]
    [TestCase("ITEM_NAME", "ITEM_NAME", null, Category = "����n")]
    [TestCase("I", "I" , null, Category = "����n")]
    [TestCase("", null, typeof(ArgumentOutOfRangeException), Category = "����n")]
    [TestCase(null, null, typeof(ArgumentNullException), Category = "����n")]
    public void ItemInstance(string itemName, string resultItemName, Type? exception = null)
    {
        if (exception is null)
            Assert.That(new Item(itemName), Is.EqualTo(new Item(resultItemName)));
        else
            Assert.Throws(exception, () => new Item(itemName));
    }

    // ItemName �̃e�X�g
    [Test(Description = "ItemName�̃C���X�^���X���e�X�g")]
    [TestCase("ITEM_NAME", "ITEM_NAME", null, Category = "����n")]
    [TestCase("I", "I", null, Category = "����n")]
    [TestCase("", null, typeof(ArgumentOutOfRangeException), Category = "����n")]
    [TestCase(null, null, typeof(ArgumentNullException), Category = "����n")]
    public void ItemNameInstance(string itemName, string resultItemName, Type? exception = null)
    {
        if (exception is null)
            Assert.That(new ItemName(itemName), Is.EqualTo(new ItemName(resultItemName)));
        else
            Assert.Throws(exception, () => new ItemName(itemName));
    }
}