namespace BlackSmith.Domain.Skill
{
    /// <summary>
    /// <see cref="Skill"/>が提供する行動
    /// </summary>
    public class SkillAction
    {
        public SkillActionID ActionID { get; }

        public ActionExecutionConditions ExecutionConditions { get; }

        internal SkillAction(SkillActionID id, ActionExecutionConditions executionConditions)
        {
            ActionID = id;
            ExecutionConditions = executionConditions;
        }
    }

    /// <summary>実行条件</summary>
    public class ActionExecutionConditions
    {

    }

    public class SkillActionID : BasicID
    {
        protected override string Prefix => "SkillAction-";
    }
}
