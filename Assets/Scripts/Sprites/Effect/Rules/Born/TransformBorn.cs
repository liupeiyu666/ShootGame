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

        public override void InitParams(string[] p_params)
        {
            m_isChild = bool.Parse(p_params[1]);
        }
        public override bool CheckBorn(float p_deltaTime)
        {
            m_gameObject.transform.position = m_shareDate.m_bornTrans.position;
            m_gameObject.transform.rotation = m_shareDate.m_bornTrans.rotation;
            if (m_isChild)
            {
                m_gameObject.transform.SetParent(m_shareDate.m_bornTrans);
            }
            return true;
        }
    }
}
