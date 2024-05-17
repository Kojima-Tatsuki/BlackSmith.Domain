using BlackSmith.Domain.Character;
using BlackSmith.Domain.Field;
using System;

#nullable enable

namespace BlackSmith.Domain.Skill
{
    /// <summary>
    /// <see cref="SkillAction"/>にてインスタンス化される
    /// </summary>
    public class SkillEffect
    {
        public string EffectName { get; }

        public EffectExecutionConditions ExecutionConditions { get; }

        public SkillTarget Target { get; }

        public SkillActivator Activator { get; }

        internal SkillEffect(string effectName, EffectExecutionConditions executionConditions, SkillTarget target, SkillActivator activator)
        {
            EffectName = effectName;
            ExecutionConditions = executionConditions;
            Target = target;
            Activator = activator;
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

        internal EffectExecutionConditions(Guid id, FieldCondition fieldCondition)
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
        public CharacterID TargetId { get; }

        public SkillTarget(CharacterID targetId)
        {
            TargetId = targetId;
        }
    }

    /// <summary>スキルの実行者</summary>
    public class SkillActivator
    {
        public CharacterID ActivatorId { get; }

        public SkillActivator(CharacterID activatorId)
        {
            ActivatorId = activatorId;
        }
    }
}
