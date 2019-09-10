using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Engine.Effect
{
    /// <summary>
    /// 根据一个transform设置出生位置
    /// </summary>
    [Serializable]
    [Des("节点出生")]
    public class TransformBorn:BornRule
    {
        /// <summary>
        /// 是否要设置父子节点
        /// </summary>
        public bool m_isChild;
        public Vector3 m_posOffset;

        public Vector3 m_rotOffset;
        public override void InitParams(string[] p_params)
        {
            m_isChild = bool.Parse(p_params[1]);
        }
        public override bool CheckBorn(float p_deltaTime)
        {
           // Debug.LogError( "  TransformBorn " + m_gameObject.transform.position + "   " + Time.frameCount);
            m_gameObject.transform.position = m_shareDate.m_bornTrans.position+ m_posOffset;
            m_gameObject.transform.rotation = Quaternion.Euler(m_shareDate.m_bornTrans.eulerAngles + m_rotOffset);
            if (m_isChild)
            {
                m_gameObject.transform.SetParent(m_shareDate.m_bornTrans);
            }
            return true;
        }
    }
}
