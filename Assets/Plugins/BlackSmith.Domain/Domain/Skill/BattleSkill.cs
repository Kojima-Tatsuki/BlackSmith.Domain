using System.Collections.Generic;
using System.Linq;

namespace BlackSmith.Domain.Skill
{
    internal class BattleSkill
    {
        public SkillID ID { get; }
        
        public IReadOnlyList<SkillActionID> ActionIds => actionIds;
        private readonly List<SkillActionID> actionIds;

        internal BattleSkill(SkillID id, IReadOnlyList<SkillActionID> actionIds)
        {
            ID = id;
            this.actionIds = actionIds.ToList();
        }
    }
}
