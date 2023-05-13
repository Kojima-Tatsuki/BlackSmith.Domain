using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackSmith.Domain.Skill
{
    internal interface ISkill
    {
        /// <summary>スキル名</summary>
        SkillName Name { get; }

        /// <summary>熟練度</summary>
        SkillProficiency Proficiency { get; }
        
        /// <summary>取得条件</summary>
        SkillAcquisitionConditions AcquisitionConditions { get; }
    }

    internal interface IItemCreateater
    {

    }

    internal interface IProductionSkill : ISkill
    {
        CreateSkillAction CreateAction { get; }

        RefineSkillAction RefineAction { get; }

        RepairSkillAction RepairAction { get; }
    }

    // 戦闘スキルの内容については要検討 (考案、実装にwiki編集)
    internal interface IBattleSkill : ISkill
    {

    }
}
