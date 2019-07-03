using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Engine.Effect
{
    /// <summary>
    /// 时间限制类型的生存
    /// </summary>
    [Serializable]
    [Des("时间检测")]
    public class TimeLimitServive:ServiveRule
    {
        public float m_aliveTime = 1;
        /// <summary>
        /// 是否开始启动生存计时
        /// </summary>
        private bool m_startServive = false;

        private float m_totalTime;
        public override bool ServiveCheck(float p_deltaTime)
        {
            if (!m_startServive)
            {
                m_totalTime = 0;
                m_startServive = true;
            }

            m_totalTime += p_deltaTime;
            if (m_totalTime>= m_aliveTime)
            {
                m_startServive = false;
                return true;
            }
            return false;
        }
    }
}
