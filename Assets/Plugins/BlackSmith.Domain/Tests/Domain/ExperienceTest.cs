using BlackSmith.Domain.Character.Player;
using NUnit.Framework;

internal class ExperienceTest
{
    [Test(Description = "���o���l�ʂ��烌�x�����v�Z����e�X�g")]
    [TestCase(0, ExpectedResult = 1, Category = "����n")]
    [TestCase(100, ExpectedResult = 2, Category = "����n")]
    [TestCase(225, ExpectedResult = 3, Category = "����n")]
    [TestCase(2407011, ExpectedResult = 39, Category = "����n")]
    [TestCase(2407012, ExpectedResult = 40, Category = "����n")]
    [TestCase(2407013, ExpectedResult = 40, Category = "����n")]
    public double CurrentLevelPasses(int exp)
    {
        return Experience.CurrentLevel(new Experience(exp));
    }

    [Test(Description = "���x�����炻�̃��x���𖞂����Œ�o���l�ʂ��v�Z����e�X�g")]
    [TestCase(1, ExpectedResult = 0, Category = "����n")]
    [TestCase(2, ExpectedResult = 100, Category = "����n")]
    [TestCase(3, ExpectedResult = 225, Category = "����n")]
    [TestCase(40, ExpectedResult = 2407012)]
    public int FromLevelPasses(int level)
    {
        return Experience.RequiredCumulativeExp(level).Value;
    }
}
