using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Engine.Effect
{
    
    /// <summary>
    /// 获取sharedate中的数据，设置数值
    /// </summary>
    [Serializable]
    [Des("点位出生")]
    public class PositionBorn:BornRule
    {
        public Vector3 m_posOffset;

        public Vector3 m_rotOffset;
        public override bool CheckBorn(float p_deltaTime)
        {
            m_gameObject.transform.position = m_shareDate.m_bornPos+ m_posOffset;
            m_gameObject.transform.rotation = Quaternion.Euler(m_shareDate.m_bornRot+ m_rotOffset);

            return true;
        }
    }
}
