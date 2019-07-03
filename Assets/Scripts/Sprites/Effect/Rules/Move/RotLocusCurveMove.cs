using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Engine.Effect
{
    /// <summary>
    /// 朝向轨迹
    /// 特效的朝向计算，主要计算特效的朝向设计
    /// </summary>
    [Des("角度旋转")]
    public class RotLocusCurveMove : MoveRule
    {
        public AnimationCurve m_xCurve;

        private bool m_hasxCurve;

        public AnimationCurve m_yCurve;

        private bool m_hasyCurve;
        public AnimationCurve m_zCurve;

        private bool m_haszCurve;
   
        private float m_totalTime;

        private int m_times = 0;
        public override void UpdateMove(float p_deltaTime)
        {
            CheckInit();
            //设置角度
            float t_xangel = m_xCurve.Evaluate(m_totalTime);

            float t_yangel = m_yCurve.Evaluate(m_totalTime);

            float t_zangel = m_zCurve.Evaluate(m_totalTime);

            //围绕某个轴旋转
            if (m_hasxCurve)
            {
                m_gameObject.transform.Rotate(m_gameObject.transform.right, t_xangel,Space.Self);
            }
            if (m_hasyCurve)
            {
                m_times++;
                m_gameObject.transform.Rotate(m_gameObject.transform.up, t_yangel, Space.Self);
               // Debug.LogError(m_gameObject.transform.localRotation.eulerAngles.y+"   "+ t_yangel+"   "+ m_times);
            }
            if (m_haszCurve)
            {
                m_gameObject.transform.Rotate(m_gameObject.transform.forward, t_zangel, Space.Self);
            }
            // m_gameObject.transform.eulerAngles=new Vector3(t_xangel,t_yangel,t_zangel);
            m_totalTime += p_deltaTime;
        }

        protected override void OnInit()
        {
            m_totalTime = 0;
            m_hasxCurve = m_xCurve.length > 1 ? true : false;
            m_hasyCurve = m_yCurve.length > 1 ? true : false;
            m_haszCurve = m_zCurve.length > 1 ? true : false;
        }
    }
}
