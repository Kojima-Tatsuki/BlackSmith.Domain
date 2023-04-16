using System;
using System.Collections.Generic;
using BlackSmith.Domain.Character;
using BlackSmith.Domain.Field;

namespace BlackSmith.Domain.Skill
{
    /// <summary>
    /// <see cref="SkillAction"/>にてインスタンス化される
    /// </summary>
    internal class SkillEffect
    {
        public string EffectName { get; }

        public EffectExecutionConditions ExecutionConditions { get; }

        public SkillTarget Target { get; }

        public SkillActivator Activator { get; }

        public SkillEffect(string effectName, EffectExecutionConditions executionConditions)
        {
            EffectName = effectName;
            ExecutionConditions = executionConditions;
        }
    }

    /// <summary>状況を提供する</summary>
    public interface IConditionProvider
    {
        FieldType GetField();
    }

    /// <summary>スキルの発動条件</summary>
    /// <remarks>指定される条件をすべて満たしたときに発動可能</remarks>
    public class EffectExecutionConditions
    {
        private Guid SkillExecutionConditionsId { get; } // DBに格納する際に必要になる識別子

        public FieldCondition Field { get; }

        public EffectExecutionConditions(Guid id, FieldCondition fieldCondition)
        {
            SkillExecutionConditionsId = id;
            Field = fieldCondition;
        }

        public class FieldCondition
        {
            public FieldType FieldType { get; }
        }
    }

    /// <summary>スキルの発動対象</summary>
    public class SkillTarget
    {
        CharacterID TargetId { get; }
    }

    /// <summary>スキルの実行者</summary>
    public class SkillActivator
    {
        CharacterID ActivatorId { get; }
    }
}
