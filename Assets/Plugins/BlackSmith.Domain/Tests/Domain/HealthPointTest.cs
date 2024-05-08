using BlackSmith.Domain.CharacterObject;
using NUnit.Framework;
using System;

public class HealthPointTest
{
    [Test(Description = "HealthPoint インスタンス時のテストを行う"), TestCase(10)]
    public void HealthPointCreatePasses(int value)
    {
        var currentHealth = new HealthPointValue(value);
        var maxHealth = new MaxHealthPointValue(value);
        var health = new HealthPoint(currentHealth, maxHealth);

        Assert.AreEqual(currentHealth, health.Value);
        Assert.AreEqual(maxHealth, health.MaximumValue);

        var healthByMax = new HealthPoint(maxHealth);

        Assert.AreEqual(currentHealth, healthByMax.Value);
        Assert.AreEqual(maxHealth, healthByMax.MaximumValue);
    }

    [Test(Description = "HealthPoint インスタンス時の異常系テスト"), TestCase(15, 10)]
    public void HealthPointInvalidPasses(int current, int max)
    {
        Assert.Catch(typeof(ArgumentException), () =>
        {
            var currentHealth = new HealthPointValue(current);
            var maxHealth = new MaxHealthPointValue(max);
            var health = new HealthPoint(currentHealth, maxHealth);
        });
    }

    [Test(Description = "HelthPointValueインスタンス化テスト")]
    [TestCase(10, 10, null, Category = "正常系")]
    [TestCase(0, 0, null, Category = "正常系")]
    [TestCase(-1, null, typeof(ArgumentException), Category = "正常系")]
    public void HealthPointValueInstancePasses(int value, int result, Type exception = null)
    {
        if (exception is null)
            Assert.That(new HealthPointValue(value), Is.EqualTo(new HealthPointValue(result)));
        else
            Assert.Throws(exception, () => new HealthPointValue(value));
    }

    // HealthPointの比較テスト
    [Test(Description = "HealthPointの比較テスト")]
    [TestCase(10, 10, false, true, false, true, Category = "正常系")]
    [TestCase(10, 20, true, true, false, false, Category = "正常系")]
    [TestCase(20, 10, false, false, true, true, Category = "正常系")]
    public void HealthPointEqualsPasses(int value1, int value2, bool less, bool lesseq, bool more, bool moreeq)
    {
        var health1 = new HealthPointValue(value1);
        var health2 = new HealthPointValue(value2);

        Assert.AreEqual(less, health1 < health2);
        Assert.AreEqual(lesseq, health1 <= health2);
        Assert.AreEqual(more, health1 > health2);
        Assert.AreEqual(moreeq, health1 >= health2);
    }

    [Test(Description = "MaxHealthPointValueインスタンス化テスト")]
    [TestCase(10, 10, null, Category = "正常系")]
    [TestCase(0, 0, null, Category = "正常系")]
    [TestCase(-1, null, typeof(ArgumentException), Category = "正常系")]
    public void MaxHealthPointValueInstancePasses(int value, int result, Type exception = null)
    {
        if (exception is null)
            Assert.That(new MaxHealthPointValue(value), Is.EqualTo(new MaxHealthPointValue(result)));
        else
            Assert.Throws(exception, () => new MaxHealthPointValue(value));
    }
}
