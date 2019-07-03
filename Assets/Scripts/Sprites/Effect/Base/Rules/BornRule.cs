using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace Engine.Effect
{
    /// <summary>
    /// 出生规则
    /// </summary>
    [Serializable]
    public abstract class BornRule : BaseRule
    {
        public BornRule()
        {
            m_ruleType = Enum_RuleType.Born;
        }

        #region 位置设置
        /// <summary>
        /// 初始化，用于完成特效的出生设定
        /// </summary>
        public abstract bool CheckBorn(float p_deltaTime);



        #endregion
    }

}
