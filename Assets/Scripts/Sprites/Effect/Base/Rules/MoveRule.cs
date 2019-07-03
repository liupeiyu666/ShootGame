using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace Engine.Effect
{

    /// <summary>
    /// 运动规则
    /// </summary>
    public abstract class MoveRule : BaseRule
    {
        public MoveRule()
        {
            m_ruleType = Enum_RuleType.Move;
        }
        public abstract void UpdateMove(float p_deltaTime);

    }


}
