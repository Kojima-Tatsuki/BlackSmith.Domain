using System;
using System.Collections;
using System.Collections.Generic;
using BlackSmith.Domain.CharacterObject;
using NUnit.Framework;

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
        Assert.Catch(typeof(ArgumentException), () => {
            var currentHealth = new HealthPointValue(current);
            var maxHealth = new MaxHealthPointValue(max);
            var health = new HealthPoint(currentHealth, maxHealth);
        });
    }
}
