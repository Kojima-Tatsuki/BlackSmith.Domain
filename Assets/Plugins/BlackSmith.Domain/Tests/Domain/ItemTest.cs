using BlackSmith.Domain.Item;
using NUnit.Framework;
using System;

#nullable enable

public class ItemTest
{
    // Item のテスト
    [Test(Description = "ItemNameのインスタンス化テスト")]
    [TestCase("ITEM_NAME", "ITEM_NAME", null, Category = "正常系")]
    [TestCase("I", "I" , null, Category = "正常系")]
    [TestCase("", null, typeof(ArgumentOutOfRangeException), Category = "正常系")]
    [TestCase(null, null, typeof(ArgumentNullException), Category = "正常系")]
    public void ItemInstance(string itemName, string resultItemName, Type? exception = null)
    {
        if (exception is null)
            Assert.That(new Item(itemName), Is.EqualTo(new Item(resultItemName)));
        else
            Assert.Throws(exception, () => new Item(itemName));
    }

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
}