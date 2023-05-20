using System;
using System.Collections;
using System.Collections.Generic;
using BlackSmith.Domain.CharacterObject;
using NUnit.Framework;

public class HealthPointTest
{
    [Test(Description = "HealthPoint �C���X�^���X���̃e�X�g���s��"), TestCase(10)]
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

    [Test(Description = "HealthPoint �C���X�^���X���ُ̈�n�e�X�g"), TestCase(15, 10)]
    public void HealthPointInvalidPasses(int current, int max)
    {
        Assert.Catch(typeof(ArgumentException), () => {
            var currentHealth = new HealthPointValue(current);
            var maxHealth = new MaxHealthPointValue(max);
            var health = new HealthPoint(currentHealth, maxHealth);
        });
    }
}
