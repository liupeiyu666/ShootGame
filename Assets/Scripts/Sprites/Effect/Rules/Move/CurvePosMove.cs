using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Engine.Effect
{
    /// <summary>
    /// 不提供追踪，有追踪无曲线
    /// 曲线轨迹移动，提供两个曲线
    ///      1.速度曲线
    ///      2.轨迹偏移曲线，Y,X
    ///
    /// ======================================
    /// 由于存在追踪的轨迹设计，所以计算方式跟水平的计算不一致：曲线百分比累加
    /// 获取偏移，>=1,按照0计算。
    /// </summary>
    [Des("曲线移动")]
    public class CurvePosMove:MoveRule
    {

        public Enum_StraightMoveType m_moveType = Enum_StraightMoveType.Distance;

        /// <summary>
        /// 移动距离
        /// </summary>
        public float m_distance;
        /// <summary>
        /// X轴速度
        /// </summary>
        public AnimationCurve m_speedCurve;
        /// <summary>
        /// 在垂直于目标方向的偏移
        /// </summary>
        public AnimationCurve m_offestUpCurve;
        public Enum_CurveType m_offestUpCurveType;
        /// <summary>
        /// 在目标方向的右侧偏移
        /// </summary>
        public AnimationCurve m_offestRightCurve;
        public Enum_CurveType m_offestRightType;
        /// <summary>
        /// 是否朝向移动的方向
        /// </summary>
        public bool m_lookAt;
        /// <summary>
        /// 移动方向
        /// </summary>
        private Vector3 m_dirction=Vector3.zero;
        /// <summary>
        /// 原始的位置点，因为追踪的时候终点是可变的，每次都要以起始点和终点计算路径
        /// </summary>
        private Vector3 m_orignPos;
        /// <summary>
        /// 用于记录已经行走过的曲线比例
        /// </summary>
        private float m_curvePercent;
        /// <summary>
        /// 行驶的总时长
        /// </summary>
        private float m_totalTime;
        public override void UpdateMove(float p_deltaTime)
        {
            CheckInit();
            
            m_totalTime += p_deltaTime;
            //获取速度，具体的在设置曲线的时候完成曲线的设置
            float t_speed = m_speedCurve.Evaluate(m_totalTime);
            //就散方向
            CaculateFollow(t_speed * p_deltaTime);
            SetLookAt();
            if (m_dirction==Vector3.zero)
            {
                Debug.LogError("Dispatch(RuleEvent_OnArrive)");
                m_gameObject.transform.position = m_shareDate.m_targetPos;
                Dispatch(RuleEvent_OnArrive);
            }
            else
            {
                m_gameObject.transform.position += m_dirction * t_speed * p_deltaTime;
            }
           
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
            m_orignPos = m_gameObject.transform.position;
            m_totalTime = 0;
        }

        private float m_recordStep;
        void CaculateFollow(float p_step)
        {
            if (true||m_shareDate.m_targetTrans&&m_shareDate.m_targetTrans.gameObject.activeInHierarchy)
            {
                //如果是追踪目标的

                //越域就别跑了
                if (Vector3.Distance(m_gameObject.transform.position, m_shareDate.m_targetPos) < m_recordStep)
                {
                 //   Debug.LogError("m_dirction = Vector3.zero");
                    //到了，别跑了
                    m_dirction = Vector3.zero;
                    return;
                }
             //   Debug.LogError("+++:"+ m_gameObject.transform.position.x+"  "+ m_gameObject.transform.position.y+"  "+ m_gameObject.transform.position.z+"   "+ m_dirction + "  "+ Vector3.Distance(m_gameObject.transform.position, m_shareDate.m_targetPos)+"  "+ m_recordStep);
                m_recordStep = p_step;
                Vector3 t_currentpos = (m_shareDate.m_targetPos - (m_gameObject.transform.position));
                Vector3 t_begin=(m_shareDate.m_targetPos -m_orignPos);
                //获取当前的原始方向上的分量
                Vector3 t_Originprotject = Vector3.Project(t_currentpos, t_begin);
                //行进一步的差值投影--2019.2.28之前是在上一次的方向进行一次移动，这样就会导致如果上一次的方向Y方向做了严重偏离就会导致再恢复正常轨迹的时候有问题
                //本次修改为根据投影去计算
                Vector3 t_stepProject = t_Originprotject.normalized* p_step;
                //当前的投影分量
                float t_evalute = 1-(t_Originprotject - t_stepProject).magnitude / t_begin.magnitude;
                float t_upOffset = 0, t_rightOffset = 0;
                if (t_evalute < 0.99f)
                {
                    float t_distance = (m_shareDate.m_targetPos - t_Originprotject + t_stepProject).magnitude;

                    switch (m_offestRightType)
                    {
                        case Enum_CurveType.Distance:
                            t_rightOffset = m_offestRightCurve.Evaluate(t_distance) ;
                            break;
                        case Enum_CurveType.Percent:
                            t_rightOffset = m_offestRightCurve.Evaluate(t_evalute) * t_begin.magnitude;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    switch (m_offestUpCurveType)
                    {
                        case Enum_CurveType.Distance:
                            t_upOffset = m_offestUpCurve.Evaluate(t_distance);
                            break;
                        case Enum_CurveType.Percent:
                            t_upOffset = m_offestUpCurve.Evaluate(t_evalute) * t_begin.magnitude;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    //计算偏移值,计算下一次的位置方向
                    Quaternion t_qua = Quaternion.LookRotation(t_begin);
                    Vector3 t_up = t_qua * Vector3.up;
                    Vector3 t_right = t_qua * Vector3.right;
                    Vector3 t_calatePos = m_orignPos + t_Originprotject + t_stepProject + t_up * t_upOffset +
                                          t_right * t_rightOffset;
                    m_dirction = (m_shareDate.m_targetPos - t_Originprotject + t_stepProject + t_up * t_upOffset + t_right * t_rightOffset - m_gameObject.transform.position).normalized;
                }
                else
                {
                    m_dirction = Vector3.zero;
                    return;
                }

            }
        }

        void SetLookAt()
        {
            if (m_lookAt&& m_dirction!=Vector3.zero)
            {
                m_gameObject.transform.forward = m_dirction;
            }
        }

    }
}
