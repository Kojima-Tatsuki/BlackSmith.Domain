using BlackSmith.Domain.Character.Player;
using NUnit.Framework;

internal class ExperienceTest
{
    [Test(Description = "総経験値量からレベルを計算するテスト")]
    [TestCase(0, ExpectedResult = 1, Category = "正常系")]
    [TestCase(100, ExpectedResult = 2, Category = "正常系")]
    [TestCase(225, ExpectedResult = 3, Category = "正常系")]
    [TestCase(2407011, ExpectedResult = 39, Category = "正常系")]
    [TestCase(2407012, ExpectedResult = 40, Category = "正常系")]
    [TestCase(2407013, ExpectedResult = 40, Category = "正常系")]
    public double CurrentLevelPasses(int exp)
    {
        return Experience.CurrentLevel(new Experience(exp));
    }

    [Test(Description = "レベルからそのレベルを満たす最低経験値量を計算するテスト")]
    [TestCase(1, ExpectedResult = 0, Category = "正常系")]
    [TestCase(2, ExpectedResult = 100, Category = "正常系")]
    [TestCase(3, ExpectedResult = 225, Category = "正常系")]
    [TestCase(40, ExpectedResult = 2407012)]
    public int FromLevelPasses(int level)
    {
        return Experience.RequiredCumulativeExp(level).Value;
    }
}
