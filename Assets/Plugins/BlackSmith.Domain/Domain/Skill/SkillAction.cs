using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackSmith.Domain.Skill
{
    /// <summary>
    /// <see cref="Skill"/>が提供する行動
    /// </summary>
    public class SkillAction
    {
        public ActionID ActionID { get; }

        public ActionExecutionConditions ExecutionConditions { get; }

        public SkillAction(ActionID id, ActionExecutionConditions executionConditions)
        {
            ActionID = id;
            ExecutionConditions = executionConditions;
        }
    }

    /// <summary>実行条件</summary>
    public class ActionExecutionConditions
    {

    }

    public class ActionID : BasicID
    {
        public ActionID(Guid id): base(id) { }
    }
}
