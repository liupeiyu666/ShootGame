using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace Engine.Effect
{
  
    /// <summary>
    /// 生存规则
    /// </summary>
    public abstract class ServiveRule : BaseRule
    {
        public ServiveRule()
        {
            m_ruleType = Enum_RuleType.Servive;
        }
        #region 位置设置

        /// <summary>
        /// 生存检测，子类去完成生存规则
        /// </summary>
        /// <returns></returns>
        public abstract bool ServiveCheck(float p_deltaTime);

        #endregion
    }
   
}
