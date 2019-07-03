using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Engine.Effect
{
    /// <summary>
    /// 动画曲线,速度曲线图
    ///     1.移动固定的位置，方向就是自己的朝向
    ///     2.移动到外部提供的目标位置，方向为自己到目标的方向，此时不会设置自己的朝向
    ///     3.移动的方向，根据targetTrans决定
    /// </summary>
    [Serializable]
    [Des("直线移动")]
    public class StraightPosMove:MoveRule
    {

        /// <summary>
        /// X轴速度
        /// </summary>
        public AnimationCurve m_speedCurveX;
        /// <summary>
        /// 移动距离
        /// </summary>
        public float m_distance;


        public Enum_StraightMoveType m_moveType = Enum_StraightMoveType.Distance;
        /// <summary>
        /// 移动方向
        /// </summary>
        private Vector3 m_dirction;

        private float m_totalTime;
        public override void UpdateMove(float p_deltaTime)
        {
            CheckInit();
            CaculateFollow();
            m_totalTime += p_deltaTime;
            //获取速度，具体的在设置曲线的时候完成曲线的设置
            float t_speed = m_speedCurveX.Evaluate(m_totalTime);
            m_gameObject.transform.position += m_dirction * t_speed * p_deltaTime;
        }
        protected override void OnInit()
        {
            //设置目标点，如果是固定距离
            switch (m_moveType)
            {
                case Enum_StraightMoveType.TargetPoint:
                    break;
                case Enum_StraightMoveType.Distance:
                    m_shareDate.m_targetPos = m_gameObject.transform.position + m_gameObject.transform.forward * m_distance;
                    break;
                case Enum_StraightMoveType.TargetTrans:
                    {
                        if (m_shareDate.m_targetTrans != null && m_shareDate.m_targetTrans.gameObject.activeInHierarchy)
                        {
                            m_shareDate.m_targetPos = m_shareDate.m_targetTrans.position;
                        }
                    }
                    break;
            }
            m_dirction = (m_shareDate.m_targetPos - m_gameObject.transform.position).normalized;
            m_totalTime = 0;
        }

        void CaculateFollow()
        {
            if (m_moveType == Enum_StraightMoveType.TargetTrans && m_shareDate.m_targetTrans.gameObject.activeInHierarchy)
            {
                if (m_shareDate.m_targetTrans != null)
                {
                    m_shareDate.m_targetPos = m_shareDate.m_targetTrans.position;
                }
                m_dirction = (m_shareDate.m_targetPos - m_gameObject.transform.position).normalized;
            }
        }
    }

    public enum Enum_StraightMoveType
    {
        /// <summary>
        /// 距离移动
        /// </summary>
        Distance,
        /// <summary>
        /// 外部传递的目标点移动
        /// </summary>
        TargetPoint,
        /// <summary>
        /// 外部传递的目标transform移动
        /// </summary>
        TargetTrans,
    }
}
