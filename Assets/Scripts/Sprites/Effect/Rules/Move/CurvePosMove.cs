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
        /// <summary>
        /// 在目标方向的右侧偏移
        /// </summary>
        public AnimationCurve m_offestRightCurve;
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
            CaculateFollow(t_speed * p_deltaTime);
            SetLookAt();
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
            m_orignPos = m_gameObject.transform.position;
            m_totalTime = 0;
        }

        void CaculateFollow(float p_step)
        {
            if (m_shareDate.m_targetTrans.gameObject.activeInHierarchy)
            {
                //if (m_shareDate.m_targetTrans != null)
                //{
                //    m_shareDate.m_targetPos = m_shareDate.m_targetTrans.position;
                //}
                //越域就别跑了
                if (Vector3.Distance(m_gameObject.transform.position, m_shareDate.m_targetPos) < p_step)
                {
                    //到了，别跑了
                    m_dirction = Vector3.zero;
                    return;
                }


                Vector3 t_currentpos = (m_shareDate.m_targetPos - (m_gameObject.transform.position));
                Vector3 t_begin=(m_shareDate.m_targetPos -m_orignPos);
                //获取当前的原始方向上的分量
                Vector3 t_Originprotject = Vector3.Project(t_currentpos, t_begin);
                //行进一步的差值投影
                Vector3 t_stepProject = Vector3.Project(m_dirction * p_step, t_begin);
                //当前的投影分量
             //   Vector3 t_currentProject = Vector3.Project(m_dirction * p_step, t_begin);

                float t_rr= (t_stepProject.magnitude / t_Originprotject.magnitude) * (1 - m_curvePercent);
                m_curvePercent += (t_stepProject.magnitude / t_Originprotject.magnitude) * (1 - m_curvePercent);
              //  Debug.LogError("m_curvePercent:"+ m_curvePercent+"   "+t_rr +"   "+ t_stepProject.magnitude / t_Originprotject.magnitude+ "   t_stepProject：" + t_stepProject+ "   m_dirction：" + m_dirction+"   "+Time.frameCount);
              //  float t_percent = m_curvePercent + (t_stepProject.magnitude / t_Originprotject.magnitude)*(1-m_curvePercent);
                float t_upOffset = 0,t_rightOffset=0;
                if (m_curvePercent < 1)
                {
                    t_upOffset = m_offestUpCurve.Evaluate(m_curvePercent) * t_begin.magnitude;
                    t_rightOffset = m_offestRightCurve.Evaluate(m_curvePercent) *t_begin.magnitude;

                }
                //Debug.LogError("UP:"+ t_upOffset);
                //计算偏移值,计算下一次的位置方向
                Quaternion t_qua=Quaternion.LookRotation(t_begin);
                Vector3 t_up = t_qua * Vector3.up;
                Vector3 t_right = t_qua * Vector3.right;

                m_dirction = (m_shareDate.m_targetPos - (t_Originprotject-t_stepProject.magnitude* t_Originprotject.normalized) + t_up* t_upOffset+ t_right*t_rightOffset - m_gameObject.transform.position).normalized;
                Vector3 t_temss = (m_shareDate.m_targetPos - (t_Originprotject - t_stepProject.magnitude * t_Originprotject.normalized) - m_gameObject.transform.position);
                Vector3 t_temttar = m_shareDate.m_targetPos - (t_Originprotject - t_stepProject.magnitude * t_Originprotject.normalized) + new Vector3(t_rightOffset, t_upOffset, 0);
              //  Debug.LogError(" 投影距离: " + t_temttar.z+ "      方向："+ t_temss.z+ "  t_Originprotject:"+ t_Originprotject.z+ "   t_stepProject:"+ t_stepProject.z+"    "+Time.frameCount);
               // Debug.LogError(m_curvePercent + "  t_protject:" + t_dis + "  m_gameObjectY:" + m_gameObject.transform.position.y + "   m_gameObjectZ:" + m_gameObject.transform.position.z + "  " + "   m_dirction.y:"+ m_dirction.y+"   " + t_temss.y+"   "+t_temss.z+ "    t_temttar:" + t_temttar);
               
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
