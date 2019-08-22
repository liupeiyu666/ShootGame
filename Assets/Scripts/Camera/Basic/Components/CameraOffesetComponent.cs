using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using UnityEngine;

namespace AdvancedUtilities.Cameras.Components
{
    [Serializable]
    public class CameraOffsetComponent:CameraComponent
    {
        public override void Initialize(CameraController cameraController)
        {
            base.Initialize(cameraController);
            OnOffset = SetOffset;
        }
        /// <summary>
        /// 摄像机移动时的偏移函数
        /// </summary>
        public Action<Vector3> OnOffset;
        //x2/a2+x2/b2=1
        public float m_circleA = 2;
        public float m_circleB = 1;
        /// <summary>
        /// 加速度增量--当速度按照时间进行叠加时使用
        /// </summary>
        public float m_addSpeed = 1;
        /// <summary>
        /// 最大速度
        /// </summary>
        public float m_maxSpeed = 2;
        /// <summary>
        /// 最小速度
        /// </summary>
        public float m_minSpeed = 2;
        private float m_Speed = 0.2f;


        /// <summary>
        /// 当前的偏移值
        /// </summary>
        private Vector3 m_offset=Vector3.zero;

        private void SetOffset(Vector3 p_offset)
        {
            //1.求取直线方程y=kx
            if (p_offset.x==0&& p_offset.z!=0)
            {
                m_offset = p_offset.z >= 0? new Vector3(0, 0, m_circleB) : new Vector3(0, 0,- m_circleB);
            }
            else if(p_offset.x == 0 && p_offset.z == 0)
            {
                m_offset=Vector3.zero;
            }
            else
            {
                float t_k = p_offset.z / p_offset.x;
                //2.求交点
                float t_chargex = m_circleA * m_circleB / (Mathf.Sqrt(Mathf.Pow(m_circleB, 2) + Mathf.Pow(m_circleA * t_k, 2)));
                float t_chargeY = t_k * t_chargex;
                Vector3 t_point = p_offset.x >= 0
                    ? new Vector3(t_chargex, 0, t_chargeY)
                    : new Vector3(-t_chargex, 0, -t_chargeY);

                //   Vector3 t_temp = m_offset + p_offset.normalized*Time.deltaTime* m_Speed;
                m_offset = t_point;
            }

            SetChangeCheck(p_offset);
        }

        #region 判断是否需要变换速度
        /// <summary>
        /// 标记是否发生了变化
        /// </summary>
        private bool m_isChange;
        /// <summary>
        /// 记录发生变化时的偏移距离
        /// </summary>
        private Vector3 m_changeOffsetRecord;

        private Vector3 m_recordInput=Vector3.zero;
        private void SetChangeCheck(Vector3 p_offset)
        {
            m_isChange = false;
            //1。如果输入有值，记录没有值认为发生变化
            if (p_offset!=Vector3.zero&& m_recordInput== Vector3.zero)
            {
                m_isChange = true;
            }
            //2.如果输入的数值为0,但是记录的不为0也认为发生了变化
            else if (p_offset == Vector3.zero && m_recordInput != Vector3.zero)
            {
                m_isChange = true;
            }
            //3.输入的数值和记录的数值角度相差一定角度认为发生了变化
            else if (p_offset != Vector3.zero&&Vector3.Angle(p_offset, m_recordInput)>2)
            {
                m_isChange = true;
            }
            if (m_isChange)
            {
                //1.记录当前位置
                m_changeOffsetRecord = m_recordOffset;
                //2.重置速度
                m_Speed = 0;
            }
            m_recordInput = p_offset;
        }

        #endregion
        /// <summary>
        /// 记录上一次获取的偏移值
        /// </summary>
        private Vector3 m_recordOffset;
        /// <summary>
        /// 获取偏移数据
        /// </summary>
        /// <returns></returns>
        public Vector3 GetCameraOffset()
        {
            //Vector3 t_normal = m_offset - m_recordOffset;
            //if (t_normal.magnitude >= Time.deltaTime * m_Speed)
            //{
            //    m_recordOffset += t_normal.normalized * Time.deltaTime * m_Speed;
            //}
            //else
            //{
            //    m_recordOffset = m_offset;
            //}

            ////差值移动
            //Vector3 t_tempOffset = Vector3.Lerp(m_recordOffset, m_offset, Time.deltaTime * m_Speed);
            //m_recordOffset = t_tempOffset;

            //加速移动
            Vector3 t_normal = m_offset - m_recordOffset;
            float t_speed = GetSpeed();
            if (t_normal.magnitude >= t_speed)
            {
                m_recordOffset += t_normal.normalized * t_speed;
            }
            else
            {
                m_recordOffset = m_offset;
                m_Speed = 0;
            }

            return m_recordOffset;
        }
        /// <summary>
        /// 获取速度
        /// </summary>
        /// <returns></returns>
        float GetSpeed()
        {
            m_Speed = m_Speed + m_addSpeed * Time.deltaTime;
            m_Speed = Mathf.Clamp(m_Speed, m_minSpeed, m_maxSpeed);
            return m_Speed;
        }
    }
}
