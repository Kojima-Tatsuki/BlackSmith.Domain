using BlackSmith.Domain.Character.Player;
using NUnit.Framework;
using System;

#nullable enable

internal class PlayerLevelTest
{
    internal static PlayerLevel[] GetPlayserLevelMock()
    {
        return new PlayerLevel[]
        {
            new PlayerLevel(), // �o���l��^���Ȃ��ꍇ(���x��1)
            new PlayerLevel(Experience.RequiredCumulativeExp(100)), // �ő僌�x��
        };
    }

    [Test(Description = "PlayerLevel�C���X�^���X���e�X�g")]
    [TestCase(null, null, null, Category ="����n")] // �o���l��^���Ȃ��ꍇ
    public void PlayerLevelInstancePasses(Experience? exp, Experience? resultExp, Type? exception)
    {
        if (exception is null)
            Assert.That(new PlayerLevel(exp ?? new Experience()), Is.EqualTo(new PlayerLevel(resultExp ?? new Experience())));
        else
            Assert.Throws(exception, () => new PlayerLevel(exp ?? new Experience()));
    }

    [Test(Description = "�X�e�[�^�X�㏸�|�C���g�̎c�萔�e�X�g")]
    [TestCase(1, 1, 1, null, ExpectedResult = 1, Category = "����n")]
    [TestCase(5, 10, 5, null, ExpectedResult = 0, Category = "����n")]
    [TestCase(1, 3, 1, typeof(ArgumentException), ExpectedResult = 0, Category = "�ُ�n")]
    public int GetRemainingParamPointPasses(int level, int str, int agi, Type? exception)
    {
        if (exception is null)
            return new LevelDependentParameters(new(Experience.RequiredCumulativeExp(level)), new(str), new(agi))
                .GetRemainingParamPoint();
        else
            Assert.Throws(exception, () => new LevelDependentParameters(new(Experience.RequiredCumulativeExp(level)), new(str), new(agi))
                           .GetRemainingParamPoint());
        
        return 0; // �����ɂ͓��B���Ȃ�
    }
}
