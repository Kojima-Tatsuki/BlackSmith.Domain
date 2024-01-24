namespace BlackSmith.Domain.Skill
{
    internal interface ISkill
    {
        /// <summary>スキル名</summary>
        SkillName SkillName { get; }

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
