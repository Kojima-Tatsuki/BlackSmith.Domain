using BlackSmith.Domain.Character.Player;
using NUnit.Framework;
using System;

internal class LevelDependentParametersTest
{
    [Test(Description = "LevelDependentParametersTestのインスタンス化テスト")]
    public void InstancePasses()
    {
        Assert.That(() => new LevelDependentParameters() != null);

        var level = new PlayerLevel();
        var str = new Strength(1);
        var agi = new Agility(1);

        Assert.That(() => new LevelDependentParameters(level, str, agi) != null);
    }

    [Test(Description = "ステータス上昇ポイントの残り数テスト")]
    [TestCase(1, 1, 1, ExpectedResult = 1, Category = "正常系")]
    [TestCase(5, 10, 5, ExpectedResult = 0, Category = "正常系")]

    public int GetRemainingParamPointPasses(int level, int str, int agi)
    {
        return new LevelDependentParameters(new(Experience.RequiredCumulativeExp(level)), new(str), new(agi))
            .GetRemainingParamPoint();
    }

    [Test()]
    [TestCase(1, 3, 1, Category = "異常系")]
    public void GetRemainingParamPointFail(int level, int str, int agi)
    {
        Assert.Throws(Is.TypeOf<ArgumentException>().And.Message.EqualTo($"指定したSTR, AGIは割当可能量を超過しています STR: {str}, AGI: {agi}"),
        () => new LevelDependentParameters(new(Experience.RequiredCumulativeExp(level)), new(str), new(agi))
                .GetRemainingParamPoint());
    }
}
