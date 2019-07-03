using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace Engine.Effect
{

    /// <summary>
    ///死亡规则
    /// </summary>
    public abstract class DeadRule : BaseRule
    {
        public DeadRule()
        {
            m_ruleType = Enum_RuleType.Dead;
        }
        public abstract void DoDead();
    }
}
