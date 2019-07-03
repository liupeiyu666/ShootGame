using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Engine.Effect
{
    /// <summary>
    /// 碰撞检测的生存,使用Trigger检测，所以会存在穿透的现象，对于高速运动的建议使用射线检测
    /// 
    /// </summary>
    [Serializable]
    [Des("碰撞检测")]
    public class CollisionServive:ServiveRule
    {
        public int[] m_collisionLayer;

        private LayerMask m_layerMask = 0;

        private bool m_serviveEnd;
        public override bool ServiveCheck(float p_deltaTime)
        {
            InitLayer();

            if (m_serviveEnd)
            {
                m_serviveEnd = false;
                return true;
            }
            return false;
        }

        public override void OnTriggerEnter(Collider p_collider)
        {
            if (!m_serviveEnd&&CheckLayer(p_collider.gameObject.layer))
            {
                m_serviveEnd = true;
            }
        }

        bool CheckLayer(int p_layer)
        {
            bool t_contains = false;
            for (int i = 0; i < m_collisionLayer.Length; i++)
            {
                if (m_collisionLayer[i]==p_layer)
                {
                    t_contains = true;
                    break;
                }
            }

            return t_contains;
        }

        void InitLayer()
        {
            for (int i = 0; i < m_collisionLayer.Length; i++)
            {
                m_layerMask |= m_collisionLayer[i];
            }
        }
    }
    
}
