using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Engine.Effect
{
    /// <summary>
    /// 基于射线的检测
    /// 为了和其他的规则去耦，这里采用，当前位置和上一次之间打射线的方式来实现,避免的对方向的需求
    /// </summary>
    [Serializable]
    [Des("射线检测")]
    public class RayCheckServive:ServiveRule
    {
        public int[] m_collisionLayer;

        private LayerMask m_layerMask = 0;

        private bool m_serviveEnd;
        /// <summary>
        /// 记录上一次的位置
        /// </summary>
        private Vector3 m_recordPos=Vector3.positiveInfinity;
        
        public override bool ServiveCheck(float p_deltaTime)
        {
            InitLayer();
            CheckCollision();
            if (m_serviveEnd)
            {
                m_serviveEnd = false;
                return true;
            }
            return false;
        }
        void InitLayer()
        {
            for (int i = 0; i < m_collisionLayer.Length; i++)
            {
                m_layerMask |=1<<m_collisionLayer[i];
            }
        }
        
        private void CheckCollision()
        {
            if (!m_serviveEnd)
            {
                if (m_recordPos==Vector3.positiveInfinity)
                {
                    m_recordPos = m_gameObject.transform.position;
                }
                Vector3 t_dir = m_gameObject.transform.position - m_recordPos;
                float t_dis = Math.Abs(Vector3.Distance(m_recordPos, m_gameObject.transform.position));
                Ray ray = new Ray();
                ray.origin = m_recordPos;
                ray.direction = t_dir;

                m_recordPos = m_gameObject.transform.position;
                RaycastHit hit;
               // Debug.DrawLine(ray.origin, t_dir.normalized *2,Color.red,2f);
                if (Physics.Raycast(ray, out hit,t_dis, m_layerMask))
                {
                    m_serviveEnd = true;
                }
            }
        }


    }
}
