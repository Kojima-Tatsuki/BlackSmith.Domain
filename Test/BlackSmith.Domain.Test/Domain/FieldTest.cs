/*using BlackSmith.Domain.Character;
using BlackSmith.Domain.Field;
using BlackSmith.Domain.Player;

namespace BlackSmith.Domain.Test
{
    [TestClass]
    public class FieldTest
    {
        [TestMethod]
        public void FieldCharacter()
        {
            var character = PlayerFactory.Create(new PlayerName("TestPlayer"));

            var field = Field.Field.NameOf("TestField");

            field.AddCharacter(character.ID);

            Assert.IsTrue(field.CharacterIds.Contains(character.ID));

            field.RemoveCharacter(character.ID);

            Assert.IsFalse(field.CharacterIds.Contains(character.ID));
        }
    }
}
*/