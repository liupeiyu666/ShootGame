using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Effect
{
    /// <summary>
    /// 扩展规则
    /// </summary>
    public abstract class ExpandRule:BaseRule
    {
        public ExpandRule()
        {
            m_ruleType = Enum_RuleType.Expand;
        }
        public abstract void DoExpand(float p_deltaTime);
    }
}
