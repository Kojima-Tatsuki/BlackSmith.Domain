using System.Collections;
using System.Collections.Generic;
using BlackSmith.Domain.Character;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CharacterLevelTest
{
    [Test(Description = "�X�L���擾�����v�Z����e�X�g����n")]
    [TestCase(1, ExpectedResult = 2)]
    [TestCase(6, ExpectedResult = 3)]
    [TestCase(11, ExpectedResult = 3)]
    [TestCase(18, ExpectedResult = 4)]
    [TestCase(100, ExpectedResult = 13)]
    public int NumberOfSkillAvailablePasses(int level)
    {
        return new CharacterLevel(level).GetNumberOfSkillsAvailable();
    }
}
