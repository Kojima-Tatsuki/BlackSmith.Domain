using BlackSmith.Domain.Character.Player;
using NUnit.Framework;

internal class ExperienceTest
{
    [Test(Description = "‘ŒoŒ±’l—Ê‚©‚çƒŒƒxƒ‹‚ğŒvZ‚·‚éƒeƒXƒg")]
    [TestCase(0, ExpectedResult = 1, Category = "³íŒn")]
    [TestCase(100, ExpectedResult = 2, Category = "³íŒn")]
    [TestCase(225, ExpectedResult = 3, Category = "³íŒn")]
    [TestCase(2407011, ExpectedResult = 39, Category = "³íŒn")]
    [TestCase(2407012, ExpectedResult = 40, Category = "³íŒn")]
    [TestCase(2407013, ExpectedResult = 40, Category = "³íŒn")]
    public double CurrentLevelPasses(int exp)
    {
        return Experience.CurrentLevel(new Experience(exp));
    }

    [Test(Description = "ƒŒƒxƒ‹‚©‚ç‚»‚ÌƒŒƒxƒ‹‚ğ–‚½‚·Å’áŒoŒ±’l—Ê‚ğŒvZ‚·‚éƒeƒXƒg")]
    [TestCase(1, ExpectedResult = 0, Category = "³íŒn")]
    [TestCase(2, ExpectedResult = 100, Category = "³íŒn")]
    [TestCase(3, ExpectedResult = 225, Category = "³íŒn")]
    [TestCase(40, ExpectedResult = 2407012)]
    public int FromLevelPasses(int level)
    {
        return Experience.RequiredCumulativeExp(level).Value;
    }
}
