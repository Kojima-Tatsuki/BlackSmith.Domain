using System;

namespace BlackSmith.Domain.Skill
{
    // このスキル自体に意味はないスキル使用条件で用いられるのみ
    internal class ProductionSkill : Skill, IProductionSkill
    {
        // アクション自体の動作実装は、ここで行わない
        // スキルは使用の際の証明書のようにふるまうと良さげ
        public CreateSkillAction CreateAction => throw new NotImplementedException();

        public RefineSkillAction RefineAction => throw new NotImplementedException();

        public RepairSkillAction RepairAction => throw new NotImplementedException();

        public ProductionSkill(SkillID id, SkillName skillName, SkillExperience exp, SkillAcquisitionConditions acquisitionConditions) : base(id, skillName, exp, acquisitionConditions)
        {

        }
    }
}
