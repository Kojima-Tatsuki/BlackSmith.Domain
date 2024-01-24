using BlackSmith.Domain.Skill;
using System.Collections.Generic;
using System.Linq;

namespace BlackSmith.Domain.Character.Skill
{
    internal class CharacterSkillModel
    {
        public CharacterID CharacterID { get; }

        public IReadOnlyList<SkillID> Skills => skills;
        private readonly List<SkillID> skills;

        internal CharacterSkillModel(CharacterID characterID, IEnumerable<SkillID> skills)
        {
            CharacterID = characterID;
            this.skills = skills.ToList();
        }
    }
}
