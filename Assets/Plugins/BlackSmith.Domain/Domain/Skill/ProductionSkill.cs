﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackSmith.Domain.Character.Player;

namespace BlackSmith.Domain.Skill
{
    // このスキル自体に意味はないスキル使用条件で用いられるのみ
    internal class ProductionSkill : IProductionSkill
    {
        SkillName ISkill.Name => BaseSkill.SkillName;

        SkillProficiency ISkill.Proficiency => BaseSkill.Proficiency;

        SkillAcquisitionConditions ISkill.AcquisitionConditions => BaseSkill.AcquisitionConditions;

        private Skill BaseSkill { get; }

        public CreateSkillAction CreateAction => throw new NotImplementedException();

        public RefineSkillAction RefineAction => throw new NotImplementedException();

        public RepairSkillAction RepairAction => throw new NotImplementedException();

        internal ProductionSkill(SkillName name, SkillExperience exp, SkillAcquisitionConditions conditions)
        {
            BaseSkill = new Skill(name, exp, conditions);
        }
    }
}
