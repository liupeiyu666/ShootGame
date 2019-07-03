using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Effect
{
    /// <summary>
    /// 获取sharedate中的数据，设置数值
    /// </summary>
    [Serializable]
    [Des("点位出生")]
    public class PositionBorn:BornRule
    {
        public override bool CheckBorn(float p_deltaTime)
        {
            m_gameObject.transform.position = m_shareDate.m_bornPos;
            m_gameObject.transform.rotation = m_shareDate.m_bornRot;

            return true;
        }
    }
}
