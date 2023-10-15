using BlackSmith.Domain.Character.Player;
using NUnit.Framework;
using System;

internal class LevelDependentParametersTest
{
    [Test(Description = "LevelDependentParametersTest�̃C���X�^���X���e�X�g")]
    public void InstancePasses()
    {
        Assert.That(() => new LevelDependentParameters() != null);

        var level = new PlayerLevel();
        var str = new Strength(1);
        var agi = new Agility(1);

        Assert.That(() => new LevelDependentParameters(level, str, agi) != null);
    }

    [Test(Description = "�X�e�[�^�X�㏸�|�C���g�̎c�萔�e�X�g")]
    [TestCase(1, 1, 1, ExpectedResult = 1, Category = "����n")]
    [TestCase(5, 10, 5, ExpectedResult = 0, Category = "����n")]

    public int GetRemainingParamPointPasses(int level, int str, int agi)
    {
        return new LevelDependentParameters(new(Experience.RequiredCumulativeExp(level)), new(str), new(agi))
            .GetRemainingParamPoint();
    }

    [Test()]
    [TestCase(1, 3, 1, Category = "�ُ�n")]
    public void GetRemainingParamPointFail(int level, int str, int agi)
    {
        Assert.Throws(Is.TypeOf<ArgumentException>().And.Message.EqualTo($"�w�肵��STR, AGI�͊����\�ʂ𒴉߂��Ă��܂� STR: {str}, AGI: {agi}"),
        () => new LevelDependentParameters(new(Experience.RequiredCumulativeExp(level)), new(str), new(agi))
                .GetRemainingParamPoint());
    }
}
