using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Engine.Effect
{
    /// <summary>
    /// 特效的共享数据
    /// 运行时执行的数据
    /// </summary>
    [Serializable]
    public class EffectShareData
    {
        /// <summary>
        /// 特效的出生节点，
        /// </summary>
        public Transform m_bornTrans;
        /// <summary>
        /// 特效的目标节点,主要针对飞行类特效
        /// </summary>
        public Transform m_targetTrans;
        /// <summary>
        /// 出生的点
        /// </summary>
        public Vector3 m_bornPos;
        /// <summary>
        /// 出生的角度
        /// </summary>
        public Vector3 m_bornRot;

        /// <summary>
        /// 要到达的点
        /// </summary>
        public Vector3 m_targetPos;
        /// <summary>
        /// 要到达的角度
        /// </summary>
        public Vector3 m_targetRot;
    }
}
